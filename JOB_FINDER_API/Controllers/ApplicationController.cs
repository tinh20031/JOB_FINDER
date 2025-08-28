using Aspose.Words;
using Aspose.Words.Drawing;
using CloudinaryDotNet;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.Background;
using JOB_FINDER_API.Models.Requests;
using JOB_FINDER_API.Models.Services;
using JOB_FINDER_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.IO.Compression;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;

using UglyToad.PdfPig;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private readonly HttpClient _httpClient;
        private readonly NotificationService _notificationService;
        private readonly EmailService _emailService;

        public ApplicationController(
            IServiceScopeFactory serviceScopeFactory,
            SemanticMatchingService semanticMatchingService,
            ILogger<ApplicationController> logger,
            IConfiguration configuration,
            HttpClient httpClient,
             NotificationService notificationService,
             EmailService emailService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _semanticMatchingService = semanticMatchingService;
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
            _notificationService = notificationService;
            _emailService = emailService;
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
           [FromServices] CloudinaryService cloudinaryService,
           [FromServices] IBackgroundTaskQueue taskQueue)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID.");

            var role = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();
            if (role != "candidate")
                return Forbid("Only candidates can apply for jobs.");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                string tempCvPath = null;
                try
                {

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
                        return BadRequest(new { Success = false, Message = "Please update your personal information before applying." });
                    }


                    /*var job = await context.Jobs.FindAsync(request.JobId);
                    if (job == null || job.Status != Job.JobStatus.active || job.DeactivatedByAdmin || job.IsExpired())
                        return BadRequest(new { Success = false, Message = "The job posting has expired" });

                    // Đếm số lượng đơn Pending của user với các job thuộc company này
                    var pendingCount = await context.Applications
                        .Where(a => a.UserId == userId
                            && a.Status == ApplicationStatus.Pending
                            && context.Jobs.Any(j => j.JobId == a.JobId && j.CompanyId == job.CompanyId))
                        .CountAsync();

                    if (pendingCount >= 3)
                    {
                        return BadRequest(new
                        {
                            Success = false,
                            Message = "You are only allowed to apply for a maximum of 3 pending positions at the same company. Please wait for the results before applying for more positions."
                        });
                    }

                    //  BẮT ĐẦU: Kiểm tra và cập nhật đơn apply cũ nếu có 
                    var existingApplication = await context.Applications
                        .FirstOrDefaultAsync(a => a.JobId == request.JobId && a.UserId == userId && a.Status == ApplicationStatus.Pending);

                    bool isUpdate = false;
                    Application application;
                    if (existingApplication != null)
                    {
                        // Nếu đã apply, cập nhật đơn cũ
                        application = existingApplication;
                        application.CoverLetter = request.CoverLetter;
                        application.Status = ApplicationStatus.Pending;
                        application.UpdatedAt = DateTime.UtcNow;
                        application.SubmittedAt = DateTime.UtcNow;
                        isUpdate = true;
                    }
                    else
                    {
                        // Nếu chưa apply, tạo mới
                        application = new Application
                        {
                            UserId = userId,
                            JobId = request.JobId,
                            CoverLetter = request.CoverLetter,
                            Status = ApplicationStatus.Pending,
                            SubmittedAt = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        context.Applications.Add(application);
                    }*/
                    // Lấy job để biết companyId
                    var job = await context.Jobs.FindAsync(request.JobId);
                    if (job == null || job.Status != Job.JobStatus.active || job.DeactivatedByAdmin || job.IsExpired())
                        return BadRequest(new { Success = false, Message = "The job posting has expired" });

                    // Kiểm tra xem đã từng apply job này và còn Pending chưa
                    var existingApplication = await context.Applications
                        .FirstOrDefaultAsync(a => a.JobId == request.JobId && a.UserId == userId && a.Status == ApplicationStatus.Pending);

                    bool isUpdate = false;
                    Application application;

                    // Nếu chưa từng apply job này (Pending) => kiểm tra số lượng Pending
                    if (existingApplication == null)
                    {
                        // Đếm số lượng đơn Pending của user với các job thuộc company này
                        var pendingCount = await context.Applications
                            .Where(a => a.UserId == userId
                                && a.Status == ApplicationStatus.Pending
                                && context.Jobs.Any(j => j.JobId == a.JobId && j.CompanyId == job.CompanyId))
                            .CountAsync();

                        if (pendingCount >= 3)
                        {
                            return BadRequest(new
                            {
                                Success = false,
                                Message = "You are only allowed to apply for a maximum of 3 pending positions at the same company. Please wait for the results before applying for more positions."
                            });
                        }

                        // Tạo mới đơn apply
                        application = new Application
                        {
                            UserId = userId,
                            JobId = request.JobId,
                            CoverLetter = request.CoverLetter,
                            Status = ApplicationStatus.Pending,
                            SubmittedAt = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        context.Applications.Add(application);
                    }
                    else
                    {
                        // Nếu đã apply job này và còn Pending => cho phép update lại đơn apply
                        application = existingApplication;
                        application.CoverLetter = request.CoverLetter;
                        application.Status = ApplicationStatus.Pending;
                        application.UpdatedAt = DateTime.UtcNow;
                        application.SubmittedAt = DateTime.UtcNow;
                        isUpdate = true;
                    }
                    //  KẾT THÚC: Kiểm tra và cập nhật đơn apply cũ nếu có 

                    await context.SaveChangesAsync();



                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        string baseUrl = _configuration["AppSettings:BaseUrl"];
                        string subject = isUpdate
                            ? "Your application has been updated"
                            : "You have successfully applied";

                        string actionText = isUpdate ? "updated/replaced" : "submitted";
                        string buttonText = isUpdate ? "View your application" : "View job details";
                        string buttonUrl = $"{baseUrl}/jobs/{job.JobId}";

                        string body = $@"
                            <html>
                              <body style='font-family: Arial, sans-serif; background: #f6f6f6; padding: 30px;'>
                                <div style='max-width: 500px; margin: auto; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px #eee; padding: 32px;'>
                                  <h2 style='color: #2d8cf0; text-align: center;'>Congratulations, {user.FullName}!</h2>
                                  <p style='font-size: 16px; color: #333;'>
                                    Your application for the job <b>""{job.Title}""</b> has been <b>{(isUpdate ? "replaced" : "submitted")}</b> at {DateTime.Now:HH:mm dd/MM/yyyy}.
                                  </p>
                                  <div style='margin: 24px 0; text-align: center;'>
                                    <a href='{baseUrl}/candidates-dashboard/applied-jobs/{job.JobId}' style='background: #2d8cf0; color: #fff; padding: 12px 24px; border-radius: 4px; text-decoration: none; font-weight: bold;'>
                                      {(isUpdate ? "View your application" : "View job details")}
                                    </a>
                                  </div>
                                  <p style='font-size: 14px; color: #888; text-align: center;'>
                                    If you have any questions, please contact our support team.<br>
                                    Thank you for being part of Job Finder!
                                  </p>
                                </div>
                              </body>
                            </html>";
                        await _emailService.SendEmailAsync(user.Email, subject, body, true);
                    }

                    if (request.CvFile != null && request.CvFile.Length > 0)
                    {
                        tempCvPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");
                        try
                        {
                            using (var stream = new FileStream(tempCvPath, FileMode.Create))
                            {
                                await request.CvFile.CopyToAsync(stream);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to save temporary CV file for User {UserId}, Job {JobId} at {Time}.", userId, request.JobId, DateTime.Now);
                            return BadRequest(new { Success = false, Message = "Failed to save CV file." });
                        }
                    }


                    //var application = new Application
                    //{
                    //    UserId = userId,
                    //    JobId = request.JobId,
                    //    CoverLetter = request.CoverLetter,
                    //    Status = ApplicationStatus.Pending,
                    //    SubmittedAt = DateTime.UtcNow,
                    //    CreatedAt = DateTime.UtcNow,
                    //    UpdatedAt = DateTime.UtcNow
                    //};
                    //context.Applications.Add(application);
                    //await context.SaveChangesAsync();
                    //_logger.LogInformation("Application {ApplicationId} created for UserId {UserId}, JobId {JobId}", application.ApplicationId, userId, request.JobId);


                    taskQueue.QueueBackgroundWorkItem(async token =>
                    {
                        using var innerScope = _serviceScopeFactory.CreateScope();
                        var innerContext = innerScope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                        var innerSemanticService = innerScope.ServiceProvider.GetRequiredService<SemanticMatchingService>();
                        var innerCloudinaryService = innerScope.ServiceProvider.GetRequiredService<CloudinaryService>();

                        using var transaction = await innerContext.Database.BeginTransactionAsync();
                        try
                        {
                            var innerApplication = await innerContext.Applications.FindAsync(application.ApplicationId);
                            if (innerApplication == null)
                            {
                                _logger.LogWarning("Application {ApplicationId} not found during background processing", application.ApplicationId);
                                return;
                            }

                            CV cv = null;
                            string uploadedCvUrl = null;
                            CVData cvData = new CVData();
                            var jsonOptions = new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

                            if (tempCvPath != null && System.IO.File.Exists(tempCvPath))
                            {

                                using var stream = new FileStream(tempCvPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                                var formFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(tempCvPath));


                                uploadedCvUrl = await innerCloudinaryService.UploadCvAsync(formFile);
                                if (string.IsNullOrEmpty(uploadedCvUrl))
                                {
                                    _logger.LogWarning("Failed to upload CV for Application {ApplicationId} at {Time}.", innerApplication.ApplicationId, DateTime.Now);
                                    return;
                                }


                                string extractedText = string.Empty;
                                try
                                {
                                    using (var pdfStream = formFile.OpenReadStream())
                                    using (var pdfDocument = PdfDocument.Open(pdfStream))
                                    {
                                        foreach (var page in pdfDocument.GetPages())
                                        {
                                            extractedText += page.Text + "\n";
                                        }
                                    }
                                    extractedText = CleanExtractedText(extractedText);

                                    if (string.IsNullOrWhiteSpace(extractedText))
                                    {
                                        _logger.LogWarning("CV content is empty or unreadable for Application {ApplicationId} at {Time}.", innerApplication.ApplicationId, DateTime.Now);
                                        return;
                                    }

                                    var (success, extractError, extractedCvData) = await innerSemanticService.ExtractCvDataAsync(null, extractedText);
                                    if (!success)
                                    {
                                        _logger.LogWarning("CV extraction failed for Application {ApplicationId} at {Time}: {Error}.", innerApplication.ApplicationId, DateTime.Now, extractError);
                                        return;
                                    }

                                    cvData = extractedCvData;


                                    cv = new CV
                                    {
                                        UserId = userId,
                                        FileUrl = uploadedCvUrl,
                                        FullCvJson = JsonSerializer.Serialize(new
                                        {
                                            Text = extractedText,
                                            TranslatedText = string.Empty,
                                            CVData = cvData
                                        }, jsonOptions),
                                        CreatedAt = DateTime.UtcNow,
                                        UpdatedAt = DateTime.UtcNow,
                                        Type = CvType.Apply
                                    };
                                    innerContext.CVs.Add(cv);
                                    await innerContext.SaveChangesAsync();


                                    innerApplication.CvId = cv.CVId;
                                    innerApplication.ResumeUrl = uploadedCvUrl;
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "PDF extraction failed for Application {ApplicationId} at {Time}.", innerApplication.ApplicationId, DateTime.Now);
                                    return;
                                }
                            }
                            else if (request.CvId.HasValue)
                            {
                                cv = await innerContext.CVs.FindAsync(request.CvId.Value);
                                if (cv == null || cv.UserId != userId)
                                {
                                    _logger.LogWarning("CV {CvId} not found or does not belong to User {UserId} at {Time}.", request.CvId, userId, DateTime.Now);
                                    return;
                                }
                                uploadedCvUrl = cv.FileUrl;

                                var (success, extractError, extractedCvData) = await innerSemanticService.ExtractCvDataAsync(cv);
                                if (!success)
                                {
                                    _logger.LogWarning("CV extraction failed for Application {ApplicationId} at {Time}: {Error}.", innerApplication.ApplicationId, DateTime.Now, extractError);
                                    return;
                                }
                                cvData = extractedCvData;


                                innerApplication.CvId = cv.CVId;
                                innerApplication.ResumeUrl = uploadedCvUrl;
                            }
                            else
                            {
                                cv = await innerContext.CVs.FirstOrDefaultAsync(c => c.UserId == userId);
                                if (cv == null)
                                {
                                    _logger.LogWarning("No CV found for Application {ApplicationId} at {Time}.", innerApplication.ApplicationId, DateTime.Now);
                                    return;
                                }
                                uploadedCvUrl = cv.FileUrl;

                                var (success, extractError, extractedCvData) = await innerSemanticService.ExtractCvDataAsync(cv);
                                if (!success)
                                {
                                    _logger.LogWarning("CV extraction failed for Application {ApplicationId} at {Time}: {Error}.", innerApplication.ApplicationId, DateTime.Now, extractError);
                                    return;
                                }
                                cvData = extractedCvData;


                                innerApplication.CvId = cv.CVId;
                                innerApplication.ResumeUrl = uploadedCvUrl;
                            }

                            var innerJob = await innerContext.Jobs.FindAsync(request.JobId);
                            if (innerJob == null || innerJob.Status != Job.JobStatus.active || innerJob.DeactivatedByAdmin)
                            {
                                _logger.LogWarning("Job {JobId} not found or inactive during background processing", request.JobId);
                                return;
                            }

                            var (jobVectorsSuccess, jobVectorsError, jobVectors, jobContext) = await innerSemanticService.GenerateVectorsForCriteria(innerJob, $"{innerJob.Description}\n{innerJob.YourSkill}\n{innerJob.YourExperience}\n{innerJob.Education}");
                            if (!jobVectorsSuccess)
                            {
                                _logger.LogError("Failed to generate job vectors for JobId {JobId}: {Error}", innerJob.JobId, jobVectorsError);
                                return;
                            }

                            var (cvVectorsSuccess, cvVectorsError, cvVectors, cvContext) = await innerSemanticService.GenerateVectorsForCVCriteria(cv, cv.FullCvJson);
                            if (!cvVectorsSuccess)
                            {
                                _logger.LogError("Failed to generate CV vectors for CVId {CVId}: {Error}", cv.CVId, cvVectorsError);
                                return;
                            }

                            var matchingResult = await innerSemanticService.CalculateTotalSimilarity(innerJob, cv);
                            if (matchingResult.Success)
                            {
                                innerApplication.SimilarityScore = matchingResult.FinalSimilarity;
                                innerApplication.SimilarityDescription = matchingResult.SimilarityDescription;
                                innerApplication.SimilaritySkills = matchingResult.SimilaritySkills;
                                innerApplication.SimilarityExperience = matchingResult.SimilarityExperience;
                                innerApplication.SimilarityEducation = matchingResult.SimilarityEducation;
                                innerApplication.UpdatedAt = DateTime.UtcNow;
                                try
                                {
                                    await innerContext.SaveChangesAsync();
                                    _logger.LogInformation("Application {ApplicationId} updated with similarity scores: Total={Total:F2}, Description={Description:F2}, Skills={Skills:F2}, Experience={Experience:F2}, Education={Education:F2}",
                                        innerApplication.ApplicationId, matchingResult.FinalSimilarity, matchingResult.SimilarityDescription,
                                        matchingResult.SimilaritySkills, matchingResult.SimilarityExperience, matchingResult.SimilarityEducation);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Failed to save application similarity scores for ApplicationId {ApplicationId}: {Error}",
                                        innerApplication.ApplicationId, ex.Message);
                                }
                            }
                            else
                            {
                                _logger.LogError("Failed to calculate similarity for ApplicationId {ApplicationId}: {Error}",
                                    innerApplication.ApplicationId, matchingResult.ErrorMessage);
                            }

                            await transaction.CommitAsync();
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            _logger.LogError(ex, "Error processing application in background for Application {ApplicationId} at {Time}.", application.ApplicationId, DateTime.Now);
                        }
                        finally
                        {

                            if (tempCvPath != null && System.IO.File.Exists(tempCvPath))
                            {
                                try
                                {
                                    System.IO.File.Delete(tempCvPath);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning(ex, "Failed to delete temporary CV file {TempCvPath} for Application {ApplicationId} at {Time}.", tempCvPath, application.ApplicationId, DateTime.Now);
                                }
                            }
                        }
                    });

                    return Ok(new
                    {
                        Success = true,
                        Message = "Application submitted successfully. Processing in background.",
                        ApplicationId = application.ApplicationId,
                        SimilarityScore = (float?)null,
                        SimilarityDescription = (float?)null,
                        SimilaritySkills = (float?)null,
                        SimilarityExperience = (float?)null,
                        SimilarityEducation = (float?)null,
                        GeminiReasoning = (string)null
                    });
                }

                catch (Exception ex)
                {

                    if (tempCvPath != null && System.IO.File.Exists(tempCvPath))
                    {
                        try
                        {
                            System.IO.File.Delete(tempCvPath);
                        }
                        catch (Exception fileEx)
                        {
                            _logger.LogWarning(fileEx, "Failed to delete temporary CV file {TempCvPath} for User {UserId}, Job {JobId} at {Time}.", tempCvPath, userId, request.JobId, DateTime.Now);
                        }
                    }
                    _logger.LogError(ex, "Error processing application for user {UserId}", userId);
                    return StatusCode(500, new { Success = false, Message = "An error occurred while processing your application." });
                }
            }
        }



        private async Task<(CV Cv, string UploadedCvUrl, CVData CVData, string Error)> ProcessCvForApplyAsync(
    ApplyJobRequest request, int userId, CloudinaryService cloudinaryService, JobFinderDbContext context)
        {
            CV cv = null;
            string uploadedCvUrl = null;
            CVData cvData = new CVData();
            string error = string.Empty;

            var jsonOptions = new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

            if (request.CvFile != null && request.CvFile.Length > 0)
            {
                uploadedCvUrl = await cloudinaryService.UploadCvAsync(request.CvFile);
                if (string.IsNullOrEmpty(uploadedCvUrl))
                {
                    _logger.LogWarning("Failed to upload CV for User {UserId}, Job {JobId} at {Time}", userId, request.JobId, DateTime.Now);
                    return (null, null, null, "Unable to upload CV to Cloudinary");
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
                        _logger.LogWarning("CV content is empty or unreadable for User {UserId}, Job {JobId} at {Time}", userId, request.JobId, DateTime.Now);
                        return (null, null, null, "CV content is empty or unreadable");
                    }

                    var (success, extractError, extractedCvData) = await _semanticMatchingService.ExtractCvDataAsync(null, extractedText);
                    if (!success)
                    {
                        _logger.LogWarning("CV extraction failed for User {UserId}, Job {JobId} at {Time}: {Error}", userId, request.JobId, DateTime.Now, extractError);
                        return (null, null, null, extractError);
                    }

                    cvData = extractedCvData;

                    cv = new CV
                    {
                        UserId = userId,
                        FileUrl = uploadedCvUrl,
                        FullCvJson = JsonSerializer.Serialize(new
                        {
                            Text = extractedText,
                            TranslatedText = string.Empty,
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
                    _logger.LogError(ex, "PDF extraction failed for User {UserId}, Job {JobId} at {Time}", userId, request.JobId, DateTime.Now);
                    return (null, null, null, $"PDF extraction failed: {ex.Message}");
                }
            }
            else if (request.CvId.HasValue)
            {
                cv = await context.CVs.FindAsync(request.CvId.Value);
                if (cv == null || cv.UserId != userId)
                {
                    _logger.LogWarning("CV {CvId} not found or does not belong to User {UserId} at {Time}", request.CvId, userId, DateTime.Now);
                    return (null, null, null, "CV not found or does not belong to the user");
                }
                uploadedCvUrl = cv.FileUrl;

                var (success, extractError, extractedCvData) = await _semanticMatchingService.ExtractCvDataAsync(cv);
                if (!success)
                {
                    _logger.LogWarning("CV extraction failed for User {UserId}, Job {JobId} at {Time}: {Error}", userId, request.JobId, DateTime.Now, extractError);
                    return (null, null, null, extractError);
                }

                cvData = extractedCvData;
            }
            else
            {
                cv = await context.CVs.FirstOrDefaultAsync(c => c.UserId == userId);
                if (cv == null)
                {
                    _logger.LogWarning("No CV found and no file uploaded for User {UserId} at {Time}", userId, DateTime.Now);
                    return (null, null, null, "No CV found and no file uploaded");
                }
                uploadedCvUrl = cv.FileUrl;

                var (success, extractError, extractedCvData) = await _semanticMatchingService.ExtractCvDataAsync(cv);
                if (!success)
                {
                    _logger.LogWarning("CV extraction failed for User {UserId}, Job {JobId} at {Time}: {Error}", userId, request.JobId, DateTime.Now, extractError);
                    return (null, null, null, extractError);
                }

                cvData = extractedCvData;
            }

            return (cv, uploadedCvUrl, cvData, error);
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
                            a.Job.Quantity,
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
                        j.Quantity,
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

                        User = new
                        {
                            a.User.UserId,
                            a.User.FullName
                        },
                        Job = new
                        {
                            a.Job.JobId,
                            a.Job.Title,
                            a.Job.Description,
                            a.Job.Quantity

                        }
                    })
                    .ToListAsync();

                return Ok(applications);
            }
        }

        /*[HttpGet("matching_job/{jobId}")]
        public async Task<IActionResult> GetApplicationsmatchingByJob(int jobId)
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
                        SimilarityDescription = a.SimilarityDescription.HasValue && a.Job != null ? $"{a.SimilarityDescription:F1}/{a.Job.DescriptionWeight * 100:F1}" : null,
                        SimilaritySkills = a.SimilaritySkills.HasValue && a.Job != null ? $"{a.SimilaritySkills:F1}/{a.Job.SkillsWeight * 100:F1}" : null,
                        SimilarityExperience = a.SimilarityExperience.HasValue && a.Job != null ? $"{a.SimilarityExperience:F1}/{a.Job.ExperienceWeight * 100:F1}" : null,
                        SimilarityEducation = a.SimilarityEducation.HasValue && a.Job != null ? $"{a.SimilarityEducation:F1}/{a.Job.EducationWeight * 100:F1}" : null,
                        User = new
                        {
                            a.User.UserId,
                            a.User.FullName
                        },
                        Job = new
                        {
                            a.Job.JobId,
                            a.Job.Title,
                            a.Job.Description,
                            a.Job.Quantity

                        }
                    })
                    .ToListAsync();

                return Ok(applications);
            }
        }*/
        [HttpGet("matching_job/{jobId}")]
        public async Task<IActionResult> GetApplicationsmatchingByJob(int jobId)
        {
            _logger.LogInformation("Fetching applications for Job {JobId}", jobId);
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();

                try
                {
                    // Get job details to check the company ID
                    var job = await context.Jobs.FindAsync(jobId);
                    if (job == null)
                        return NotFound("Job not found.");

                    var companyId = job.CompanyId;

                    // Get company subscription tier to determine limits
                    var subscription = await context.CompanySubscriptions
                        .Where(s => s.UserId == companyId && s.IsActive && s.EndDate > DateTime.UtcNow)
                        .Include(s => s.SubscriptionType)
                        .OrderByDescending(s => s.EndDate)
                        .FirstOrDefaultAsync();

                    // Default values for Free tier
                    string subscriptionTier = "Free";
                    int cvDisplayLimit = 5;

                    if (subscription != null)
                    {
                        subscriptionTier = subscription.SubscriptionType.Name;
                        cvDisplayLimit = subscription.SubscriptionType.CvMatchLimit;

                        if (subscription.SubscriptionType.PackageType == CompanySubscriptionPackageType.Premium)
                        {
                            // Premium tier has no limit
                            cvDisplayLimit = int.MaxValue;
                        }
                    }
                    else
                    {
                        // Try to fetch default Free tier settings
                        var freeTier = await context.CompanySubscriptionTypes
                            .FirstOrDefaultAsync(t => t.PackageType == CompanySubscriptionPackageType.Free);

                        if (freeTier != null)
                        {
                            cvDisplayLimit = freeTier.CvMatchLimit;
                        }
                    }

                    // Get all applications sorted by similarity score
                    var applications = await context.Applications
                        .Where(a => a.JobId == jobId)
                        .Include(a => a.User)
                        .Include(a => a.Job)
                        .OrderByDescending(a => a.SimilarityScore)
                        .ToListAsync();

                    // Apply limits based on subscription tier
                    var limitedApplications = applications.Take(cvDisplayLimit).ToList();

                    var result = limitedApplications.Select(a => new
                    {
                        ApplicationId = a.ApplicationId,
                        a.UserId,
                        a.JobId,
                        a.Status,
                        a.SubmittedAt,
                        a.CoverLetter,
                        a.ResumeUrl,
                        a.SimilarityScore,
                        SimilarityDescription = a.SimilarityDescription.HasValue && a.Job != null ? $"{a.SimilarityDescription:F1}/{a.Job.DescriptionWeight * 100:F1}" : null,
                        SimilaritySkills = a.SimilaritySkills.HasValue && a.Job != null ? $"{a.SimilaritySkills:F1}/{a.Job.SkillsWeight * 100:F1}" : null,
                        SimilarityExperience = a.SimilarityExperience.HasValue && a.Job != null ? $"{a.SimilarityExperience:F1}/{a.Job.ExperienceWeight * 100:F1}" : null,
                        SimilarityEducation = a.SimilarityEducation.HasValue && a.Job != null ? $"{a.SimilarityEducation:F1}/{a.Job.EducationWeight * 100:F1}" : null,
                        User = new
                        {
                            a.User.UserId,
                            a.User.FullName
                        },
                        Job = new
                        {
                            a.Job.JobId,
                            a.Job.Title,
                            a.Job.Description,
                            a.Job.Quantity
                        }
                    }).ToList();

                    var responseMessage = $"Displaying top {Math.Min(cvDisplayLimit, applications.Count)} CV applications " +
                                       $"based on your {subscriptionTier} subscription tier. " +
                                       $"Total applications: {applications.Count}.";

                    if (applications.Count > limitedApplications.Count)
                    {
                        responseMessage += $" Upgrade your subscription to see all {applications.Count} applications.";
                    }

                    return Ok(new
                    {
                        SubscriptionTier = subscriptionTier,
                        JobPostLimit = subscriptionTier == "Premium" ? "Unlimited" : (subscriptionTier == "Basic" ? "10" : "2"),
                        CvDisplayLimit = subscriptionTier == "Premium" ? "Unlimited" : (subscriptionTier == "Basic" ? "10" : "5"),
                        Applications = result,
                        TotalApplications = applications.Count,
                        DisplayedApplications = limitedApplications.Count,
                        Message = responseMessage
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving matching applications for Job {JobId}", jobId);
                    return StatusCode(500, "An error occurred while retrieving matching applications.");
                }
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
                [FromServices] IBackgroundTaskQueue taskQueue,
                [FromServices] NotificationService notificationService,
                [FromServices] IServiceScopeFactory serviceScopeFactory)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
            {
                _logger.LogWarning("Invalid user ID in token at {Time}.", DateTime.Now);
                return Unauthorized("Invalid user ID.");
                _logger.LogInformation("User {UserId} started trying match for Job {JobId}", userId, request.JobId);
            }

            var role = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();
            if (role != "candidate")
            {
                _logger.LogWarning("User {UserId} is not a candidate at {Time}.", userId, DateTime.Now);
                return Forbid("Only candidates can try to match jobs.");
            }
            string tempCvPath = null;
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                
                var activeSubscription = await context.CandidateSubscriptions
                    .Where(s => s.UserId == userId && s.IsActive) // Removed EndDate condition
                    .Include(s => s.SubscriptionType)
                    .OrderByDescending(s => s.CreatedAt)
                    .FirstOrDefaultAsync();

                if (activeSubscription == null)
                {
                   
                    var tryMatchCount = await context.TryMatchRecords
                        .Where(r => r.UserId == userId)
                        .CountAsync();

                    if (tryMatchCount > 0)
                    {
                        return BadRequest(new
                        {
                            Success = false,
                            ErrorMessage = "You have used up all your free Try-Match attempts. Please purchase a plan to continue using it.",
                            RequiresSubscription = true
                        });
                    }

                    
                    _logger.LogInformation("User {UserId} is using their free try-match attempt", userId);
                }
                else if (activeSubscription.RemainingTryMatches <= 0)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        ErrorMessage = "You have used up all the Try-Match attempts in your current package. Please upgrade your package to get more attempts.",
                        RequiresUpgrade = true,
                        CurrentSubscription = new
                        {
                            PackageName = activeSubscription.SubscriptionType.Name,
                            RemainingTryMatches = activeSubscription.RemainingTryMatches
                        }
                    });
                }
                try
                {
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
                        return BadRequest(new { Success = false, Message = "Please update your personal information before applying." });
                    }

                    var job = await context.Jobs.FindAsync(request.JobId);
                    if (job == null)
                    {
                        _logger.LogWarning("Job {JobId} not found for User {UserId} at {Time}.", request.JobId, userId, DateTime.Now);
                        return BadRequest(new { Success = false, ErrorMessage = "Job not found." });
                    }
                    if (job.Status != Job.JobStatus.active || job.DeactivatedByAdmin)
                    {
                        _logger.LogWarning("Job {JobId} is inactive or deactivated for User {UserId} at {Time}.", request.JobId, userId, DateTime.Now);
                        return BadRequest(new { Success = false, ErrorMessage = "Job is inactive or deactivated." });
                    }

                   
                    if (request.CvFile == null && !request.CvId.HasValue)
                    {
                        var existingCv = await context.CVs.FirstOrDefaultAsync(c => c.UserId == userId);
                        if (existingCv == null)
                        {
                            _logger.LogWarning("No CV found and no file uploaded for User {UserId} at {Time}.", userId, DateTime.Now);
                            return BadRequest(new { Success = false, ErrorMessage = "No CV found and no file uploaded." });
                        }
                        request.CvId = existingCv.CVId;
                    }



                    if (request.CvFile != null && request.CvFile.Length > 0)
                    {
                        tempCvPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");
                        try
                        {
                            using (var stream = new FileStream(tempCvPath, FileMode.Create))
                            {
                                await request.CvFile.CopyToAsync(stream);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to save temporary CV file for User {UserId}, Job {JobId} at {Time}.", userId, request.JobId, DateTime.Now);
                            return BadRequest(new { Success = false, ErrorMessage = "Failed to save CV file." });
                        }
                    }

                  
                    var existingRecord = await context.TryMatchRecords
                        .FirstOrDefaultAsync(r => r.UserId == userId && r.JobId == request.JobId &&
                            (r.CvId == request.CvId || (request.CvFile != null && r.CvId == null)) && r.Status == "Processing");
                    if (existingRecord != null)
                    {
                        if (tempCvPath != null && System.IO.File.Exists(tempCvPath))
                        {
                            try
                            {
                                System.IO.File.Delete(tempCvPath);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Failed to delete temporary CV file {TempCvPath} for User {UserId}, Job {JobId} at {Time}.", tempCvPath, userId, request.JobId, DateTime.Now);
                            }
                        }
                        _logger.LogWarning("A try-match request is already processing for User {UserId}, Job {JobId}, CV {CvId} at {Time}.",
                            userId, request.JobId, request.CvId, DateTime.Now);
                        return BadRequest(new { Success = false, ErrorMessage = "A try-match request is already being processed for this job and CV." });
                    }

                    
                    var tryMatchRecord = new TryMatchRecord
                    {
                        UserId = userId,
                        JobId = request.JobId,
                        CvId = request.CvId,
                        Status = "Processing",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,

                    };
                    context.TryMatchRecords.Add(tryMatchRecord);
                    if (activeSubscription != null)
                    {
                        activeSubscription.RemainingTryMatches = Math.Max(0, activeSubscription.RemainingTryMatches - 1);
                        activeSubscription.UpdatedAt = DateTime.UtcNow;
                    }
                    await context.SaveChangesAsync();

                   
                    try
                    {
                        await notificationService.CreateTryMatchNotification(tryMatchRecord, request.JobId, job.Title);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send initial notification for TryMatch {TryMatchId}, User {UserId}, Job {JobId} at {Time}.",
                            tryMatchRecord.TryMatchId, userId, request.JobId, DateTime.Now);
                    }

                
                    taskQueue.QueueBackgroundWorkItem(async token =>
                    {
                        using var innerScope = _serviceScopeFactory.CreateScope();
                        var innerContext = innerScope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                        var innerSemanticService = innerScope.ServiceProvider.GetRequiredService<SemanticMatchingService>();
                        var innerCloudinaryService = innerScope.ServiceProvider.GetRequiredService<CloudinaryService>();
                        var innerNotificationService = innerScope.ServiceProvider.GetRequiredService<NotificationService>();
                        Job innerJob = null;


                        try
                        {
                            using var transaction = await innerContext.Database.BeginTransactionAsync();
                            try
                            {
                                // Retrieve TryMatch record
                                var record = await innerContext.TryMatchRecords.FindAsync(tryMatchRecord.TryMatchId);
                                if (record == null)
                                {
                                    _logger.LogError("TryMatch record {TryMatchId} not found during background processing at {Time}.", tryMatchRecord.TryMatchId, DateTime.Now);
                                    return;
                                }

                              
                                innerJob = await innerContext.Jobs.FindAsync(request.JobId);
                                if (innerJob == null || innerJob.Status != Job.JobStatus.active || innerJob.DeactivatedByAdmin)
                                {
                                    record.Status = "Failed";
                                    record.ErrorMessage = "Job not found or inactive.";
                                    record.UpdatedAt = DateTime.UtcNow;

                                    await innerContext.SaveChangesAsync();
                                    await transaction.CommitAsync();
                                    try
                                    {
                                        await innerNotificationService.CreateTryMatchNotification(record, request.JobId, innerJob?.Title ?? "Unknown Job");
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, "Failed to send notification for TryMatch {TryMatchId} at {Time}.", tryMatchRecord.TryMatchId, DateTime.Now);
                                    }
                                    _logger.LogWarning("Job {JobId} not found or inactive for TryMatch {TryMatchId} at {Time}.", request.JobId, tryMatchRecord.TryMatchId, DateTime.Now);
                                    return;
                                }

                                // Process CV in background
                                CV cv = null;
                                string uploadedCvUrl = null;
                                CVData cvData = new CVData();
                                var jsonOptions = new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

                                if (tempCvPath != null && System.IO.File.Exists(tempCvPath))
                                {
                                   
                                    using var stream = new FileStream(tempCvPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                                    var formFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(tempCvPath));

                                
                                    uploadedCvUrl = await innerCloudinaryService.UploadCvAsync(formFile);
                                    if (string.IsNullOrEmpty(uploadedCvUrl))
                                    {
                                        _logger.LogWarning("Failed to upload CV for User {UserId}, Job {JobId} at {Time}.", userId, request.JobId, DateTime.Now);
                                        record.Status = "Failed";
                                        record.ErrorMessage = "Unable to upload CV to Cloudinary.";
                                        record.UpdatedAt = DateTime.UtcNow;

                                        await innerContext.SaveChangesAsync();
                                        await transaction.CommitAsync();
                                        await innerNotificationService.CreateTryMatchNotification(record, request.JobId, innerJob.Title);
                                        return;
                                    }

                                   
                                    string extractedText = string.Empty;
                                    try
                                    {
                                        using (var pdfStream = formFile.OpenReadStream())
                                        using (var pdfDocument = PdfDocument.Open(pdfStream))
                                        {
                                            foreach (var page in pdfDocument.GetPages())
                                            {
                                                extractedText += page.Text + "\n";
                                            }
                                        }
                                        extractedText = CleanExtractedText(extractedText);

                                        if (string.IsNullOrWhiteSpace(extractedText))
                                        {
                                            _logger.LogWarning("CV content is empty or unreadable for User {UserId}, Job {JobId} at {Time}.", userId, request.JobId, DateTime.Now);
                                            record.Status = "Failed";
                                            record.ErrorMessage = "CV content is empty or unreadable.";
                                            record.UpdatedAt = DateTime.UtcNow;

                                            await innerContext.SaveChangesAsync();
                                            await transaction.CommitAsync();
                                            await innerNotificationService.CreateTryMatchNotification(record, request.JobId, innerJob.Title);
                                            return;
                                        }

                                     
                                        var (success, extractError, extractedCvData) = await innerSemanticService.ExtractCvDataAsync(null, extractedText);
                                        if (!success)
                                        {
                                            _logger.LogWarning("CV extraction failed for User {UserId}, Job {JobId} at {Time}: {Error}.", userId, request.JobId, DateTime.Now, extractError);
                                            record.Status = "Failed";
                                            record.ErrorMessage = extractError;
                                            record.UpdatedAt = DateTime.UtcNow;

                                            await innerContext.SaveChangesAsync();
                                            await transaction.CommitAsync();
                                            await innerNotificationService.CreateTryMatchNotification(record, request.JobId, innerJob.Title);
                                            return;
                                        }

                                        cvData = extractedCvData;

                                        
                                        cv = new CV
                                        {
                                            UserId = userId,
                                            FileUrl = uploadedCvUrl,
                                            FullCvJson = JsonSerializer.Serialize(new
                                            {
                                                Text = extractedText,
                                                TranslatedText = string.Empty,
                                                CVData = cvData
                                            }, jsonOptions),
                                            CreatedAt = DateTime.UtcNow,
                                            UpdatedAt = DateTime.UtcNow,
                                            Type = CvType.Apply
                                        };
                                        innerContext.CVs.Add(cv);
                                        await innerContext.SaveChangesAsync();
                                        record.CvId = cv.CVId;
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, "PDF extraction failed for User {UserId}, Job {JobId} at {Time}.", userId, request.JobId, DateTime.Now);
                                        record.Status = "Failed";
                                        record.ErrorMessage = $"PDF extraction failed: {ex.Message}";
                                        record.UpdatedAt = DateTime.UtcNow;

                                        await innerContext.SaveChangesAsync();
                                        await transaction.CommitAsync();
                                        await innerNotificationService.CreateTryMatchNotification(record, request.JobId, innerJob.Title);
                                        return;
                                    }
                                }
                                else if (request.CvId.HasValue)
                                {
                                    cv = await innerContext.CVs.FindAsync(request.CvId.Value);
                                    if (cv == null || cv.UserId != userId)
                                    {
                                        _logger.LogWarning("CV {CvId} not found or does not belong to User {UserId} at {Time}.", request.CvId, userId, DateTime.Now);
                                        record.Status = "Failed";
                                        record.ErrorMessage = "CV not found or does not belong to the user.";
                                        record.UpdatedAt = DateTime.UtcNow;

                                        await innerContext.SaveChangesAsync();
                                        await transaction.CommitAsync();
                                        await innerNotificationService.CreateTryMatchNotification(record, request.JobId, innerJob.Title);
                                        return;
                                    }
                                    uploadedCvUrl = cv.FileUrl;

                                    var (success, extractError, extractedCvData) = await innerSemanticService.ExtractCvDataAsync(cv);
                                    if (!success)
                                    {
                                        _logger.LogWarning("CV extraction failed for User {UserId}, Job {JobId} at {Time}: {Error}.", userId, request.JobId, DateTime.Now, extractError);
                                        record.Status = "Failed";
                                        record.ErrorMessage = extractError;
                                        record.UpdatedAt = DateTime.UtcNow;

                                        await innerContext.SaveChangesAsync();
                                        await transaction.CommitAsync();
                                        await innerNotificationService.CreateTryMatchNotification(record, request.JobId, innerJob.Title);
                                        return;
                                    }
                                    cvData = extractedCvData;
                                }
                                else
                                {
                                    cv = await innerContext.CVs.FirstOrDefaultAsync(c => c.UserId == userId);
                                    if (cv == null)
                                    {
                                        _logger.LogWarning("No CV found for User {UserId} at {Time}.", userId, DateTime.Now);
                                        record.Status = "Failed";
                                        record.ErrorMessage = "No CV found.";
                                        record.UpdatedAt = DateTime.UtcNow;

                                        await innerContext.SaveChangesAsync();
                                        await transaction.CommitAsync();
                                        await innerNotificationService.CreateTryMatchNotification(record, request.JobId, innerJob.Title);
                                        return;
                                    }
                                    uploadedCvUrl = cv.FileUrl;

                                    var (success, extractError, extractedCvData) = await innerSemanticService.ExtractCvDataAsync(cv);
                                    if (!success)
                                    {
                                        _logger.LogWarning("CV extraction failed for User {UserId}, Job {JobId} at {Time}: {Error}.", userId, request.JobId, DateTime.Now, extractError);
                                        record.Status = "Failed";
                                        record.ErrorMessage = extractError;
                                        record.UpdatedAt = DateTime.UtcNow;

                                        await innerContext.SaveChangesAsync();
                                        await transaction.CommitAsync();
                                        await innerNotificationService.CreateTryMatchNotification(record, request.JobId, innerJob.Title);
                                        return;
                                    }
                                    cvData = extractedCvData;
                                }

                              
                                var matchingResult = await innerSemanticService.CalculateTotalSimilarity(innerJob, cv);
                                var suggestions = await innerSemanticService.GenerateImprovementSuggestions(innerJob, cv,
                                    matchingResult.SimilarityDescription, matchingResult.SimilaritySkills,
                                    matchingResult.SimilarityExperience, matchingResult.SimilarityEducation,
                                    matchingResult.DescriptionMaxScore, matchingResult.SkillsMaxScore,
                                    matchingResult.ExperienceMaxScore, matchingResult.EducationMaxScore);

                                
                                record.SimilarityScore = matchingResult.Success ? matchingResult.FinalSimilarity : null;
                                record.Suggestions = suggestions != null ? JsonSerializer.Serialize(suggestions) : null;
                                record.Status = matchingResult.Success ? "Completed" : "Failed";
                                record.ErrorMessage = matchingResult.Success ? null : matchingResult.ErrorMessage;
                                record.UpdatedAt = DateTime.UtcNow;

                                await innerContext.SaveChangesAsync();
                                await transaction.CommitAsync();

                                try
                                {
                                    await innerNotificationService.CreateTryMatchNotification(record, request.JobId, innerJob.Title);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Failed to send notification for TryMatch {TryMatchId} at {Time}.", tryMatchRecord.TryMatchId, DateTime.Now);
                                }

                                _logger.LogInformation("TryMatch {TryMatchId} completed successfully for User {UserId}, Job {JobId} at {Time}.",
                                    tryMatchRecord.TryMatchId, userId, request.JobId, DateTime.Now);
                            }
                            catch (Exception ex)
                            {
                                await transaction.RollbackAsync();
                                _logger.LogError(ex, "Error processing try-match in background for User {UserId}, Job {JobId} at {Time}. Details: {Message}, InnerException: {InnerException}",
                                    userId, request.JobId, DateTime.Now, ex.Message, ex.InnerException?.Message);
                                var record = await innerContext.TryMatchRecords.FindAsync(tryMatchRecord.TryMatchId);
                                if (record != null)
                                {
                                    record.Status = "Failed";
                                    record.ErrorMessage = $"Background processing error: {ex.Message}";
                                    record.UpdatedAt = DateTime.UtcNow;

                                    await innerContext.SaveChangesAsync();
                                    try
                                    {
                                        await innerNotificationService.CreateTryMatchNotification(record, request.JobId, innerJob?.Title ?? "Unknown Job");
                                    }
                                    catch (Exception notificationEx)
                                    {
                                        _logger.LogError(notificationEx, "Failed to send notification for TryMatch {TryMatchId} at {Time}.", tryMatchRecord.TryMatchId, DateTime.Now);
                                    }
                                }
                            }
                        }
                        finally
                        {
                            
                            if (tempCvPath != null && System.IO.File.Exists(tempCvPath))
                            {
                                try
                                {
                                    System.IO.File.Delete(tempCvPath);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning(ex, "Failed to delete temporary CV file {TempCvPath} for TryMatch {TryMatchId} at {Time}.", tempCvPath, tryMatchRecord.TryMatchId, DateTime.Now);
                                }
                            }
                        }
                    });

                    return Ok(new
                    {
                        Success = true,
                        Message = "Match attempt submitted successfully. Processing in background.",
                        TryMatchId = tryMatchRecord.TryMatchId
                    });
                }
                catch (Exception ex)
                {
                    
                    if (tempCvPath != null && System.IO.File.Exists(tempCvPath))
                    {
                        try
                        {
                            System.IO.File.Delete(tempCvPath);
                        }
                        catch (Exception fileEx)
                        {
                            _logger.LogWarning(fileEx, "Failed to delete temporary CV file {TempCvPath} for User {UserId}, Job {JobId} at {Time}.", tempCvPath, userId, request.JobId, DateTime.Now);
                        }
                    }
                    _logger.LogError(ex, "Error processing try-match for User {UserId}, Job {JobId} at {Time}. Details: {Message}, InnerException: {InnerException}",
                        userId, request.JobId, DateTime.Now, ex.Message, ex.InnerException?.Message);
                    return StatusCode(500, new { Success = false, ErrorMessage = $"An error occurred while processing your match attempt: {ex.Message}" });
                }
            }
        }

        private async Task<(CV Cv, string UploadedCvUrl, CVData CVData, string Error)> ProcessCvForTryMatchAsync(
    TryMatchRequest request, int userId, CloudinaryService cloudinaryService, JobFinderDbContext context)
        {
            CV cv = null;
            string uploadedCvUrl = null;
            CVData cvData = new CVData();
            string error = string.Empty;

            var jsonOptions = new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

            if (request.CvFile != null && request.CvFile.Length > 0)
            {
                uploadedCvUrl = await cloudinaryService.UploadCvAsync(request.CvFile);
                if (string.IsNullOrEmpty(uploadedCvUrl))
                {
                    _logger.LogWarning("Failed to upload CV for User {UserId}, Job {JobId} at {Time}", userId, request.JobId, DateTime.Now);
                    return (null, null, null, "Unable to upload CV to Cloudinary");
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
                        _logger.LogWarning("CV content is empty or unreadable for User {UserId}, Job {JobId} at {Time}", userId, request.JobId, DateTime.Now);
                        return (null, null, null, "CV content is empty or unreadable");
                    }

                    var (success, extractError, extractedCvData) = await _semanticMatchingService.ExtractCvDataAsync(null, extractedText);
                    if (!success)
                    {
                        _logger.LogWarning("CV extraction failed for User {UserId}, Job {JobId} at {Time}: {Error}", userId, request.JobId, DateTime.Now, extractError);
                        return (null, null, null, extractError);
                    }

                    cvData = extractedCvData;

                    cv = new CV
                    {
                        UserId = userId,
                        FileUrl = uploadedCvUrl,
                        FullCvJson = JsonSerializer.Serialize(new
                        {
                            Text = extractedText,
                            TranslatedText = string.Empty,
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
                    _logger.LogError(ex, "PDF extraction failed for User {UserId}, Job {JobId} at {Time}", userId, request.JobId, DateTime.Now);
                    return (null, null, null, $"PDF extraction failed: {ex.Message}");
                }
            }
            else if (request.CvId.HasValue)
            {
                cv = await context.CVs.FindAsync(request.CvId.Value);
                if (cv == null || cv.UserId != userId)
                {
                    _logger.LogWarning("CV {CvId} not found or does not belong to User {UserId} at {Time}", request.CvId, userId, DateTime.Now);
                    return (null, null, null, "CV not found or does not belong to the user");
                }
                uploadedCvUrl = cv.FileUrl;

                var (success, extractError, extractedCvData) = await _semanticMatchingService.ExtractCvDataAsync(cv);
                if (!success)
                {
                    _logger.LogWarning("CV extraction failed for User {UserId}, Job {JobId} at {Time}: {Error}", userId, request.JobId, DateTime.Now, extractError);
                    return (null, null, null, extractError);
                }

                cvData = extractedCvData;
            }
            else
            {
                cv = await context.CVs.FirstOrDefaultAsync(c => c.UserId == userId);
                if (cv == null)
                {
                    _logger.LogWarning("No CV found and no file uploaded for User {UserId} at {Time}", userId, DateTime.Now);
                    return (null, null, null, "No CV found and no file uploaded");
                }
                uploadedCvUrl = cv.FileUrl;

                var (success, extractError, extractedCvData) = await _semanticMatchingService.ExtractCvDataAsync(cv);
                if (!success)
                {
                    _logger.LogWarning("CV extraction failed for User {UserId}, Job {JobId} at {Time}: {Error}", userId, request.JobId, DateTime.Now, extractError);
                    return (null, null, null, extractError);
                }

                cvData = extractedCvData;
            }

            return (cv, uploadedCvUrl, cvData, error);
        }



        [Authorize]
        [HttpGet("try-match-details/{tryMatchId}")]
        public async Task<IActionResult> GetTryMatchDetail(int tryMatchId)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
            {
                _logger.LogWarning("Invalid user ID in token at {Time}.", DateTime.Now);
                return Unauthorized("Invalid user ID.");
            }

            _logger.LogInformation("Fetching try-match details for TryMatchId {TryMatchId} by User {UserId} at {Time}.", tryMatchId, userId, DateTime.Now);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                try
                {
                    var tryMatchRecord = await context.TryMatchRecords
                        .Where(r => r.TryMatchId == tryMatchId && r.UserId == userId)
                        .Include(r => r.Job)
                        .Include(r => r.CV)
                        .FirstOrDefaultAsync();

                    if (tryMatchRecord == null)
                    {
                        _logger.LogWarning("TryMatch record {TryMatchId} not found or does not belong to User {UserId} at {Time}.", tryMatchId, userId, DateTime.Now);
                        return NotFound(new { Success = false, ErrorMessage = "Try-match record not found or access denied." });
                    }

                    var suggestions = !string.IsNullOrEmpty(tryMatchRecord.Suggestions)
                        ? JsonSerializer.Deserialize<List<string>>(tryMatchRecord.Suggestions)
                        : new List<string>();

                    var result = new
                    {
                        TryMatchId = tryMatchRecord.TryMatchId,
                        JobId = tryMatchRecord.JobId,
                        JobTitle = tryMatchRecord.Job?.Title,
                        CvId = tryMatchRecord.CvId,
                        CvFileUrl = tryMatchRecord.CV?.FileUrl,
                        SimilarityScore = tryMatchRecord.SimilarityScore,
                        Suggestions = suggestions,
                        Status = tryMatchRecord.Status,
                        ErrorMessage = tryMatchRecord.ErrorMessage,
                        CreatedAt = tryMatchRecord.CreatedAt,
                        UpdatedAt = tryMatchRecord.UpdatedAt
                    };

                    return Ok(new { Success = true, Data = result });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching try-match details for TryMatchId {TryMatchId}, User {UserId} at {Time}. Details: {Message}", tryMatchId, userId, DateTime.Now, ex.Message);
                    return StatusCode(500, new { Success = false, ErrorMessage = "An error occurred while fetching try-match details." });
                }
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

                    })
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                return Ok(tryMatchRecords);
            }
        }
        [Authorize]
        [HttpPost("export-applications")]
        public async Task<IActionResult> ExportApplications([FromBody] ExportApplicationsRequest request)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
            {
                _logger.LogWarning("Invalid user ID in token at {Time}.", DateTime.Now);
                return Unauthorized("Invalid user ID.");
            }

            var role = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();
            if (role != "company" && role != "admin")
            {
                _logger.LogWarning("User {UserId} is not authorized to export applications at {Time}.", userId, DateTime.Now);
                return Forbid("Only companies or admins can export applications.");
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                try
                {
                  
                    IQueryable<Application> query = context.Applications
                        .Include(a => a.Job)
                            .ThenInclude(j => j.Company);

                    if (role == "company")
                    {
                        var company = await context.Users
                            .Include(u => u.CompanyProfile)
                            .FirstOrDefaultAsync(u => u.UserId == userId);
                        if (company?.CompanyProfile == null)
                        {
                            _logger.LogWarning("Company profile not found for User {UserId} at {Time}.", userId, DateTime.Now);
                            return BadRequest(new { Success = false, Message = "Company profile not found." });
                        }

                        var jobIds = await context.Jobs
                            .Where(j => j.CompanyId == company.UserId)
                            .Select(j => j.JobId)
                            .ToListAsync();
                        query = query.Where(a => jobIds.Contains(a.JobId));
                    }

                    if (request.ApplicationIds != null && request.ApplicationIds.Any())
                    {
                        query = query.Where(a => request.ApplicationIds.Contains(a.ApplicationId));
                    }

                    var applications = await query
                        .Select(a => new
                        {
                            a.ApplicationId,
                            a.ResumeUrl
                        })
                        .ToListAsync();

                    if (!applications.Any())
                    {
                        _logger.LogWarning("No applications found for export by User {UserId} at {Time}.", userId, DateTime.Now);
                        return NotFound(new { Success = false, Message = "No applications found for export." });
                    }

                   
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                        {
                            foreach (var app in applications)
                            {
                                if (string.IsNullOrEmpty(app.ResumeUrl))
                                {
                                    _logger.LogWarning("ResumeUrl is empty for Application {ApplicationId} at {Time}.", app.ApplicationId, DateTime.Now);
                                    continue;
                                }

                                try
                                {
                                    var response = await _httpClient.GetAsync(app.ResumeUrl);
                                    if (!response.IsSuccessStatusCode)
                                    {
                                        _logger.LogWarning("Failed to download CV for Application {ApplicationId} from {ResumeUrl} at {Time}.", app.ApplicationId, app.ResumeUrl, DateTime.Now);
                                        continue;
                                    }

                                    var fileBytes = await response.Content.ReadAsByteArrayAsync();
                                    // Extract filename from ResumeUrl (e.g., from "https://cloudinary.com/resume/john_doe_cv.pdf")
                                    var fileName = Path.GetFileName(new Uri(app.ResumeUrl).AbsolutePath) ?? $"CV_Application_{app.ApplicationId}.pdf";
                                    var entry = zipArchive.CreateEntry(fileName, CompressionLevel.Optimal);
                                    using (var entryStream = entry.Open())
                                    {
                                        await entryStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error downloading CV for Application {ApplicationId} from {ResumeUrl} at {Time}.", app.ApplicationId, app.ResumeUrl, DateTime.Now);
                                }
                            }
                        }

                        if (memoryStream.Length == 0)
                        {
                            _logger.LogWarning("No valid CVs were included in the ZIP for User {UserId} at {Time}.", userId, DateTime.Now);
                            return NotFound(new { Success = false, Message = "No valid CVs found for export." });
                        }

                        var zipFileName = $"CVs_{DateTime.UtcNow:yyyyMMddHHmmss}.zip";
                        _logger.LogInformation("ZIP file created successfully with {Count} CVs by User {UserId} at {Time}.", applications.Count, userId, DateTime.Now);
                        return File(memoryStream.ToArray(), "application/zip", zipFileName);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error exporting applications for User {UserId} at {Time}.", userId, DateTime.Now);
                    return StatusCode(500, new { Success = false, Message = "An error occurred while exporting applications." });
                }
            }
        }
        [Authorize]
        [HttpPut("confirm/{applicationId}")]
        public async Task<IActionResult> ConfirmApplication(int applicationId, [FromBody] ConfirmApplicationRequest request)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
            {
                _logger.LogWarning("Invalid user ID in token at {Time}.", DateTime.Now);
                return Unauthorized("Invalid user ID.");
            }

            var role = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();
            if (role != "company")
            {
                _logger.LogWarning("User {UserId} is not authorized to confirm applications at {Time}.", userId, DateTime.Now);
                return Forbid("Only companies can confirm applications.");
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                try
                {
                    
                    var application = await context.Applications
                        .Include(a => a.Job)
                        .Include(a => a.User)
                        .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

                    if (application == null)
                    {
                        _logger.LogWarning("Application {ApplicationId} not found for User {UserId} at {Time}.", applicationId, userId, DateTime.Now);
                        return NotFound(new { Success = false, Message = "Application not found." });
                    }

                   
                    if (application.Job.CompanyId != userId)
                    {
                        _logger.LogWarning("User {UserId} attempted to confirm Application {ApplicationId} for Job {JobId} not owned by them at {Time}.", userId, applicationId, application.JobId, DateTime.Now);
                        return Forbid("You are not authorized to confirm applications for this job.");
                    }

                   
                    if (!Enum.IsDefined(typeof(ApplicationStatus), request.Status))
                    {
                        _logger.LogWarning("Invalid application status {Status} for Application {ApplicationId} by User {UserId} at {Time}.", request.Status, applicationId, userId, DateTime.Now);
                        return BadRequest(new { Success = false, Message = "Invalid application status." });
                    }

                  
                    if (application.Status == request.Status)
                    {
                        _logger.LogWarning("Application {ApplicationId} already has status {Status} for User {UserId} at {Time}.", applicationId, request.Status, userId, DateTime.Now);
                        return BadRequest(new { Success = false, Message = $"Application already has status {request.Status}." });
                    }

                    application.Status = request.Status;
                    application.UpdatedAt = DateTime.UtcNow;

                    context.Applications.Update(application);
                    await context.SaveChangesAsync();

                    
                    string baseUrl = _configuration["AppSettings:BaseUrl"];
                    if (string.IsNullOrEmpty(baseUrl))
                    {
                        _logger.LogError("BaseUrl is not configured in appsettings.json for Application {ApplicationId} at {Time}.", applicationId, DateTime.Now);
                        throw new InvalidOperationException("BaseUrl is not configured in appsettings.json.");
                    }

                    string applicationEndpoint = $"/application-details/{applicationId}";
                    string title = request.Status switch
                    {
                        ApplicationStatus.Accepted => $"Application for {application.Job.Title} Accepted",
                        ApplicationStatus.Rejected => $"Application for {application.Job.Title} Rejected",
                        _ => $"Update on Your Application for {application.Job.Title}"
                    };

                    
                    try
                    {
                        await _notificationService.SendDirectNotification(
                            application.UserId,
                            title,
                            applicationEndpoint,
                            Notification.NotificationType.ApplicationStatusUpdate
                        );
                        _logger.LogInformation("Notification sent to User {UserId} for Application {ApplicationId} status update to {Status} at {Time}.", application.UserId, applicationId, request.Status, DateTime.Now);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send notification for Application {ApplicationId} to User {UserId} at {Time}.", applicationId, application.UserId, DateTime.Now);
                    }

                   
                    if (!string.IsNullOrEmpty(application.User?.Email))
                    {
                        string emailBody = request.Status switch
                        {
                            ApplicationStatus.Accepted => $@"
                                <div style='font-family: Arial, sans-serif; background: #f6f6f6; padding: 30px;'>
                                    <div style='max-width: 600px; margin: auto; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px #eee; padding: 32px;'>
                                        <h2 style='color: #2d8cf0;'>Application for {application.Job.Title} Accepted</h2>
                                        <p>Dear candidate, we are delighted to inform you that your application for the {application.Job.Title} position has been shortlisted for the next stage. We will contact you soon with further details.</p>
                                        <p><strong>Job:</strong> {application.Job.Title}</p>
                                        <p><strong>Status:</strong> {request.Status}</p>
                                        <div style='margin:20px 0;'>
                                            <a href='{baseUrl}{applicationEndpoint}' style='background:#2d8cf0;color:#fff;padding:10px 20px;border-radius:4px;text-decoration:none;font-weight:bold;'>View Application Details</a>
                                        </div>
                                        <p>Best regards,<br>Job Finder Team</p>
                                    </div>
                                </div>",
                            ApplicationStatus.Rejected => $@"
                                <div style='font-family: Arial, sans-serif; background: #f6f6f6; padding: 30px;'>
                                    <div style='max-width: 600px; margin: auto; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px #eee; padding: 32px;'>
                                        <h2 style='color: #2d8cf0;'>Application for {application.Job.Title} Rejected</h2>
                                        <p>Dear candidate, thank you for applying for the {application.Job.Title} position. After careful consideration, we have decided to pursue other candidates whose qualifications more closely align with our current needs. We appreciate your interest and wish you success in your job search.</p>
                                        <p><strong>Job:</strong> {application.Job.Title}</p>
                                        <p><strong>Status:</strong> {request.Status}</p>
                                        <p>Best regards,<br>Job Finder Team</p>
                                    </div>
                                </div>",
                            _ => $@"
                                <div style='font-family: Arial, sans-serif; background: #f6f6f6; padding: 30px;'>
                                    <div style='max-width: 600px; margin: auto; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px #eee; padding: 32px;'>
                                        <h2 style='color: #2d8cf0;'>Update on Your Application for {application.Job.Title}</h2>
                                        <p>Dear candidate, your application for the {application.Job.Title} position has been updated to '{request.Status}'. Please check your application dashboard for more details.</p>
                                        <p><strong>Job:</strong> {application.Job.Title}</p>
                                        <p><strong>Status:</strong> {request.Status}</p>
                                        <div style='margin:20px 0;'>
                                            <a href='{baseUrl}{applicationEndpoint}' style='background:#2d8cf0;color:#fff;padding:10px 20px;border-radius:4px;text-decoration:none;font-weight:bold;'>View Application Details</a>
                                        </div>
                                        <p>Best regards,<br>Job Finder Team</p>
                                    </div>
                                </div>"
                        };

                        try
                        {
                            _emailService.SendEmail(
                                application.User.Email,
                                title,
                                emailBody,
                                true
                            );
                            _logger.LogInformation("Email sent to {Email} for Application {ApplicationId} status update to {Status} at {Time}.", application.User.Email, applicationId, request.Status, DateTime.Now);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send email for Application {ApplicationId} to {Email} at {Time}.", applicationId, application.User.Email, DateTime.Now);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No email address found for User {UserId} for Application {ApplicationId} at {Time}.", application.UserId, applicationId, DateTime.Now);
                    }

                    _logger.LogInformation("Application {ApplicationId} status updated to {Status} by User {UserId} at {Time}.", applicationId, request.Status, userId, DateTime.Now);
                    return Ok(new { Success = true, Message = $"Application status updated to {request.Status}." });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error confirming application {ApplicationId} for User {UserId} at {Time}.", applicationId, userId, DateTime.Now);
                    return StatusCode(500, new { Success = false, Message = "An error occurred while confirming the application." });
                }
            }
        }
        [HttpGet("unique-applicants-by-job")]
        public async Task<IActionResult> GetUniqueApplicantsByJob()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
            {
                _logger.LogWarning("Invalid user ID in token at {Time}.", DateTime.Now);
                return Unauthorized("Invalid user ID.");
            }

            var role = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();
            if (role != "company" && role != "admin")
            {
                _logger.LogWarning("User {UserId} is not authorized to view unique applicants at {Time}.", userId, DateTime.Now);
                return Forbid("Only companies or admins can view unique applicants.");
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                try
                {
                    IQueryable<Job> query = context.Jobs;

                    if (role == "company")
                    {
                        var company = await context.Users
                            .Include(u => u.CompanyProfile)
                            .FirstOrDefaultAsync(u => u.UserId == userId);
                        if (company?.CompanyProfile == null)
                        {
                            _logger.LogWarning("Company profile not found for User {UserId} at {Time}.", userId, DateTime.Now);
                            return BadRequest(new { Success = false, Message = "Company profile not found." });
                        }
                        query = query.Where(j => j.CompanyId == userId);
                    }

                    var jobData = await query
                        .Select(j => new
                        {
                            JobId = j.JobId,
                            Title = j.Title,
                            UniqueApplicants = context.Applications
                                .Where(a => a.JobId == j.JobId)
                                .Select(a => a.UserId)
                                .Distinct()
                                .Count()
                        })
                        .ToListAsync();

                    if (!jobData.Any())
                    {
                        _logger.LogWarning("No jobs or applicants found for User {UserId} at {Time}.", userId, DateTime.Now);
                        return NotFound(new { Success = false, Message = "No jobs or applicants found." });
                    }

                    _logger.LogInformation("Fetched unique applicants by job for User {UserId} at {Time}.", userId, DateTime.Now);
                    return Ok(new { Success = true, Data = jobData });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching unique applicants by job for User {UserId} at {Time}.", userId, DateTime.Now);
                    return StatusCode(500, new { Success = false, Message = "An error occurred while fetching unique applicants." });
                }
            }
        }

    }
}