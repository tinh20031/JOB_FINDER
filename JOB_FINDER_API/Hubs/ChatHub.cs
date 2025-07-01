using FireSharp.Interfaces;
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
        private readonly IFirebaseClient _firebaseClient;
        public static ConcurrentDictionary<string, int> OnlineUsers = new();

        public ChatHub(ILogger<ChatHub> logger, IFirebaseClient firebaseClient)
        {
            _logger = logger;
            _firebaseClient = firebaseClient;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    // Cập nhật trạng thái online trong Firebase
                    var status = new
                    {
                        status = "online",
                        last_seen = DateTime.UtcNow.ToString("o")
                    };
                    await _firebaseClient.SetAsync($"users/{userId}/status", status);

                    // Cập nhật OnlineUsers
                    OnlineUsers.AddOrUpdate(userIdClaim, 1, (key, oldValue) => oldValue + 1);
                    await Groups.AddToGroupAsync(Context.ConnectionId, userIdClaim);
                    _logger.LogInformation("User {UserId} connected with connection {ConnectionId}", userId, Context.ConnectionId);

                    // Thông báo trạng thái online cho tất cả client
                    await Clients.All.SendAsync("UserOnlineStatusChanged", new { userId = userId, isOnline = true });

                    // Gửi danh sách người dùng online cho client hiện tại
                    var allOnlineUserIds = OnlineUsers.Where(x => x.Value > 0).Select(x => int.Parse(x.Key)).ToList();
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
                var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    if (OnlineUsers.ContainsKey(userIdClaim))
                    {
                        OnlineUsers[userIdClaim]--;
                        if (OnlineUsers[userIdClaim] <= 0)
                        {
                            OnlineUsers.TryRemove(userIdClaim, out _);

                            // Cập nhật trạng thái offline trong Firebase
                            var status = new
                            {
                                status = "offline",
                                last_seen = DateTime.UtcNow.ToString("o")
                            };
                            await _firebaseClient.SetAsync($"users/{userId}/status", status);

                            // Thông báo trạng thái offline cho tất cả client
                            await Clients.All.SendAsync("UserOnlineStatusChanged", new { userId = userId, isOnline = false });
                        }
                    }
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, userIdClaim);
                    _logger.LogInformation("User {UserId} disconnected with connection {ConnectionId}", userId, Context.ConnectionId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnDisconnectedAsync");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinUserGroup(int userId)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
                _logger.LogInformation("User {UserId} manually joined group", userId);
                OnlineUsers.AddOrUpdate(userId.ToString(), 1, (key, oldValue) => oldValue + 1);

                // Cập nhật trạng thái online trong Firebase
                var status = new
                {
                    status = "online",
                    last_seen = DateTime.UtcNow.ToString("o")
                };
                await _firebaseClient.SetAsync($"users/{userId}/status", status);

                // Thông báo trạng thái cho tất cả client
                await Clients.All.SendAsync("UserOnlineStatusChanged", new { userId, isOnline = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining user group for user {UserId}", userId);
            }
        }

        public async Task LeaveUserGroup(int userId)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
                _logger.LogInformation("User {UserId} left group", userId);
                if (OnlineUsers.ContainsKey(userId.ToString()))
                {
                    OnlineUsers[userId.ToString()]--;
                    if (OnlineUsers[userId.ToString()] <= 0)
                    {
                        OnlineUsers.TryRemove(userId.ToString(), out _);

                        // Cập nhật trạng thái offline trong Firebase
                        var status = new
                        {
                            status = "offline",
                            last_seen = DateTime.UtcNow.ToString("o")
                        };
                        await _firebaseClient.SetAsync($"users/{userId}/status", status);

                        // Thông báo trạng thái cho tất cả client
                        await Clients.All.SendAsync("UserOnlineStatusChanged", new { userId, isOnline = false });
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