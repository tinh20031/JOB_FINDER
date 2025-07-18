using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CandidateToCompanyController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        private readonly IConfiguration _config;

        public CandidateToCompanyController(JobFinderDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestUpgrade([FromBody] JOB_FINDER_API.Models.Requests.CandidateToCompanyRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return BadRequest("User not found.");

            var existingRequest = await _context.CandidateToCompanyRequests
                .FirstOrDefaultAsync(r => r.UserId == request.UserId);
            if (existingRequest != null)
            {
                return BadRequest("You have submitted a request before please wait");
            }

            var entity = new JOB_FINDER_API.Models.CandidateToCompanyRequest
            {
                UserId = request.UserId,
                CompanyName = request.CompanyName,
                CompanyProfileDescription = request.CompanyProfileDescription,
                Location = request.Location,
                TeamSize = request.TeamSize,
                Website = request.Website,
                Contact = request.Contact,
                IndustryId = request.IndustryId,
                CreatedAt = DateTime.UtcNow
            };
            _context.CandidateToCompanyRequests.Add(entity);
            await _context.SaveChangesAsync();

            string baseUrl = _config["AppSettings:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("BaseUrl is not configured in appsettings.json.");
            }

            var adminEmail = _config["Admin:Email"];
            var subject = "Yêu cầu xác thực lên Company";
            var htmlBody = $@"
<html>
  <body style='font-family: Arial, sans-serif; background: #f6f6f6; padding: 30px;'>
    <div style='max-width: 600px; margin: auto; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px #eee; padding: 32px;'>
      <h2 style='color: #2d8cf0;'>Company Verification Request</h2>
      <p><b>Applicant:</b> {user.FullName} ({user.Email})</p>
      <p><b>Company Name:</b> {request.CompanyName}</p>
      <p><b>Description:</b> {request.CompanyProfileDescription}</p>
      <p><b>Address:</b> {request.Location}</p>
      <p><b>Team Size:</b> {request.TeamSize}</p>
      <p><b>Website:</b> {request.Website}</p>
      <p><b>Contact:</b> {request.Contact}</p>
      <p><b>Industry ID:</b> {request.IndustryId}</p>
      <div style='margin: 24px 0;'>
        <a href='{baseUrl}/admin-dashboard/user-manager/{user.UserId}' style='background: #2d8cf0; color: #fff; padding: 12px 24px; border-radius: 4px; text-decoration: none; font-weight: bold;'>Verify Now</a>
      </div>
      <p style='font-size: 13px; color: #888;'>Please verify this request if the information is valid.</p>
    </div>
  </body>
</html>
";
            SendEmail(adminEmail, subject, htmlBody, true);

            return Ok("Đã gửi yêu cầu lên admin.");
        }

        [HttpPost("verify/{userId}")]
        public async Task<IActionResult> VerifyUpgrade(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var request = await _context.CandidateToCompanyRequests
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync(r => r.UserId == userId);
            if (request == null)
                return BadRequest("Không tìm thấy request gốc.");

            var industry = await _context.Industries.FindAsync(request.IndustryId);
            if (industry == null)
                return BadRequest("IndustryId không hợp lệ.");

            user.RoleId = 2;

            if (!await _context.CompanyProfile.AnyAsync(c => c.UserId == userId))
            {
                var companyProfile = new CompanyProfile
                {
                    UserId = userId,
                    CompanyName = request.CompanyName,
                    CompanyProfileDescription = request.CompanyProfileDescription,
                    Location = request.Location,
                    TeamSize = request.TeamSize,
                    Website = request.Website,
                    Contact = request.Contact,
                    IndustryId = request.IndustryId,
                    IsVerified = true,
                    IsActive = true
                };
                _context.CompanyProfile.Add(companyProfile);
            }

            await _context.SaveChangesAsync();

            string baseUrl = _config["AppSettings:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("BaseUrl is not configured in appsettings.json.");
            }

            var htmlBody = $@"
<html>
  <body style='font-family: Arial, sans-serif; background: #f6f6f6; padding: 30px;'>
    <div style='max-width: 500px; margin: auto; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px #eee; padding: 32px;'>
      <h2 style='color: #2d8cf0; text-align: center;'>Congratulations, {user.FullName}!</h2>
      <p style='font-size: 16px; color: #333;'>
        Your account has been <b>successfully verified as a Company</b> on the <b>Job Finder</b> system.
      </p>
      <div style='margin: 24px 0; text-align: center;'>
        <a href='{baseUrl}/login' style='background: #2d8cf0; color: #fff; padding: 12px 24px; border-radius: 4px; text-decoration: none; font-weight: bold;'>Log in now</a>
      </div>
      <p style='font-size: 14px; color: #888; text-align: center;'>
        If you have any questions, please contact our support team.<br>
        Thank you for being part of Job Finder!
      </p>
    </div>
  </body>
</html>
";
            SendEmail(
                user.Email,
                "Thông báo xác thực lên Company thành công",
                htmlBody,
                true
            );

            return Ok("Đã xác thực và chuyển role thành công.");
        }

        private void SendEmail(string to, string subject, string body, bool isHtml = false)
        {
            var smtpHost = _config["Smtp:Host"];
            var smtpPort = int.Parse(_config["Smtp:Port"]);
            var smtpUser = _config["Smtp:User"];
            var smtpPass = _config["Smtp:Pass"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };
            var mail = new MailMessage(smtpUser, to, subject, body)
            {
                IsBodyHtml = isHtml
            };
            client.Send(mail);
        }
    }
}