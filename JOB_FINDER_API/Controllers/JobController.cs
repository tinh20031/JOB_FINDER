using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.DTO;
using JOB_FINDER_API.Models.filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<Job>> CreateJob([FromBody] JobCreateDto dto)
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
                Status = dto.Status,
                ImageJob = dto.ImageJob,
                ProvinceName = dto.ProvinceName,
                AddressDetail = dto.AddressDetail,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetJob), new { id = job.JobId }, job);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, Job job)
        {
            if (id != job.JobId) return BadRequest();
            _context.Entry(job).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound();
            if (job.Status == Job.JobStatus.Posted) // Đúng, so sánh enum với enum
                return BadRequest("Cannot delete a job that is already posted.");
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        //filter job 
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Job>>> FilterJobs([FromQuery] JobFilterParams filter)
        {
            var query = _context.Jobs
                .Include(j => j.Industry)
                .AsQueryable();

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
    }
}