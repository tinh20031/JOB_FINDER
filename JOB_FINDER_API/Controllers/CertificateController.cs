using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;

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

            var certificates = await _context.Certificates
                .Where(c => c.CandidateProfileId == candidateProfile.CandidateProfileId)
                .ToListAsync();

            return Ok(certificates);
        }

        [HttpPost("me")]
        public async Task<IActionResult> CreateForMe([FromBody] Certificate model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");
          
            model.CandidateProfileId = candidateProfile.CandidateProfileId;
            _context.Certificates.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("me/{id}")]
        public async Task<IActionResult> UpdateForMe(int id, [FromBody] Certificate model)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var candidateProfile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (candidateProfile == null) return NotFound("Bạn chưa có CandidateProfile.");

            var certificate = await _context.Certificates.FirstOrDefaultAsync(c => c.CertificateId == id && c.CandidateProfileId == candidateProfile.CandidateProfileId);
            if (certificate == null) return NotFound();

            certificate.CertificateName = model.CertificateName;
            certificate.Organization = model.Organization;
            certificate.Month = model.Month;
            certificate.Year = model.Year;
            certificate.CertificateUrl = model.CertificateUrl;
            certificate.CertificateDescription = model.CertificateDescription;
            certificate.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null) return NotFound();
            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}