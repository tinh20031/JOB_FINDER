using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace JOB_FINDER_API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        public static ConcurrentDictionary<string, int> OnlineUsers = new();

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                // SỬA Ở ĐÂY: Dùng NameIdentifier thay vì Name
                var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    var userId = userIdClaim;
                    OnlineUsers.AddOrUpdate(userId, 1, (key, oldValue) => oldValue + 1);
                    await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                    _logger.LogInformation("User {UserId} connected with connection {ConnectionId}", userId, Context.ConnectionId);

                    if (OnlineUsers[userId] == 1)
                        await Clients.All.SendAsync("UserOnlineStatusChanged", new { userId, isOnline = true });

                    var allOnlineUserIds = OnlineUsers.Where(x => x.Value > 0).Select(x => x.Key).ToList();
                    await Clients.Caller.SendAsync("OnlineUsersList", allOnlineUserIds);
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
                // SỬA Ở ĐÂY: Dùng NameIdentifier thay vì Name
                var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    var userId = userIdClaim;
                    if (OnlineUsers.ContainsKey(userId))
                    {
                        OnlineUsers[userId]--;
                        if (OnlineUsers[userId] <= 0)
                        {
                            OnlineUsers.TryRemove(userId, out _);
                            await Clients.All.SendAsync("UserOnlineStatusChanged", new { userId, isOnline = false });
                        }
                    }
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

        // Các phương thức còn lại giữ nguyên
        public async Task JoinUserGroup(string userId)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                _logger.LogInformation("User {UserId} manually joined group", userId);
                OnlineUsers.AddOrUpdate(userId, 1, (key, oldValue) => oldValue + 1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining user group for user {UserId}", userId);
            }
        }

        public async Task LeaveUserGroup(string userId)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
                _logger.LogInformation("User {UserId} left group", userId);
                if (OnlineUsers.ContainsKey(userId))
                {
                    OnlineUsers[userId]--;
                    if (OnlineUsers[userId] <= 0)
                    {
                        OnlineUsers.TryRemove(userId, out _);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving user group for user {UserId}", userId);
            }
        }
    }
}