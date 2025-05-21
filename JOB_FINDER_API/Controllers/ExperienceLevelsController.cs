using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JOB_FINDER_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExperienceLevelsController : ControllerBase
    {
        private readonly JobFinderDbContext _context;

        public ExperienceLevelsController(JobFinderDbContext context)
        {
            _context = context;
        }

        // GET: api/ExperienceLevels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExperienceLevel>>> GetExperienceLevels()
        {
            return await _context.ExperienceLevel.ToListAsync();
        }

        // GET: api/ExperienceLevels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExperienceLevel>> GetExperienceLevel(int id)
        {
            var experienceLevel = await _context.ExperienceLevel.FindAsync(id);

            if (experienceLevel == null)
            {
                return NotFound();
            }

            return experienceLevel;
        }

        // PUT: api/ExperienceLevels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExperienceLevel(int id, ExperienceLevel experienceLevel)
        {
            if (id != experienceLevel.id)
            {
                return BadRequest();
            }

            _context.Entry(experienceLevel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExperienceLevelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ExperienceLevels
        [HttpPost]
        public async Task<ActionResult<ExperienceLevel>> PostExperienceLevel(ExperienceLevel experienceLevel)
        {
            _context.ExperienceLevel.Add(experienceLevel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExperienceLevel), new { id = experienceLevel.id }, experienceLevel);
        }

        // DELETE: api/ExperienceLevels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExperienceLevel(int id)
        {
            var experienceLevel = await _context.ExperienceLevel.FindAsync(id);
            if (experienceLevel == null)
            {
                return NotFound();
            }

            _context.ExperienceLevel.Remove(experienceLevel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExperienceLevelExists(int id)
        {
            return _context.ExperienceLevel.Any(e => e.id == id);
        }
    }
}