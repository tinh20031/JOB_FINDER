using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.Requests;
using JOB_FINDER_API.Models.Services;
using JOB_FINDER_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public ApplicationController(JobFinderDbContext context) => _context = context;

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
    [FromServices] CloudinaryService cloudinaryService) // Inject CloudinaryService
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.Name)!.Value);

            CV? cv = null;
            string? uploadedCvUrl = null;

            if (request.CvFile != null && request.CvFile.Length > 0)
            {
                // Upload CV to Cloudinary (like CVController)
                uploadedCvUrl = await cloudinaryService.UploadCvAsync(request.CvFile);
                if (string.IsNullOrEmpty(uploadedCvUrl))
                    return StatusCode(500, "Failed to upload CV to Cloudinary.");

                // Create new CV record
                cv = new CV
                {
                    UserId = userId,
                    FileUrl = uploadedCvUrl,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.CVs.Add(cv);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Use existing CV if no file uploaded
                cv = await _context.CVs.FirstOrDefaultAsync(c => c.UserId == userId);
                if (cv == null)
                    return BadRequest("CV not found and no file uploaded");
            }

            // Snapshot CV
            var snapshotUrls = await cvSnapshotService.CaptureCvAsImagesAsync(cv);
            if (snapshotUrls == null || snapshotUrls.Count == 0)
                return BadRequest("Failed to snapshot CV");

            var application = new Application
            {
                UserId = userId,
                JobId = request.JobId,
                CvId = cv.Id,
                CoverLetter = request.CoverLetter,
                ResumeUrl = cv.FileUrl,
                SnapshotCv = snapshotUrls.First(),
                Status = ApplicationStatus.Pending,
                SubmittedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Applied successfully",
                ApplicationId = application.Id,
                SnapshotUrls = snapshotUrls
            });
        }

        // Add this endpoint inside your ApplicationController class

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
                        a.Job.Salary,
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

            // Get job IDs the current user has applied to
            var appliedJobIds = await _context.Applications
                .Where(a => a.UserId == userId)
                .Select(a => a.JobId)
                .Distinct()
                .ToListAsync();

            // For each job, get job info and all CVs that have been applied to that job
            var jobs = await _context.Jobs
                .Where(j => appliedJobIds.Contains(j.JobId))
                .Select(j => new
                {
                    j.JobId,
                    j.Title,
                    j.Description,
                    j.CompanyId,
                    j.Salary,
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
            // Lấy userId từ claim
            var userIdStr = User.Identity?.Name;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID.");

            // Lấy danh sách company profile mà user đã yêu thích
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

            if (!applications.Any())
                return NotFound("No applications found for this job.");

            return Ok(applications);
        }

    }
}