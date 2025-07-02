using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public SkillController(JobFinderDbContext context) => _context = context;

        [HttpGet("profile/{candidateProfileId}")]
        public async Task<IActionResult> GetByProfile(int candidateProfileId)
        {
            var skills = await _context.Skills.Where(s => s.CandidateProfileId == candidateProfileId).ToListAsync();
            return Ok(skills);
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null)
                return NotFound("Không tìm thấy CandidateProfile cho userId này.");

            var skills = await _context.Skills
                .Where(s => s.CandidateProfileId == candidateProfile.CandidateProfileId)
                .ToListAsync();

            return Ok(skills);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Skill model)
        {
            _context.Skills.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Skill model)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null) return NotFound();

            skill.SkillName = model.SkillName;
            skill.GroupName = model.GroupName;
            skill.Experience = model.Experience;
            skill.Type = model.Type;
            skill.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /*[HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null) return NotFound();
            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
            return NoContent();
        }*/
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null) return NotFound();

            // Nếu skill đang liên kết với CandidateProfile, chỉ xóa liên kết
            skill.CandidateProfileId = null; // hoặc null nếu cho phép null
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetForMe()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var skills = await _context.Skills
                .Where(s => s.CandidateProfileId == candidateProfile.CandidateProfileId)
                .ToListAsync();

            return Ok(skills);
        }
        [HttpPost("me")]
        public async Task<IActionResult> CreateForMe([FromBody] Skill model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            model.CandidateProfileId = candidateProfile.CandidateProfileId;
            _context.Skills.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("me/{id}")]
        public async Task<IActionResult> UpdateForMe(int id, [FromBody] Skill model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var skill = await _context.Skills.FirstOrDefaultAsync(s => s.SkillId == id && s.CandidateProfileId == candidateProfile.CandidateProfileId);
            if (skill == null) return NotFound();

            skill.SkillName = model.SkillName;
            skill.GroupName = model.GroupName;
            skill.Experience = model.Experience;
            skill.Type = model.Type;
            skill.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}