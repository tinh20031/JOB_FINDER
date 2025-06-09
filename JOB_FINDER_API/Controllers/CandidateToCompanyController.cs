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

        /*// Candidate gửi request lên admin
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
            // Gửi mail thông báo cho user
            SendEmail(
                user.Email,
                "Thông báo xác thực lên Company thành công",
                $"Chúc mừng {user.FullName}, tài khoản của bạn đã được xác thực lên Company thành công!"
            );
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
    }*/
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

            // Gửi mail cho admin với form HTML đẹp
            var adminEmail = _config["Admin:Email"];
            var subject = "Yêu cầu xác thực lên Company";
            var htmlBody = $@"
<html>
  <body style='font-family: Arial, sans-serif; background: #f6f6f6; padding: 30px;'>
    <div style='max-width: 600px; margin: auto; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px #eee; padding: 32px;'>
      <h2 style='color: #2d8cf0;'>Yêu cầu xác thực lên Company</h2>
      <p><b>Ứng viên:</b> {user.FullName} ({user.Email})</p>
      <p><b>Tên công ty:</b> {request.CompanyName}</p>
      <p><b>Mô tả:</b> {request.CompanyProfileDescription}</p>
      <p><b>Địa chỉ:</b> {request.Location}</p>
      <p><b>TeamSize:</b> {request.TeamSize}</p>
      <p><b>Website:</b> {request.Website}</p>
      <p><b>Contact:</b> {request.Contact}</p>
      <p><b>IndustryId:</b> {request.IndustryId}</p>
      <div style='margin: 24px 0;'>
        <a href='http://localhost:3000/admin-dashboard/user-manager/{user.Id}' style='background: #2d8cf0; color: #fff; padding: 12px 24px; border-radius: 4px; text-decoration: none; font-weight: bold;'>Xác thực ngay</a>
      </div>
      <p style='font-size: 13px; color: #888;'>Vui lòng xác thực yêu cầu này nếu hợp lệ.</p>
    </div>
  </body>
</html>
";
            SendEmail(adminEmail, subject, htmlBody, true);

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

            // Gửi mail thông báo cho user với form HTML đẹp
            var htmlBody = $@"
<html>
  <body style='font-family: Arial, sans-serif; background: #f6f6f6; padding: 30px;'>
    <div style='max-width: 500px; margin: auto; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px #eee; padding: 32px;'>
      <h2 style='color: #2d8cf0; text-align: center;'>Chúc mừng {user.FullName}!</h2>
      <p style='font-size: 16px; color: #333;'>
        Tài khoản của bạn đã được <b>xác thực lên Company</b> thành công trên hệ thống <b>Job Finder</b>.
      </p>
      <div style='margin: 24px 0; text-align: center;'>
        <a href='http://localhost:3000/login' style='background: #2d8cf0; color: #fff; padding: 12px 24px; border-radius: 4px; text-decoration: none; font-weight: bold;'>Đăng nhập ngay</a>
      </div>
      <p style='font-size: 14px; color: #888; text-align: center;'>
        Nếu bạn có bất kỳ thắc mắc nào, vui lòng liên hệ bộ phận hỗ trợ của chúng tôi.<br>
        Cảm ơn bạn đã đồng hành cùng Job Finder!
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

        // Hỗ trợ gửi email HTML
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