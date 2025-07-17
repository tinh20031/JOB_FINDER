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
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null) return NotFound();

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
                profile.AboutMeDescription,
                Skills = profile.Skills,
                Educations = profile.Educations,
                WorkExperiences = profile.WorkExperiences,
                HighlightProjects = profile.HighlightProjects,
                Certificates = profile.Certificates,
                Awards = profile.Awards,
                ForeignLanguages = profile.ForeignLanguages
            });
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
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return NotFound();

                return Ok(new
                {
                    CandidateProfileId = (int?)null,
                    UserId = user.UserId,
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
                    AboutMeDescription = string.Empty,
                    Skills = new List<SkillInfo>(),
                    Educations = new List<EducationInfo>(),
                    WorkExperiences = new List<WorkExperienceInfo>(),
                    HighlightProjects = new List<HighlightProjectInfo>(),
                    Certificates = new List<CertificateInfo>(),
                    Awards = new List<AwardInfo>(),
                    ForeignLanguages = new List<ForeignLanguageInfo>()
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
                profile.AboutMeDescription,
                Skills = profile.Skills,
                Educations = profile.Educations,
                WorkExperiences = profile.WorkExperiences,
                HighlightProjects = profile.HighlightProjects,
                Certificates = profile.Certificates,
                Awards = profile.Awards,
                ForeignLanguages = profile.ForeignLanguages
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
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null) return NotFound();

            var result = profileStrengthService.Calculate(profile);
            return Ok(result);
        }
    }
}