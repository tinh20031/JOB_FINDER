using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.DTO;
using JOB_FINDER_API.Models.filter;
using JOB_FINDER_API.Models.Requests;
using JOB_FINDER_API.Services;
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
                    
                 
                })
                .ToListAsync();

            return Ok(result);
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

        
        [HttpPost]
        public async Task<IActionResult> AddCompanyProfile(
    [FromForm] CompanyProfileRequest request,
    IFormFile? logoFile,
    IFormFile? logoLgrFile,
    [FromServices] CloudinaryService cloudinaryService)
        {
            if (request.UserId == 0)
                return BadRequest("UserId is required.");

            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
            if (!userExists)
                return BadRequest("User does not exist.");

            string? logoUrl = null;
            string? logoLgrUrl = null;
            if (logoFile != null)
                logoUrl = await cloudinaryService.UploadImageAsync(logoFile);
            if (logoLgrFile != null)
                logoLgrUrl = await cloudinaryService.UploadImageAsync(logoLgrFile);

            var companyProfile = new CompanyProfile
            {
                UserId = request.UserId,
                CompanyName = request.CompanyName,
                CompanyProfileDescription = request.CompanyProfileDescription,
                Location = request.Location,
                TeamSize = request.TeamSize,
                Website = request.Website,
                Contact = request.Contact,
                IndustryId = request.IndustryId,
                UrlCompanyLogo = logoUrl,
                ImageLogoLgr = logoLgrUrl,
                IsActive = false
            };

            _context.CompanyProfile.Add(companyProfile);
            await _context.SaveChangesAsync();

            return Ok(companyProfile);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateCompanyProfile(
            int userId,
            [FromForm] CompanyProfileRequest request,
            IFormFile? logoFile,
            IFormFile? logoLgrFile,
            [FromServices] CloudinaryService cloudinaryService)
        {
            var companyProfile = await _context.CompanyProfile.FirstOrDefaultAsync(c => c.UserId == userId);
            if (companyProfile == null)
                return NotFound("Company profile not found.");

            if (!string.IsNullOrWhiteSpace(request.CompanyName))
                companyProfile.CompanyName = request.CompanyName;
            companyProfile.CompanyProfileDescription = request.CompanyProfileDescription;
            companyProfile.Location = request.Location;
            companyProfile.TeamSize = request.TeamSize;
            companyProfile.Website = request.Website;
            companyProfile.Contact = request.Contact;
            companyProfile.IndustryId = request.IndustryId;

            if (logoFile != null)
                companyProfile.UrlCompanyLogo = await cloudinaryService.UploadImageAsync(logoFile);
            if (logoLgrFile != null)
                companyProfile.ImageLogoLgr = await cloudinaryService.UploadImageAsync(logoLgrFile);

            await _context.SaveChangesAsync();
            return Ok(companyProfile);
        }
    }

}
