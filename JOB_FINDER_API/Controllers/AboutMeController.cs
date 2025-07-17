using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using System.Text.Json;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AboutMeController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public AboutMeController(JobFinderDbContext context) => _context = context;

        [HttpGet("me")]
        public async Task<IActionResult> GetForMe()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            // Trả về thông tin AboutMe trực tiếp từ CandidateProfile
            var aboutMe = new
            {
                AboutMeDescription = candidateProfile.AboutMeDescription
            };

            return Ok(aboutMe);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null)
                return NotFound("Không tìm thấy CandidateProfile cho userId này.");

            var aboutMe = new
            {
                AboutMeDescription = candidateProfile.AboutMeDescription
            };

            return Ok(aboutMe);
        }

        [HttpPost("me")]
        public async Task<IActionResult> CreateForMe([FromBody] AboutMeRequest model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            candidateProfile.AboutMeDescription = model.AboutMeDescription;
            candidateProfile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateForMe([FromBody] AboutMeRequest model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            candidateProfile.AboutMeDescription = model.AboutMeDescription;
            candidateProfile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    public class AboutMeRequest
    {
        public string? AboutMeDescription { get; set; }
    }
}