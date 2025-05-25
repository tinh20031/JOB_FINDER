using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CandidateProfileController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public CandidateProfileController(JobFinderDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var profiles = await _context.CandidateProfiles
                .Include(cp => cp.CandidateSkills)
                .ToListAsync();
            return Ok(profiles);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(int userId)
        {
            var item = await _context.CandidateProfiles
                .Include(cp => cp.CandidateSkills)
                .FirstOrDefaultAsync(cp => cp.UserId == userId);
            return item == null ? NotFound() : Ok(item);
        }

        /*[HttpPost]
        public async Task<IActionResult> CreateCandidateProfile([FromBody] CreateCandidateProfileDto dto)
        {
            // Kiểm tra UserId đã có CandidateProfile chưa
            if (_context.CandidateProfiles.Any(cp => cp.UserId == dto.UserId))
                return BadRequest("CandidateProfile already exists for this UserId.");

            // Tạo mới CandidateProfile
            var profile = new CandidateProfile
            {
                UserId = dto.UserId,
                Gender = dto.Gender,
                Dob = dto.Dob,
                JobTitle = dto.JobTitle,
                Description = dto.Description,
                Address = dto.Address,
                Province = dto.Province,
                City = dto.City,
                Language = dto.Language
            };
            _context.CandidateProfiles.Add(profile);

            // Thêm các CandidateSkill
            foreach (var skillId in dto.SkillIds)
            {
                _context.CandidateSkill.Add(new CandidateSkill
                {
                    UserId = dto.UserId,
                    SkillId = skillId
                });
            }

            await _context.SaveChangesAsync();
            return Ok();
        }*/
        [HttpPost]
        public async Task<IActionResult> CreateCandidateProfile([FromBody] CreateCandidateProfileDto dto)
        {
            if (_context.CandidateProfiles.Any(cp => cp.UserId == dto.UserId))
                return BadRequest("CandidateProfile already exists for this UserId.");

            var profile = new CandidateProfile
            {
                UserId = dto.UserId,
                Gender = dto.Gender,
                Dob = dto.Dob,
                JobTitle = dto.JobTitle,
                Description = dto.Description,
                Address = dto.Address,
                Province = dto.Province,
                City = dto.City,
                Language = dto.Language
            };
            _context.CandidateProfiles.Add(profile);

            // Thêm nhiều skill cho candidate dựa trên danh sách SkillIds
            foreach (var skillId in dto.SkillIds)
            {
                _context.CandidateSkill.Add(new CandidateSkill
                {
                    UserId = dto.UserId,
                    SkillId = skillId
                });
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(int userId, CandidateProfile model)
        {
            if (userId != model.UserId) return BadRequest();

            var existingProfile = await _context.CandidateProfiles
                .Include(cp => cp.CandidateSkills)
                .FirstOrDefaultAsync(cp => cp.UserId == userId);

            if (existingProfile == null) return NotFound();

            // Cập nhật thông tin cơ bản
            existingProfile.Gender = model.Gender;
            existingProfile.Dob = model.Dob;
            existingProfile.JobTitle = model.JobTitle;
            existingProfile.Description = model.Description;
            existingProfile.Address = model.Address;
            existingProfile.Province = model.Province;
            existingProfile.City = model.City;
            existingProfile.Language = model.Language;

            // Cập nhật danh sách kỹ năng (nếu có)
            if (model.CandidateSkills != null)
            {
                // Xóa kỹ năng cũ
                _context.CandidateSkill.RemoveRange(existingProfile.CandidateSkills ?? new List<CandidateSkill>());
                // Thêm kỹ năng mới
                existingProfile.CandidateSkills = model.CandidateSkills;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(int userId)
        {
            var item = await _context.CandidateProfiles
                .Include(cp => cp.CandidateSkills)
                .FirstOrDefaultAsync(cp => cp.UserId == userId);
            if (item == null) return NotFound();
            _context.CandidateProfiles.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}