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

            var awards = candidateProfile.Awards ?? new List<AwardInfo>();
            return Ok(awards);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null)
                return NotFound("Không tìm thấy CandidateProfile cho userId này.");

            var awards = candidateProfile.Awards ?? new List<AwardInfo>();
            return Ok(awards);
        }

        [HttpPost("me")]
        public async Task<IActionResult> CreateForMe([FromBody] AwardInfo model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            // Lấy danh sách awards hiện tại
            var awards = candidateProfile.Awards ?? new List<AwardInfo>();

            // Thêm ID mới và timestamps
            model.Id = awards.Count > 0 ? awards.Max(a => a.Id) + 1 : 1;
            model.CreatedAt = DateTime.UtcNow;
            model.UpdatedAt = DateTime.UtcNow;

            // Thêm award mới
            awards.Add(model);

            // Cập nhật lại danh sách
            candidateProfile.Awards = awards;
            candidateProfile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("me/{id}")]
        public async Task<IActionResult> UpdateForMe(int id, [FromBody] AwardInfo model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var awards = candidateProfile.Awards ?? new List<AwardInfo>();
            var award = awards.FirstOrDefault(a => a.Id == id);
            if (award == null) return NotFound("Không tìm thấy giải thưởng.");

            // Cập nhật thông tin
            award.AwardName = model.AwardName;
            award.AwardOrganization = model.AwardOrganization;
            award.Month = model.Month;
            award.Year = model.Year;
            award.AwardDescription = model.AwardDescription;
            award.UpdatedAt = DateTime.UtcNow;

            // Cập nhật lại danh sách
            candidateProfile.Awards = awards;
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

            var awards = candidateProfile.Awards ?? new List<AwardInfo>();
            var award = awards.FirstOrDefault(a => a.Id == id);
            if (award == null) return NotFound("Không tìm thấy giải thưởng.");

            // Xóa award và cập nhật lại danh sách
            awards.Remove(award);
            candidateProfile.Awards = awards;
            candidateProfile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}