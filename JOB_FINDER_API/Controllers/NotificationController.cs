using JOB_FINDER_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JOB_FINDER_API.Models;
using System.Threading.Tasks;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(
            NotificationService notificationService,
            ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        /*[HttpGet]
        public async Task<IActionResult> GetUserNotifications(
            [FromQuery] bool? isRead = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user ID.");

            var notifications = await _notificationService.GetUserNotifications(userId, isRead, page, pageSize);
            return Ok(notifications);
        }*/
        [HttpGet]
        public async Task<IActionResult> GetUserNotifications(
    [FromQuery] bool? isRead = null,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    _logger.LogWarning("GetUserNotifications: No user ID claim found");
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                if (!int.TryParse(userIdClaim, out var userId))
                {
                    _logger.LogWarning($"GetUserNotifications: Invalid user ID format: {userIdClaim}");
                    return Unauthorized(new { message = "Invalid user ID format" });
                }

                _logger.LogInformation($"Getting notifications for user {userId}, page {page}, pageSize {pageSize}");
                var notifications = await _notificationService.GetUserNotifications(userId, isRead, page, pageSize);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetUserNotifications: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { error = "An error occurred while retrieving notifications" });
            }
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user ID.");

            int count = await _notificationService.GetUnreadCount(userId);
            return Ok(new { count });
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user ID.");

            await _notificationService.MarkAsRead(id, userId);
            return Ok();
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user ID.");

            await _notificationService.MarkAllAsRead(userId);
            return Ok();
        }
        [HttpPost("send")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            try
            {
                if (request.UserIds != null && request.UserIds.Any())
                {
                    foreach (var userId in request.UserIds)
                    {
                        await _notificationService.SendDirectNotification(
                            userId,
                            request.Title,
                            request.Link,
                            request.Type
                        );
                    }
                }
                return Ok(new { message = "Notifications sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending notifications: {ex.Message}");
                return StatusCode(500, new { error = "Failed to send notifications" });
            }
        }
    }

    public class NotificationRequest
    {
        public List<int> UserIds { get; set; } = new List<int>();
        public string Title { get; set; } = "";
        public string? Link { get; set; } 
        public Notification.NotificationType Type { get; set; } = Notification.NotificationType.SystemNotification;
    }
}