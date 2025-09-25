/*using Cronos;
using JOB_FINDER_API.Controllers;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JOB_FINDER_API.Models.Services
{
    public class UpcomingJobNotificationService : BackgroundService
    {
        private readonly ILogger<UpcomingJobNotificationService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeZoneInfo _vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public UpcomingJobNotificationService(ILogger<UpcomingJobNotificationService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("UpcomingJobNotificationService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var cron = Cronos.CronExpression.Parse("0 0 * * *", CronFormat.Standard); // Chạy lúc 00:00 AM hàng ngày
                var utcNow = DateTime.UtcNow;
                var nextRunUtc = cron.GetNextOccurrence(utcNow);
                if (nextRunUtc.HasValue)
                {
                    var nextRunVietnam = TimeZoneInfo.ConvertTimeFromUtc(nextRunUtc.Value, _vietnamTimeZone);
                    var delay = nextRunVietnam - DateTime.Now;
                    _logger.LogInformation("Next run scheduled at {Time} (Vietnam)", nextRunVietnam);

                    try
                    {
                        await Task.Delay(delay, stoppingToken);
                        if (!stoppingToken.IsCancellationRequested)
                        {
                            using var scope = _scopeFactory.CreateScope();
                            var controller = scope.ServiceProvider.GetRequiredService<JobController>();
                            await controller.NotifyUpcomingStartNew(2); // Vẫn gọi GET endpoint
                          
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        _logger.LogWarning("Job execution canceled.");
                        break;
                    }
                }
            }

            _logger.LogInformation("UpcomingJobNotificationService is stopping.");
        }
    }
}*/
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cronos;

namespace JOB_FINDER_API.Models.Services
{
    public class UpcomingJobNotificationService : BackgroundService
    {
        private readonly ILogger<UpcomingJobNotificationService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeZoneInfo _vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public UpcomingJobNotificationService(ILogger<UpcomingJobNotificationService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("UpcomingJobNotificationService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var cron = Cronos.CronExpression.Parse("0 0 * * *", CronFormat.Standard); // 00:00 AM hàng ngày
                var utcNow = DateTime.UtcNow;
                var nextRunUtc = cron.GetNextOccurrence(utcNow);
                if (nextRunUtc.HasValue)
                {
                    var delay = nextRunUtc.Value - utcNow;
                    _logger.LogInformation("Next run scheduled at {Time} (UTC)", nextRunUtc.Value);

                    try
                    {
                        await Task.Delay(delay, stoppingToken);
                        if (!stoppingToken.IsCancellationRequested)
                        {
                            using var scope = _scopeFactory.CreateScope();
                            var dbContext = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                            var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                            var now = DateTime.UtcNow;
                            var soon = now.AddDays(1);

                            var jobs = await dbContext.Jobs
                                .Include(j => j.Company)
                                .Include(j => j.Applications)
                                .Where(j => j.TimeEnd > now && j.TimeEnd <= soon)
                                .ToListAsync(stoppingToken);

                            foreach (var job in jobs)
                            {
                                bool hasPending = job.Applications.Any(a => a.Status == ApplicationStatus.Pending);
                                if (hasPending && job.Company != null && !string.IsNullOrEmpty(job.Company.Email))
                                {
                                    string subject = $"[JobFinder] Your job '{job.Title}' is expiring soon and has pending applications";
                                    string body = $"Dear {job.Company.FullName},\n\n" +
                                                  $"Your job posting \"{job.Title}\" will expire on {TimeZoneInfo.ConvertTimeFromUtc(job.TimeEnd, _vietnamTimeZone):yyyy-MM-dd HH:mm}. " +
                                                  $"There are still applications in 'Pending' status. Please review them before the job expires.\n\n" +
                                                  $"Best regards,\nJobFinder Team";

                                    await emailService.SendEmailAsync(job.Company.Email, subject, body);
                                    _logger.LogInformation($"Sent expiring job notification to company {job.Company.Email} for job {job.JobId}");
                                }
                            }
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        _logger.LogWarning("Job execution canceled.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in UpcomingJobNotificationService");
                    }
                }
            }

            _logger.LogInformation("UpcomingJobNotificationService is stopping.");
        }
    }
}