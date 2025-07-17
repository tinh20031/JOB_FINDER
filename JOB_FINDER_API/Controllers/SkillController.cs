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

            // Lấy danh sách kỹ năng từ JSON
            var skills = candidateProfile.Skills ?? new List<SkillInfo>();
            return Ok(skills);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null)
                return NotFound("Không tìm thấy CandidateProfile cho userId này.");

            var skills = candidateProfile.Skills ?? new List<SkillInfo>();
            return Ok(skills);
        }

        
        [HttpPost("me")]
        public async Task<IActionResult> CreateForMe([FromBody] object modelData)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            // Lấy danh sách kỹ năng hiện tại
            var skills = candidateProfile.Skills ?? new List<SkillInfo>();

            // Lấy tất cả Id đã tồn tại
            var existingIds = skills.Select(s => s.Id).ToHashSet();
            int nextId = skills.Count > 0 ? skills.Max(s => s.Id) + 1 : 1;

            List<SkillInfo> newSkills = new List<SkillInfo>();

            try
            {
                // Thử chuyển đổi thành một mảng SkillInfo
                var skillArray = System.Text.Json.JsonSerializer.Deserialize<List<SkillInfo>>(
                    System.Text.Json.JsonSerializer.Serialize(modelData),
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (skillArray != null && skillArray.Count > 0)
                {
                    // Xử lý khi dữ liệu là mảng
                    foreach (var model in skillArray)
                    {
                        // Nếu model.Id đã tồn tại hoặc không hợp lệ, gán Id mới
                        if (model.Id <= 0 || existingIds.Contains(model.Id))
                        {
                            model.Id = nextId++;
                        }
                        existingIds.Add(model.Id);
                        model.CreatedAt = DateTime.UtcNow;
                        model.UpdatedAt = DateTime.UtcNow;
                        newSkills.Add(model);
                        skills.Add(model);
                    }
                }
                else
                {
                    // Trường hợp mảng rỗng
                    return BadRequest("Không có dữ liệu kỹ năng hợp lệ.");
                }
            }
            catch
            {
                // Nếu không phải mảng, thử chuyển đổi thành một đối tượng SkillInfo đơn lẻ
                try
                {
                    var singleModel = System.Text.Json.JsonSerializer.Deserialize<SkillInfo>(
                        System.Text.Json.JsonSerializer.Serialize(modelData),
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (singleModel != null)
                    {
                        if (singleModel.Id <= 0 || existingIds.Contains(singleModel.Id))
                        {
                            singleModel.Id = nextId;
                        }
                        singleModel.CreatedAt = DateTime.UtcNow;
                        singleModel.UpdatedAt = DateTime.UtcNow;
                        newSkills.Add(singleModel);
                        skills.Add(singleModel);
                    }
                    else
                    {
                        return BadRequest("Dữ liệu kỹ năng không hợp lệ.");
                    }
                }
                catch
                {
                    return BadRequest("Định dạng dữ liệu không hợp lệ.");
                }
            }

            // Cập nhật lại danh sách
            candidateProfile.Skills = skills;
            candidateProfile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(newSkills);
        }

        [HttpPut("me/{id}")]
        public async Task<IActionResult> UpdateForMe(int id, [FromBody] SkillInfo model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            // Lấy danh sách kỹ năng hiện tại
            var skills = candidateProfile.Skills ?? new List<SkillInfo>();

            // Tìm kỹ năng cần cập nhật
            var skill = skills.FirstOrDefault(s => s.Id == id);
            if (skill == null) return NotFound("Không tìm thấy kỹ năng.");

            // Cập nhật thông tin
            skill.SkillName = model.SkillName;
            skill.GroupName = model.GroupName;
            skill.Experience = model.Experience;
            skill.Type = model.Type;
            skill.UpdatedAt = DateTime.UtcNow;

            // Lưu lại danh sách đã cập nhật
            candidateProfile.Skills = skills;
            candidateProfile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        
        [HttpDelete("me/{id}")]
        public async Task<IActionResult> DeleteForMe(int id)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var skills = candidateProfile.Skills ?? new List<SkillInfo>();
            // Log trước khi xóa
            Console.WriteLine("Before delete: " + string.Join(",", skills.Select(s => s.Id)));

            var skill = skills.FirstOrDefault(s => s.Id == id);
            if (skill == null) return NotFound("Không tìm thấy kỹ năng.");

            skills.Remove(skill);
            candidateProfile.Skills = skills;
            candidateProfile.UpdatedAt = DateTime.UtcNow;

            // Log sau khi xóa
            Console.WriteLine("After delete: " + string.Join(",", skills.Select(s => s.Id)));

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}