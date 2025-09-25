using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JobStatisticsController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        private readonly ApplyPercentageCalculator _applyPercentageCalculator;

        public JobStatisticsController(JobFinderDbContext context, ApplyPercentageCalculator applyPercentageCalculator)
        {
            _context = context;
            _applyPercentageCalculator = applyPercentageCalculator;
        }

        /* [HttpGet("job/{jobId}")]
         public async Task<IActionResult> GetJobStatistics(int jobId, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
         {
             var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
             if (!int.TryParse(userIdClaim, out var userId))
                 return Unauthorized("Invalid user ID.");

             var role = User.FindFirst(ClaimTypes.Role)?.Value.ToLower();

             // Kiểm tra job tồn tại
             var job = await _context.Jobs.FindAsync(jobId);
             if (job == null) return NotFound("Job không tồn tại");

             // Chỉ company sở hữu job hoặc admin mới có thể xem thống kê
             if (role == "company" && job.CompanyId != userId)
                 return Forbid("Bạn không có quyền xem thống kê của job này");

             // Set default date range if not provided
             var effectiveFromDate = fromDate ?? DateTime.UtcNow.AddDays(-7);
             var effectiveToDate = toDate ?? DateTime.UtcNow;

             // Ensure toDate is at the end of the day
             effectiveToDate = effectiveToDate.Date.AddDays(1).AddSeconds(-1);

             // Đếm số lượt xem job từ bảng JobView trong khoảng thời gian được chọn
             int viewCount = await _context.JobViews
                 .CountAsync(v => v.JobId == jobId && v.ViewedAt >= effectiveFromDate && v.ViewedAt <= effectiveToDate);

             // Đếm số lượt apply trong khoảng thời gian được chọn
             int applyCount = await _context.Applications
                 .CountAsync(a => a.JobId == jobId && a.SubmittedAt >= effectiveFromDate && a.SubmittedAt <= effectiveToDate);

             // Tính phần trăm apply
             bool success = _applyPercentageCalculator.CalculateApplyPercentage(
                 viewCount,
                 applyCount,
                 2, // Làm tròn đến 2 chữ số thập phân
                 out double applyPercentage,
                 out string errorMessage
             );

             if (!success)
             {
                 return BadRequest(new { ErrorMessage = errorMessage });
             }

             // Get raw data first, then perform client-side operations
             var jobViews = await _context.JobViews
                 .Where(v => v.JobId == jobId && v.ViewedAt >= effectiveFromDate && v.ViewedAt <= effectiveToDate)
                 .ToListAsync();

             var dailyViewStats = jobViews
                 .GroupBy(v => v.ViewedAt.Date)
                 .Select(g => new
                 {
                     Date = g.Key.ToString("yyyy-MM-dd"),
                     ViewCount = g.Count()
                 })
                 .OrderBy(x => x.Date)
                 .ToList();

             // Do the same for applications
             var applications = await _context.Applications
                 .Where(a => a.JobId == jobId && a.SubmittedAt >= effectiveFromDate && a.SubmittedAt <= effectiveToDate)
                 .ToListAsync();

             var dailyApplyStats = applications
                 .GroupBy(a => a.SubmittedAt.Date)
                 .Select(g => new
                 {
                     Date = g.Key.ToString("yyyy-MM-dd"),
                     ApplyCount = g.Count()
                 })
                 .OrderBy(x => x.Date)
                 .ToList();

             return Ok(new
             {
                 JobId = jobId,
                 ViewCount = viewCount,
                 ApplyCount = applyCount,
                 ApplyPercentage = applyPercentage,
                 Title = job.Title,
                 Period = new
                 {
                     From = effectiveFromDate.ToString("yyyy-MM-dd"),
                     To = effectiveToDate.ToString("yyyy-MM-dd")
                 },
                 DailyViewStats = dailyViewStats,
                 DailyApplyStats = dailyApplyStats
             });
         }*/
        [HttpGet("job/{jobId}")]
        public async Task<IActionResult> GetJobStatistics(int jobId, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user ID.");

            var role = User.FindFirst(ClaimTypes.Role)?.Value.ToLower();

            // Kiểm tra job tồn tại
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null) return NotFound("Job không tồn tại");

            // Chỉ company sở hữu job hoặc admin mới có thể xem thống kê
            if (role == "company" && job.CompanyId != userId)
                return Forbid("Bạn không có quyền xem thống kê của job này");

            // Set default date range if not provided
            var effectiveFromDate = fromDate ?? DateTime.UtcNow.AddDays(-7);
            var effectiveToDate = toDate ?? DateTime.UtcNow;

            // Ensure correct date order (swap if fromDate is after toDate)
            if (effectiveFromDate > effectiveToDate)
            {
                var temp = effectiveFromDate;
                effectiveFromDate = effectiveToDate;
                effectiveToDate = temp;
            }

            // Ensure toDate is at the end of the day
            effectiveToDate = effectiveToDate.Date.AddDays(1).AddSeconds(-1);

            // Log date range for debugging
            Console.WriteLine($"Date range for query: {effectiveFromDate:yyyy-MM-dd} to {effectiveToDate:yyyy-MM-dd}");

            // Đếm số lượt xem job từ bảng JobView trong khoảng thời gian được chọn
            int viewCount = await _context.JobViews
                .CountAsync(v => v.JobId == jobId && v.ViewedAt >= effectiveFromDate && v.ViewedAt <= effectiveToDate);

            // Đếm số lượt apply trong khoảng thời gian được chọn
            int applyCount = await _context.Applications
                .CountAsync(a => a.JobId == jobId && a.SubmittedAt >= effectiveFromDate && a.SubmittedAt <= effectiveToDate);

            // Tính phần trăm apply
            bool success = _applyPercentageCalculator.CalculateApplyPercentage(
                viewCount,
                applyCount,
                2, // Làm tròn đến 2 chữ số thập phân
                out double applyPercentage,
                out string errorMessage
            );

            if (!success)
            {
                return BadRequest(new { ErrorMessage = errorMessage });
            }

            // Get raw data first, then perform client-side operations
            var jobViews = await _context.JobViews
                .Where(v => v.JobId == jobId && v.ViewedAt >= effectiveFromDate && v.ViewedAt <= effectiveToDate)
                .ToListAsync();

            var dailyViewStats = jobViews
                .GroupBy(v => v.ViewedAt.Date)
                .Select(g => new
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    ViewCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();

            // Do the same for applications
            var applications = await _context.Applications
                .Where(a => a.JobId == jobId && a.SubmittedAt >= effectiveFromDate && a.SubmittedAt <= effectiveToDate)
                .ToListAsync();

            var dailyApplyStats = applications
                .GroupBy(a => a.SubmittedAt.Date)
                .Select(g => new
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    ApplyCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();

            return Ok(new
            {
                JobId = jobId,
                ViewCount = viewCount,
                ApplyCount = applyCount,
                ApplyPercentage = applyPercentage,
                Title = job.Title,
                Period = new
                {
                    From = effectiveFromDate.ToString("yyyy-MM-dd"),
                    To = effectiveToDate.ToString("yyyy-MM-dd")
                },
                DailyViewStats = dailyViewStats,
                DailyApplyStats = dailyApplyStats
            });
        }

        [HttpGet("company/{companyId}")]
        [Authorize(Roles = "Company,Admin")]
        public async Task<IActionResult> GetCompanyStatistics(int companyId, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid user ID.");

            var role = User.FindFirst(ClaimTypes.Role)?.Value.ToLower();

            // Chỉ company chủ sở hữu hoặc admin mới có thể xem
            if (role == "company" && companyId != userId)
                return Forbid("Bạn không có quyền xem thống kê của công ty này");

            // Set default date range if not provided
            var effectiveFromDate = fromDate ?? DateTime.UtcNow.AddDays(-7);
            var effectiveToDate = toDate ?? DateTime.UtcNow;

            // Ensure toDate is at the end of the day
            effectiveToDate = effectiveToDate.Date.AddDays(1).AddSeconds(-1);

            // Lấy danh sách các job của công ty
            var jobs = await _context.Jobs
                .Where(j => j.CompanyId == companyId)
                .Select(j => new { j.JobId, j.Title })
                .ToListAsync();

            var result = new List<object>();

            // Tính toán số lượt xem và apply cho mỗi job trong khoảng thời gian
            foreach (var job in jobs)
            {
                // Đếm số lượt xem trong khoảng thời gian
                int viewCount = await _context.JobViews
                    .CountAsync(v => v.JobId == job.JobId && v.ViewedAt >= effectiveFromDate && v.ViewedAt <= effectiveToDate);

                // Đếm số lượt apply trong khoảng thời gian
                int applyCount = await _context.Applications
                    .CountAsync(a => a.JobId == job.JobId && a.SubmittedAt >= effectiveFromDate && a.SubmittedAt <= effectiveToDate);

                // Tính phần trăm apply
                _applyPercentageCalculator.CalculateApplyPercentage(
                    viewCount,
                    applyCount,
                    2,
                    out double applyPercentage,
                    out _
                );

                result.Add(new
                {
                    job.JobId,
                    job.Title,
                    ViewCount = viewCount,
                    ApplyCount = applyCount,
                    ApplyPercentage = applyPercentage
                });
            }

            // Tính tổng thống kê cho công ty trong khoảng thời gian
            int totalViews = await _context.JobViews
                .Where(v => jobs.Select(j => j.JobId).Contains(v.JobId) && v.ViewedAt >= effectiveFromDate && v.ViewedAt <= effectiveToDate)
                .CountAsync();

            int totalApplies = await _context.Applications
                .Where(a => jobs.Select(j => j.JobId).Contains(a.JobId) && a.SubmittedAt >= effectiveFromDate && a.SubmittedAt <= effectiveToDate)
                .CountAsync();

            _applyPercentageCalculator.CalculateApplyPercentage(
                totalViews,
                totalApplies,
                2,
                out double totalApplyPercentage,
                out _
            );

            // Biểu đồ thống kê theo ngày
            var dailyViewStats = await _context.JobViews
                .Where(v => jobs.Select(j => j.JobId).Contains(v.JobId) && v.ViewedAt >= effectiveFromDate && v.ViewedAt <= effectiveToDate)
                .ToListAsync();

            var companyDailyViewStats = dailyViewStats
                .GroupBy(v => v.ViewedAt.Date)
                .Select(g => new
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    ViewCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();

            var dailyApplyStats = await _context.Applications
                .Where(a => jobs.Select(j => j.JobId).Contains(a.JobId) && a.SubmittedAt >= effectiveFromDate && a.SubmittedAt <= effectiveToDate)
                .ToListAsync();

            var companyDailyApplyStats = dailyApplyStats
                .GroupBy(a => a.SubmittedAt.Date)
                .Select(g => new
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    ApplyCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();

            return Ok(new
            {
                CompanyId = companyId,
                TotalViews = totalViews,
                TotalApplies = totalApplies,
                TotalApplyPercentage = totalApplyPercentage,
                Period = new
                {
                    From = effectiveFromDate.ToString("yyyy-MM-dd"),
                    To = effectiveToDate.ToString("yyyy-MM-dd")
                },
                Jobs = result,
                DailyViewStats = companyDailyViewStats,
                DailyApplyStats = companyDailyApplyStats
            });
        }

        [HttpGet("summary")]
        [Authorize(Roles = "Company,Admin")]
        public async Task<IActionResult> GetSystemSummary([FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
        {
            // Set default date range if not provided
            var effectiveFromDate = fromDate ?? DateTime.UtcNow.AddDays(-7);
            var effectiveToDate = toDate ?? DateTime.UtcNow;

            // Ensure toDate is at the end of the day
            effectiveToDate = effectiveToDate.Date.AddDays(1).AddSeconds(-1);

            // Tổng số user trong hệ thống
            var totalUsers = await _context.Users.CountAsync();

            // Tổng số job trong hệ thống
            var totalJobs = await _context.Jobs.CountAsync();

            // Tổng số lượt xem trong khoảng thời gian
            var totalViews = await _context.JobViews
                .CountAsync(v => v.ViewedAt >= effectiveFromDate && v.ViewedAt <= effectiveToDate);

            // Tổng số lượt apply trong khoảng thời gian
            var totalApplies = await _context.Applications
                .CountAsync(a => a.SubmittedAt >= effectiveFromDate && a.SubmittedAt <= effectiveToDate);

            _applyPercentageCalculator.CalculateApplyPercentage(
                totalViews,
                totalApplies,
                2,
                out double systemApplyPercentage,
                out _
            );

            // Get raw status data first
            var jobs = await _context.Jobs.ToListAsync();

            // Then do the grouping on the client side
            var jobsByStatus = jobs
                .GroupBy(j => j.Status)
                .Select(g => new
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToList();

            // Thống kê theo ngày
            var dailyViewStats = await _context.JobViews
                .Where(v => v.ViewedAt >= effectiveFromDate && v.ViewedAt <= effectiveToDate)
                .ToListAsync();

            var systemDailyViewStats = dailyViewStats
                .GroupBy(v => v.ViewedAt.Date)
                .Select(g => new
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    ViewCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();

            var dailyApplyStats = await _context.Applications
                .Where(a => a.SubmittedAt >= effectiveFromDate && a.SubmittedAt <= effectiveToDate)
                .ToListAsync();

            var systemDailyApplyStats = dailyApplyStats
                .GroupBy(a => a.SubmittedAt.Date)
                .Select(g => new
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    ApplyCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();

            return Ok(new
            {
                TotalUsers = totalUsers,
                TotalJobs = totalJobs,
                TotalViews = totalViews,
                TotalApplies = totalApplies,
                SystemApplyPercentage = systemApplyPercentage,
                Period = new
                {
                    From = effectiveFromDate.ToString("yyyy-MM-dd"),
                    To = effectiveToDate.ToString("yyyy-MM-dd")
                },
                JobsByStatus = jobsByStatus,
                DailyViewStats = systemDailyViewStats,
                DailyApplyStats = systemDailyApplyStats
            });
        }
    }
}