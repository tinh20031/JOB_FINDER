using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;

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

            var aboutMe = await _context.AboutMes
                .FirstOrDefaultAsync(a => a.CandidateProfileId == candidateProfile.CandidateProfileId);

            return Ok(aboutMe);
        }

        [HttpPost("me")]
        public async Task<IActionResult> CreateForMe([FromBody] AboutMe model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            model.CandidateProfileId = candidateProfile.CandidateProfileId;
            _context.AboutMes.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("me/{id}")]
        public async Task<IActionResult> UpdateForMe(int id, [FromBody] AboutMe model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var aboutMe = await _context.AboutMes
                .FirstOrDefaultAsync(a => a.AboutMeId == id && a.CandidateProfileId == candidateProfile.CandidateProfileId);
            if (aboutMe == null) return NotFound();

            aboutMe.AboutMeDescription = model.AboutMeDescription;
            aboutMe.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            // Tìm CandidateProfile theo userId
            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null)
                return NotFound("Không tìm thấy CandidateProfile cho userId này.");

            // Tìm AboutMe theo CandidateProfileId
            var aboutMe = await _context.AboutMes
                .FirstOrDefaultAsync(a => a.CandidateProfileId == candidateProfile.CandidateProfileId);

            if (aboutMe == null)
                return NotFound("Không tìm thấy AboutMe cho userId này.");  

            return Ok(aboutMe);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var aboutme = await _context.AboutMes.FindAsync(id);
            if (aboutme == null) return NotFound();
            _context.AboutMes.Remove(aboutme);
            await _context.SaveChangesAsync();
            return NoContent();
        }



        //[HttpGet("{userId}")]
        //public async Task<IActionResult> GetByUserId(int userId)
        //{
        //    // Tìm CandidateProfile theo userId
        //    var candidateProfile = await _context.CandidateProfiles
        //        .FirstOrDefaultAsync(p => p.UserId == userId);
        //    if (candidateProfile == null)
        //        return NotFound("Không tìm thấy CandidateProfile cho userId này.");

        //    // Tìm AboutMe theo CandidateProfileId
        //    var aboutMe = await _context.AboutMes
        //        .FirstOrDefaultAsync(a => a.CandidateProfileId == candidateProfile.CandidateProfileId);

        //    if (aboutMe == null)
        //        return NotFound("Không tìm thấy AboutMe cho userId này.");

        //    return Ok(aboutMe);
        //}



    }
}