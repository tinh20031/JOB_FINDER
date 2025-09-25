using JOB_FINDER_API.Data;
using Microsoft.EntityFrameworkCore;

namespace JOB_FINDER_API.Models.Services
{
    public class JobStatusService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<JobStatusService> _logger;

        public JobStatusService(
            IServiceProvider serviceProvider,
            ILogger<JobStatusService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        private static DateTime NowVN()
        {
            var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Job Status Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await UpdateJobStatuses();
                    await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Job Status Service");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }

        private async Task UpdateJobStatuses()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();

            var now = NowVN();
            var updatedCount = 0;

        
            var jobsToInactive = await context.Jobs
                .Where(j => j.Status == Job.JobStatus.active &&
                           j.TimeStart.Date > now.Date)
                .ToListAsync();

            foreach (var job in jobsToInactive)
            {
                job.Status = Job.JobStatus.inactive;
                job.UpdatedAt = now;
                updatedCount++;
                _logger.LogInformation($"Job {job.JobId} ({job.Title}) set to inactive - not started yet");
            }

           
            var jobsToActive = await context.Jobs
                .Where(j => j.Status == Job.JobStatus.inactive &&
                           !j.DeactivatedByAdmin &&
                           j.TimeStart.Date <= now.Date &&
                           j.TimeEnd.Date >= now.Date)
                .ToListAsync();

            foreach (var job in jobsToActive)
            {
                job.Status = Job.JobStatus.active;
                job.UpdatedAt = now;
                updatedCount++;
                _logger.LogInformation($"Job {job.JobId} ({job.Title}) set to active - start date reached");
            }

         
            var expiredJobs = await context.Jobs
                .Where(j => j.Status == Job.JobStatus.active &&
                           j.TimeEnd.Date < now.Date)
                .ToListAsync();

            foreach (var job in expiredJobs)
            {
                job.Status = Job.JobStatus.inactive;
                job.UpdatedAt = now;
                updatedCount++;
                _logger.LogInformation($"Job {job.JobId} ({job.Title}) set to inactive - expired");
            }

           
            if (updatedCount > 0)
            {
                await context.SaveChangesAsync();
                _logger.LogInformation($"Updated {updatedCount} jobs");
            }
        }
    }
}