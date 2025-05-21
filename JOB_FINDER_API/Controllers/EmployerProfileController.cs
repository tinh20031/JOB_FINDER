using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployerProfileController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public EmployerProfileController(JobFinderDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.CompanyProfiles.ToListAsync());

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(int userId)
        {
            var item = await _context.CompanyProfiles.FindAsync(userId);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CompanyProfile model)
        {
            _context.CompanyProfiles.Add(model);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { userId = model.UserId }, model);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(int userId, CompanyProfile model)
        {
            if (userId != model.UserId) return BadRequest();
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(int userId)
        {
            var item = await _context.CompanyProfiles.FindAsync(userId);
            if (item == null) return NotFound();
            _context.CompanyProfiles.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}