using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CandidateSkillController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public CandidateSkillController(JobFinderDbContext context) => _context = context;

        // Lấy tất cả kỹ năng của 1 user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetSkillsForUser(int userId)
        {
            var skills = await _context.CandidateSkill
                .Where(cs => cs.UserId == userId)
                .ToListAsync();
            return Ok(skills);
        }

        // Lấy 1 kỹ năng theo id
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var skill = await _context.CandidateSkill.FindAsync(id);
            return skill == null ? NotFound() : Ok(skill);
        }

        // Thêm kỹ năng cho user
        [HttpPost]
        public async Task<IActionResult> AddSkill([FromBody] CandidateSkill model)
        {
            // Đảm bảo UserId hợp lệ (nên kiểm tra tồn tại User nếu cần)
            _context.CandidateSkill.Add(model);
            await _context.SaveChangesAsync();
            // Sử dụng đúng tên hàm Get để tránh lỗi
            return CreatedAtAction(nameof(Get), new { id = model.CandidateSkillId }, model);
        }

        // Xóa kỹ năng khỏi user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var skill = await _context.CandidateSkill.FindAsync(id);
            if (skill == null) return NotFound();
            _context.CandidateSkill.Remove(skill);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}