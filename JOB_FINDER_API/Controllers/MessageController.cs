using FireSharp.Interfaces;
using FireSharp.Response;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Hubs;
using JOB_FINDER_API.Models.DTO;
using JOB_FINDER_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JOB_FINDER_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        private readonly ILogger<MessageController> _logger;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IFirebaseClient _firebaseClient;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageController(
            JobFinderDbContext context,
            ILogger<MessageController> logger,
            CloudinaryService cloudinaryService,
            IFirebaseClient firebaseClient,
            IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _logger = logger;
            _cloudinaryService = cloudinaryService;
            _firebaseClient = firebaseClient;
            _hubContext = hubContext;
        }

        public class Message
        {
            public int sender_id { get; set; }
            public int receiver_id { get; set; }
            public string content { get; set; }
            public string media_url { get; set; }
            public string media_type { get; set; }
            public string file_name { get; set; }
            public DateTime sent_at { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public int? related_job_id { get; set; }
        }

        [HttpGet("history/{userId1}/{userId2}")]
        public async Task<IActionResult> GetMessageHistory(int userId1, int userId2)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state: {@ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                string roomId = GenerateRoomId(userId1, userId2);
                FirebaseResponse response = await _firebaseClient.GetAsync("messages/" + roomId);
                var messages = response.ResultAs<Dictionary<string, Message>>();
                var result = new List<object>();

                if (messages != null)
                {
                    foreach (var msg in messages)
                    {
                        var sender = await _context.Users.FirstOrDefaultAsync(u => u.Id == msg.Value.sender_id);
                        var receiver = await _context.Users.FirstOrDefaultAsync(u => u.Id == msg.Value.receiver_id);
                        result.Add(new
                        {
                            MessageId = msg.Key,
                            SenderId = msg.Value.sender_id,
                            ReceiverId = msg.Value.receiver_id,
                            MessageText = msg.Value.content,
                            FileUrl = msg.Value.media_url,
                            FileType = msg.Value.media_type,
                            FileName = msg.Value.file_name,
                            SentAt = msg.Value.sent_at,
                            SenderFullName = sender?.FullName,
                            SenderImage = sender?.Image,
                            SenderRole = sender?.Role?.RoleName,
                            ReceiverFullName = receiver?.FullName,
                            ReceiverImage = receiver?.Image,
                            ReceiverRole = receiver?.Role?.RoleName
                        });
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving message history");
                return StatusCode(500, "Error retrieving messages");
            }
        }

        [HttpGet("candidates-messaged/{companyId}")]
        public async Task<IActionResult> GetCandidatesMessagedByCompany(int companyId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state: {@ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                FirebaseResponse response = await _firebaseClient.GetAsync("messages");
                var messages = response.ResultAs<Dictionary<string, Dictionary<string, Message>>>();
                var candidateIds = new List<int>();

                if (messages != null)
                {
                    foreach (var room in messages)
                    {
                        foreach (var msg in room.Value)
                        {
                            if (msg.Value.sender_id == companyId || msg.Value.receiver_id == companyId)
                            {
                                var otherId = msg.Value.sender_id == companyId ? msg.Value.receiver_id : msg.Value.sender_id;
                                if (await _context.Users.AnyAsync(u => u.Id == otherId && u.Role.RoleName == "Candidate"))
                                {
                                    candidateIds.Add(otherId);
                                }
                            }
                        }
                    }
                }

                candidateIds = candidateIds.Distinct().ToList();
                var companyProfile = await _context.CompanyProfile.FirstOrDefaultAsync(c => c.UserId == companyId);
                var result = new List<object>();

                foreach (var candidateId in candidateIds)
                {
                    FirebaseResponse lastMessageResponse = await _firebaseClient.GetAsync("messages");
                    var lastMessages = lastMessageResponse.ResultAs<Dictionary<string, Dictionary<string, Message>>>();
                    var lastMessage = lastMessages
                        .SelectMany(r => r.Value)
                        .Where(m => (m.Value.sender_id == companyId && m.Value.receiver_id == candidateId) ||
                                    (m.Value.sender_id == candidateId && m.Value.receiver_id == companyId))
                        .OrderByDescending(m => m.Value.sent_at)
                        .FirstOrDefault();

                    if (lastMessage.Value != null)
                    {
                        var candidateUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == candidateId);
                        result.Add(new
                        {
                            CandidateId = candidateId,
                            MessageId = Guid.NewGuid().ToString(),
                            SenderId = lastMessage.Value.sender_id,
                            ReceiverId = lastMessage.Value.receiver_id,
                            SentAt = lastMessage.Value.sent_at,
                            MessageText = lastMessage.Value.content,
                            CompanyName = companyProfile?.CompanyName,
                            UrlCompanyLogo = companyProfile?.UrlCompanyLogo,
                            CandidateFullName = candidateUser?.FullName,
                            CandidateImage = candidateUser?.Image
                        });
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving candidates messaged");
                return StatusCode(500, "Error retrieving candidates");
            }
        }

        [HttpGet("companies-messaged/{candidateId}")]
        public async Task<IActionResult> GetCompaniesMessagedByCandidate(int candidateId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state: {@ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                FirebaseResponse response = await _firebaseClient.GetAsync("messages");
                var messages = response.ResultAs<Dictionary<string, Dictionary<string, Message>>>();
                var companyIds = new List<int>();

                if (messages != null)
                {
                    foreach (var room in messages)
                    {
                        foreach (var msg in room.Value)
                        {
                            if (msg.Value.sender_id == candidateId || msg.Value.receiver_id == candidateId)
                            {
                                var otherId = msg.Value.sender_id == candidateId ? msg.Value.receiver_id : msg.Value.sender_id;
                                if (await _context.Users.AnyAsync(u => u.Id == otherId && u.Role.RoleName == "Company"))
                                {
                                    companyIds.Add(otherId);
                                }
                            }
                        }
                    }
                }

                companyIds = companyIds.Distinct().ToList();
                var result = new List<object>();

                foreach (var companyId in companyIds)
                {
                    FirebaseResponse lastMessageResponse = await _firebaseClient.GetAsync("messages");
                    var lastMessages = lastMessageResponse.ResultAs<Dictionary<string, Dictionary<string, Message>>>();
                    var lastMessage = lastMessages
                        .SelectMany(r => r.Value)
                        .Where(m => (m.Value.sender_id == candidateId && m.Value.receiver_id == companyId) ||
                                    (m.Value.sender_id == companyId && m.Value.receiver_id == candidateId))
                        .OrderByDescending(m => m.Value.sent_at)
                        .FirstOrDefault();

                    if (lastMessage.Value != null)
                    {
                        var sender = await _context.Users.FirstOrDefaultAsync(u => u.Id == lastMessage.Value.sender_id);
                        var receiver = await _context.Users.FirstOrDefaultAsync(u => u.Id == lastMessage.Value.receiver_id);
                        var companyProfile = await _context.CompanyProfile.FirstOrDefaultAsync(c => c.UserId == companyId);
                        result.Add(new
                        {
                            CompanyId = companyId,
                            MessageId = Guid.NewGuid().ToString(),
                            SenderId = lastMessage.Value.sender_id,
                            ReceiverId = lastMessage.Value.receiver_id,
                            SentAt = lastMessage.Value.sent_at,
                            MessageText = lastMessage.Value.content,
                            SenderFullName = sender?.FullName,
                            SenderImage = sender?.Image,
                            CompanyName = companyProfile?.CompanyName,
                            UrlCompanyLogo = companyProfile?.UrlCompanyLogo
                        });
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving companies messaged");
                return StatusCode(500, "Error retrieving companies");
            }
        }

        [HttpPost("send")]
        [Authorize]
        public async Task<IActionResult> SendMessage([FromForm] SendMessageDto dto)
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

                if (sender == null || receiver == null || string.IsNullOrEmpty(sender.FirebaseUid) || string.IsNullOrEmpty(receiver.FirebaseUid))
                {
                    _logger.LogWarning("Invalid sender or receiver. SenderId: {SenderId}, ReceiverId: {ReceiverId}", dto.SenderId, dto.ReceiverId);
                    return BadRequest("Invalid sender or receiver");
                }

                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserIdClaim) || int.Parse(currentUserIdClaim) != dto.SenderId)
                {
                    _logger.LogWarning("UserId mismatch or invalid token");
                    return Unauthorized("Invalid token or sender");
                }

                if (sender.Role.RoleName == "Candidate" && !await IsValidReceiverForCandidate(dto.SenderId, dto.ReceiverId))
                {
                    _logger.LogWarning("Candidate {SenderId} tried to send to invalid receiver {ReceiverId}", dto.SenderId, dto.ReceiverId);
                    return Forbid("Candidates can only send to Company or Admin.");
                }

                if (string.IsNullOrWhiteSpace(dto.MessageText) && (dto.File == null || dto.File.Length == 0))
                {
                    _logger.LogWarning("Message text and file are both empty for SenderId: {SenderId}", dto.SenderId);
                    return BadRequest("Message text or file must be provided.");
                }

                string? mediaUrl = null;
                string? mediaType = null;
                string? fileName = null;

                if (dto.File != null && dto.File.Length > 0)
                {
                    (mediaUrl, mediaType, fileName) = await _cloudinaryService.UploadMediaAsync(dto.File, dto.IsSticker);
                    if (string.IsNullOrEmpty(mediaUrl))
                    {
                        _logger.LogError("Cloudinary upload failed for SenderId: {SenderId}", dto.SenderId);
                        return StatusCode(500, "File upload failed.");
                    }
                }

                string roomId = GenerateRoomId(dto.SenderId, dto.ReceiverId);
                var message = new Message
                {
                    sender_id = dto.SenderId,
                    receiver_id = dto.ReceiverId,
                    content = dto.MessageText,
                    media_url = mediaUrl,
                    media_type = mediaType,
                    file_name = fileName,
                    sent_at = DateTime.UtcNow,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow,
                    related_job_id = dto.RelatedJobId
                };

                FirebaseResponse response = await _firebaseClient.PushAsync("messages/" + roomId, message);

                // Thông báo tin nhắn qua SignalR
                var messageData = new
                {
                    Id = Guid.NewGuid().ToString(),
                    SenderId = dto.SenderId,
                    ReceiverId = dto.ReceiverId,
                    MessageText = dto.MessageText,
                    FileUrl = mediaUrl,
                    FileType = mediaType,
                    FileName = fileName,
                    SentAt = DateTime.UtcNow,
                    RelatedJobId = dto.RelatedJobId,
                    SenderFullName = sender.FullName,
                    SenderImage = sender.Image,
                    ReceiverFullName = receiver.FullName,
                    ReceiverImage = receiver.Image
                };
                await _hubContext.Clients.Group(roomId).SendAsync("ReceiveMessage", messageData);

                _logger.LogInformation("Message sent successfully.");
                return Ok(messageData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message. SenderId: {SenderId}, ReceiverId: {ReceiverId}", dto?.SenderId, dto?.ReceiverId);
                return StatusCode(500, "An error occurred while sending the message.");
            }
        }

        private string GenerateRoomId(int senderId, int receiverId)
        {
            return Math.Min(senderId, receiverId) + "_" + Math.Max(senderId, receiverId);
        }

        private async Task<bool> IsValidReceiverForCandidate(int senderId, int receiverId)
        {
            var receiver = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == receiverId);
            return receiver != null && (receiver.Role.RoleName == "Company" || receiver.Role.RoleName == "Admin");
        }
    }
}