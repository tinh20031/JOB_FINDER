using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.DTO;
using JOB_FINDER_API.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace JOB_FINDER_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<MessageController> _logger;

        public MessageController(JobFinderDbContext context, IHubContext<ChatHub> hubContext, ILogger<MessageController> logger)
        {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;
        }

        [HttpGet("history/{userId1}/{userId2}")]
        public async Task<IActionResult> GetMessageHistory(int userId1, int userId2)
        {
            var messages = await _context.Messages
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                            (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderBy(m => m.SentAt)
                .Select(m => new
                {
                    MessageId = m.Id,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    SentAt = m.SentAt,
                    MessageText = m.MessageText,
                    // Lấy thông tin sender
                    SenderFullName = _context.Users.Where(u => u.Id == m.SenderId).Select(u => u.FullName).FirstOrDefault(),
                    SenderImage = _context.Users.Where(u => u.Id == m.SenderId).Select(u => u.Image).FirstOrDefault(),
                    SenderRole = _context.Users
                        .Where(u => u.Id == m.SenderId)
                        .Join(_context.Roles, u => u.RoleId, r => r.RoleId, (u, r) => r.RoleName)
                        .FirstOrDefault(),
                    // Lấy thông tin receiver
                    ReceiverFullName = _context.Users.Where(u => u.Id == m.ReceiverId).Select(u => u.FullName).FirstOrDefault(),
                    ReceiverImage = _context.Users.Where(u => u.Id == m.ReceiverId).Select(u => u.Image).FirstOrDefault(),
                    ReceiverRole = _context.Users
                        .Where(u => u.Id == m.ReceiverId)
                        .Join(_context.Roles, u => u.RoleId, r => r.RoleId, (u, r) => r.RoleName)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(messages);
        }

        [HttpGet("candidates-messaged/{companyId}")]
        public async Task<IActionResult> GetCandidatesMessagedByCompany(int companyId)
        {
            var messages = await _context.Messages
                .Where(m => (m.SenderId == companyId && _context.Users.Any(u => u.Id == m.ReceiverId && _context.Roles.Any(r => r.RoleId == u.RoleId && r.RoleName == "Candidate")))
                      || (m.ReceiverId == companyId && _context.Users.Any(u => u.Id == m.SenderId && _context.Roles.Any(r => r.RoleId == u.RoleId && r.RoleName == "Candidate"))))
                .ToListAsync();

            var candidateIds = messages
                .Select(m => m.SenderId == companyId ? m.ReceiverId : m.SenderId)
                .Distinct()
                .ToList();

            // Lấy thông tin công ty 1 lần duy nhất
            var companyProfile = await _context.CompanyProfile.FirstOrDefaultAsync(c => c.UserId == companyId);

            var result = new List<object>();

            foreach (var candidateId in candidateIds)
            {
                var lastMessage = messages
                    .Where(m => (m.SenderId == companyId && m.ReceiverId == candidateId) || (m.SenderId == candidateId && m.ReceiverId == companyId))
                    .OrderByDescending(m => m.SentAt)
                    .FirstOrDefault();

                if (lastMessage == null) continue;

                var candidateUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == candidateId);

                result.Add(new
                {
                    CandidateId = candidateId,
                    MessageId = lastMessage.Id,
                    SenderId = lastMessage.SenderId,
                    ReceiverId = lastMessage.ReceiverId,
                    SentAt = lastMessage.SentAt,
                    MessageText = lastMessage.MessageText,
                    // Thông tin người gửi là công ty
                    CompanyName = companyProfile?.CompanyName,
                    UrlCompanyLogo = companyProfile?.UrlCompanyLogo,
                    CandidateFullName = candidateUser?.FullName,
                    CandidateImage = candidateUser?.Image
                });
            }

            return Ok(result);
        }

        [HttpGet("companies-messaged/{candidateId}")]
        public async Task<IActionResult> GetCompaniesMessagedByCandidate(int candidateId)
        {
            // Lấy tất cả tin nhắn mà candidate là sender hoặc receiver, và đối phương là company
            var messages = await _context.Messages
                .Where(m => (m.SenderId == candidateId && _context.Users.Any(u => u.Id == m.ReceiverId && _context.Roles.Any(r => r.RoleId == u.RoleId && r.RoleName == "Company")))
                         || (m.ReceiverId == candidateId && _context.Users.Any(u => u.Id == m.SenderId && _context.Roles.Any(r => r.RoleId == u.RoleId && r.RoleName == "Company"))))
                .ToListAsync();

            // Lấy danh sách các companyId đã từng nhắn với candidate
            var companyIds = messages
                .Select(m => m.SenderId == candidateId ? m.ReceiverId : m.SenderId)
                .Distinct()
                .ToList();

            var result = new List<object>();

            foreach (var companyId in companyIds)
            {
                // Lấy tin nhắn cuối cùng giữa candidate và company này
                var lastMessage = messages
                    .Where(m => (m.SenderId == candidateId && m.ReceiverId == companyId) || (m.SenderId == companyId && m.ReceiverId == candidateId))
                    .OrderByDescending(m => m.SentAt)
                    .FirstOrDefault();

                if (lastMessage == null) continue;

                var sender = await _context.Users.FirstOrDefaultAsync(u => u.Id == lastMessage.SenderId);
                var receiver = await _context.Users.FirstOrDefaultAsync(u => u.Id == lastMessage.ReceiverId);
                var companyProfile = await _context.CompanyProfile.FirstOrDefaultAsync(c => c.UserId == (sender.Id == companyId ? sender.Id : receiver.Id));

                result.Add(new
                {
                    CompanyId = companyId,
                    MessageId = lastMessage.Id,
                    SenderId = lastMessage.SenderId,
                    ReceiverId = lastMessage.ReceiverId,
                    SentAt = lastMessage.SentAt,
                    MessageText = lastMessage.MessageText,
                    SenderFullName = sender?.FullName,
                    SenderImage = sender?.Image,
                    CompanyName = companyProfile?.CompanyName,
                    UrlCompanyLogo = companyProfile?.UrlCompanyLogo,
                });
            }

            return Ok(result);
        }

        [HttpPost("send")]
        [Authorize]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            try
            {
                _logger.LogInformation("Received SendMessage request: {@Dto}", dto);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state: {@ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                var sender = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == dto.SenderId);
                var receiver = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == dto.ReceiverId);

                if (sender == null || receiver == null)
                {
                    _logger.LogWarning("Invalid sender or receiver. SenderId: {SenderId}, ReceiverId: {ReceiverId}", dto.SenderId, dto.ReceiverId);
                    return BadRequest("Invalid sender or receiver");
                }

                if (string.IsNullOrWhiteSpace(dto.MessageText))
                {
                    _logger.LogWarning("Message text is empty or whitespace for SenderId: {SenderId}", dto.SenderId);
                    return BadRequest("Message text cannot be empty.");
                }

                var currentUserIdClaim = User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(currentUserIdClaim))
                {
                    _logger.LogWarning("No UserId found in JWT token");
                    return Unauthorized("Invalid token");
                }

                var currentUserId = int.Parse(currentUserIdClaim);
                if (currentUserId != dto.SenderId)
                {
                    _logger.LogWarning("UserId mismatch. Current: {CurrentUserId}, SenderId: {SenderId}", currentUserId, dto.SenderId);
                    return Forbid("You can only send messages as yourself.");
                }

                if (sender.Role.RoleName == "Candidate" && !await IsValidReceiverForCandidate(dto.SenderId, dto.ReceiverId))
                {
                    _logger.LogWarning("Candidate {SenderId} tried to send to invalid receiver {ReceiverId}", dto.SenderId, dto.ReceiverId);
                    return Forbid("Candidates can only send messages to Company or Admin.");
                }

                var message = new Message
                {
                    SenderId = dto.SenderId,
                    ReceiverId = dto.ReceiverId,
                    MessageText = dto.MessageText,
                    SentAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                var senderInfo = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == dto.SenderId);

                var receiverInfo = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == dto.ReceiverId);

                var messageData = new
                {
                    Id = message.Id,
                    SenderId = dto.SenderId,
                    ReceiverId = dto.ReceiverId,
                    MessageText = dto.MessageText,
                    SentAt = message.SentAt,
                    SenderFullName = senderInfo?.FullName,
                    SenderImage = senderInfo?.Image,
                    ReceiverFullName = receiverInfo?.FullName,
                    ReceiverImage = receiverInfo?.Image,
                    RelatedJobId = dto.RelatedJobId
                };

                await _hubContext.Clients.Group(dto.SenderId.ToString())
                    .SendAsync("ReceiveMessage", messageData);
                await _hubContext.Clients.Group(dto.ReceiverId.ToString())
                    .SendAsync("ReceiveMessage", messageData);

                await _hubContext.Clients.Group(dto.SenderId.ToString())
                    .SendAsync("UpdateContactList");
                await _hubContext.Clients.Group(dto.ReceiverId.ToString())
                    .SendAsync("UpdateContactList");

                _logger.LogInformation("Message sent successfully. MessageId: {MessageId}", message.Id);
                return Ok(messageData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message. SenderId: {SenderId}, ReceiverId: {ReceiverId}", dto?.SenderId, dto?.ReceiverId);
                return StatusCode(500, "An error occurred while sending the message.");
            }
        }

        [HttpPost("join-group")]
        [Authorize]
        public async Task<IActionResult> JoinSignalRGroup()
        {
            try
            {
                var currentUserIdClaim = User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(currentUserIdClaim))
                {
                    return Unauthorized("Invalid token");
                }

                var currentUserId = int.Parse(currentUserIdClaim);

                await _hubContext.Groups.AddToGroupAsync(currentUserId.ToString(), currentUserId.ToString());

                _logger.LogInformation("User {UserId} joined SignalR group", currentUserId);
                return Ok(new { message = "Successfully joined SignalR group" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining SignalR group");
                return StatusCode(500, "An error occurred while joining SignalR group.");
            }
        }

        private async Task<bool> IsValidReceiverForCandidate(int senderId, int receiverId)
        {
            var receiver = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == receiverId);

            if (receiver == null) return false;

            return receiver.Role.RoleName == "Company" || receiver.Role.RoleName == "Admin";
        }
    }
}       