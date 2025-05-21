using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HRProfileController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public HRProfileController(JobFinderDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.Set<HRProfile>().ToListAsync());

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(int userId)
        {
            var item = await _context.Set<HRProfile>().FindAsync(userId);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HRProfile model)
        {
            _context.Set<HRProfile>().Add(model);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { userId = model.UserId }, model);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(int userId, HRProfile model)
        {
            if (userId != model.UserId) return BadRequest();
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(int userId)
        {
            var item = await _context.Set<HRProfile>().FindAsync(userId);
            if (item == null) return NotFound();
            _context.Set<HRProfile>().Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}