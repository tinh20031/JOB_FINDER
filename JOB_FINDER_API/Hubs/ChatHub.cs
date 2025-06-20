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
        // Dictionary lưu số lượng kết nối online của user
        public static ConcurrentDictionary<string, int> OnlineUsers = new();

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var userIdClaim = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    var userId = userIdClaim;
                    // Tăng số lượng kết nối
                    OnlineUsers.AddOrUpdate(userId, 1, (key, oldValue) => oldValue + 1);
                    await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                    _logger.LogInformation("User {UserId} connected with connection {ConnectionId}", userId, Context.ConnectionId);

                    // Chỉ gửi sự kiện online khi user thực sự online lần đầu
                    if (OnlineUsers[userId] == 1)
                        await Clients.All.SendAsync("UserOnlineStatusChanged", new { userId, isOnline = true });

                    // Gửi danh sách tất cả user đang online cho user vừa kết nối
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
                var userIdClaim = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    var userId = userIdClaim;
                    // Giảm số lượng kết nối
                    if (OnlineUsers.ContainsKey(userId))
                    {
                        OnlineUsers[userId]--;
                        if (OnlineUsers[userId] <= 0)
                        {
                            OnlineUsers.TryRemove(userId, out _);
                            // Chỉ gửi sự kiện offline khi user thực sự không còn kết nối nào
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

        public async Task JoinUserGroup(string userId)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                _logger.LogInformation("User {UserId} manually joined group", userId);
                // Tăng số lượng kết nối khi join group thủ công
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
                // Giảm số lượng kết nối khi leave group thủ công
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