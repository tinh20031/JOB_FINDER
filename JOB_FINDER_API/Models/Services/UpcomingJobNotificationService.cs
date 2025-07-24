using JOB_FINDER_API.Controllers;
using JOB_FINDER_API.Models;
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
                var cron = CronExpression.Parse("0 0 * * *", CronFormat.Standard); // Chạy lúc 00:00 AM hàng ngày
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
}