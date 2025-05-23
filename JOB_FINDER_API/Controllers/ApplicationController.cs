using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.Requests;
using JOB_FINDER_API.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public ApplicationController(JobFinderDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.Applications.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _context.Applications.FindAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Application model)
        {
            _context.Applications.Add(model);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Application model)
        {
            if (id != model.Id) return BadRequest();
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Applications.FindAsync(id);
            if (item == null) return NotFound();
            _context.Applications.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        
        [Authorize]
        [HttpPost("apply")]
        public async Task<IActionResult> Apply(
    [FromBody] ApplyJobRequest request,
    [FromServices] ICvSnapshotService cvSnapshotService)
        {
            // Lấy userId từ claim
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.Name)!.Value);

            // Lấy CV của user
            var cv = await _context.CVs.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cv == null)
                return BadRequest("CV not found");

            // Snapshot toàn bộ các trang CV
            var snapshotUrls = await cvSnapshotService.CaptureCvAsImagesAsync(cv);

            // Nếu không có snapshot nào, báo lỗi
            if (snapshotUrls == null || snapshotUrls.Count == 0)
                return BadRequest("Failed to snapshot CV");

            // Tạo Application mới, lưu snapshot đầu tiên hoặc tất cả snapshot (tùy nhu cầu)
            var application = new Application
            {
                UserId = userId,
                JobId = request.JobId,
                CvId = cv.Id,
                CoverLetter = request.CoverLetter,
                ResumeUrl = cv.FileUrl,
                SnapshotCv = snapshotUrls.First(), // hoặc string.Join(";", snapshotUrls) nếu muốn lưu nhiều
                Status = ApplicationStatus.Pending,
                SubmittedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Applied successfully",
                ApplicationId = application.Id,
                SnapshotUrls = snapshotUrls // trả về danh sách snapshot cho client nếu muốn
            });
        }
    }
}