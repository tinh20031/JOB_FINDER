using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            var projects = candidateProfile.HighlightProjects ?? new List<HighlightProjectInfo>();
            return Ok(projects);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null)
                return NotFound("Không tìm thấy CandidateProfile cho userId này.");

            var projects = candidateProfile.HighlightProjects ?? new List<HighlightProjectInfo>();
            return Ok(projects);
        }

        [HttpPost("me")]
        public async Task<IActionResult> CreateForMe([FromBody] HighlightProjectInfo model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            // Lấy danh sách projects hiện tại
            var projects = candidateProfile.HighlightProjects ?? new List<HighlightProjectInfo>();

            // Thêm ID mới và timestamps
            model.Id = projects.Count > 0 ? projects.Max(p => p.Id) + 1 : 1;
            model.CreatedAt = DateTime.UtcNow;
            model.UpdatedAt = DateTime.UtcNow;

            // Thêm project mới
            projects.Add(model);

            // Cập nhật lại danh sách
            candidateProfile.HighlightProjects = projects;
            candidateProfile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("me/{id}")]
        public async Task<IActionResult> UpdateForMe(int id, [FromBody] HighlightProjectInfo model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var projects = candidateProfile.HighlightProjects ?? new List<HighlightProjectInfo>();
            var project = projects.FirstOrDefault(p => p.Id == id);
            if (project == null) return NotFound("Không tìm thấy dự án nổi bật.");

            // Cập nhật thông tin
            project.ProjectName = model.ProjectName;
            project.IsWorking = model.IsWorking;
            project.MonthStart = model.MonthStart;
            project.YearStart = model.YearStart;
            project.MonthEnd = model.MonthEnd;
            project.YearEnd = model.YearEnd;
            project.ProjectDescription = model.ProjectDescription;
            project.Technologies = model.Technologies;
            project.Responsibilities = model.Responsibilities;
            project.TeamSize = model.TeamSize;
            project.Achievements = model.Achievements;
            project.ProjectLink = model.ProjectLink;
            project.UpdatedAt = DateTime.UtcNow;

            // Cập nhật lại danh sách
            candidateProfile.HighlightProjects = projects;
            candidateProfile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var projects = candidateProfile.HighlightProjects ?? new List<HighlightProjectInfo>();
            var project = projects.FirstOrDefault(p => p.Id == id);
            if (project == null) return NotFound("Không tìm thấy dự án nổi bật.");

            // Xóa project và cập nhật lại danh sách
            projects.Remove(project);
            candidateProfile.HighlightProjects = projects;
            candidateProfile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}