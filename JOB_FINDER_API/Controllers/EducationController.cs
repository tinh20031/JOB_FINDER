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

            var educations = candidateProfile.Educations ?? new List<EducationInfo>();
            return Ok(educations);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null)
                return NotFound("Không tìm thấy CandidateProfile cho userId này.");

            var educations = candidateProfile.Educations ?? new List<EducationInfo>();
            return Ok(educations);
        }

        [HttpPost("me")]
        public async Task<IActionResult> CreateForMe([FromBody] EducationInfo model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            // Lấy danh sách educations hiện tại
            var educations = candidateProfile.Educations ?? new List<EducationInfo>();

            // Thêm ID mới và timestamps
            model.Id = educations.Count > 0 ? educations.Max(e => e.Id) + 1 : 1;
            model.CreatedAt = DateTime.UtcNow;
            model.UpdatedAt = DateTime.UtcNow;

            // Thêm education mới
            educations.Add(model);

            // Cập nhật lại danh sách
            candidateProfile.Educations = educations;
            candidateProfile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("me/{id}")]
        public async Task<IActionResult> UpdateForMe(int id, [FromBody] EducationInfo model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var educations = candidateProfile.Educations ?? new List<EducationInfo>();
            var education = educations.FirstOrDefault(e => e.Id == id);
            if (education == null) return NotFound("Không tìm thấy thông tin giáo dục.");

            // Cập nhật thông tin
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

            // Cập nhật lại danh sách
            candidateProfile.Educations = educations;
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

            var educations = candidateProfile.Educations ?? new List<EducationInfo>();
            var education = educations.FirstOrDefault(e => e.Id == id);
            if (education == null) return NotFound("Không tìm thấy thông tin giáo dục.");

            // Xóa education và cập nhật lại danh sách
            educations.Remove(education);
            candidateProfile.Educations = educations;
            candidateProfile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}