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

        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] CompanyProfileFilterParams filter)
        {
            var query = _context.CompanyProfile.Include(c => c.Industry).AsQueryable();

            if (!string.IsNullOrEmpty(filter.CompanyName))
                query = query.Where(c => c.CompanyName.Contains(filter.CompanyName));
            if (!string.IsNullOrEmpty(filter.Location))
                query = query.Where(c => c.Location.Contains(filter.Location));
            if (!string.IsNullOrEmpty(filter.TeamSize))
                query = query.Where(c => c.TeamSize == filter.TeamSize);
            if (filter.IndustryId.HasValue)
                query = query.Where(c => c.IndustryId == filter.IndustryId);

            var result = await query
                .Select(c => new CompanyProfileDto
                {
                    UserId = c.UserId,
                    CompanyName = c.CompanyName,
                    CompanyProfileDescription = c.CompanyProfileDescription,
                    Location = c.Location,
                    UrlCompanyLogo = c.UrlCompanyLogo,
                    ImageLogoLgr = c.ImageLogoLgr,
                    TeamSize = c.TeamSize,
                    Website = c.Website,
                    Contact = c.Contact,
                    IndustryId = c.IndustryId,
                    IndustryName = c.Industry.IndustryName
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}