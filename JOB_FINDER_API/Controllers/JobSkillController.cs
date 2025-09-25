/*using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.DTO;
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
        public async Task<IActionResult> AddSkillToJob([FromBody] JobSkillDto dto)
        {
            // Kiểm tra tồn tại
            if (!_context.Jobs.Any(j => j.JobId == dto.JobId) || !_context.Skills.Any(s => s.SkillId == dto.SkillId))
                return BadRequest("Job hoặc Skill không tồn tại.");

            // Kiểm tra trùng
            if (_context.JobSkills.Any(js => js.JobId == dto.JobId && js.SkillId == dto.SkillId))
                return BadRequest("Skill đã tồn tại trong job.");

            var jobSkill = new JobSkill
            {
                JobId = dto.JobId,
                SkillId = dto.SkillId
            };

            _context.JobSkills.Add(jobSkill);
            await _context.SaveChangesAsync();
            return Ok();
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
}*/