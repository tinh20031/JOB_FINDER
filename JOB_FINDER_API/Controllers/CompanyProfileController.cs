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
    public class CompanyProfileController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public CompanyProfileController(JobFinderDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.CompanyProfile.ToListAsync());

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(int userId)
        {
            var item = await _context.CompanyProfile.FindAsync(userId);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CompanyProfile model)
        {
            _context.CompanyProfile.Add(model);
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
            var item = await _context.CompanyProfile.FindAsync(userId);
            if (item == null) return NotFound();
            _context.CompanyProfile.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        

        [HttpPut("{userId}/lock")]
        public async Task<IActionResult> LockCompany(int userId)
        {
            var company = await _context.CompanyProfile.FindAsync(userId);
            if (company == null)
                return NotFound("Company not found.");

            company.IsActive = false;
            await _context.SaveChangesAsync();
            return Ok("Company has been locked.");
        }

        [HttpPut("{userId}/unlock")]
        public async Task<IActionResult> UnlockCompany(int userId)
        {
            var company = await _context.CompanyProfile.FindAsync(userId);
            if (company == null)
                return NotFound("Company not found.");

            company.IsActive = true;
            await _context.SaveChangesAsync();
            return Ok("Company has been unlocked.");
        }
    }
}