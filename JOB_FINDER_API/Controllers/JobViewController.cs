using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobViewController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        private readonly TimeSpan _viewCooldown = TimeSpan.FromMinutes(1); // Minimum time between views from same source

        public JobViewController(JobFinderDbContext context)
        {
            _context = context;
        }

        [HttpPost("track")]
        public async Task<IActionResult> TrackJobView([FromBody] JobViewTrackRequest request)
        {
            if (request == null || request.JobId <= 0)
            {
                return BadRequest("Invalid job ID");
            }

            // Check if job exists
            var job = await _context.Jobs.FindAsync(request.JobId);
            if (job == null)
            {
                return NotFound("Job not found");
            }

            // Get current user ID if authenticated
            int? userId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int parsedUserId))
                {
                    userId = parsedUserId;
                }
            }

            // Get IP address
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            // Get User Agent
            string? userAgent = Request.Headers["User-Agent"].ToString();

            // Current time
            var currentTime = DateTime.UtcNow;

            // Check for recent views from the same source (user or IP)
            var recentView = await _context.JobViews
                .Where(v => v.JobId == request.JobId &&
                          ((userId.HasValue && v.UserId == userId) || // Same user
                           (!userId.HasValue && v.IpAddress == ipAddress))) // Same IP for anonymous
                .OrderByDescending(v => v.ViewedAt)
                .FirstOrDefaultAsync();

            // If a recent view exists and it's within the cooldown period, don't count as a new view
            if (recentView != null && (currentTime - recentView.ViewedAt) < _viewCooldown)
            {
                return Ok(new
                {
                    message = "View not counted due to cooldown period",
                    counted = false,
                    lastViewedAt = recentView.ViewedAt
                });
            }

            // Create new job view record
            var jobView = new JobView
            {
                JobId = request.JobId,
                UserId = userId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                ViewedAt = currentTime
            };

            _context.JobViews.Add(jobView);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "View tracked successfully",
                jobViewId = jobView.JobViewId,
                counted = true
            });
        }

        [HttpGet("count/{jobId}")]
        public async Task<IActionResult> GetJobViewCount(int jobId)
        {
            // Check if job exists
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null)
            {
                return NotFound("Job not found");
            }

            // Get total view count
            int viewCount = await _context.JobViews.CountAsync(v => v.JobId == jobId);

            // Get unique viewer count (by distinct users and IPs)
            var uniqueUserViews = await _context.JobViews
                .Where(v => v.JobId == jobId && v.UserId.HasValue)
                .Select(v => v.UserId)
                .Distinct()
                .CountAsync();

            var uniqueIpViews = await _context.JobViews
                .Where(v => v.JobId == jobId && !v.UserId.HasValue && v.IpAddress != null)
                .Select(v => v.IpAddress)
                .Distinct()
                .CountAsync();

            int uniqueViewerCount = uniqueUserViews + uniqueIpViews;

            return Ok(new
            {
                jobId,
                totalViews = viewCount,
                uniqueViewers = uniqueViewerCount
            });
        }

        [HttpGet("recent/{jobId}")]
        public async Task<IActionResult> GetRecentJobViews(int jobId, [FromQuery] int hours = 24)
        {
            // Check if job exists
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null)
            {
                return NotFound("Job not found");
            }

            // Get recent views within the specified hours
            var startTime = DateTime.UtcNow.AddHours(-hours);

            var recentViews = await _context.JobViews
                .Where(v => v.JobId == jobId && v.ViewedAt >= startTime)
                .ToListAsync();

            var recentViewsCount = recentViews.Count;

            // Group by hour for charting
            var hourlyStats = recentViews
                .GroupBy(v => new { Hour = v.ViewedAt.Hour, Date = v.ViewedAt.Date })
                .Select(g => new
                {
                    DateTime = g.Key.Date.AddHours(g.Key.Hour),
                    Hour = g.Key.Hour,
                    Date = g.Key.Date.ToString("yyyy-MM-dd"),
                    Count = g.Count()
                })
                .OrderBy(x => x.DateTime)
                .ToList();

            return Ok(new
            {
                jobId,
                period = $"Last {hours} hours",
                totalViews = recentViewsCount,
                hourlyStats
            });
        }
    }
}