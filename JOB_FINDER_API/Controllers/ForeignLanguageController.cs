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
    public class ForeignLanguageController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public ForeignLanguageController(JobFinderDbContext context) => _context = context;

        [HttpGet("me")]
        public async Task<IActionResult> GetForMe()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var languages = candidateProfile.ForeignLanguages ?? new List<ForeignLanguageInfo>();
            return Ok(languages);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null)
                return NotFound("Không tìm thấy CandidateProfile cho userId này.");

            var languages = candidateProfile.ForeignLanguages ?? new List<ForeignLanguageInfo>();
            return Ok(languages);
        }

        /*[HttpPost("me")]
        public async Task<IActionResult> CreateForMe([FromBody] ForeignLanguageInfo model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            // Lấy danh sách languages hiện tại
            var languages = candidateProfile.ForeignLanguages ?? new List<ForeignLanguageInfo>();

            // Thêm ID mới và timestamps
            model.Id = languages.Count > 0 ? languages.Max(l => l.Id) + 1 : 1;
            model.CreatedAt = DateTime.UtcNow;
            model.UpdatedAt = DateTime.UtcNow;

            // Thêm language mới
            languages.Add(model);

            // Cập nhật lại danh sách
            candidateProfile.ForeignLanguages = languages;
            candidateProfile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(model);
        }*/
        [HttpPost("me")]
        public async Task<IActionResult> CreateForMe([FromBody] object modelData)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            // Lấy danh sách languages hiện tại
            var languages = candidateProfile.ForeignLanguages ?? new List<ForeignLanguageInfo>();

            // Lấy tất cả Id đã tồn tại
            var existingIds = languages.Select(l => l.Id).ToHashSet();
            int nextId = languages.Count > 0 ? languages.Max(l => l.Id) + 1 : 1;

            List<ForeignLanguageInfo> newLanguages = new List<ForeignLanguageInfo>();

            try
            {
                // Thử chuyển đổi thành một mảng ForeignLanguageInfo
                var langArray = System.Text.Json.JsonSerializer.Deserialize<List<ForeignLanguageInfo>>(
                    System.Text.Json.JsonSerializer.Serialize(modelData),
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (langArray != null && langArray.Count > 0)
                {
                    foreach (var model in langArray)
                    {
                        if (model.Id <= 0 || existingIds.Contains(model.Id))
                        {
                            model.Id = nextId++;
                        }
                        existingIds.Add(model.Id);
                        model.CreatedAt = DateTime.UtcNow;
                        model.UpdatedAt = DateTime.UtcNow;
                        newLanguages.Add(model);
                        languages.Add(model);
                    }
                }
                else
                {
                    return BadRequest("Không có dữ liệu ngoại ngữ hợp lệ.");
                }
            }
            catch
            {
                // Nếu không phải mảng, thử chuyển đổi thành một đối tượng ForeignLanguageInfo đơn lẻ
                try
                {
                    var singleModel = System.Text.Json.JsonSerializer.Deserialize<ForeignLanguageInfo>(
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
                        newLanguages.Add(singleModel);
                        languages.Add(singleModel);
                    }
                    else
                    {
                        return BadRequest("Dữ liệu ngoại ngữ không hợp lệ.");
                    }
                }
                catch
                {
                    return BadRequest("Định dạng dữ liệu không hợp lệ.");
                }
            }

            // Cập nhật lại danh sách
            candidateProfile.ForeignLanguages = languages;
            candidateProfile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(newLanguages);
        }

        [HttpPut("me/{id}")]
        public async Task<IActionResult> UpdateForMe(int id, [FromBody] ForeignLanguageInfo model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var languages = candidateProfile.ForeignLanguages ?? new List<ForeignLanguageInfo>();
            var language = languages.FirstOrDefault(l => l.Id == id);
            if (language == null) return NotFound("Không tìm thấy ngoại ngữ.");

            // Cập nhật thông tin
            language.LanguageName = model.LanguageName;
            language.LanguageLevel = model.LanguageLevel;
            language.UpdatedAt = DateTime.UtcNow;

            // Cập nhật lại danh sách
            candidateProfile.ForeignLanguages = languages;
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

            var languages = candidateProfile.ForeignLanguages ?? new List<ForeignLanguageInfo>();
            var language = languages.FirstOrDefault(l => l.Id == id);
            if (language == null) return NotFound("Không tìm thấy ngoại ngữ.");

            // Xóa language và cập nhật lại danh sách
            languages.Remove(language);
            candidateProfile.ForeignLanguages = languages;
            candidateProfile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}