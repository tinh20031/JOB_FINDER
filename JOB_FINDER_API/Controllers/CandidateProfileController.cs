using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.DTO;
using JOB_FINDER_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CandidateProfileController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public CandidateProfileController(JobFinderDbContext context) => _context = context;

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(int userId)
        {
            var profile = await _context.CandidateProfiles
                .Include(p => p.User)
                .Include(p => p.AboutMes)
                .Include(p => p.Skills)
                .Include(p => p.Educations)
                .Include(p => p.WorkExperiences)
                .Include(p => p.HighlightProjects)
                .Include(p => p.Certificates)
                .Include(p => p.Awards)
                .Include(p => p.ForeginLanguages)
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Không tìm thấy thông tin user id trong token.");

            var userId = int.Parse(userIdClaim.Value);

            var profile = await _context.CandidateProfiles
                .Include(p => p.User)
                .Include(p => p.AboutMes)
                .Include(p => p.Skills)
                .Include(p => p.Educations)
                .Include(p => p.WorkExperiences)
                .Include(p => p.HighlightProjects)
                .Include(p => p.Certificates)
                .Include(p => p.Awards)
                .Include(p => p.ForeginLanguages)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return NotFound();

                return Ok(new
                {
                    CandidateProfileId = (int?)null,
                    UserId = user.Id,
                    Gender = string.Empty,
                    Dob = (DateTime?)null,
                    JobTitle = string.Empty,
                    Address = string.Empty,
                    Province = string.Empty,
                    City = string.Empty,
                    PersonalLink = string.Empty,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName ?? string.Empty,
                    Phone = user.Phone ?? string.Empty,
                    Image = user.Image ?? string.Empty,
                    AboutMes = new List<object>(),
                    Skills = new List<object>(),
                    Educations = new List<object>(),
                    WorkExperiences = new List<object>(),
                    HighlightProjects = new List<object>(),
                    Certificates = new List<object>(),
                    Awards = new List<object>(),
                    ForeginLanguages = new List<object>()
                });
            }

            return Ok(new
            {
                profile.CandidateProfileId,
                profile.UserId,
                profile.Gender,
                profile.Dob,
                profile.JobTitle,
                profile.Address,
                profile.Province,
                profile.City,
                profile.PersonalLink,
                Email = profile.User?.Email ?? string.Empty,
                FullName = profile.User?.FullName ?? string.Empty,
                Phone = profile.User?.Phone ?? string.Empty,
                Image = profile.User?.Image ?? string.Empty,
                AboutMes = profile.AboutMes,
                Skills = profile.Skills,
                Educations = profile.Educations,
                WorkExperiences = profile.WorkExperiences,
                HighlightProjects = profile.HighlightProjects,
                Certificates = profile.Certificates,
                Awards = profile.Awards,
                ForeginLanguages = profile.ForeginLanguages
            });
        }
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromForm] UpdateCandidateProfileDto model, IFormFile? imageFile, [FromServices] CloudinaryService cloudinaryService)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Không tìm thấy thông tin user id trong token.");

            var userId = int.Parse(userIdClaim.Value);

            var profile = await _context.CandidateProfiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null) return NotFound();

            // Cập nhật các trường của CandidateProfile
            profile.Gender = model.Gender;
            profile.Dob = model.Dob;
            profile.JobTitle = model.JobTitle;
            profile.Address = model.Address;
            profile.Province = model.Province;
            profile.City = model.City;
            profile.PersonalLink = model.PersonalLink;

            // Chỉ cho phép cập nhật FullName
            if (!string.IsNullOrEmpty(model.FullName))
            {
                profile.User.FullName = model.FullName;
                profile.User.Phone = model.Phone;
            }
          
            if (imageFile != null)
            {
                profile.User.Image = await cloudinaryService.UploadImageAsync(imageFile);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(int userId)
        {
            var profile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null) return NotFound();
            _context.CandidateProfiles.Remove(profile);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("me/profile-strength")]
        public async Task<IActionResult> GetMyProfileStrength([FromServices] ProfileStrengthService profileStrengthService)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Không tìm thấy thông tin user id trong token.");

            var userId = int.Parse(userIdClaim.Value);

            var profile = await _context.CandidateProfiles
                .Include(p => p.User)
                .Include(p => p.AboutMes)
                .Include(p => p.Educations)
                .Include(p => p.WorkExperiences)
                .Include(p => p.Skills)
                .Include(p => p.Certificates)
                .Include(p => p.HighlightProjects)
                .Include(p => p.Awards)
                .Include(p => p.ForeginLanguages)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null) return NotFound();

            var result = profileStrengthService.Calculate(profile);
            return Ok(result);
        }
    }
}