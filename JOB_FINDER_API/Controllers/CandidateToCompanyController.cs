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

        // Candidate gửi request lên admin
        [HttpPost("request")]
        public async Task<IActionResult> RequestUpgrade([FromBody] JOB_FINDER_API.Models.Requests.CandidateToCompanyRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return BadRequest("User not found.");

            // Lưu request vào database
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

            // Gửi mail cho admin
            var adminEmail = _config["Admin:Email"];
            var subject = "Yêu cầu xác thực lên Company";
            var body = $"User {user.FullName} ({user.Email}) muốn lên Company.\n" +
                       $"Tên công ty: {request.CompanyName}\n" +
                       $"Mô tả: {request.CompanyProfileDescription}\n" +
                       $"Địa chỉ: {request.Location}\n" +
                       $"TeamSize: {request.TeamSize}\n" +
                       $"Website: {request.Website}\n" +
                       $"Contact: {request.Contact}\n" +
                       $"IndustryId: {request.IndustryId}\n\n" +
                       $"Để xác thực, truy cập endpoint: /api/CandidateToCompany/verify/{user.Id}";

            SendEmail(adminEmail, subject, body);

            return Ok("Đã gửi yêu cầu lên admin.");
        }

        // Admin xác thực, tạo CompanyProfile và đổi role
        [HttpPost("verify/{userId}")]
        public async Task<IActionResult> VerifyUpgrade(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            // Lấy lại request gốc từ bảng tạm
            var request = await _context.CandidateToCompanyRequests
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync(r => r.UserId == userId);
            if (request == null)
                return BadRequest("Không tìm thấy request gốc.");

            // Kiểm tra IndustryId hợp lệ
            var industry = await _context.Industries.FindAsync(request.IndustryId);
            if (industry == null)
                return BadRequest("IndustryId không hợp lệ.");

            // Đổi role sang Company
            user.RoleId = 2;

            // Tạo CompanyProfile
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
            return Ok("Đã xác thực và chuyển role thành công.");
        }

        private void SendEmail(string to, string subject, string body)
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
            var mail = new MailMessage(smtpUser, to, subject, body);
            client.Send(mail);
        }
    }
}