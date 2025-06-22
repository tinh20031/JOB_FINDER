using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForeignLanguageController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public ForeignLanguageController(JobFinderDbContext context) => _context = context;

        [HttpGet("me")]
        public async Task<IActionResult> GetForMe()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var langs = await _context.ForeignLanguages
                .Where(f => f.CandidateProfileId == candidateProfile.CandidateProfileId)
                .ToListAsync();

            return Ok(langs);
        }

        [HttpPost("me")]
        public async Task<IActionResult> CreateForMe([FromBody] ForeignLanguage model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            model.CandidateProfileId = candidateProfile.CandidateProfileId;
            _context.ForeignLanguages.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("me/{id}")]
        public async Task<IActionResult> UpdateForMe(int id, [FromBody] ForeignLanguage model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var lang = await _context.ForeignLanguages.FirstOrDefaultAsync(f => f.ForeignLanguageId == id && f.CandidateProfileId == candidateProfile.CandidateProfileId);
            if (lang == null) return NotFound();

            lang.LanguageName = model.LanguageName;
            lang.LanguageLevel = model.LanguageLevel;
            lang.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var lang = await _context.ForeignLanguages.FindAsync(id);
            if (lang == null) return NotFound();
            _context.ForeignLanguages.Remove(lang);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}