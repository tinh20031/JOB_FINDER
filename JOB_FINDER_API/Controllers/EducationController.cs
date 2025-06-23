using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EducationController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public EducationController(JobFinderDbContext context) => _context = context;

        [HttpGet("me")]
        public async Task<IActionResult> GetForMe()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var educations = await _context.Educations
                .Where(e => e.CandidateProfileId == candidateProfile.CandidateProfileId)
                .ToListAsync();

            return Ok(educations);
        }

        [HttpPost("me")]
        public async Task<IActionResult> CreateForMe([FromBody] Education model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            model.CandidateProfileId = candidateProfile.CandidateProfileId;
            _context.Educations.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("me/{id}")]
        public async Task<IActionResult> UpdateForMe(int id, [FromBody] Education model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var education = await _context.Educations.FirstOrDefaultAsync(e => e.EducationId == id && e.CandidateProfileId == candidateProfile.CandidateProfileId);
            if (education == null) return NotFound();

            education.School = model.School;
            education.Degree = model.Degree;
            education.Major = model.Major;
            education.IsStudying = model.IsStudying;
            education.MonthStart = model.MonthStart;
            education.YearStart = model.YearStart;
            education.MonthEnd = model.MonthEnd;
            education.YearEnd = model.YearEnd;
            education.Detail = model.Detail;
            education.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var education = await _context.Educations.FindAsync(id);
            if (education == null) return NotFound();
            _context.Educations.Remove(education);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}