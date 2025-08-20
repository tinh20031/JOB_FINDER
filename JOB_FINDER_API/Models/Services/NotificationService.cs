/*
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JOB_FINDER_API.Services
{
    public class NotificationService
    {
        private readonly JobFinderDbContext _context;
        private readonly EmailService _emailService;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly ILogger<NotificationService> _logger;
        private readonly IConfiguration _configuration;

        public NotificationService(
            JobFinderDbContext context,
            EmailService emailService,
            IHubContext<NotificationHub> notificationHubContext,
            ILogger<NotificationService> logger,
            IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _notificationHubContext = notificationHubContext;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task CreateNewJobNotification(Job job, User companyUser, IEnumerable<User> notificationRecipients, IEnumerable<User> emailRecipients = null)
        {
            try
            {
                _logger.LogInformation($"Starting notification creation for job {job.JobId} with {notificationRecipients.Count()} notification recipients");
                if (emailRecipients != null)
                {
                    _logger.LogInformation($"Email will be sent to {emailRecipients.Count()} users who favorited the company");
                }

                var companyProfile = await _context.CompanyProfile
                    .FirstOrDefaultAsync(c => c.UserId == companyUser.UserId);

                string companyName = companyProfile?.CompanyName ?? companyUser.FullName ?? "Company";
                string baseUrl = _configuration["AppSettings:BaseUrl"];
                if (string.IsNullOrEmpty(baseUrl))
                {
                    _logger.LogError("BaseUrl is not configured in appsettings.json.");
                    throw new InvalidOperationException("BaseUrl is not configured in appsettings.json.");
                }
                string jobEndpoint = $"/job-single-v3/{job.JobId}";
                string jobUrl = $"{baseUrl}{jobEndpoint}";

                string jobTitle = job.Title.Length > 30 ? job.Title.Substring(0, 30) + "..." : job.Title;
                string province = job.ProvinceName.Length > 20 ? job.ProvinceName.Substring(0, 20) + "..." : job.ProvinceName;
                string title = $"{companyName} posted new job {jobTitle} in {province}";
                string startDate = job.TimeStart.ToString("dd/MM/yyyy");

                string mailBody = $@"
                    <div style='font-family: Arial, sans-serif;'>
                        <h2 style='color:#2d8cf0;'>Company {companyName} Just posted a new job!</h2>
                        <p><b>Title Job:</b> {job.Title}</p>
                        <p><b>Location:</b> {job.ProvinceName}</p>
                        <p><b>Deadline:</b> {job.ExpiryDate:dd/MM/yyyy}</p>
                        <p><b>Description:</b> {(job.Description?.Length > 200 ? job.Description?.Substring(0, 200) + "..." : job.Description)}</p>
                        <div style='margin:20px 0;'>
                            <a href='{jobUrl}' style='background:#2d8cf0;color:#fff;padding:10px 20px;border-radius:4px;text-decoration:none;font-weight:bold;'>View details & Apply</a>
                        </div>
                    </div>
                ";

                var notifications = new List<Notification>();
                int notificationsCreated = 0;

                foreach (var recipient in notificationRecipients)
                {
                    if (recipient?.UserId.HasValue == true)
                    {
                        try
                        {
                            _logger.LogInformation($"Creating notification for user ID: {recipient.UserId.Value}");

                            var notification = new Notification
                            {
                                UserId = recipient.UserId.Value,
                                Title = title,
                                Link = jobEndpoint, // Chỉ lưu endpoint
                                Type = Notification.NotificationType.NewJob,
                                IsRead = false,
                                CreatedAt = DateTime.UtcNow
                            };

                            notifications.Add(notification);
                            notificationsCreated++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error creating notification for user {recipient.UserId}: {ex.Message}");
                        }
                    }
                }

                if (emailRecipients != null)
                {
                    foreach (var recipient in emailRecipients)
                    {
                        if (!string.IsNullOrEmpty(recipient?.Email))
                        {
                            try
                            {
                                _logger.LogInformation($"Sending email to user {recipient.UserId}: {recipient.Email}");

                                _emailService.SendEmail(
                                    recipient.Email,
                                    $"[{companyName}] just posted new job: {job.Title}",
                                    mailBody,
                                    true
                                );
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Error sending email to {recipient.Email}: {ex.Message}");
                            }
                        }
                    }
                }

                _logger.LogInformation($"Created {notificationsCreated} notifications, saving to database");

                if (notifications.Any())
                {
                    _context.Notifications.AddRange(notifications);
                    var savedCount = await _context.SaveChangesAsync();
                    _logger.LogInformation($"Successfully saved {savedCount} notifications to database");

                    if (_notificationHubContext != null)
                    {
                        foreach (var notification in notifications)
                        {
                            try
                            {
                                await _notificationHubContext.Clients
                                    .Group($"User_{notification.UserId}")
                                    //.SendAsync("ReceiveNotification", notification);
                                    .SendAsync("ReceiveNotification", new
                                    {
                                        notification.NotificationId,
                                        notification.UserId,
                                        notification.Title,
                                        notification.Link,
                                        notification.Type,
                                        notification.IsRead,
                                        notification.CreatedAt,
                                        JobTimeStart = job.TimeStart.ToString("dd/MM/yyyy") // Add TimeStart
                                    });
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Error sending real-time notification: {ex.Message}");
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No notifications created for this job");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateNewJobNotification: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task CreateNewJobNotification(Job job, User companyUser, IEnumerable<User> recipients)
        {
            await CreateNewJobNotification(job, companyUser, recipients, recipients);
        }


        public async Task CreateJobStatusNotification(Job job, bool isApproved)
        {
            try
            {
                if (job.CompanyId > 0)
                {
                    var companyUserId = job.CompanyId;
                    _logger.LogInformation($"Creating job status notification for company user {companyUserId}, job #{job.JobId}");

                    string jobTitle = job.Title.Length > 30 ? job.Title.Substring(0, 30) + "..." : job.Title;
                    string title = isApproved ? $"Your job {jobTitle} was approved by admin" : $"Your job {jobTitle} was not approved";
                    string jobEndpoint = $"/job-single-v3/{job.JobId}";

                    var notification = new Notification
                    {
                        UserId = companyUserId,
                        Title = title,
                        Link = jobEndpoint,
                        Type = isApproved ? Notification.NotificationType.JobApproved : Notification.NotificationType.JobRejected,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Job status notification created for company user {companyUserId}");

                    if (_notificationHubContext != null)
                    {
                        try
                        {
                            await _notificationHubContext.Clients
                                .Group($"User_{companyUserId}")
                                //.SendAsync("ReceiveNotification", notification);
                                .SendAsync("ReceiveNotification", new
                                {
                                    notification.NotificationId,
                                    notification.UserId,
                                    notification.Title,
                                    notification.Link,
                                    notification.Type,
                                    notification.IsRead,
                                    notification.CreatedAt,
                                    JobTimeStart = job.TimeStart.ToString("dd/MM/yyyy") // Add TimeStart
                                });
                            _logger.LogInformation($"Real-time job status notification sent to company user {companyUserId}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error sending SignalR notification: {ex.Message}");
                        }
                    }
                }
                else
                {
                    _logger.LogWarning($"Cannot create company notification: Job #{job.JobId} has no valid company ID");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateJobStatusNotification: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
            }
        }

        public async Task<List<Notification>> GetUserNotifications(int userId, bool? isRead = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Notifications
                .Where(n => n.UserId == userId);

            if (isRead.HasValue)
            {
                query = query.Where(n => n.IsRead == isRead.Value);
            }

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task MarkAsRead(int notificationId, int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();

                await _notificationHubContext.Clients
                    .Group($"User_{userId}")
                    .SendAsync("NotificationRead", notificationId);
            }
        }


        public async Task MarkAllAsRead(int userId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();

            await _notificationHubContext.Clients
                .Group($"User_{userId}")
                .SendAsync("AllNotificationsRead");
        }


        public async Task<int> GetUnreadCount(int userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task SendDirectNotification(int userId, string title, string link, Notification.NotificationType type)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Link = link, // Chỉ chứa endpoint
                    Type = type,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                await _notificationHubContext.Clients.Group($"User_{userId}")
                    .SendAsync("ReceiveNotification", new
                    {
                        id = notification.NotificationId,
                        title = notification.Title,
                        link = notification.Link,
                        type = notification.Type,
                        isRead = notification.IsRead,
                        createdAt = notification.CreatedAt
                    });

                _logger.LogInformation($"Direct notification sent to user {userId}: {title}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending direct notification: {ex.Message}");
                throw;
            }
        }

        public async Task CreateNewApplicationNotification(Application application, User candidate, Job job)
        {
            try
            {
                if (job.CompanyId <= 0)
                {
                    _logger.LogWarning($"Cannot create application notification: Job #{job.JobId} has no valid company ID");
                    return;
                }

                var companyUserId = job.CompanyId;
                var company = await _context.Users.FindAsync(companyUserId);
                if (company == null)
                {
                    _logger.LogWarning($"Cannot create application notification: Company user #{companyUserId} not found");
                    return;
                }

                
                string applicationEndpoint = $"/company-dashboard/candidates/details/{application.ApplicationId}";
                string candidateName = candidate.FullName?.Length > 20 ? candidate.FullName.Substring(0, 20) + "..." : candidate.FullName ?? "User";
                string jobTitle = job.Title.Length > 30 ? job.Title.Substring(0, 30) + "..." : job.Title;
                string title = $"{candidateName} applied for your job {jobTitle}";

                var notification = new Notification
                {
                    UserId = companyUserId,
                    Title = title,
                    Link = applicationEndpoint,
                    Type = Notification.NotificationType.NewJobApplication,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"New application notification created for company #{companyUserId}");

                if (_notificationHubContext != null)
                {
                    try
                    {
                        await _notificationHubContext.Clients
                            .Group($"User_{companyUserId}")
                            .SendAsync("ReceiveNotification", notification);
                        _logger.LogInformation($"Real-time application notification sent to company #{companyUserId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error sending SignalR notification: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateNewApplicationNotification: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
            }
        }

        public async Task CreateCompanyFavoritedNotification(UserFavoriteCompany favoriteCompany, User candidate)
        {
            try
            {
                var companyUserId = favoriteCompany.CompanyProfileId;
      
                string candidateEndpoint = $"/candidate-profile/{candidate.UserId}";
                string candidateName = candidate.FullName?.Length > 20 ? candidate.FullName.Substring(0, 20) + "..." : candidate.FullName ?? "Candidate";
                string title = $"{candidateName} favorited your company";

                var notification = new Notification
                {
                    UserId = companyUserId,
                    Title = title,
                    Link = candidateEndpoint,
                    Type = Notification.NotificationType.CompanyFavorited,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Company favorited notification created for company #{companyUserId}");

                if (_notificationHubContext != null)
                {
                    try
                    {
                        await _notificationHubContext.Clients
                            .Group($"User_{companyUserId}")
                            .SendAsync("ReceiveNotification", notification);
                        _logger.LogInformation($"Real-time company favorited notification sent to company #{companyUserId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error sending SignalR notification: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateCompanyFavoritedNotification: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
            }
        }

        public async Task CreateJobFavoritedNotification(UserFavoriteJob favoriteJob, User candidate)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(favoriteJob.JobId);
                if (job == null)
                {
                    _logger.LogWarning($"Cannot create job favorited notification: Job #{favoriteJob.JobId} not found");
                    return;
                }

                if (job.CompanyId <= 0)
                {
                    _logger.LogWarning($"Cannot create job favorited notification: Job #{job.JobId} has no valid company ID");
                    return;
                }

                var companyUserId = job.CompanyId;
                
                string candidateEndpoint = $"/candidate-profile/{candidate.UserId}";
                string candidateName = candidate.FullName?.Length > 20 ? candidate.FullName.Substring(0, 20) + "..." : candidate.FullName ?? "User";
                string jobTitle = job.Title.Length > 30 ? job.Title.Substring(0, 30) + "..." : job.Title;
                string title = $"{candidateName} favorited your job {jobTitle}";

                var notification = new Notification
                {
                    UserId = companyUserId,
                    Title = title,
                    Link = candidateEndpoint,
                    Type = Notification.NotificationType.JobFavorited,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Job favorited notification created for company #{companyUserId}");

                if (_notificationHubContext != null)
                {
                    try
                    {
                        await _notificationHubContext.Clients
                            .Group($"User_{companyUserId}")
                            .SendAsync("ReceiveNotification", notification);
                        _logger.LogInformation($"Real-time job favorited notification sent to company #{companyUserId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error sending SignalR notification: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateJobFavoritedNotification: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
            }
        }





        public async Task CreateTryMatchNotification(TryMatchRecord tryMatchRecord, int jobId, string jobTitle)
        {
            try
            {
                _logger.LogInformation($"Creating try-match notification for user {tryMatchRecord.UserId}");

                string tryMatchEndpoint = $"/try-match-details/{tryMatchRecord.TryMatchId}";
                string shortJobTitle = jobTitle.Length > 30 ? jobTitle.Substring(0, 30) + "..." : jobTitle;
                string title = tryMatchRecord.Status switch
                {
                    "Processing" => $"Try-match for {shortJobTitle} is processing",
                    "Completed" => $"Try-match for {shortJobTitle} completed",
                    "Failed" => $"Try-match for {shortJobTitle} failed",
                    _ => $"Try-match update for {shortJobTitle}"
                };

                var notification = new Notification
                {
                    UserId = tryMatchRecord.UserId,
                    Title = title,
                    Link = tryMatchEndpoint,
                    Type = Notification.NotificationType.TryMatchUpdate,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Try-match notification created for user {tryMatchRecord.UserId}");

                try
                {
                    await _notificationHubContext.Clients
                        .Group($"User_{tryMatchRecord.UserId}")
                        .SendAsync("ReceiveNotification", notification);
                    _logger.LogInformation($"Real-time try-match notification sent to user {tryMatchRecord.UserId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error sending SignalR try-match notification: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateTryMatchNotification: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

    }
}*/
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JOB_FINDER_API.Services
{
    public class NotificationService
    {
        private readonly JobFinderDbContext _context;
        private readonly EmailService _emailService;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly ILogger<NotificationService> _logger;
        private readonly IConfiguration _configuration;

        public NotificationService(
            JobFinderDbContext context,
            EmailService emailService,
            IHubContext<NotificationHub> notificationHubContext,
            ILogger<NotificationService> logger,
            IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _notificationHubContext = notificationHubContext;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task CreateNewJobNotification(Job job, User companyUser, IEnumerable<User> notificationRecipients, IEnumerable<User> emailRecipients = null)
        {
            try
            {
                var now = GetVietnamTime();

                // Check if job's start date has arrived yet
                if (job.TimeStart.Date > now.Date)
                {
                    _logger.LogInformation($"Job {job.JobId} start date ({job.TimeStart.Date}) is in the future. " +
                        $"Notifications will be sent on start date via scheduled task.");
                    return; // Exit without sending notifications if job start date is in the future
                }

                _logger.LogInformation($"Starting notification creation for job {job.JobId} with {notificationRecipients.Count()} notification recipients");
                if (emailRecipients != null)
                {
                    _logger.LogInformation($"Email will be sent to {emailRecipients.Count()} users who favorited the company");
                }

                var companyProfile = await _context.CompanyProfile
                    .FirstOrDefaultAsync(c => c.UserId == companyUser.UserId);

                string companyName = companyProfile?.CompanyName ?? companyUser.FullName ?? "Company";
                string baseUrl = _configuration["AppSettings:BaseUrl"];
                if (string.IsNullOrEmpty(baseUrl))
                {
                    _logger.LogError("BaseUrl is not configured in appsettings.json.");
                    throw new InvalidOperationException("BaseUrl is not configured in appsettings.json.");
                }
                string jobEndpoint = $"/job-single-v3/{job.JobId}";
                string jobUrl = $"{baseUrl}{jobEndpoint}";

                string jobTitle = job.Title.Length > 30 ? job.Title.Substring(0, 30) + "..." : job.Title;
                string province = job.ProvinceName.Length > 20 ? job.ProvinceName.Substring(0, 20) + "..." : job.ProvinceName;
                string title = $"{companyName} posted new job {jobTitle} in {province}";
                string startDate = job.TimeStart.ToString("dd/MM/yyyy");

                string mailBody = $@"
                    <div style='font-family: Arial, sans-serif;'>
                        <h2 style='color:#2d8cf0;'>Company {companyName} Just posted a new job!</h2>
                        <p><b>Title Job:</b> {job.Title}</p>
                        <p><b>Location:</b> {job.ProvinceName}</p>
                        <p><b>Deadline:</b> {job.ExpiryDate:dd/MM/yyyy}</p>
                        <p><b>Description:</b> {(job.Description?.Length > 200 ? job.Description?.Substring(0, 200) + "..." : job.Description)}</p>
                        <div style='margin:20px 0;'>
                            <a href='{jobUrl}' style='background:#2d8cf0;color:#fff;padding:10px 20px;border-radius:4px;text-decoration:none;font-weight:bold;'>View details & Apply</a>
                        </div>
                    </div>
                ";

                var notifications = new List<Notification>();
                int notificationsCreated = 0;

                foreach (var recipient in notificationRecipients)
                {
                    if (recipient?.UserId.HasValue == true)
                    {
                        try
                        {
                            _logger.LogInformation($"Creating notification for user ID: {recipient.UserId.Value}");

                            var notification = new Notification
                            {
                                UserId = recipient.UserId.Value,
                                Title = title,
                                Link = jobEndpoint, // Chỉ lưu endpoint
                                Type = Notification.NotificationType.NewJob,
                                IsRead = false,
                                CreatedAt = DateTime.UtcNow,
                                JobTimeStart = job.TimeStart.ToString("dd/MM/yyyy")
                            };

                            notifications.Add(notification);
                            notificationsCreated++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error creating notification for user {recipient.UserId}: {ex.Message}");
                        }
                    }
                }

                if (emailRecipients != null)
                {
                    foreach (var recipient in emailRecipients)
                    {
                        if (!string.IsNullOrEmpty(recipient?.Email))
                        {
                            try
                            {
                                _logger.LogInformation($"Sending email to user {recipient.UserId}: {recipient.Email}");

                                _emailService.SendEmail(
                                    recipient.Email,
                                    $"[{companyName}] just posted new job: {job.Title}",
                                    mailBody,
                                    true
                                );
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Error sending email to {recipient.Email}: {ex.Message}");
                            }
                        }
                    }
                }

                _logger.LogInformation($"Created {notificationsCreated} notifications, saving to database");

                if (notifications.Any())
                {
                    _context.Notifications.AddRange(notifications);
                    var savedCount = await _context.SaveChangesAsync();
                    _logger.LogInformation($"Successfully saved {savedCount} notifications to database");

                    if (_notificationHubContext != null)
                    {
                        foreach (var notification in notifications)
                        {
                            try
                            {
                                await _notificationHubContext.Clients
                                    .Group($"User_{notification.UserId}")
                                    .SendAsync("ReceiveNotification", new
                                    {
                                        notification.NotificationId,
                                        notification.UserId,
                                        notification.Title,
                                        notification.Link,
                                        notification.Type,
                                        notification.IsRead,
                                        notification.CreatedAt,
                                        JobTimeStart = job.TimeStart.ToString("dd/MM/yyyy") // Add TimeStart
                                    });
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Error sending real-time notification: {ex.Message}");
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No notifications created for this job");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateNewJobNotification: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        private static DateTime GetVietnamTime()
        {
            var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);
        }

        public async Task SendStartDateReachedNotifications(Job job)
        {
            try
            {
                if (job == null)
                {
                    _logger.LogError("Cannot send start date notifications: Job is null");
                    return;
                }

                var now = GetVietnamTime();
                if (job.TimeStart.Date != now.Date)
                {
                    _logger.LogWarning($"Job {job.JobId} start date ({job.TimeStart.Date}) does not match today's date ({now.Date})");
                    return;
                }

                // Only process active jobs
                if (job.Status != Job.JobStatus.active || job.DeactivatedByAdmin)
                {
                    _logger.LogInformation($"Job {job.JobId} is not active or is deactivated by admin. Skipping notifications.");
                    return;
                }

                var companyUser = await _context.Users.FindAsync(job.CompanyId);
                if (companyUser == null)
                {
                    _logger.LogWarning($"Cannot send start date notifications: Company user #{job.CompanyId} not found");
                    return;
                }

                // Get favorite users for emails
                var favoriteUsers = await _context.UserFavoriteCompanies
                    .Where(f => f.CompanyProfileId == job.CompanyId)
                    .Include(f => f.User)
                    .Select(f => f.User)
                    .ToListAsync();

                // Get all candidates
                var allCandidates = await _context.Users
                    .Where(u => u.RoleId == 1) // Assuming 1 is Candidate role
                    .ToListAsync();

                await CreateNewJobNotification(job, companyUser, allCandidates, favoriteUsers);
                _logger.LogInformation($"Start date reached notifications sent for job #{job.JobId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in SendStartDateReachedNotifications: {ex.Message}, StackTrace: {ex.StackTrace}");
            }
        }

        public async Task CreateNewJobNotification(Job job, User companyUser, IEnumerable<User> recipients)
        {
            await CreateNewJobNotification(job, companyUser, recipients, recipients);
        }


        public async Task CreateJobStatusNotification(Job job, bool isApproved)
        {
            try
            {
                if (job.CompanyId > 0)
                {
                    var companyUserId = job.CompanyId;
                    _logger.LogInformation($"Creating job status notification for company user {companyUserId}, job #{job.JobId}");

                    string jobTitle = job.Title.Length > 30 ? job.Title.Substring(0, 30) + "..." : job.Title;
                    string title = isApproved ? $"Your job {jobTitle} was approved by admin" : $"Your job {jobTitle} was not approved";
                    string jobEndpoint = $"/job-single-v3/{job.JobId}";

                    var notification = new Notification
                    {
                        UserId = companyUserId,
                        Title = title,
                        Link = jobEndpoint,
                        Type = isApproved ? Notification.NotificationType.JobApproved : Notification.NotificationType.JobRejected,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Job status notification created for company user {companyUserId}");

                    if (_notificationHubContext != null)
                    {
                        try
                        {
                            await _notificationHubContext.Clients
                                .Group($"User_{companyUserId}")
                                //.SendAsync("ReceiveNotification", notification);
                                .SendAsync("ReceiveNotification", new
                                {
                                    notification.NotificationId,
                                    notification.UserId,
                                    notification.Title,
                                    notification.Link,
                                    notification.Type,
                                    notification.IsRead,
                                    notification.CreatedAt,
                                    JobTimeStart = job.TimeStart.ToString("dd/MM/yyyy") // Add TimeStart
                                });
                            _logger.LogInformation($"Real-time job status notification sent to company user {companyUserId}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error sending SignalR notification: {ex.Message}");
                        }
                    }
                }
                else
                {
                    _logger.LogWarning($"Cannot create company notification: Job #{job.JobId} has no valid company ID");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateJobStatusNotification: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
            }
        }

        public async Task<List<Notification>> GetUserNotifications(int userId, bool? isRead = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Notifications
                .Where(n => n.UserId == userId);

            if (isRead.HasValue)
            {
                query = query.Where(n => n.IsRead == isRead.Value);
            }

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task MarkAsRead(int notificationId, int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();

                await _notificationHubContext.Clients
                    .Group($"User_{userId}")
                    .SendAsync("NotificationRead", notificationId);
            }
        }


        public async Task MarkAllAsRead(int userId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();

            await _notificationHubContext.Clients
                .Group($"User_{userId}")
                .SendAsync("AllNotificationsRead");
        }


        public async Task<int> GetUnreadCount(int userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task SendDirectNotification(int userId, string title, string link, Notification.NotificationType type)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Link = link, // Chỉ chứa endpoint
                    Type = type,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                await _notificationHubContext.Clients.Group($"User_{userId}")
                    .SendAsync("ReceiveNotification", new
                    {
                        id = notification.NotificationId,
                        title = notification.Title,
                        link = notification.Link,
                        type = notification.Type,
                        isRead = notification.IsRead,
                        createdAt = notification.CreatedAt
                    });

                _logger.LogInformation($"Direct notification sent to user {userId}: {title}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending direct notification: {ex.Message}");
                throw;
            }
        }

        public async Task CreateNewApplicationNotification(Application application, User candidate, Job job)
        {
            try
            {
                if (job.CompanyId <= 0)
                {
                    _logger.LogWarning($"Cannot create application notification: Job #{job.JobId} has no valid company ID");
                    return;
                }

                var companyUserId = job.CompanyId;
                var company = await _context.Users.FindAsync(companyUserId);
                if (company == null)
                {
                    _logger.LogWarning($"Cannot create application notification: Company user #{companyUserId} not found");
                    return;
                }


                string applicationEndpoint = $"/company-dashboard/candidates/details/{application.ApplicationId}";
                string candidateName = candidate.FullName?.Length > 20 ? candidate.FullName.Substring(0, 20) + "..." : candidate.FullName ?? "User";
                string jobTitle = job.Title.Length > 30 ? job.Title.Substring(0, 30) + "..." : job.Title;
                string title = $"{candidateName} applied for your job {jobTitle}";

                var notification = new Notification
                {
                    UserId = companyUserId,
                    Title = title,
                    Link = applicationEndpoint,
                    Type = Notification.NotificationType.NewJobApplication,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"New application notification created for company #{companyUserId}");

                if (_notificationHubContext != null)
                {
                    try
                    {
                        await _notificationHubContext.Clients
                            .Group($"User_{companyUserId}")
                            .SendAsync("ReceiveNotification", notification);
                        _logger.LogInformation($"Real-time application notification sent to company #{companyUserId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error sending SignalR notification: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateNewApplicationNotification: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
            }
        }

        public async Task CreateCompanyFavoritedNotification(UserFavoriteCompany favoriteCompany, User candidate)
        {
            try
            {
                var companyUserId = favoriteCompany.CompanyProfileId;

                string candidateEndpoint = $"/candidates-single-v1/{candidate.UserId}";
                string candidateName = candidate.FullName?.Length > 20 ? candidate.FullName.Substring(0, 20) + "..." : candidate.FullName ?? "Candidate";
                string title = $"{candidateName} favorited your company";

                var notification = new Notification
                {
                    UserId = companyUserId,
                    Title = title,
                    Link = candidateEndpoint,
                    Type = Notification.NotificationType.CompanyFavorited,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Company favorited notification created for company #{companyUserId}");

                if (_notificationHubContext != null)
                {
                    try
                    {
                        await _notificationHubContext.Clients
                            .Group($"User_{companyUserId}")
                            .SendAsync("ReceiveNotification", notification);
                        _logger.LogInformation($"Real-time company favorited notification sent to company #{companyUserId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error sending SignalR notification: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateCompanyFavoritedNotification: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
            }
        }

        public async Task CreateJobFavoritedNotification(UserFavoriteJob favoriteJob, User candidate)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(favoriteJob.JobId);
                if (job == null)
                {
                    _logger.LogWarning($"Cannot create job favorited notification: Job #{favoriteJob.JobId} not found");
                    return;
                }

                if (job.CompanyId <= 0)
                {
                    _logger.LogWarning($"Cannot create job favorited notification: Job #{job.JobId} has no valid company ID");
                    return;
                }

                var companyUserId = job.CompanyId;

                string candidateEndpoint = $"/candidates-single-v1/{candidate.UserId}";
                string candidateName = candidate.FullName?.Length > 20 ? candidate.FullName.Substring(0, 20) + "..." : candidate.FullName ?? "User";
                string jobTitle = job.Title.Length > 30 ? job.Title.Substring(0, 30) + "..." : job.Title;
                string title = $"{candidateName} favorited your job {jobTitle}";

                var notification = new Notification
                {
                    UserId = companyUserId,
                    Title = title,
                    Link = candidateEndpoint,
                    Type = Notification.NotificationType.JobFavorited,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Job favorited notification created for company #{companyUserId}");

                if (_notificationHubContext != null)
                {
                    try
                    {
                        await _notificationHubContext.Clients
                            .Group($"User_{companyUserId}")
                            .SendAsync("ReceiveNotification", notification);
                        _logger.LogInformation($"Real-time job favorited notification sent to company #{companyUserId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error sending SignalR notification: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateJobFavoritedNotification: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
            }
        }

        public async Task CreateTryMatchNotification(TryMatchRecord tryMatchRecord, int jobId, string jobTitle)
        {
            try
            {
                _logger.LogInformation($"Creating try-match notification for user {tryMatchRecord.UserId}");

                string tryMatchEndpoint = $"/try-match-details/{tryMatchRecord.TryMatchId}";
                string shortJobTitle = jobTitle.Length > 30 ? jobTitle.Substring(0, 30) + "..." : jobTitle;
                string title = tryMatchRecord.Status switch
                {
                    "Processing" => $"Try-match for {shortJobTitle} is processing",
                    "Completed" => $"Try-match for {shortJobTitle} completed",
                    "Failed" => $"Try-match for {shortJobTitle} failed",
                    _ => $"Try-match update for {shortJobTitle}"
                };

                var notification = new Notification
                {
                    UserId = tryMatchRecord.UserId,
                    Title = title,
                    Link = tryMatchEndpoint,
                    Type = Notification.NotificationType.TryMatchUpdate,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Try-match notification created for user {tryMatchRecord.UserId}");

                try
                {
                    await _notificationHubContext.Clients
                        .Group($"User_{tryMatchRecord.UserId}")
                        .SendAsync("ReceiveNotification", notification);
                    _logger.LogInformation($"Real-time try-match notification sent to user {tryMatchRecord.UserId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error sending SignalR try-match notification: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateTryMatchNotification: {ex.Message}, StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}