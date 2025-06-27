using CloudinaryDotNet;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.Requests;
using JOB_FINDER_API.Models.Services;
using JOB_FINDER_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly JobFinderDbContext _context;
        private readonly SemanticMatchingService _semanticMatchingService;
        private readonly ILogger<ApplicationController> _logger;
        private readonly IConfiguration _configuration;

        public ApplicationController(JobFinderDbContext context, IHttpClientFactory httpClientFactory,
            SemanticMatchingService semanticMatchingService, ILogger<ApplicationController> logger,
            IConfiguration configuration)
        {
            _context = context;
            _semanticMatchingService = semanticMatchingService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.Applications.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _context.Applications.FindAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Application model)
        {
            if (id != model.Id) return BadRequest();
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Applications.FindAsync(id);
            if (item == null) return NotFound();
            _context.Applications.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpPost("apply")]
        public async Task<IActionResult> Apply(
            [FromForm] ApplyJobRequest request,
            [FromServices] ICvSnapshotService cvSnapshotService,
            [FromServices] CloudinaryService cloudinaryService,
            [FromServices] EmailService emailService)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID.");
            _logger.LogInformation("User {UserId} started applying for Job {JobId}", userId, request.JobId);

            // Xử lý CV
            var (cv, uploadedCvUrl, cvData, cvSummary, error) = await ProcessCvAsync(request, userId, cloudinaryService);
            if (cv == null)
            {
                _logger.LogError("Failed to process CV for User {UserId}: {Error}", userId, error);
                return BadRequest(new { Success = false, ErrorMessage = error });
            }

            // Lấy Job và tóm tắt
            var job = await _context.Jobs.FindAsync(request.JobId);
            if (job == null)
            {
                _logger.LogWarning("Job {JobId} not found for User {UserId}", request.JobId, userId);
                return NotFound(new { Success = false, ErrorMessage = "Job not found" });
            }

            string jobSummary = await SummarizeJobAsync(job);

            // Lưu application
            var application = await SaveApplicationAsync(userId, request, cv, uploadedCvUrl);
            _logger.LogInformation("Application submitted successfully for User {UserId}, Job {JobId}", userId, request.JobId);

            // Tính similarity
            float similarityThreshold = _configuration.GetValue<float>("Matching:SimilarityThreshold", 0.7f);
            var matchingResult = await _semanticMatchingService.CalculateTotalSimilarity(job, cv, cvSummary, jobSummary);
            if (matchingResult.Success)
            {
                application.SimilarityScore = matchingResult.TotalSimilarity;
                await _context.SaveChangesAsync();

                if (matchingResult.TotalSimilarity > similarityThreshold)
                {
                    var company = await _context.Users.FindAsync(job.CompanyId);
                    if (company != null && !string.IsNullOrEmpty(company.Email))
                    {
                        string mailBody = $@"<div style='font-family: Arial, sans-serif;'>
                            <h2>New application for {job.Title}</h2>
                            <p>Applicant: {userId}</p>
                            <p>Similarity score: {matchingResult.TotalSimilarity:F2}</p>
                            <p><a href='http://localhost:3000/application/{application.Id}'>View application</a></p>
                        </div>";
                        emailService.SendEmail(company.Email, "New Job Application", mailBody, true);
                        _logger.LogInformation("Email sent to company {CompanyEmail} for Application {ApplicationId}", company.Email, application.Id);
                    }
                }
            }

            return Ok(new
            {
                Success = true,
                Message = "Application submitted successfully",
                ApplicationId = application.Id,
                SimilarityScore = matchingResult.Success ? matchingResult.TotalSimilarity : (float?)null,
                CvSummary = cvSummary,
                JobSummary = jobSummary,
                ITRelevance = cvData.ITRelevance
            });
        }

        private async Task<(CV Cv, string UploadedCvUrl, CVData CvData, string CvSummary, string Error)> ProcessCvAsync(
            ApplyJobRequest request, int userId, CloudinaryService cloudinaryService)
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
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.CVs.Add(cv);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return (null, null, null, null, $"PDF extraction failed: {ex.Message}");
                }
            }
            else
            {
                cv = await _context.CVs.FirstOrDefaultAsync(c => c.UserId == userId);
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

                cv.FullCvJson = JsonSerializer.Serialize(new
                {
                    Text = JsonSerializer.Deserialize<Dictionary<string, object>>(cv.FullCvJson ?? "{}")?.GetValueOrDefault("Text")?.ToString() ?? string.Empty,
                    TranslatedText = string.Empty,
                    Summary = cvSummary,
                    CVData = cvData
                }, jsonOptions);
                await _context.SaveChangesAsync();
            }

            return (cv, uploadedCvUrl, cvData, cvSummary, error);
        }

        private async Task<string> SummarizeJobAsync(Job job)
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

        private async Task<Application> SaveApplicationAsync(int userId, ApplyJobRequest request, CV cv, string resumeUrl)
        {
            var application = new Application
            {
                UserId = userId,
                JobId = request.JobId,
                CvId = cv.Id,
                CoverLetter = request.CoverLetter,
                ResumeUrl = resumeUrl,
                Status = ApplicationStatus.Pending,
                SubmittedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
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

            var applications = await _context.Applications
                .Where(a => a.UserId == userId)
                .Include(a => a.Job)
                .Select(a => new
                {
                    ApplicationId = a.Id,
                    a.Status,
                    a.SubmittedAt,
                    a.CoverLetter,
                    a.ResumeUrl,
                    a.SimilarityScore,
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

        [Authorize]
        [HttpGet("my-applied-jobs-with-cvs")]
        public async Task<IActionResult> GetMyAppliedJobsWithCvs()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID.");
            _logger.LogInformation("Fetching applied jobs with CVs for User {UserId}", userId);

            var appliedJobIds = await _context.Applications
                .Where(a => a.UserId == userId)
                .Select(a => a.JobId)
                .Distinct()
                .ToListAsync();

            var jobs = await _context.Jobs
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
                        a.Id,
                        a.UserId,
                        a.CvId,
                        a.Status,
                        a.SubmittedAt,
                        a.CoverLetter,
                        a.ResumeUrl,
                        a.SimilarityScore,
                        CvInfo = new
                        {
                            a.CV.Id,
                            a.CV.FileUrl,
                            a.CV.CreatedAt,
                            a.CV.UpdatedAt
                        }
                    }).ToList()
                })
                .ToListAsync();

            return Ok(jobs);
        }

        [Authorize]
        [HttpPost("favorite-company/{companyId}")]
        public async Task<IActionResult> FavoriteCompany(int companyId)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID.");
            if (await _context.UserFavoriteCompanies.AnyAsync(f => f.UserId == userId && f.CompanyId == companyId))
                return BadRequest("Company is already favorited");

            var favorite = new UserFavoriteCompany
            {
                UserId = userId,
                CompanyId = companyId,
                CreatedAt = DateTime.UtcNow
            };
            _context.UserFavoriteCompanies.Add(favorite);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Company {CompanyId} favorited by User {UserId}", companyId, userId);
            return Ok("Company added to favorites successfully");
        }



        [Authorize]
        [HttpGet("my-favorite-companies")]
        public async Task<IActionResult> GetMyFavoriteCompanies()
        {
            var userIdStr = User.Identity?.Name;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID");

            _logger.LogInformation("Fetching favorite companies for User {UserId}", userId);
            var companies = await _context.UserFavoriteCompanies
                .Where(f => f.UserId == userId)
                .Join(
                    _context.CompanyProfile,
                    fav => fav.CompanyId,
                    cp => cp.UserId,
                    (fav, cp) => new
                    {
                        cp.UserId,
                        cp.CompanyName,
                        cp.CompanyProfileDescription,
                        cp.Location,
                        cp.UrlCompanyLogo,
                        cp.ImageLogoLgr,
                        cp.TeamSize,
                        cp.Industry,
                        cp.Website,
                        cp.Contact
                    }
                )
                .ToListAsync();

            return Ok(companies);
        }

        [Authorize]
        [HttpDelete("favorite-company/{companyId}")]
        public async Task<IActionResult> UnfavoriteCompany(int companyId)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID.");
            var favorite = await _context.UserFavoriteCompanies
                .FirstOrDefaultAsync(f => f.UserId == userId && f.CompanyId == companyId);
            if (favorite == null)
                return NotFound("Company not found in favorites");

            _context.UserFavoriteCompanies.Remove(favorite);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Company {CompanyId} removed from favorites by User {UserId}", companyId, userId);
            return Ok("Company removed from favorites successfully");
        }





        [HttpGet("job/{jobId}")]
        public async Task<IActionResult> GetApplicationsByJob(int jobId)
        {
            _logger.LogInformation("Fetching applications for Job {JobId}", jobId);
            var applications = await _context.Applications
                .Where(a => a.JobId == jobId)
                .Include(a => a.User)
                .Include(a => a.Job)
                .Select(a => new
                {
                    ApplicationId = a.Id,
                    a.UserId,
                    a.JobId,
                    a.Status,
                    a.SubmittedAt,
                    a.CoverLetter,
                    a.ResumeUrl,
                    a.SimilarityScore,
                    User = new
                    {
                        a.User.Id,
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
    }
}
