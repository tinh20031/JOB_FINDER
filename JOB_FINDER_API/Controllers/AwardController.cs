using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AwardController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public AwardController(JobFinderDbContext context) => _context = context;

        [HttpGet("me")]
        public async Task<IActionResult> GetForMe()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var awards = await _context.Awards
                .Where(a => a.CandidateProfileId == candidateProfile.CandidateProfileId)
                .ToListAsync();

            return Ok(awards);
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null)
                return NotFound("Không tìm thấy CandidateProfile cho userId này.");

            var awards = await _context.Awards
                .Where(a => a.CandidateProfileId == candidateProfile.CandidateProfileId)
                .ToListAsync();

            return Ok(awards);
        }

        [HttpPost("me")]
        public async Task<IActionResult> CreateForMe([FromBody] Award model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            model.CandidateProfileId = candidateProfile.CandidateProfileId;
            _context.Awards.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("me/{id}")]
        public async Task<IActionResult> UpdateForMe(int id, [FromBody] Award model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var award = await _context.Awards.FirstOrDefaultAsync(a => a.AwardId == id && a.CandidateProfileId == candidateProfile.CandidateProfileId);
            if (award == null) return NotFound();

            award.AwardName = model.AwardName;
            award.AwardOrganization = model.AwardOrganization;
            award.Month = model.Month;
            award.Year = model.Year;
            award.AwardDescription = model.AwardDescription;
            award.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var award = await _context.Awards.FindAsync(id);
            if (award == null) return NotFound();
            _context.Awards.Remove(award);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}