using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.DTO;
using JOB_FINDER_API.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CandidateToCompanyController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<CandidateToCompanyController> _logger;

        public CandidateToCompanyController(JobFinderDbContext context, IConfiguration config, ILogger<CandidateToCompanyController> _logger)
        {
            _context = context;
            _config = config;
            this._logger = _logger;
        }
        [HttpPost("request")]
        public async Task<IActionResult> RequestUpgrade([FromBody] JOB_FINDER_API.Models.Requests.CandidateToCompanyRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var existingRequest = await _context.CandidateToCompanyRequests
                .FirstOrDefaultAsync(r => r.UserId == request.UserId);

            if (existingRequest != null)
            {

                if (existingRequest.Status == RequestStatus.Pending || existingRequest.Status == RequestStatus.Approved)
                {
                    return BadRequest("You have submitted a request before please wait");
                }

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
                CreatedAt = DateTime.UtcNow,
                Status = RequestStatus.Pending
            };
            _context.CandidateToCompanyRequests.Add(entity);
            await _context.SaveChangesAsync();

            string baseUrl = _config["AppSettings:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("BaseUrl is not configured in appsettings.json.");
            }

            var adminEmail = _config["Admin:Email"];
            var subject = "Send authentication request to the Company";
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
        <a href='{baseUrl}/admin-dashboard/upgrade-requests' style='background: #2d8cf0; color: #fff; padding: 12px 24px; border-radius: 4px; text-decoration: none; font-weight: bold;'>Verify Now</a>
      </div>
      <p style='font-size: 13px; color: #888;'>Please verify this request if the information is valid.</p>
    </div>
  </body>
</html>
";
            SendEmail(adminEmail, subject, htmlBody, true);

            return Ok("The request has been sent to the admin.");
        }
        private async Task DeleteCandidateSubscription(int userId)
        {
            try
            {
                var candidateSubscriptions = await _context.CandidateSubscriptions
                    .Where(s => s.UserId == userId)
                    .ToListAsync();

                if (candidateSubscriptions.Any())
                {
                    _context.CandidateSubscriptions.RemoveRange(candidateSubscriptions);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Deleted candidate subscriptions for user {userId} during role upgrade");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting candidate subscriptions for user {userId}");
            }
        }

        private async Task CreateInitialCompanySubscription(int userId)
        {
            try
            {
                // Find the free subscription type
                var freeSubscription = await _context.CompanySubscriptionTypes
                    .FirstOrDefaultAsync(s => s.PackageType == CompanySubscriptionPackageType.Free);

                if (freeSubscription == null)
                {
                    _logger.LogError("Free company subscription type not found");
                    return;
                }

                // Create a new subscription entry for the free tier
                var subscription = new CompanySubscription
                {
                    UserId = userId,
                    CompanySubscriptionTypeId = freeSubscription.CompanySubscriptionTypeId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(freeSubscription.DurationInDays),
                    IsActive = true,
                    RemainingJobPosts = freeSubscription.JobPostLimit,
                    RemainingTrendingJobPosts = freeSubscription.TrendingJobLimit, // Set trending job posts limit
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.CompanySubscriptions.Add(subscription);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Created initial free subscription for company user {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating initial company subscription for user {userId}");
            }
        }



        [HttpGet("requests")]
        public async Task<IActionResult> GetRequests()
        {
            try
            {
                var requests = await _context.CandidateToCompanyRequests
                    .Include(r => r.User)
                    .Include(r => r.Industry)
                    .Select(r => new UpgradeRequestDto
                    {
                        CandidateToCompanyRequestId = r.CandidateToCompanyRequestId,
                        UserId = r.UserId,
                        CompanyName = r.CompanyName,
                        CompanyProfileDescription = r.CompanyProfileDescription,
                        Location = r.Location,
                        TeamSize = r.TeamSize,
                        Website = r.Website,
                        Contact = r.Contact,
                        IndustryId = r.IndustryId,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt,
                        Status = r.Status,
                        FullName = r.User.FullName,
                        Email = r.User.Email,
                        Image = r.User.Image,
                        IndustryName = r.Industry.IndustryName
                    })
                    .ToListAsync();

                if (requests == null || !requests.Any())
                {
                    return NotFound("No pending requests found.");
                }

                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving candidate to company requests");
                return StatusCode(500, "An error occurred while retrieving requests.");
            }
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



        [HttpPost("process-upgrade/{userId}")]
        public async Task<IActionResult> ProcessRoleUpgrade(int userId, [FromBody] UpgradeDecision decision)
        {

            _logger.LogInformation("Processing role upgrade request for userId: {UserId}", userId);


            if (decision == null || string.IsNullOrEmpty(decision.Decision))
            {
                _logger.LogWarning("Invalid request body: Decision is required");
                return BadRequest("Request body must contain a valid Decision (approve or reject).");
            }

            string decisionValue = decision.Decision.ToLower();
            if (decisionValue != "approve" && decisionValue != "reject")
            {
                _logger.LogWarning("Invalid decision value: {Decision}", decisionValue);
                return BadRequest("Decision must be 'approve' or 'reject'.");
            }


            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found for userId: {UserId}", userId);
                return NotFound("User not found.");
            }


            var request = await _context.CandidateToCompanyRequests
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync(r => r.UserId == userId);
            if (request == null)
            {
                _logger.LogWarning("No request found for userId: {UserId}", userId);
                return NotFound("Không tìm thấy request gốc.");
            }


            var industry = await _context.Industries.FindAsync(request.IndustryId);
            if (industry == null)
            {
                _logger.LogWarning("Invalid IndustryId: {IndustryId} for userId: {UserId}", request.IndustryId, userId);
                return BadRequest("IndustryId không hợp lệ.");
            }


            string baseUrl = _config["AppSettings:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                _logger.LogError("BaseUrl is not configured in appsettings.json.");
                return StatusCode(500, "Server configuration error: BaseUrl is missing.");
            }

            try
            {
                if (decisionValue == "approve")
                {

                    user.RoleId = 2;
                    request.Status = RequestStatus.Approved;

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

                    await DeleteCandidateSubscription(userId);
                    await CreateInitialCompanySubscription(userId);

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
</html>";
                    SendEmail(user.Email, "Company verification successful", htmlBody, true);
                }
                else // reject
                {
                    // Từ chối (reject)
                    request.Status = RequestStatus.Rejected;

                    var htmlBody = $@"
<html>
  <body style='font-family: Arial, sans-serif; background: #f6f6f6; padding: 30px;'>
    <div style='max-width: 500px; margin: auto; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px #eee; padding: 32px;'>
      <h2 style='color: #dc3545; text-align: center;'>Verification Declined, {user.FullName}</h2>
      <p style='font-size: 16px; color: #333;'>
        We regret to inform you that your request to become a Company on the <b>Job Finder</b> system has been declined.
      </p>
      <p style='font-size: 14px; color: #888; text-align: center;'>
        If you believe this is an error or need assistance, please contact our support team.<br>
        Thank you for your understanding!
      </p>
    </div>
  </body>
</html>";
                    SendEmail(user.Email, "Company verification request denied", htmlBody, true);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Role upgrade request processed for userId: {UserId} with decision: {Decision}", userId, decisionValue);
                return Ok($"Role upgrade request processed with status: {decisionValue}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing role upgrade for userId: {UserId}", userId);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        public class UpgradeDecision
        {
            public string Decision { get; set; } = string.Empty;
        }
    }
}

