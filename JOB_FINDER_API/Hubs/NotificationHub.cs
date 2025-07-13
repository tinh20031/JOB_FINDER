using Microsoft.AspNetCore.SignalR;
using JOB_FINDER_API.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace JOB_FINDER_API.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public async Task SendNotification(Notification notification)
        {
            _logger.LogInformation($"Sending notification to user {notification.UserId}");
            await Clients.User(notification.UserId.ToString()).SendAsync("ReceiveNotification", notification);
        }

        public async Task JoinUserGroup(string userId)
        {
            _logger.LogInformation($"User {userId} joining group User_{userId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            await Clients.Caller.SendAsync("JoinedGroup", $"User_{userId}");
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation($"Client connected to NotificationHub: {Context.ConnectionId}, UserId: {userId ?? "unknown"}");

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
                _logger.LogInformation($"User {userId} automatically added to group User_{userId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"Client disconnected from NotificationHub: {Context.ConnectionId}, Exception: {exception?.Message ?? "none"}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}