/*using JOB_FINDER_API.Data;
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

        public NotificationService(
            JobFinderDbContext context,
            EmailService emailService,
            IHubContext<NotificationHub> notificationHubContext,
            ILogger<NotificationService> logger)
        {
            _context = context;
            _emailService = emailService;
            _notificationHubContext = notificationHubContext;
            _logger = logger;
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

                string companyName = companyProfile?.CompanyName ?? companyUser.FullName ?? "Công ty";
                string jobUrl = $"http://localhost:3000/job-single-v3/{job.JobId}";

                string title = $"New job from {companyName}";
                string message = $"{companyName}just posted new job: {job.Title}";

                string mailBody = $@"
            <div style='font-family: Arial, sans-serif;'>
                <h2 style='color:#2d8cf0;'>Company {companyName} Just posted a new job!</h2>
                <p><b>Title Job:</b> {job.Title}</p>
                <p><b>Location:</b> {job.ProvinceName}</p>
                <p><b>Submission deadline:</b> {job.ExpiryDate:dd/MM/yyyy}</p>
                <p><b>Describe:</b> {(job.Description?.Length > 200 ? job.Description?.Substring(0, 200) + "..." : job.Description)}</p>
                <div style='margin:20px 0;'>
                    <a href='{jobUrl}' style='background:#2d8cf0;color:#fff;padding:10px 20px;border-radius:4px;text-decoration:none;font-weight:bold;'>View details & Apply</a>
                </div>
            </div>
        ";

                // Create notifications for all candidates
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
                                Message = message,
                                Link = jobUrl,
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

                // Send emails only to users who favorited the company
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
                                    $"[{companyName}] Just posted a new job: {job.Title}",
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

                // Save notifications to database
                if (notifications.Any())
                {
                    // Add all notifications to the context
                    _context.Notifications.AddRange(notifications);

                    // Save changes
                    var savedCount = await _context.SaveChangesAsync();

                    _logger.LogInformation($"Successfully saved {savedCount} notifications to database");

                    // Send real-time notifications
                    if (_notificationHubContext != null)
                    {
                        foreach (var notification in notifications)
                        {
                            try
                            {
                                await _notificationHubContext.Clients
                                    .Group($"User_{notification.UserId}")
                                    .SendAsync("ReceiveNotification", notification);
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

        // Keep the original method for backward compatibility
        public async Task CreateNewJobNotification(Job job, User companyUser, IEnumerable<User> recipients)
        {
            // Call the new overloaded method, sending emails to all recipients
            await CreateNewJobNotification(job, companyUser, recipients, recipients);
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

                // Notify the user that notification has been marked as read
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

            // Notify the user that all notifications have been marked as read
            await _notificationHubContext.Clients
                .Group($"User_{userId}")
                .SendAsync("AllNotificationsRead");
        }

        public async Task<int> GetUnreadCount(int userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task SendDirectNotification(int userId, string title, string message, string link, Notification.NotificationType type)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    Link = link,
                    Type = type,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                // Save to database
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                // Send real-time via SignalR
                await _notificationHubContext.Clients.Group($"User_{userId}")
                    .SendAsync("ReceiveNotification", new
                    {
                        id = notification.NotificationId,
                        title = notification.Title,
                        message = notification.Message,
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

        public NotificationService(
            JobFinderDbContext context,
            EmailService emailService,
            IHubContext<NotificationHub> notificationHubContext,
            ILogger<NotificationService> logger)
        {
            _context = context;
            _emailService = emailService;
            _notificationHubContext = notificationHubContext;
            _logger = logger;
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
                string jobUrl = $"http://localhost:3000/job-single-v3/{job.JobId}";

                string title = $"New job from{companyName}";
                string message = $"{companyName} just posted a new job: {job.Title}";

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

                // Create notifications for all candidates
                var notifications = new List<Notification>();
                int notificationsCreated = 0;

                foreach (var recipient in notificationRecipients)
                {
                    if (recipient?.UserId.HasValue == true) // UserId is nullable (int?), so this check is valid
                    {
                        try
                        {
                            _logger.LogInformation($"Creating notification for user ID: {recipient.UserId.Value}");

                            var notification = new Notification
                            {
                                UserId = recipient.UserId.Value, // Use Value to convert int? to int
                                Title = title,
                                Message = message,
                                Link = jobUrl,
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

                // Send emails only to users who favorited the company
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

                // Save notifications to database
                if (notifications.Any())
                {
                    // Add all notifications to the context
                    _context.Notifications.AddRange(notifications);

                    // Save changes
                    var savedCount = await _context.SaveChangesAsync();

                    _logger.LogInformation($"Successfully saved {savedCount} notifications to database");

                    // Send real-time notifications
                    if (_notificationHubContext != null)
                    {
                        foreach (var notification in notifications)
                        {
                            try
                            {
                                await _notificationHubContext.Clients
                                    .Group($"User_{notification.UserId}")
                                    .SendAsync("ReceiveNotification", notification);
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

        // Keep the original method for backward compatibility
        public async Task CreateNewJobNotification(Job job, User companyUser, IEnumerable<User> recipients)
        {
            // Call the new overloaded method, sending emails to all recipients
            await CreateNewJobNotification(job, companyUser, recipients, recipients);
        }

        public async Task CreateJobStatusNotification(Job job, bool isApproved)
        {
            try
            {
                // Check if CompanyId is valid (greater than 0)
                // instead of using HasValue which only works on nullable types
                if (job.CompanyId > 0)
                {
                    var companyUserId = job.CompanyId; // Direct assignment, no .Value needed
                    _logger.LogInformation($"Creating job status notification for company user {companyUserId}, job #{job.JobId}");

                    string title = isApproved ?
                        "Job approved" :
                        "Job not approved";

                    string message = isApproved ?
                        $"Job '{job.Title}' Yours has been approved by the admin and displayed on the system." :
                        $"Job '{job.Title}' Your request has been rejected by the admin.";

                    string jobUrl = $"http://localhost:3000/job-single-v3/{job.JobId}";

                    var notification = new Notification
                    {
                        UserId = companyUserId,
                        Title = title,
                        Message = message,
                        Link = jobUrl,
                        Type = isApproved ? Notification.NotificationType.JobApproved : Notification.NotificationType.JobRejected,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    // Add notification to database
                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Job status notification created for company user {companyUserId}");

                    // Send real-time notification via SignalR
                    if (_notificationHubContext != null)
                    {
                        try
                        {
                            await _notificationHubContext.Clients
                                .Group($"User_{companyUserId}")
                                .SendAsync("ReceiveNotification", notification);

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

                // Notify the user that notification has been marked as read
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

            // Notify the user that all notifications have been marked as read
            await _notificationHubContext.Clients
                .Group($"User_{userId}")
                .SendAsync("AllNotificationsRead");
        }

        public async Task<int> GetUnreadCount(int userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task SendDirectNotification(int userId, string title, string message, string link, Notification.NotificationType type)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    Link = link,
                    Type = type,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                // Save to database
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                // Send real-time via SignalR
                await _notificationHubContext.Clients.Group($"User_{userId}")
                    .SendAsync("ReceiveNotification", new
                    {
                        id = notification.NotificationId,
                        title = notification.Title,
                        message = notification.Message,
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
                // Make sure we have a valid company ID
                if (job.CompanyId <= 0)
                {
                    _logger.LogWarning($"Cannot create application notification: Job #{job.JobId} has no valid company ID");
                    return;
                }

                var companyUserId = job.CompanyId;

                // Get company user details for notification content
                var company = await _context.Users.FindAsync(companyUserId);
                if (company == null)
                {
                    _logger.LogWarning($"Cannot create application notification: Company user #{companyUserId} not found");
                    return;
                }

                string candidateName = candidate.FullName ?? "User";
                string jobUrl = $"http://localhost:3000/job-single-v3/{job.JobId}";
                string candidateProfileUrl = $"http://localhost:3000/candidate-profile/{candidate.UserId}";
                string applicationUrl = $"http://localhost:3000/company-dashboard/candidates/details/{application.ApplicationId}";

                string title = $"Candidate's new job: {job.Title}";
                string message = $"{candidateName} applied for the job {job.Title} your";

                var notification = new Notification
                {
                    UserId = companyUserId,
                    Title = title,
                    Message = message,
                    Link = applicationUrl,  // Link to view the application details
                    Type = Notification.NotificationType.NewJobApplication,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                // Add to database
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"New application notification created for company #{companyUserId}");

                // Send real-time notification via SignalR
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
                var companyUserId = favoriteCompany.CompanyId;

                string candidateName = candidate.FullName ?? "Ứng viên";
                string candidateProfileUrl = $"http://localhost:3000/candidate-profile/{candidate.UserId}";

                string title = "Your company is loved";
                string message = $"{candidateName} added your company to favorites";

                var notification = new Notification
                {
                    UserId = companyUserId,
                    Title = title,
                    Message = message,
                    Link = candidateProfileUrl,  // Link to view the candidate's profile
                    Type = Notification.NotificationType.CompanyFavorited,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                // Add to database
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Company favorited notification created for company #{companyUserId}");

                // Send real-time notification via SignalR
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
                // Get the job details
                var job = await _context.Jobs.FindAsync(favoriteJob.JobId);
                if (job == null)
                {
                    _logger.LogWarning($"Cannot create job favorited notification: Job #{favoriteJob.JobId} not found");
                    return;
                }

                // Make sure we have a valid company ID
                if (job.CompanyId <= 0)
                {
                    _logger.LogWarning($"Cannot create job favorited notification: Job #{job.JobId} has no valid company ID");
                    return;
                }

                var companyUserId = job.CompanyId;

                string candidateName = candidate.FullName ?? "User";
                string jobUrl = $"http://localhost:3000/job-single-v3/{job.JobId}";
                string candidateProfileUrl = $"http://localhost:3000/candidate-profile/{candidate.UserId}";

                string title = "Favorite job";
                string message = $"{candidateName} added job {job.Title} to favorites";

                var notification = new Notification
                {
                    UserId = companyUserId,
                    Title = title,
                    Message = message,
                    Link = candidateProfileUrl, // Link to view the candidate's profile
                    Type = Notification.NotificationType.JobFavorited,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                // Add to database
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Job favorited notification created for company #{companyUserId}");

                // Send real-time notification via SignalR
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
    }
}