using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public SkillController(JobFinderDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkills() =>
            await _context.Skills.ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Skill>> GetSkill(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            return skill == null ? NotFound() : skill;
        }

        [HttpPost]
        public async Task<ActionResult<Skill>> CreateSkill(Skill skill)
        {
            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSkill), new { id = skill.SkillId }, skill);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSkill(int id, Skill skill)
        {
            if (id != skill.SkillId) return BadRequest();
            _context.Entry(skill).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null) return NotFound();
            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}