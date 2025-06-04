using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.DTO;
using JOB_FINDER_API.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace JOB_FINDER_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageController(JobFinderDbContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // Lấy lịch sử tin nhắn giữa 2 người dùng
        [HttpGet("history/{userId1}/{userId2}")]
        public async Task<IActionResult> GetMessageHistory(int userId1, int userId2)
        {
            var messages = await _context.Messages
                .Where(m =>
                    (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                    (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return Ok(messages);
        }


        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            var sender = await _context.Users.FindAsync(dto.SenderId);
            var receiver = await _context.Users.FindAsync(dto.ReceiverId);

            if (sender == null || receiver == null)
                return BadRequest("Invalid sender or receiver");

            var message = new Message
            {
                SenderId = dto.SenderId,
                ReceiverId = dto.ReceiverId,
                RelatedJobId = dto.RelatedJobId,
                MessageText = dto.MessageText,
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

    
            await _hubContext.Clients.Group(dto.ReceiverId.ToString())
                .SendAsync("ReceiveMessage", dto.SenderId, dto.MessageText);

            return Ok(message);
        }
    }
}
