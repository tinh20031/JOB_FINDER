using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.Requests;
using JOB_FINDER_API.Models.Services;
using JOB_FINDER_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using Tesseract;
using ImageMagick;
using UglyToad.PdfPig;
using Google.Cloud.Translation.V2;
using System.Text.RegularExpressions;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _tessDataPath = @"./tessdata";

        public ApplicationController(JobFinderDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
            if (!Directory.Exists(_tessDataPath))
                throw new DirectoryNotFoundException($"Tesseract data directory not found: {_tessDataPath}");
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
            [FromServices] SemanticMatchingService semanticMatchingService,
            [FromServices] EmailService emailService)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.Name)!.Value);

            CV? cv = null;
            string? uploadedCvUrl = null;
            string extractedText = string.Empty;

            if (request.CvFile != null && request.CvFile.Length > 0)
            {
                // Upload file PDF lên Cloudinary
                uploadedCvUrl = await cloudinaryService.UploadCvAsync(request.CvFile);
                if (string.IsNullOrEmpty(uploadedCvUrl))
                    return StatusCode(500, new { Success = false, ErrorMessage = "Failed to upload CV to Cloudinary" });

                // Đọc trực tiếp file PDF từ IFormFile
                try
                {
                    using (var stream = request.CvFile.OpenReadStream())
                    using (var pdfDocument = PdfDocument.Open(stream))
                    {
                        foreach (var page in pdfDocument.GetPages())
                        {
                            extractedText += page.Text + "\n";
                        }
                        Console.WriteLine($"PDF extracted (full): {extractedText}");
                        Console.WriteLine($"PDF extracted (length): {extractedText.Length} chars");
                        Console.WriteLine($"PDF extracted (first 200 chars): {extractedText.Substring(0, Math.Min(extractedText.Length, 200))}...");
                        if (string.IsNullOrEmpty(extractedText.Trim()))
                        {
                            Console.WriteLine("Warning: No text extracted from PDF. It may be a scanned document.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"PDF extraction error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    extractedText = "PDF extraction failed";
                }

                // Lưu CV vào database
                cv = new CV
                {
                    UserId = userId,
                    FileUrl = uploadedCvUrl,
                    FullCvJson = JsonSerializer.Serialize(new { Text = extractedText }),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.CVs.Add(cv);
                await _context.SaveChangesAsync();
            }
            else
            {
                cv = await _context.CVs.FirstOrDefaultAsync(c => c.UserId == userId);
                if (cv == null)
                    return BadRequest(new { Success = false, ErrorMessage = "CV not found and no file uploaded" });
                uploadedCvUrl = cv.FileUrl;
            }

            // Tạo snapshot (giữ nguyên để hỗ trợ các chức năng khác nếu cần)
            var snapshotUrls = await cvSnapshotService.CaptureCvAsImagesAsync(cv);
            if (snapshotUrls == null || snapshotUrls.Count == 0)
                return BadRequest(new { Success = false, ErrorMessage = "Failed to snapshot CV" });

            // Tìm job
            var job = await _context.Jobs.FindAsync(request.JobId);
            if (job == null)
                return NotFound(new { Success = false, ErrorMessage = "Job not found" });

            // Lưu application
            var application = new Application
            {
                UserId = userId,
                JobId = request.JobId,
                CvId = cv.Id,
                CoverLetter = request.CoverLetter, // Lấy nội dung thư xin việc từ request.CoverLetter
                ResumeUrl = cv.FileUrl,
                SnapshotCv = snapshotUrls.First(),
                Status = ApplicationStatus.Pending,
                SubmittedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            // Tính similarity score
            var weights = new MatchingWeights();
            var matchingResult = await semanticMatchingService.CalculateTotalSimilarity(job, cv, weights);
            if (matchingResult.Success)
            {
                application.SimilarityScore = matchingResult.TotalSimilarity;
                await _context.SaveChangesAsync();

                if (matchingResult.TotalSimilarity > 0.7)
                {
                    var company = await _context.Users.FindAsync(job.CompanyId);
                    if (company != null && !string.IsNullOrEmpty(company.Email))
                    {
                        string mailBody = $@"
                    <div style='font-family: Arial, sans-serif;'>
                        <h2>New Application for {job.Title}</h2>
                        <p>Applicant: {userId}</p>
                        <p>Similarity Score: {matchingResult.TotalSimilarity:F2}</p>
                        <p><a href='http://localhost:3000/application/{application.Id}'>View Application</a></p>
                    </div>";
                        emailService.SendEmail(company.Email, "New Application", mailBody, true);
                    }
                }
            }

            return Ok(new
            {
                Success = true,
                Message = "Applied successfully",
                ApplicationId = application.Id,
                SnapshotUrls = snapshotUrls,
                SimilarityScore = matchingResult.Success ? matchingResult.TotalSimilarity : (float?)null
            });
        }


        [Authorize]
        [HttpGet("my-applications")]
        public async Task<IActionResult> GetMyApplications()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.Name)!.Value);

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
                    a.SnapshotCv,
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
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.Name)!.Value);

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
                        a.SnapshotCv,
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
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.Name)!.Value);

            if (await _context.UserFavoriteCompanies.AnyAsync(f => f.UserId == userId && f.CompanyId == companyId))
                return BadRequest("Already favorited");

            var favorite = new UserFavoriteCompany
            {
                UserId = userId,
                CompanyId = companyId,
                CreatedAt = DateTime.UtcNow
            };
            _context.UserFavoriteCompanies.Add(favorite);
            await _context.SaveChangesAsync();
            return Ok("Favorited company");
        }

        [Authorize]
        [HttpGet("my-favorite-companies")]
        public async Task<IActionResult> GetMyFavoriteCompanies()
        {
            var userIdStr = User.Identity?.Name;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID.");

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
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.Name)!.Value);
            var favorite = await _context.UserFavoriteCompanies
                .FirstOrDefaultAsync(f => f.UserId == userId && f.CompanyId == companyId);
            if (favorite == null)
                return NotFound("Not favorited");
            _context.UserFavoriteCompanies.Remove(favorite);
            await _context.SaveChangesAsync();
            return Ok("Unfavorited company");
        }

        [HttpGet("job/{jobId}")]
        public async Task<IActionResult> GetApplicationsByJob(int jobId)
        {
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
                    a.SnapshotCv,
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
    }


}