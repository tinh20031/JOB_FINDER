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

        /*[Authorize]
        [HttpGet("my-applications")]
        public async Task<IActionResult> GetMyApplications()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.Name)!.Value);

            var applications = await _context.Applications
                .Where(a => a.UserId == userId)
                .Include(a => a.Job) // Optional: include job details if needed
                .ToListAsync();

            return Ok(applications);
        }*/
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