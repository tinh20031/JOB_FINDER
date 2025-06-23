using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkExperienceController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public WorkExperienceController(JobFinderDbContext context) => _context = context;

        [HttpGet("me")]
        public async Task<IActionResult> GetForMe()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var works = await _context.WorkExperiences
                .Where(w => w.CandidateProfileId == candidateProfile.CandidateProfileId)
                .ToListAsync();

            return Ok(works);
        }

        [HttpPost("me")]
        public async Task<IActionResult> CreateForMe([FromBody] WorkExperience model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            model.CandidateProfileId = candidateProfile.CandidateProfileId;
            _context.WorkExperiences.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("me/{id}")]
        public async Task<IActionResult> UpdateForMe(int id, [FromBody] WorkExperience model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var work = await _context.WorkExperiences.FirstOrDefaultAsync(w => w.WorkExperienceId == id && w.CandidateProfileId == candidateProfile.CandidateProfileId);
            if (work == null) return NotFound();

            work.JobTitle = model.JobTitle;
            work.CompanyName = model.CompanyName;
            work.IsWorking = model.IsWorking;
            work.MonthStart = model.MonthStart;
            work.YearStart = model.YearStart;
            work.MonthEnd = model.MonthEnd;
            work.YearEnd = model.YearEnd;
            work.WorkDescription = model.WorkDescription;
            work.ProJects = model.ProJects;
            work.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var work = await _context.WorkExperiences.FindAsync(id);
            if (work == null) return NotFound();
            _context.WorkExperiences.Remove(work);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}