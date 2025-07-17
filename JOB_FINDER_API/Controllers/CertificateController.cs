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
    public class CertificateController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        public CertificateController(JobFinderDbContext context) => _context = context;

        [HttpGet("me")]
        public async Task<IActionResult> GetForMe()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var certificates = candidateProfile.Certificates ?? new List<CertificateInfo>();
            return Ok(certificates);
        }

        [HttpPost("me")]
        public async Task<IActionResult> CreateForMe([FromBody] CertificateInfo model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            // Lấy danh sách certificates hiện tại
            var certificates = candidateProfile.Certificates ?? new List<CertificateInfo>();

            // Thêm ID mới và timestamps
            model.Id = certificates.Count > 0 ? certificates.Max(c => c.Id) + 1 : 1;
            model.CreatedAt = DateTime.UtcNow;
            model.UpdatedAt = DateTime.UtcNow;

            // Thêm certificate mới
            certificates.Add(model);

            // Cập nhật lại danh sách
            candidateProfile.Certificates = certificates;
            candidateProfile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("me/{id}")]
        public async Task<IActionResult> UpdateForMe(int id, [FromBody] CertificateInfo model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var certificates = candidateProfile.Certificates ?? new List<CertificateInfo>();
            var certificate = certificates.FirstOrDefault(c => c.Id == id);
            if (certificate == null) return NotFound("Không tìm thấy chứng chỉ.");

            // Cập nhật thông tin
            certificate.CertificateName = model.CertificateName;
            certificate.Organization = model.Organization;
            certificate.Month = model.Month;
            certificate.Year = model.Year;
            certificate.CertificateUrl = model.CertificateUrl;
            certificate.CertificateDescription = model.CertificateDescription;
            certificate.UpdatedAt = DateTime.UtcNow;

            // Cập nhật lại danh sách
            candidateProfile.Certificates = certificates;
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

            var certificates = candidateProfile.Certificates ?? new List<CertificateInfo>();
            var certificate = certificates.FirstOrDefault(c => c.Id == id);
            if (certificate == null) return NotFound("Không tìm thấy chứng chỉ.");

            // Xóa certificate và cập nhật lại danh sách
            certificates.Remove(certificate);
            candidateProfile.Certificates = certificates;
            candidateProfile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}