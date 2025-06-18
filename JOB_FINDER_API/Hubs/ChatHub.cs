using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace JOB_FINDER_API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                // Lấy user ID từ JWT token
                var userIdClaim = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    var userId = userIdClaim;

                    // Join user vào group của chính họ
                    await Groups.AddToGroupAsync(Context.ConnectionId, userId);

                    _logger.LogInformation("User {UserId} connected with connection {ConnectionId}", userId, Context.ConnectionId);
                }
                else
                {
                    _logger.LogWarning("User connected without valid user ID claim");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnConnectedAsync");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var userIdClaim = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    var userId = userIdClaim;

                    // Remove user from their group
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);

                    _logger.LogInformation("User {UserId} disconnected with connection {ConnectionId}", userId, Context.ConnectionId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnDisconnectedAsync");
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Method để client có thể join group thủ công nếu cần
        public async Task JoinUserGroup(string userId)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                _logger.LogInformation("User {UserId} manually joined group", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining user group for user {UserId}", userId);
            }
        }

        // Method để client có thể leave group
        public async Task LeaveUserGroup(string userId)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
                _logger.LogInformation("User {UserId} left group", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving user group for user {UserId}", userId);
            }
        }
    }
}