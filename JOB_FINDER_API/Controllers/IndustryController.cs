using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndustryController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public IndustryController(JobFinderDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Industry>>> GetIndustries() =>
            await _context.Industries.ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Industry>> GetIndustry(int id)
        {
            var industry = await _context.Industries.FindAsync(id);
            return industry == null ? NotFound() : industry;
        }

        [HttpPost]
        public async Task<ActionResult<Industry>> CreateIndustry(Industry industry)
        {
            _context.Industries.Add(industry);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetIndustry), new { id = industry.IndustryId }, industry);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIndustry(int id, Industry industry)
        {
            if (id != industry.IndustryId) return BadRequest();
            _context.Entry(industry).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIndustry(int id)
        {
            var industry = await _context.Industries.FindAsync(id);
            if (industry == null) return NotFound();
            _context.Industries.Remove(industry);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}