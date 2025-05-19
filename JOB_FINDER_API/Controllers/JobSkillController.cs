using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobSkillController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public JobSkillController(JobFinderDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobSkill>>> GetJobSkills() =>
            await _context.JobSkills.Include(js => js.Job).Include(js => js.Skill).ToListAsync();

        [HttpGet("{jobId}/{skillId}")]
        public async Task<ActionResult<JobSkill>> GetJobSkill(int jobId, int skillId)
        {
            var jobSkill = await _context.JobSkills
                .Include(js => js.Job)
                .Include(js => js.Skill)
                .FirstOrDefaultAsync(js => js.JobId == jobId && js.SkillId == skillId);
            return jobSkill == null ? NotFound() : jobSkill;
        }

        [HttpPost]
        public async Task<ActionResult<JobSkill>> CreateJobSkill(JobSkill jobSkill)
        {
            _context.JobSkills.Add(jobSkill);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetJobSkill), new { jobId = jobSkill.JobId, skillId = jobSkill.SkillId }, jobSkill);
        }

        [HttpDelete("{jobId}/{skillId}")]
        public async Task<IActionResult> DeleteJobSkill(int jobId, int skillId)
        {
            var jobSkill = await _context.JobSkills.FindAsync(jobId, skillId);
            if (jobSkill == null) return NotFound();
            _context.JobSkills.Remove(jobSkill);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}