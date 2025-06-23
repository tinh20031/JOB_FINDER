using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HighlightProjectController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public HighlightProjectController(JobFinderDbContext context) => _context = context;

        [HttpGet("me")]
        public async Task<IActionResult> GetForMe()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var projects = await _context.HighlightProjects
                .Where(p => p.CandidateProfileId == candidateProfile.CandidateProfileId)
                .ToListAsync();

            return Ok(projects);
        }

        [HttpPost("me")]
        public async Task<IActionResult> CreateForMe([FromBody] HighlightProject model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            model.CandidateProfileId = candidateProfile.CandidateProfileId;
            _context.HighlightProjects.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("me/{id}")]
        public async Task<IActionResult> UpdateForMe(int id, [FromBody] HighlightProject model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var project = await _context.HighlightProjects.FirstOrDefaultAsync(p => p.HighlightProjectId == id && p.CandidateProfileId == candidateProfile.CandidateProfileId);
            if (project == null) return NotFound();

            project.ProjectName = model.ProjectName;
            project.IsWorking = model.IsWorking;
            project.MonthStart = model.MonthStart;
            project.YearStart = model.YearStart;
            project.MonthEnd = model.MonthEnd;
            project.YearEnd = model.YearEnd;
            project.ProjectDescription = model.ProjectDescription;
            project.ProjectLink = model.ProjectLink;
            project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.HighlightProjects.FindAsync(id);
            if (project == null) return NotFound();
            _context.HighlightProjects.Remove(project);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}