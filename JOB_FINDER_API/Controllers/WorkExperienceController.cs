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

            var workExperiences = candidateProfile.WorkExperiences ?? new List<WorkExperienceInfo>();
            return Ok(workExperiences);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null)
                return NotFound("Không tìm thấy CandidateProfile cho userId này.");

            var workExperiences = candidateProfile.WorkExperiences ?? new List<WorkExperienceInfo>();
            return Ok(workExperiences);
        }

        [HttpPost("me")]
        public async Task<IActionResult> CreateForMe([FromBody] WorkExperienceInfo model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            // Lấy danh sách work experiences hiện tại
            var workExperiences = candidateProfile.WorkExperiences ?? new List<WorkExperienceInfo>();

            // Thêm ID mới và timestamps
            model.Id = workExperiences.Count > 0 ? workExperiences.Max(w => w.Id) + 1 : 1;
            model.CreatedAt = DateTime.UtcNow;
            model.UpdatedAt = DateTime.UtcNow;

            // Thêm work experience mới
            workExperiences.Add(model);

            // Cập nhật lại danh sách
            candidateProfile.WorkExperiences = workExperiences;
            candidateProfile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("me/{id}")]
        public async Task<IActionResult> UpdateForMe(int id, [FromBody] WorkExperienceInfo model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var workExperiences = candidateProfile.WorkExperiences ?? new List<WorkExperienceInfo>();
            var workExperience = workExperiences.FirstOrDefault(w => w.Id == id);
            if (workExperience == null) return NotFound("Không tìm thấy kinh nghiệm làm việc.");

            // Cập nhật thông tin
            workExperience.JobTitle = model.JobTitle;
            workExperience.CompanyName = model.CompanyName;
            workExperience.IsWorking = model.IsWorking;
            workExperience.MonthStart = model.MonthStart;
            workExperience.YearStart = model.YearStart;
            workExperience.MonthEnd = model.MonthEnd;
            workExperience.YearEnd = model.YearEnd;
            workExperience.WorkDescription = model.WorkDescription;
            workExperience.Technologies = model.Technologies;
            workExperience.Responsibilities = model.Responsibilities;
            workExperience.ProjectName = model.ProjectName;
            workExperience.Achievements = model.Achievements;
            workExperience.UpdatedAt = DateTime.UtcNow;

            // Cập nhật lại danh sách
            candidateProfile.WorkExperiences = workExperiences;
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

            var workExperiences = candidateProfile.WorkExperiences ?? new List<WorkExperienceInfo>();
            var workExperience = workExperiences.FirstOrDefault(w => w.Id == id);
            if (workExperience == null) return NotFound("Không tìm thấy kinh nghiệm làm việc.");

            // Xóa work experience và cập nhật lại danh sách
            workExperiences.Remove(workExperience);
            candidateProfile.WorkExperiences = workExperiences;
            candidateProfile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}