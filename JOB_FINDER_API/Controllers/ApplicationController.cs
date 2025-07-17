using CloudinaryDotNet;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.Requests;
using JOB_FINDER_API.Models.Services;
using JOB_FINDER_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UglyToad.PdfPig;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly SemanticMatchingService _semanticMatchingService;
        private readonly ILogger<ApplicationController> _logger;
        private readonly IConfiguration _configuration;

        public ApplicationController(
            IServiceScopeFactory serviceScopeFactory,
            SemanticMatchingService semanticMatchingService,
            ILogger<ApplicationController> logger,
            IConfiguration configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _semanticMatchingService = semanticMatchingService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                return Ok(await context.Applications.ToListAsync());
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                var item = await context.Applications.FindAsync(id);
                return item == null ? NotFound() : Ok(item);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Application model)
        {
            if (id != model.ApplicationId) return BadRequest();
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                var item = await context.Applications.FindAsync(id);
                if (item == null) return NotFound();
                context.Applications.Remove(item);
                await context.SaveChangesAsync();
                return NoContent();
            }
        }

        [Authorize]
        [HttpPost("apply")]
        public async Task<IActionResult> Apply(
            [FromForm] ApplyJobRequest request,
            [FromServices] ICvSnapshotService cvSnapshotService,
            [FromServices] CloudinaryService cloudinaryService)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID.");

            var role = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();
            if (role != "candidate")
                return Forbid("Only candidates can apply for jobs.");

            _logger.LogInformation("User {UserId} started applying for Job {JobId}", userId, request.JobId);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                try
                {
                    // Check user profile completeness
                    var user = await context.Users
                        .Include(u => u.CandidateProfile)
                        .FirstOrDefaultAsync(u => u.UserId == userId);

                    if (user == null)
                        return Unauthorized("User not found.");

                    var profile = user.CandidateProfile;
                    if (string.IsNullOrWhiteSpace(user.FullName) ||
                        string.IsNullOrWhiteSpace(profile?.JobTitle) ||
                        string.IsNullOrWhiteSpace(user.Phone) ||
                        profile?.Dob == null ||
                        string.IsNullOrWhiteSpace(profile?.Province) ||
                        string.IsNullOrWhiteSpace(profile?.City))
                    {
                        _logger.LogWarning("User {UserId} has incomplete profile for job application", userId);
                        return BadRequest(new { Success = false, ErrorMessage = "Please update your personal information (full name, job title, phone, date of birth, province, city) before applying." });
                    }

                    // Process CV
                    var (cv, uploadedCvUrl, cvData, cvSummary, error) = await ProcessCvAsync(request, userId, cloudinaryService, context);
                    if (cv == null)
                    {
                        _logger.LogError("Failed to process CV for User {UserId}: {Error}", userId, error);
                        return BadRequest(new { Success = false, ErrorMessage = error });
                    }

                    // Check job existence and validity
                    var job = await context.Jobs.FindAsync(request.JobId);
                    if (job == null)
                    {
                        _logger.LogWarning("Job {JobId} not found for User {UserId}", request.JobId, userId);
                        return NotFound(new { Success = false, ErrorMessage = "Job not found" });
                    }

                    if (job.Status != Job.JobStatus.active || job.DeactivatedByAdmin)
                    {
                        _logger.LogWarning("Job {JobId} is not active or deactivated for User {UserId}", request.JobId, userId);
                        return BadRequest(new { Success = false, ErrorMessage = "Cannot apply to an inactive or deactivated job." });
                    }

                    // Save application
                    var application = await SaveApplicationAsync(userId, request, cv, uploadedCvUrl, context);
                    _logger.LogInformation("Application submitted successfully for User {UserId}, Job {JobId}", userId, request.JobId);

                    // Summarize job
                    string jobSummary = await SummarizeJobAsync(job, context);

                    // Calculate similarity scores
                    var matchingResult = await _semanticMatchingService.CalculateTotalSimilarity(job, cv, cvSummary, jobSummary);
                    if (matchingResult.Success)
                    {
                        application.SimilarityScore = matchingResult.FinalSimilarity;
                        application.SimilarityDescription = matchingResult.SimilarityDescription;
                        application.SimilaritySkills = matchingResult.SimilaritySkills;
                        application.SimilarityExperience = matchingResult.SimilarityExperience;
                        application.SimilarityEducation = matchingResult.SimilarityEducation;
                        await context.SaveChangesAsync();
                        _logger.LogInformation("Saved similarity scores for Application {ApplicationId}: Total={Total:F2}, Description={Desc:F2}, Skills={Skills:F2}, Experience={Exp:F2}, Education={Edu:F2}",
                            application.ApplicationId, matchingResult.FinalSimilarity, matchingResult.SimilarityDescription, matchingResult.SimilaritySkills, matchingResult.SimilarityExperience, matchingResult.SimilarityEducation);
                    }
                    else
                    {
                        _logger.LogWarning("Similarity calculation failed for Application {ApplicationId}: {Error}", application.ApplicationId, matchingResult.ErrorMessage);
                    }

                    return Ok(new
                    {
                        Success = true,
                        Message = "Application submitted successfully",
                        ApplicationId = application.ApplicationId,
                        SimilarityScore = matchingResult.Success ? matchingResult.FinalSimilarity : (float?)null,
                        SimilarityDescription = matchingResult.Success ? matchingResult.SimilarityDescription : (float?)null,
                        SimilaritySkills = matchingResult.Success ? matchingResult.SimilaritySkills : (float?)null,
                        SimilarityExperience = matchingResult.Success ? matchingResult.SimilarityExperience : (float?)null,
                        SimilarityEducation = matchingResult.Success ? matchingResult.SimilarityEducation : (float?)null,
                        CvSummary = cvSummary,
                        JobSummary = jobSummary,
                        GeminiReasoning = matchingResult.Success ? matchingResult.GeminiReasoning : null
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing job application for User {UserId}, Job {JobId}", userId, request.JobId);
                    return StatusCode(500, new { Success = false, ErrorMessage = "An error occurred while processing your application." });
                }
            }
        }

        private async Task<(CV Cv, string UploadedCvUrl, CVData CvData, string CvSummary, string Error)> ProcessCvAsync(
            ApplyJobRequest request, int userId, CloudinaryService cloudinaryService, JobFinderDbContext context)
        {
            CV cv = null;
            string uploadedCvUrl = null;
            CVData cvData = new CVData();
            string cvSummary = string.Empty;
            string error = string.Empty;

            var jsonOptions = new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

            if (request.CvId.HasValue)
            {
                cv = await context.CVs.FirstOrDefaultAsync(c => c.CVId == request.CvId && c.UserId == userId);
                if (cv == null)
                {
                    return (null, null, null, null, "Selected CV not found");
                }
                uploadedCvUrl = cv.FileUrl;

                var (success, extractError, extractedCvData, extractedSummary) = await _semanticMatchingService.ExtractCvDataAsync(cv);
                if (!success)
                {
                    return (null, null, null, null, extractError);
                }
                cvData = extractedCvData;
                cvSummary = extractedSummary;
            }
            else if (request.CvFile != null && request.CvFile.Length > 0)
            {
                uploadedCvUrl = await cloudinaryService.UploadCvAsync(request.CvFile);
                if (string.IsNullOrEmpty(uploadedCvUrl))
                {
                    return (null, null, null, null, "Unable to upload CV to Cloudinary");
                }

                string extractedText = string.Empty;
                try
                {
                    using (var stream = request.CvFile.OpenReadStream())
                    using (var pdfDocument = PdfDocument.Open(stream))
                    {
                        foreach (var page in pdfDocument.GetPages())
                        {
                            extractedText += page.Text + "\n";
                        }
                    }
                    extractedText = CleanExtractedText(extractedText);

                    if (string.IsNullOrWhiteSpace(extractedText))
                    {
                        return (null, null, null, null, "CV content is empty or unreadable");
                    }

                    var (success, extractError, extractedCvData, extractedSummary) = await _semanticMatchingService.ExtractCvDataAsync(null, extractedText);
                    if (!success)
                    {
                        return (null, null, null, null, extractError);
                    }

                    cvData = extractedCvData;
                    cvSummary = extractedSummary;

                    cv = new CV
                    {
                        UserId = userId,
                        FileUrl = uploadedCvUrl,
                        FullCvJson = JsonSerializer.Serialize(new
                        {
                            Text = extractedText,
                            TranslatedText = string.Empty,
                            Summary = cvSummary,
                            CVData = cvData
                        }, jsonOptions),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Type = CvType.Apply
                    };
                    context.CVs.Add(cv);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return (null, null, null, null, $"PDF extraction failed: {ex.Message}");
                }
            }
            else
            {
                return (null, null, null, null, "No CV selected or uploaded");
            }

            return (cv, uploadedCvUrl, cvData, cvSummary, error);
        }

        private async Task<string> SummarizeJobAsync(Job job, JobFinderDbContext context)
        {
            try
            {
                string jobText = $"{job.Description}\n{job.YourSkill}\n{job.YourExperience}\n{job.Education}";
                var (success, summaryText, _) = await _semanticMatchingService.SummarizeAndTranslate(jobText, "en");
                return success ? summaryText : "Job summary temporarily unavailable";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Job summarization failed for Job {JobId}", job.JobId);
                return "Job summary temporarily unavailable";
            }
        }

        private async Task<Application> SaveApplicationAsync(int userId, ApplyJobRequest request, CV cv, string resumeUrl, JobFinderDbContext context)
        {
            var application = new Application
            {
                UserId = userId,
                JobId = request.JobId,
                CvId = cv.CVId,
                CoverLetter = request.CoverLetter,
                ResumeUrl = resumeUrl,
                Status = ApplicationStatus.Pending,
                SubmittedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Applications.Add(application);
            await context.SaveChangesAsync();
            return application;
        }

        private string CleanExtractedText(string text)
        {
            text = Regex.Replace(text, @"(Page|Trang)\s*\d+\s*(of|\/|-)\s*\d+\s*(-?\s*©.*)?", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"©\s*[^\n]+", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\n{2,}", "\n");
            text = Regex.Replace(text, @"\s{2,}", " ");
            text = Regex.Replace(text, @"[\t\r]+", " ");
            return text.Trim();
        }

        [Authorize]
        [HttpGet("my-applications")]
        public async Task<IActionResult> GetMyApplications()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID.");
            _logger.LogInformation("Fetching applications for User {UserId}", userId);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                var applications = await context.Applications
                    .Where(a => a.UserId == userId)
                    .Include(a => a.Job)
                    .Select(a => new
                    {
                        ApplicationId = a.ApplicationId,
                        a.Status,
                        a.SubmittedAt,
                        a.CoverLetter,
                        a.ResumeUrl,
                        a.SimilarityScore,
                        a.SimilarityDescription,
                        a.SimilaritySkills,
                        a.SimilarityExperience,
                        a.SimilarityEducation,
                        Job = new
                        {
                            a.Job.JobId,
                            a.Job.Title,
                            a.Job.Description,
                            a.Job.CompanyId,
                            a.Job.MinSalary,
                            a.Job.MaxSalary,
                            a.Job.IsSalaryNegotiable,
                            a.Job.IndustryId,
                            a.Job.ExpiryDate,
                            a.Job.LevelId,
                            a.Job.JobTypeId,
                            a.Job.ExperienceLevelId,
                            a.Job.TimeStart,
                            a.Job.TimeEnd,
                            a.Job.Status,
                            a.Job.ProvinceName,
                            a.Job.AddressDetail
                        }
                    })
                    .ToListAsync();

                return Ok(applications);
            }
        }

        [Authorize]
        [HttpGet("my-applied-jobs-with-cvs")]
        public async Task<IActionResult> GetMyAppliedJobsWithCvs()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID.");
            _logger.LogInformation("Fetching applied jobs with CVs for User {UserId}", userId);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                var appliedJobIds = await context.Applications
                    .Where(a => a.UserId == userId)
                    .Select(a => a.JobId)
                    .Distinct()
                    .ToListAsync();

                var jobs = await context.Jobs
                    .Where(j => appliedJobIds.Contains(j.JobId))
                    .Select(j => new
                    {
                        j.JobId,
                        j.Title,
                        j.Description,
                        j.CompanyId,
                        j.MinSalary,
                        j.MaxSalary,
                        j.IsSalaryNegotiable,
                        j.IndustryId,
                        j.ExpiryDate,
                        j.LevelId,
                        j.JobTypeId,
                        j.ExperienceLevelId,
                        j.TimeStart,
                        j.TimeEnd,
                        j.Status,
                        j.ProvinceName,
                        j.AddressDetail,
                        AppliedCvs = j.Applications.Select(a => new
                        {
                            a.ApplicationId,
                            a.UserId,
                            a.CvId,
                            a.Status,
                            a.SubmittedAt,
                            a.CoverLetter,
                            a.ResumeUrl,
                            a.SimilarityScore,
                            a.SimilarityDescription,
                            a.SimilaritySkills,
                            a.SimilarityExperience,
                            a.SimilarityEducation,
                            CvInfo = new
                            {
                                a.CV.CVId,
                                a.CV.FileUrl,
                                a.CV.CreatedAt,
                                a.CV.UpdatedAt
                            }
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(jobs);
            }
        }

        [Authorize]
        [HttpPost("favorite-company/{userId}")]
        public async Task<IActionResult> FavoriteCompany(int userId)
        {
            var candidateIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(candidateIdStr, out var candidateId))
                return Unauthorized("Invalid candidate ID.");

            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();

            // Find CompanyProfile by UserId and ensure User.RoleId = 2 (Company)
            var companyProfile = await context.CompanyProfile
                .Include(cp => cp.User)
                .FirstOrDefaultAsync(cp => cp.UserId == userId && cp.User.RoleId == 2);

            if (companyProfile == null)
                return BadRequest("Company profile not found or user is not a company.");

            // Check if the company is already favorited
            if (await context.UserFavoriteCompanies.AnyAsync(f => f.UserId == candidateId && f.CompanyProfileId == companyProfile.CompanyProfileId))
                return BadRequest("Company already favorited.");

            // Add to favorites using the actual CompanyProfileId
            var favorite = new UserFavoriteCompany
            {
                UserId = candidateId,
                CompanyProfileId = companyProfile.CompanyProfileId, // Use CompanyProfileId, not userId
                CreatedAt = DateTime.UtcNow
            };
            context.UserFavoriteCompanies.Add(favorite);
            await context.SaveChangesAsync();

            _logger.LogInformation("CompanyProfile {CompanyProfileId} favorited by User {UserId}", companyProfile.CompanyProfileId, candidateId);
            return Ok("Company added to favorites successfully.");
        }

        [Authorize]
        [HttpGet("my-favorite-companies")]
        public async Task<IActionResult> GetMyFavoriteCompanies()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID");

            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();

            var companies = await context.UserFavoriteCompanies
                .Where(f => f.UserId == userId)
                .Include(f => f.CompanyProfile)
                    .ThenInclude(cp => cp.User)
                .Include(f => f.CompanyProfile.Industry)
                .Select(f => new
                {
                    f.CompanyProfile.UserId,
                    f.CompanyProfile.CompanyProfileId,
                    f.CompanyProfile.CompanyName,
                    f.CompanyProfile.CompanyProfileDescription,
                    f.CompanyProfile.Location,
                    f.CompanyProfile.UrlCompanyLogo,
                    f.CompanyProfile.ImageLogoLgr,
                    f.CompanyProfile.TeamSize,
                    IndustryName = f.CompanyProfile.Industry.IndustryName,
                    f.CompanyProfile.Website,
                    f.CompanyProfile.Contact
                })
                .ToListAsync();

            return Ok(companies);
        }


        [Authorize]
        [HttpDelete("favorite-company/{userId}")]
        public async Task<IActionResult> UnfavoriteCompany(int userId)
        {
            var candidateIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(candidateIdStr, out var candidateId))
                return Unauthorized("Invalid candidate ID.");

            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();

            var companyProfile = await context.CompanyProfile
                .Include(cp => cp.User)
                .FirstOrDefaultAsync(cp => cp.UserId == userId && cp.User.RoleId == 2);

            if (companyProfile == null)
                return NotFound("Company profile not found or user is not a company.");

         
            var favorite = await context.UserFavoriteCompanies
                .FirstOrDefaultAsync(f => f.UserId == candidateId && f.CompanyProfileId == companyProfile.CompanyProfileId);

            if (favorite == null)
                return NotFound("Company not found in favorites");

            context.UserFavoriteCompanies.Remove(favorite);
            await context.SaveChangesAsync();

            _logger.LogInformation("CompanyProfile {CompanyProfileId} removed from favorites by User {UserId}", companyProfile.CompanyProfileId, candidateId);
            return Ok("Company removed from favorites successfully");
        }

        [HttpGet("job/{jobId}")]
        public async Task<IActionResult> GetApplicationsByJob(int jobId)
        {
            _logger.LogInformation("Fetching applications for Job {JobId}", jobId);
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                var applications = await context.Applications
                    .Where(a => a.JobId == jobId)
                    .Include(a => a.User)
                    .Include(a => a.Job)
                    .Select(a => new
                    {
                        ApplicationId = a.ApplicationId,
                        a.UserId,
                        a.JobId,
                        a.Status,
                        a.SubmittedAt,
                        a.CoverLetter,
                        a.ResumeUrl,
                        a.SimilarityScore,
                        a.SimilarityDescription,
                        a.SimilaritySkills,
                        a.SimilarityExperience,
                        a.SimilarityEducation,
                        User = new
                        {
                            a.User.UserId,
                            a.User.FullName
                        },
                        Job = new
                        {
                            a.Job.JobId,
                            a.Job.Title,
                            a.Job.Description
                        }
                    })
                    .ToListAsync();

                return Ok(applications);
            }
        }

        [HttpGet("auth/callback")]
        public async Task<IActionResult> OAuthCallback(string code, string state, [FromServices] IOptions<GeminiConfig> geminiConfig)
        {
            if (string.IsNullOrEmpty(code))
            {
                _logger.LogWarning("Invalid authorization code received");
                return BadRequest("Invalid authorization code.");
            }

            var client = new HttpClient();
            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", geminiConfig.Value.ClientId),
                new KeyValuePair<string, string>("client_secret", geminiConfig.Value.ClientSecret),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", geminiConfig.Value.RedirectUri ?? "http://localhost:5194/auth/callback"),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
            });

            var response = await client.PostAsync("https://oauth2.googleapis.com/token", requestContent);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var tokenData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse);
                var accessToken = tokenData["access_token"];
                var refreshToken = tokenData["refresh_token"];

                HttpContext.Session.SetString("AccessToken", accessToken);
                HttpContext.Session.SetString("RefreshToken", refreshToken);
                _logger.LogInformation("OAuth callback successful, tokens stored in session");

                return Redirect("http://localhost:5194/success");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Token retrieval error: {ErrorContent}", errorContent);
                return StatusCode(500, "Unable to retrieve token from Google.");
            }
        }

        [HttpGet("jobs-applied-by-user-in-company")]
        public async Task<IActionResult> GetJobsAppliedByUserInCompany(int userId, int companyId)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                var jobIds = await context.Jobs
                    .Where(j => j.CompanyId == companyId)
                    .Select(j => j.JobId)
                    .ToListAsync();

                var jobs = await context.Applications
                    .Where(a => a.UserId == userId && jobIds.Contains(a.JobId))
                    .Include(a => a.Job)
                    .Select(a => new
                    {
                        a.Job.JobId,
                        a.Job.Title,
                        a.Job.Description,
                        a.Status,
                        a.SubmittedAt
                    })
                    .Distinct()
                    .ToListAsync();

                return Ok(jobs);
            }
        }

        [HttpGet("company/{companyId}/unique-candidates")]
        public async Task<IActionResult> GetUniqueCandidatesByCompany(int companyId)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                var jobIds = await context.Jobs
                    .Where(j => j.CompanyId == companyId)
                    .Select(j => j.JobId)
                    .ToListAsync();

                var candidateIds = await context.Applications
                    .Where(a => jobIds.Contains(a.JobId))
                    .Select(a => a.UserId)
                    .Distinct()
                    .ToListAsync();

                return Ok(new { count = candidateIds.Count });
            }
        }

        [HttpGet("company/{companyId}/recent-applicants")]
        public async Task<IActionResult> GetRecentApplicantsByCompany(int companyId, int take = 10)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                var jobIds = await context.Jobs
                    .Where(j => j.CompanyId == companyId)
                    .Select(j => j.JobId)
                    .ToListAsync();

                var applications = await context.Applications
                    .Where(a => jobIds.Contains(a.JobId))
                    .OrderByDescending(a => a.SubmittedAt)
                    .Take(take)
                    .Include(a => a.User)
                        .ThenInclude(u => u.CandidateProfile)
                    .Include(a => a.Job)
                    .ToListAsync();

                var result = applications.Select(a => new
                {
                    ApplicationId = a.ApplicationId,
                    UserId = a.UserId,
                    FullName = a.User?.FullName ?? "N/A",
                    Gender = a.User?.CandidateProfile?.Gender ?? "N/A",
                    Address = a.User?.CandidateProfile?.Address ?? "N/A",
                    SubmittedAt = a.SubmittedAt,
                    JobId = a.JobId,
                    JobTitle = a.Job?.Title ?? "N/A"
                });

                return Ok(result);
            }
        }

        [HttpGet("distinct-job-count-by-user-in-company")]
        public async Task<IActionResult> GetDistinctJobCountByUserInCompany(int userId, int companyId)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                var jobIds = await context.Jobs
                    .Where(j => j.CompanyId == companyId)
                    .Select(j => j.JobId)
                    .ToListAsync();

                var count = await context.Applications
                    .Where(a => a.UserId == userId && jobIds.Contains(a.JobId))
                    .Select(a => a.JobId)
                    .Distinct()
                    .CountAsync();

                return Ok(new { userId, companyId, distinctJobCount = count });
            }
        }

        [Authorize]
        [HttpPost("try-match")]
        public async Task<IActionResult> TryMatch(
            [FromForm] TryMatchRequest request,
            [FromServices] ICvSnapshotService cvSnapshotService,
            [FromServices] CloudinaryService cloudinaryService,
            [FromServices] IServiceScopeFactory serviceScopeFactory)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID.");
            _logger.LogInformation("User {UserId} started trying match for Job {JobId}", userId, request.JobId);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                var (cv, uploadedCvUrl, cvData, cvSummary, error) = await ProcessCvForTryMatchAsync(request, userId, cloudinaryService, context);
                if (cv == null)
                {
                    _logger.LogError("Failed to process CV for User {UserId}: {Error}", userId, error);
                    return BadRequest(new { Success = false, ErrorMessage = error });
                }

                var job = await context.Jobs.FindAsync(request.JobId);
                if (job == null)
                {
                    _logger.LogWarning("Job {JobId} not found for User {UserId}", request.JobId, userId);
                    return NotFound(new { Success = false, ErrorMessage = "Job not found" });
                }

                string jobSummary = await SummarizeJobAsync(job, context);

                var matchingResult = await _semanticMatchingService.CalculateTotalSimilarity(job, cv, cvSummary, jobSummary);
                if (!matchingResult.Success)
                {
                    return BadRequest(new { Success = false, ErrorMessage = matchingResult.ErrorMessage });
                }

                var suggestions = GenerateImprovementSuggestions(matchingResult, cvData, job);

                using (var innerScope = _serviceScopeFactory.CreateScope())
                {
                    var innerContext = innerScope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                    var tryMatchRecord = new TryMatchRecord
                    {
                        UserId = userId,
                        JobId = request.JobId,
                        CvId = cv.CVId,
                        SimilarityScore = matchingResult.FinalSimilarity,
                        Suggestions = JsonSerializer.Serialize(suggestions),
                        CreatedAt = DateTime.UtcNow,
                        CvSummary = cvSummary,
                        JobSummary = jobSummary
                    };
                    innerContext.TryMatchRecords.Add(tryMatchRecord);
                    await innerContext.SaveChangesAsync();
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Match attempt successful",
                    SimilarityScore = matchingResult.FinalSimilarity,
                    SimilarityDescription = matchingResult.SimilarityDescription,
                    SimilaritySkills = matchingResult.SimilaritySkills,
                    SimilarityExperience = matchingResult.SimilarityExperience,
                    SimilarityEducation = matchingResult.SimilarityEducation,
                    CvSummary = cvSummary,
                    JobSummary = jobSummary,
                    Suggestions = suggestions
                });
            }
        }

        [Authorize]
        [HttpGet("my-try-match-history")]
        public async Task<IActionResult> MyTryMatchHistory()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID.");
            _logger.LogInformation("Fetching try match history for User {UserId} at {DateTime}", userId, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                var tryMatchRecords = await context.TryMatchRecords
                    .Where(r => r.UserId == userId)
                    .Include(r => r.Job)
                    .Include(r => r.CV)
                    .Select(r => new
                    {
                        TryMatchId = r.TryMatchId,
                        JobId = r.JobId,
                        JobTitle = r.Job.Title,
                        CvId = r.CvId,
                        CvFileUrl = r.CV.FileUrl,
                        SimilarityScore = r.SimilarityScore,
                        Suggestions = r.Suggestions,
                        CreatedAt = r.CreatedAt,
                        CvSummary = r.CvSummary,
                        JobSummary = r.JobSummary
                    })
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                return Ok(tryMatchRecords);
            }
        }

        private async Task<(CV Cv, string UploadedCvUrl, CVData CvData, string CvSummary, string Error)> ProcessCvForTryMatchAsync(
            TryMatchRequest request, int userId, CloudinaryService cloudinaryService, JobFinderDbContext context)
        {
            CV cv = null;
            string uploadedCvUrl = null;
            CVData cvData = new CVData();
            string cvSummary = string.Empty;
            string error = string.Empty;

            var jsonOptions = new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

            if (request.CvFile != null && request.CvFile.Length > 0)
            {
                uploadedCvUrl = await cloudinaryService.UploadCvAsync(request.CvFile);
                if (string.IsNullOrEmpty(uploadedCvUrl))
                {
                    return (null, null, null, null, "Unable to upload CV to Cloudinary");
                }

                string extractedText = string.Empty;
                try
                {
                    using (var stream = request.CvFile.OpenReadStream())
                    using (var pdfDocument = PdfDocument.Open(stream))
                    {
                        foreach (var page in pdfDocument.GetPages())
                        {
                            extractedText += page.Text + "\n";
                        }
                    }
                    extractedText = CleanExtractedText(extractedText);

                    if (string.IsNullOrWhiteSpace(extractedText))
                    {
                        return (null, null, null, null, "CV content is empty or unreadable");
                    }

                    var (success, extractError, extractedCvData, extractedSummary) = await _semanticMatchingService.ExtractCvDataAsync(null, extractedText);
                    if (!success)
                    {
                        return (null, null, null, null, extractError);
                    }

                    cvData = extractedCvData;
                    cvSummary = extractedSummary;

                    cv = new CV
                    {
                        UserId = userId,
                        FileUrl = uploadedCvUrl,
                        FullCvJson = JsonSerializer.Serialize(new
                        {
                            Text = extractedText,
                            TranslatedText = string.Empty,
                            Summary = cvSummary,
                            CVData = cvData
                        }, jsonOptions),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Type = CvType.Apply
                    };
                    context.CVs.Add(cv);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return (null, null, null, null, $"PDF extraction failed: {ex.Message}");
                }
            }
            else if (request.CvId.HasValue)
            {
                cv = await context.CVs.FindAsync(request.CvId.Value);
                if (cv == null || cv.UserId != userId)
                {
                    return (null, null, null, null, "CV not found or does not belong to the user");
                }
                uploadedCvUrl = cv.FileUrl;

                var (success, extractError, extractedCvData, extractedSummary) = await _semanticMatchingService.ExtractCvDataAsync(cv);
                if (!success)
                {
                    return (null, null, null, null, extractError);
                }

                cvData = extractedCvData;
                cvSummary = extractedSummary;
            }
            else
            {
                cv = await context.CVs.FirstOrDefaultAsync(c => c.UserId == userId);
                if (cv == null)
                {
                    return (null, null, null, null, "No CV found and no file uploaded");
                }
                uploadedCvUrl = cv.FileUrl;

                var (success, extractError, extractedCvData, extractedSummary) = await _semanticMatchingService.ExtractCvDataAsync(cv);
                if (!success)
                {
                    return (null, null, null, null, extractError);
                }

                cvData = extractedCvData;
                cvSummary = extractedSummary;
            }

            return (cv, uploadedCvUrl, cvData, cvSummary, error);
        }

        private List<string> GenerateImprovementSuggestions((bool Success, string ErrorMessage, float FinalSimilarity, float SimilarityDescription, float SimilaritySkills, float SimilarityExperience, float SimilarityEducation, string GeminiReasoning) matchingResult, CVData cvData, Job job)
        {
            var suggestions = new List<string>();

            if (matchingResult.SimilarityDescription < 0.3)
                suggestions.Add("Improve your CV description to better align with the job requirements. Focus on including key responsibilities mentioned in the job description.");

            if (matchingResult.SimilaritySkills < 0.3)
                suggestions.Add($"Add more skills relevant to {job.YourSkill ?? "job requirements"}. Consider including {string.Join(", ", job.YourSkill?.Split(',').Take(3) ?? new[] { "technical skills" })}.");

            if (matchingResult.SimilarityExperience < 0.3)
                suggestions.Add("Expand your experience section with detailed roles and years that match the job's experience level (e.g., Fresher, Intern).");

            if (matchingResult.SimilarityEducation < 0.3)
                suggestions.Add("Highlight relevant degrees or certifications in IT (e.g., Java, RESTful API) that align with the job's education requirements.");

            if (matchingResult.FinalSimilarity < 0.5)
                suggestions.Add("Overall, your CV has low compatibility. Tailor it closer to the job by addressing the above points and seeking additional training if needed.");

            return suggestions.Any() ? suggestions : new List<string> { "Your CV is well-aligned with the job. No major improvements suggested!" };
        }
    }
}