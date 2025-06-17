using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.filter;
using JOB_FINDER_API.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public JobController(JobFinderDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobs() =>
            await _context.Jobs.Include(j => j.Industry).ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetJob(int id)
        {
            var job = await _context.Jobs.Include(j => j.Industry).FirstOrDefaultAsync(j => j.JobId == id);
            return job == null ? NotFound() : job;
        }

        [HttpPost]
        public async Task<ActionResult<Job>> CreateJob(Job job)
        {
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetJob), new { id = job.JobId }, job);
        }

        [HttpPost("create")]
        public async Task<ActionResult<Job>> CreateJob([FromForm] JobCreateRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.TimeEnd <= dto.TimeStart)
                return BadRequest("TimeEnd must be after TimeStart.");
            if (dto.ExpiryDate <= DateTime.UtcNow)
                return BadRequest("ExpiryDate must be in the future.");

            var job = new Job
            {
                Title = dto.Title,
                Description = dto.Description,
                CompanyId = dto.CompanyId,
                Salary = dto.Salary,
                IndustryId = dto.IndustryId,
                ExpiryDate = dto.ExpiryDate,
                LevelId = dto.LevelId,
                JobTypeId = dto.JobTypeId,
                ExperienceLevelId = dto.ExperienceLevelId,
                TimeStart = dto.TimeStart,
                TimeEnd = dto.TimeEnd,
                ProvinceName = dto.ProvinceName,
                AddressDetail = dto.AddressDetail,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = Job.JobStatus.pending 
            };

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetJob), new { id = job.JobId }, job);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromForm] JobCreateRequest dto)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound();

            job.Title = dto.Title;
            job.Description = dto.Description;
            job.CompanyId = dto.CompanyId;
            job.Salary = dto.Salary;
            job.IndustryId = dto.IndustryId;
            job.ExpiryDate = dto.ExpiryDate;
            job.LevelId = dto.LevelId;
            job.JobTypeId = dto.JobTypeId;
            job.ExperienceLevelId = dto.ExperienceLevelId;
            job.TimeStart = dto.TimeStart;
            job.TimeEnd = dto.TimeEnd;
            job.Status = dto.Status;
            job.ProvinceName = dto.ProvinceName;
            job.AddressDetail = dto.AddressDetail;
            job.UpdatedAt = DateTime.UtcNow;


            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound();
            if (job.Status == Job.JobStatus.active)
                return BadRequest("Cannot delete a job that is already active.");

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Job>>> FilterJobs([FromQuery] JobFilterParams filter)
        {
            var query = _context.Jobs.Include(j => j.Industry).AsQueryable();

            if (!string.IsNullOrEmpty(filter.Title))
                query = query.Where(j => j.Title.Contains(filter.Title));
            if (filter.IndustryId.HasValue)
                query = query.Where(j => j.IndustryId == filter.IndustryId);
            if (filter.LevelId.HasValue)
                query = query.Where(j => j.LevelId == filter.LevelId);
            if (filter.JobTypeId.HasValue)
                query = query.Where(j => j.JobTypeId == filter.JobTypeId);
            if (filter.ExperienceLevelId.HasValue)
                query = query.Where(j => j.ExperienceLevelId == filter.ExperienceLevelId);
            if (filter.MinSalary.HasValue)
                query = query.Where(j => j.Salary >= filter.MinSalary);
            if (filter.MaxSalary.HasValue)
                query = query.Where(j => j.Salary <= filter.MaxSalary);
            if (!string.IsNullOrEmpty(filter.ProvinceName))
                query = query.Where(j => j.ProvinceName.Contains(filter.ProvinceName));
            if (!string.IsNullOrEmpty(filter.Status) && Enum.TryParse<Job.JobStatus>(filter.Status, out var status))
                query = query.Where(j => j.Status == status);
            if (filter.CompanyId.HasValue)
                query = query.Where(j => j.CompanyId == filter.CompanyId);
            if (filter.TimeStart.HasValue)
                query = query.Where(j => j.TimeStart >= filter.TimeStart);
            if (filter.TimeEnd.HasValue)
                query = query.Where(j => j.TimeEnd <= filter.TimeEnd);

            var jobs = await query.ToListAsync();
            return jobs;
        }
        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateJobStatus(int id, [FromQuery] Job.JobStatus newStatus)
        {
            await AutoDeactivateExpiredJobs();

            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound("Job not found.");

            // Lấy userId từ User.Identity.Name (theo NameClaimType đã cấu hình)
            var userIdStr = User.Identity?.Name;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID.");

            // Lấy role theo ClaimTypes.Role (đã ánh xạ theo RoleClaimType)
            var role = User.FindFirst(ClaimTypes.Role)?.Value.ToLower();

            if (job.TimeEnd < DateTime.UtcNow)
                return BadRequest("Cannot change status of expired job.");

            if (role == "admin")
            {
                if (job.Status == newStatus)
                    return BadRequest("Job already in specified status.");

                job.Status = newStatus;
                job.UpdatedAt = DateTime.UtcNow;

                // Nếu admin inactive job thì set flag DeactivatedByAdmin = true, ngược lại false
                if (newStatus == Job.JobStatus.inactive)
                    job.DeactivatedByAdmin = true;
                else
                    job.DeactivatedByAdmin = false;

                await _context.SaveChangesAsync();
                return Ok($"Admin updated job #{id} status to {newStatus}.");
            }

            if (role == "company")
            {
                if (job.CompanyId != userId)
                    return Forbid("You are not the owner of this job.");

                if (job.Status == Job.JobStatus.pending)
                    return Forbid("Job is pending approval. Only admin can update its status.");

                if (job.Status == Job.JobStatus.active && newStatus == Job.JobStatus.inactive)
                {
                    job.Status = Job.JobStatus.inactive;
                    job.DeactivatedByAdmin = false; 
                    job.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return Ok("Company deactivated the job successfully.");
                }

                if (job.Status == Job.JobStatus.inactive && newStatus == Job.JobStatus.active)
                {
                    if (job.DeactivatedByAdmin)
                        return Forbid("Job was deactivated by admin. Company cannot reactivate it.");

                    job.Status = Job.JobStatus.active;
                    job.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return Ok("Company reactivated the job successfully.");
                }

                return BadRequest("Company can only deactivate an active job or activate an inactive job.");
            }

            return Forbid("You do not have permission to update job status.");
        }









        private async Task AutoDeactivateExpiredJobs()
        {
            var now = DateTime.UtcNow;
            var expiredJobs = await _context.Jobs
                .Where(j => j.Status == Job.JobStatus.active && j.TimeEnd < now)
                .ToListAsync();

            foreach (var job in expiredJobs)
            {
                job.Status = Job.JobStatus.inactive;
                job.UpdatedAt = now;
            }

            if (expiredJobs.Count > 0)
                await _context.SaveChangesAsync();
        }



    }
}
