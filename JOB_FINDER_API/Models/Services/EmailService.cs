using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace JOB_FINDER_API.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(string to, string subject, string body, bool isHtml = false)
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

        // Bổ sung hàm async cho background service
        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
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
            await client.SendMailAsync(mail);
        }

        public void SendVerificationEmail(string to, string verificationCode)
        {
            string subject = "Verify Job Finder account email";

            string body = $@"
<html>
  <body style='font-family: Arial, sans-serif; background: #f6f6f6; padding: 30px;'>
    <div style='max-width: 600px; margin: auto; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px #eee; padding: 32px;'>
      <h2 style='color: #2d8cf0;'>Verify your email</h2>
      <p>Hello,</p>
      <p>Thank you for registering an account on the Job Finder system. To complete the registration process, please use the verification code below:</p>
      <div style='margin: 24px 0; text-align: center;'>
        <h1 style='font-size: 32px; letter-spacing: 5px; color: #2d8cf0; border: 2px dashed #2d8cf0; display: inline-block; padding: 10px 20px; border-radius: 5px;'>{verificationCode}</h1>
      </div>
      <p>The verification code is valid for 24 hours. If you did not request this code, please ignore this email.</p>
      <p>Best regards,<br>Job Finder Team</p>
    </div>
  </body>
</html>
";

            SendEmail(to, subject, body, true);
        }
    }
}