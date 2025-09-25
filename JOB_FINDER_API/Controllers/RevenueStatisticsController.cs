using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using UglyToad.PdfPig.Graphics.Operations.PathPainting;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Only admin can access these endpoints
    public class RevenueStatisticsController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        private readonly ILogger<RevenueStatisticsController> _logger;

        public RevenueStatisticsController(
            JobFinderDbContext context,
            ILogger<RevenueStatisticsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetRevenueSummary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                // Set default date range if not provided (last 30 days)
                var end = endDate ?? DateTime.UtcNow;
                var start = startDate ?? end.AddDays(-30);

                // Query completed payments within the date range
                var payments = await _context.Payments
                    .Where(p => p.Status == PaymentStatus.Completed &&
                                p.CreatedAt >= start &&
                                p.CreatedAt <= end)
                    .ToListAsync();

                // Calculate total revenue
                var totalRevenue = payments.Sum(p => p.Amount);

                // Calculate candidate revenue
                var candidateRevenue = payments
                    .Where(p => string.IsNullOrEmpty(p.PaymentType) || p.PaymentType != "CompanySubscription")
                    .Sum(p => p.Amount);

                // Calculate company revenue
                var companyRevenue = payments
                    .Where(p => p.PaymentType == "CompanySubscription")
                    .Sum(p => p.Amount);

                // Get payment count
                var totalPayments = payments.Count;
                var candidatePayments = payments.Count(p => string.IsNullOrEmpty(p.PaymentType) || p.PaymentType != "CompanySubscription");
                var companyPayments = payments.Count(p => p.PaymentType == "CompanySubscription");

                return Ok(new
                {
                    StartDate = start,
                    EndDate = end,
                    TotalRevenue = totalRevenue,
                    CandidateRevenue = candidateRevenue,
                    CompanyRevenue = companyRevenue,
                    TotalPayments = totalPayments,
                    CandidatePayments = candidatePayments,
                    CompanyPayments = companyPayments
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting revenue summary");
                return StatusCode(500, new { Error = "Error retrieving revenue statistics", Message = ex.Message });
            }
        }

        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyRevenue([FromQuery] int year = 0)
        {
            try
            {
                // If year not specified, use current year
                if (year <= 0)
                {
                    year = DateTime.UtcNow.Year;
                }

                // Get start and end date for the specified year
                var startDate = new DateTime(year, 1, 1);
                var endDate = new DateTime(year, 12, 31, 23, 59, 59);

                // Get all completed payments for the year
                var payments = await _context.Payments
                    .Where(p => p.Status == PaymentStatus.Completed &&
                                p.CreatedAt >= startDate &&
                                p.CreatedAt <= endDate)
                    .ToListAsync();

                // Group by month and calculate revenue
                var monthlyRevenue = Enumerable.Range(1, 12)
                    .Select(month => new
                    {
                        Month = month,
                        MonthName = new DateTime(year, month, 1).ToString("MMMM"),
                        TotalRevenue = payments
                            .Where(p => p.CreatedAt.Month == month)
                            .Sum(p => p.Amount),
                        CandidateRevenue = payments
                            .Where(p => p.CreatedAt.Month == month &&
                                  (string.IsNullOrEmpty(p.PaymentType) || p.PaymentType != "CompanySubscription"))
                            .Sum(p => p.Amount),
                        CompanyRevenue = payments
                            .Where(p => p.CreatedAt.Month == month && p.PaymentType == "CompanySubscription")
                            .Sum(p => p.Amount),
                        PaymentCount = payments.Count(p => p.CreatedAt.Month == month)
                    })
                    .ToList();

                return Ok(new
                {
                    Year = year,
                    MonthlyRevenue = monthlyRevenue,
                    TotalYearlyRevenue = payments.Sum(p => p.Amount),
                    TotalCandidateRevenue = payments
                        .Where(p => string.IsNullOrEmpty(p.PaymentType) || p.PaymentType != "CompanySubscription")
                        .Sum(p => p.Amount),
                    TotalCompanyRevenue = payments
                        .Where(p => p.PaymentType == "CompanySubscription")
                        .Sum(p => p.Amount),
                    TotalPaymentCount = payments.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monthly revenue for year {Year}", year);
                return StatusCode(500, new { Error = "Error retrieving monthly revenue", Message = ex.Message });
            }
        }
        [HttpGet("by-package-type")]
        public async Task<IActionResult> GetRevenueByPackageType([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                // Set default date range if not provided (last 30 days)
                var end = endDate ?? DateTime.UtcNow;
                var start = startDate ?? end.AddDays(-30);

                // Get all candidate subscription types
                var candidatePackages = await _context.SubscriptionTypes.ToListAsync();

                // Get all company subscription types
                var companyPackages = await _context.CompanySubscriptionTypes.ToListAsync();

                // Get completed payments within the date range with user information
                var paymentsWithUsers = await _context.Payments
                    .Where(p => p.Status == PaymentStatus.Completed &&
                                p.CreatedAt >= start &&
                                p.CreatedAt <= end)
                    .Join(_context.Users.Include(u => u.CompanyProfile),
                          payment => payment.UserId,
                          user => user.UserId,
                          (payment, user) => new
                          {
                              Payment = payment,
                              User = user
                          })
                    .ToListAsync();

                // Group candidate payments by subscription type with transaction details
                var candidateTransactionsByPackage = candidatePackages
                    .Select(package => new
                    {
                        PackageId = package.SubscriptionTypeId,
                        PackageName = package.Name,
                        PackageType = package.PackageType.ToString(),
                        Revenue = paymentsWithUsers
                            .Where(p => p.Payment.SubscriptionTypeId == package.SubscriptionTypeId &&
                                    (string.IsNullOrEmpty(p.Payment.PaymentType) || p.Payment.PaymentType != "CompanySubscription"))
                            .Sum(p => p.Payment.Amount),
                        PaymentCount = paymentsWithUsers
                            .Count(p => p.Payment.SubscriptionTypeId == package.SubscriptionTypeId &&
                                   (string.IsNullOrEmpty(p.Payment.PaymentType) || p.Payment.PaymentType != "CompanySubscription")),
                        Transactions = paymentsWithUsers
                            .Where(p => p.Payment.SubscriptionTypeId == package.SubscriptionTypeId &&
                                    (string.IsNullOrEmpty(p.Payment.PaymentType) || p.Payment.PaymentType != "CompanySubscription"))
                            .OrderByDescending(p => p.Payment.UpdatedAt)
                            .Select(p => new
                            {
                                TransactionId = p.Payment.PaymentId,
                                OrderCode = p.Payment.TransactionCode,
                                Amount = p.Payment.Amount,
                                UserId = p.User.UserId,
                                UserEmail = p.User.Email,
                                UserName = p.User.FullName, // Use FullName for candidates
                                PaymentType = "CandidateSubscription",
                                PackageName = package.Name,
                                Date = p.Payment.UpdatedAt
                            })
                            .ToList()
                    })
                    .Where(x => x.PaymentCount > 0)
                    .ToList();

                // Group company payments by subscription type with transaction details
                var companyTransactionsByPackage = companyPackages
                    .Select(package => new
                    {
                        PackageId = package.CompanySubscriptionTypeId,
                        PackageName = package.Name,
                        PackageType = package.PackageType.ToString(),
                        Revenue = paymentsWithUsers
                            .Where(p => p.Payment.SubscriptionTypeId == package.CompanySubscriptionTypeId &&
                                    p.Payment.PaymentType == "CompanySubscription")
                            .Sum(p => p.Payment.Amount),
                        PaymentCount = paymentsWithUsers
                            .Count(p => p.Payment.SubscriptionTypeId == package.CompanySubscriptionTypeId &&
                                   p.Payment.PaymentType == "CompanySubscription"),
                        Transactions = paymentsWithUsers
                            .Where(p => p.Payment.SubscriptionTypeId == package.CompanySubscriptionTypeId &&
                                    p.Payment.PaymentType == "CompanySubscription")
                            .OrderByDescending(p => p.Payment.UpdatedAt)
                            .Select(p => new
                            {
                                TransactionId = p.Payment.PaymentId,
                                OrderCode = p.Payment.TransactionCode,
                                Amount = p.Payment.Amount,
                                UserId = p.User.UserId,
                                UserEmail = p.User.Email,
                                UserName = p.User.CompanyProfile != null ? p.User.CompanyProfile.CompanyName : p.User.FullName, // Use CompanyName for companies
                                PaymentType = "CompanySubscription",
                                PackageName = package.Name,
                                Date = p.Payment.UpdatedAt
                            })
                            .ToList()
                    })
                    .Where(x => x.PaymentCount > 0)
                    .ToList();

                // Get all transactions for summary
                var allCandidateTransactions = candidateTransactionsByPackage
                    .SelectMany(p => p.Transactions)
                    .OrderByDescending(t => t.Date)
                    .ToList();

                var allCompanyTransactions = companyTransactionsByPackage
                    .SelectMany(p => p.Transactions)
                    .OrderByDescending(t => t.Date)
                    .ToList();

                return Ok(new
                {
                    StartDate = start,
                    EndDate = end,
                    CandidateRevenue = new
                    {
                        Packages = candidateTransactionsByPackage,
                        TotalRevenue = candidateTransactionsByPackage.Sum(x => x.Revenue),
                        TotalTransactions = candidateTransactionsByPackage.Sum(x => x.PaymentCount),
                        AllTransactions = allCandidateTransactions
                    },
                    CompanyRevenue = new
                    {
                        Packages = companyTransactionsByPackage,
                        TotalRevenue = companyTransactionsByPackage.Sum(x => x.Revenue),
                        TotalTransactions = companyTransactionsByPackage.Sum(x => x.PaymentCount),
                        AllTransactions = allCompanyTransactions
                    },
                    TotalRevenue = candidateTransactionsByPackage.Sum(x => x.Revenue) + companyTransactionsByPackage.Sum(x => x.Revenue),
                    TotalTransactions = candidateTransactionsByPackage.Sum(x => x.PaymentCount) + companyTransactionsByPackage.Sum(x => x.PaymentCount)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting revenue by package type");
                return StatusCode(500, new { Error = "Error retrieving revenue by package type", Message = ex.Message });
            }
        }

        [HttpGet("recent-transactions")]
        public async Task<IActionResult> GetRecentTransactions(
     [FromQuery] int count = 10,
     [FromQuery] DateTime? startDate = null,
     [FromQuery] DateTime? endDate = null)
        {
            try
            {
                // Limit the count to a reasonable number
                count = Math.Min(count, 100);

                // Build query with base conditions
                var query = _context.Payments
                    .Where(p => p.Status == PaymentStatus.Completed);

                // Add date range filters if provided
                if (startDate.HasValue)
                {
                    query = query.Where(p => p.UpdatedAt.Date >= startDate.Value.Date);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(p => p.UpdatedAt.Date <= endDate.Value.Date);
                }

                // Get recent completed payments
                var recentPayments = await query
                    .OrderByDescending(p => p.UpdatedAt)
                    .Take(count)
                    .Join(_context.Users.Include(u => u.CompanyProfile),
                          payment => payment.UserId,
                          user => user.UserId,
                          (payment, user) => new
                          {
                              TransactionId = payment.PaymentId,
                              OrderCode = payment.TransactionCode,
                              Amount = payment.Amount,
                              UserId = payment.UserId,
                              UserEmail = user.Email,
                              UserName = string.IsNullOrEmpty(payment.PaymentType) || payment.PaymentType != "CompanySubscription"
                                  ? user.FullName
                                  : (user.CompanyProfile != null ? user.CompanyProfile.CompanyName : user.FullName),
                              PaymentType = string.IsNullOrEmpty(payment.PaymentType)
                                  ? "CandidateSubscription"
                                  : payment.PaymentType,
                              Date = payment.UpdatedAt
                          })
                    .ToListAsync();

                return Ok(new
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Count = recentPayments.Count,
                    MaxRequested = count,
                    Transactions = recentPayments
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent transactions");
                return StatusCode(500, new { Error = "Error retrieving recent transactions", Message = ex.Message });
            }
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStatistics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                // Get current date and calculate date ranges
                var today = DateTime.UtcNow.Date;
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                var startOfMonth = new DateTime(today.Year, today.Month, 1);
                var startOfYear = new DateTime(today.Year, 1, 1);

                // Use provided date range if specified, otherwise use default ranges
                var end = endDate ?? today;
                var start = startDate ?? startOfYear; // Default to beginning of year if not specified

                // Validate date range
                if (end < start)
                {
                    return BadRequest(new
                    {
                        Error = "Invalid date range",
                        Message = "End date must be greater than or equal to start date",
                        ProvidedStartDate = start,
                        ProvidedEndDate = end
                    });
                }

                // Get completed payments
                var allPayments = await _context.Payments
                    .Where(p => p.Status == PaymentStatus.Completed)
                    .ToListAsync();

                // Filter payments by the selected date range
                var rangePayments = allPayments
                    .Where(p => p.CreatedAt.Date >= start.Date && p.CreatedAt.Date <= end.Date)
                    .ToList();

                // Calculate revenue for different time periods - these are independent of the selected date range
                var todayRevenue = allPayments
                    .Where(p => p.CreatedAt.Date == today)
                    .Sum(p => p.Amount);

                var weekRevenue = allPayments
                    .Where(p => p.CreatedAt.Date >= startOfWeek && p.CreatedAt.Date <= today)
                    .Sum(p => p.Amount);

                var monthRevenue = allPayments
                    .Where(p => p.CreatedAt.Date >= startOfMonth && p.CreatedAt.Date <= today)
                    .Sum(p => p.Amount);

                var yearRevenue = allPayments
                    .Where(p => p.CreatedAt.Date >= startOfYear && p.CreatedAt.Date <= today)
                    .Sum(p => p.Amount);

                var totalRevenue = allPayments.Sum(p => p.Amount);

                // Calculate revenue for the selected date range
                var rangeRevenue = rangePayments.Sum(p => p.Amount);

                // Calculate revenue by user type for the selected range
                var rangeCandidateRevenue = rangePayments
                    .Where(p => string.IsNullOrEmpty(p.PaymentType) || p.PaymentType != "CompanySubscription")
                    .Sum(p => p.Amount);

                var rangeCompanyRevenue = rangePayments
                    .Where(p => p.PaymentType == "CompanySubscription")
                    .Sum(p => p.Amount);

                // Get transaction counts
                var todayTransactions = allPayments.Count(p => p.CreatedAt.Date == today);
                var totalTransactions = allPayments.Count;
                var rangeTransactions = rangePayments.Count;

                // Get active subscriptions count
                var activeUserSubscriptions = await _context.CandidateSubscriptions
                    .Where(s => s.IsActive && s.EndDate > DateTime.UtcNow)
                    .CountAsync();

                var activeCompanySubscriptions = await _context.CompanySubscriptions
                    .Where(s => s.IsActive && s.EndDate > DateTime.UtcNow)
                    .CountAsync();

                // Calculate revenue percentages for the range
                decimal candidatePercentage = rangeRevenue > 0
                    ? Math.Round(rangeCandidateRevenue * 100 / rangeRevenue, 1)
                    : 0;

                decimal companyPercentage = rangeRevenue > 0
                    ? Math.Round(rangeCompanyRevenue * 100 / rangeRevenue, 1)
                    : 0;

                return Ok(new
                {
                    // Date range info
                    StartDate = start,
                    EndDate = end,
                    IsCustomDateRange = startDate.HasValue || endDate.HasValue,

                    // Selected range data
                    RangeRevenue = rangeRevenue,
                    RangeTransactions = rangeTransactions,
                    RangeCandidateRevenue = rangeCandidateRevenue,
                    RangeCompanyRevenue = rangeCompanyRevenue,

                    // Standard time period revenues (always show these)
                    TodayRevenue = todayRevenue,
                    WeekRevenue = weekRevenue,
                    MonthRevenue = monthRevenue,
                    YearRevenue = yearRevenue,
                    TotalRevenue = totalRevenue,

                    // Transaction counts
                    TodayTransactions = todayTransactions,
                    TotalTransactions = totalTransactions,

                    // Subscription counts
                    ActiveUserSubscriptions = activeUserSubscriptions,
                    ActiveCompanySubscriptions = activeCompanySubscriptions,

                    // Revenue breakdown within the selected range
                    RevenueBreakdown = new
                    {
                        CandidatePercentage = candidatePercentage,
                        CompanyPercentage = companyPercentage
                    },

                    // Flag to indicate if data exists in the selected range
                    HasDataInRange = rangePayments.Any()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard statistics");
                return StatusCode(500, new { Error = "Error retrieving dashboard statistics", Message = ex.Message });
            }
        }


        [HttpGet("export")]
        public async Task<IActionResult> ExportRevenueData([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                // Set default date range if not provided (last 30 days)
                var end = endDate ?? DateTime.UtcNow;
                var start = startDate ?? end.AddDays(-30);

                // Get candidate and company subscription types
                var candidatePackages = await _context.SubscriptionTypes.ToListAsync();
                var companyPackages = await _context.CompanySubscriptionTypes.ToListAsync();

                // Get completed payments within the date range with detailed information
                var paymentsWithDetails = await _context.Payments
                    .Where(p => p.Status == PaymentStatus.Completed &&
                                p.CreatedAt >= start &&
                                p.CreatedAt <= end)
                    .Join(_context.Users.Include(u => u.CompanyProfile),
                        payment => payment.UserId,
                        user => user.UserId,
                        (payment, user) => new
                        {
                            Payment = payment,
                            User = user
                        })
                    .ToListAsync();

                // Extract payments for easier usage
                var completedPayments = paymentsWithDetails.Select(p => p.Payment).ToList();

                // Separate payments by type for upgrade analysis
                var candidatePayments = completedPayments
                    .Where(p => string.IsNullOrEmpty(p.PaymentType) || p.PaymentType != "CompanySubscription")
                    .ToList();

                var companyPayments = completedPayments
                    .Where(p => p.PaymentType == "CompanySubscription")
                    .ToList();

                // Get all users who made payments
                var userIds = completedPayments.Select(p => p.UserId).Distinct().ToList();

                // Get all payments for these users (even outside the date range) to determine upgrade patterns
                var allUserPayments = await _context.Payments
                    .Where(p => p.Status == PaymentStatus.Completed && userIds.Contains(p.UserId))
                    .OrderBy(p => p.UserId)
                    .ThenBy(p => p.CreatedAt)
                    .ToListAsync();

                // Group by users to analyze package upgrade patterns
                var userPaymentsGroups = allUserPayments.GroupBy(p => p.UserId).ToList();

                // Calculate upgrade statistics for candidates
                var candidateUpgrades = CalculateCandidatePackageUpgrades(
                    userPaymentsGroups,
                    candidatePayments,
                    candidatePackages,
                    start,
                    end);

                // Calculate upgrade statistics for companies
                var companyUpgrades = CalculateCompanyPackageUpgrades(
                    userPaymentsGroups,
                    companyPayments,
                    companyPackages,
                    start,
                    end);

                // Calculate return rate statistics
                var candidateReturnRate = CalculateReturnRate(candidatePayments);
                var companyReturnRate = CalculateReturnRate(companyPayments);

                // Process user upgrade patterns - FIX: Sử dụng paymentsWithDetails thay vì paymentsWithUsers
                var userUpgradePatterns = GetUserUpgradePatterns(
                    userPaymentsGroups,
                    candidatePackages,
                    companyPackages,
                    paymentsWithDetails.Cast<object>().ToList(), // Sử dụng paymentsWithDetails đã được định nghĩa
                    start,
                    end);

                // Process payments with package information
                var detailedTransactions = paymentsWithDetails.Select(p =>
                {
                    // Determine package information
                    string packageName = "";
                    string packageType = "";
                    decimal packagePrice = 0;
                    int packageDuration = 0;

                    bool isCompanySubscription = p.Payment.PaymentType == "CompanySubscription";
                    if (isCompanySubscription)
                    {
                        var package = companyPackages.FirstOrDefault(cp => cp.CompanySubscriptionTypeId == p.Payment.SubscriptionTypeId);
                        if (package != null)
                        {
                            packageName = package.Name;
                            packageType = package.PackageType.ToString();
                            packagePrice = package.Price;
                            packageDuration = package.DurationInDays;
                        }
                    }
                    else
                    {
                        var package = candidatePackages.FirstOrDefault(cp => cp.SubscriptionTypeId == p.Payment.SubscriptionTypeId);
                        if (package != null)
                        {
                            packageName = package.Name;
                            packageType = package.PackageType.ToString();
                            packagePrice = package.Price;
                            packageDuration = package.DurationInDays;
                        }
                    }

                    // Return detailed transaction information
                    return new
                    {
                        TransactionId = p.Payment.PaymentId,
                        OrderCode = p.Payment.TransactionCode,
                        Amount = p.Payment.Amount,
                        PaymentProvider = p.Payment.PaymentProvider,
                        UserId = p.User.UserId,
                        UserEmail = p.User.Email,
                        UserName = isCompanySubscription && p.User.CompanyProfile != null
                            ? p.User.CompanyProfile.CompanyName
                            : p.User.FullName,
                        AccountType = isCompanySubscription ? "Company" : "Candidate",
                        PaymentType = isCompanySubscription ? "CompanySubscription" : "CandidateSubscription",
                        SubscriptionTypeId = p.Payment.SubscriptionTypeId,
                        PackageName = packageName,
                        PackageType = packageType,
                        PackagePrice = packagePrice,
                        PackageDuration = packageDuration,
                        CreatedAt = p.Payment.CreatedAt,
                        CompletedAt = p.Payment.UpdatedAt,
                        PaymentResponse = p.Payment.PaymentResponse
                    };
                }).OrderByDescending(p => p.CompletedAt).ToList();

                // Calculate payment statistics by package
                var candidatePackageStats = candidatePackages
                    .Select(package => new
                    {
                        PackageId = package.SubscriptionTypeId,
                        PackageName = package.Name,
                        PackageType = package.PackageType.ToString(),
                        Price = package.Price,
                        DurationDays = package.DurationInDays,
                        Revenue = detailedTransactions
                            .Where(t => t.PaymentType == "CandidateSubscription" && t.SubscriptionTypeId == package.SubscriptionTypeId)
                            .Sum(t => t.Amount),
                        PaymentCount = detailedTransactions
                            .Count(t => t.PaymentType == "CandidateSubscription" && t.SubscriptionTypeId == package.SubscriptionTypeId)
                    })
                    .Where(x => x.PaymentCount > 0)
                    .ToList();

                var companyPackageStats = companyPackages
                    .Select(package => new
                    {
                        PackageId = package.CompanySubscriptionTypeId,
                        PackageName = package.Name,
                        PackageType = package.PackageType.ToString(),
                        Price = package.Price,
                        DurationDays = package.DurationInDays,
                        JobPostLimit = package.JobPostLimit,
                        CvMatchLimit = package.CvMatchLimit,
                        Revenue = detailedTransactions
                            .Where(t => t.PaymentType == "CompanySubscription" && t.SubscriptionTypeId == package.CompanySubscriptionTypeId)
                            .Sum(t => t.Amount),
                        PaymentCount = detailedTransactions
                            .Count(t => t.PaymentType == "CompanySubscription" && t.SubscriptionTypeId == package.CompanySubscriptionTypeId)
                    })
                    .Where(x => x.PaymentCount > 0)
                    .ToList();

                // Get monthly revenue breakdown for the period
                var months = new List<DateTime>();
                for (var date = new DateTime(start.Year, start.Month, 1); date <= end; date = date.AddMonths(1))
                {
                    months.Add(date);
                }

                var monthlyRevenue = months.Select(month => new
                {
                    Month = month.ToString("yyyy-MM"),
                    MonthName = month.ToString("MMMM yyyy"),
                    TotalRevenue = detailedTransactions
                        .Where(t => t.CompletedAt.Year == month.Year && t.CompletedAt.Month == month.Month)
                        .Sum(t => t.Amount),
                    CandidateRevenue = detailedTransactions
                        .Where(t => t.PaymentType == "CandidateSubscription" &&
                              t.CompletedAt.Year == month.Year && t.CompletedAt.Month == month.Month)
                        .Sum(t => t.Amount),
                    CompanyRevenue = detailedTransactions
                        .Where(t => t.PaymentType == "CompanySubscription" &&
                              t.CompletedAt.Year == month.Year && t.CompletedAt.Month == month.Month)
                        .Sum(t => t.Amount),
                    TransactionCount = detailedTransactions
                        .Count(t => t.CompletedAt.Year == month.Year && t.CompletedAt.Month == month.Month),
                    CandidateTransactions = detailedTransactions
                        .Count(t => t.PaymentType == "CandidateSubscription" &&
                              t.CompletedAt.Year == month.Year && t.CompletedAt.Month == month.Month),
                    CompanyTransactions = detailedTransactions
                        .Count(t => t.PaymentType == "CompanySubscription" &&
                              t.CompletedAt.Year == month.Year && t.CompletedAt.Month == month.Month)
                }).ToList();

                // Get daily revenue breakdown for the period
                var dailyRevenue = Enumerable.Range(0, (end - start).Days + 1)
                    .Select(offset => start.AddDays(offset))
                    .Select(day => new
                    {
                        Date = day.ToString("yyyy-MM-dd"),
                        TotalRevenue = detailedTransactions
                            .Where(t => t.CompletedAt.Date == day.Date)
                            .Sum(t => t.Amount),
                        CandidateRevenue = detailedTransactions
                            .Where(t => t.PaymentType == "CandidateSubscription" && t.CompletedAt.Date == day.Date)
                            .Sum(t => t.Amount),
                        CompanyRevenue = detailedTransactions
                            .Where(t => t.PaymentType == "CompanySubscription" && t.CompletedAt.Date == day.Date)
                            .Sum(t => t.Amount),
                        TransactionCount = detailedTransactions
                            .Count(t => t.CompletedAt.Date == day.Date)
                    })
                    .Where(d => d.TransactionCount > 0)
                    .ToList();

                // Get user statistics
                var userRevenue = detailedTransactions
                    .GroupBy(t => new { t.UserId, t.UserEmail, t.UserName, t.AccountType })
                    .Select(g => new
                    {
                        UserId = g.Key.UserId,
                        Email = g.Key.UserEmail,
                        Name = g.Key.UserName,
                        AccountType = g.Key.AccountType,
                        TotalSpent = g.Sum(t => t.Amount),
                        TransactionCount = g.Count(),
                        FirstPurchase = g.Min(t => t.CompletedAt),
                        LastPurchase = g.Max(t => t.CompletedAt),
                        Transactions = g.OrderByDescending(t => t.CompletedAt).Select(t => t.TransactionId).ToList()
                    })
                    .OrderByDescending(u => u.TotalSpent)
                    .ToList();

                // Calculate upgrade metrics
                // Calculate upgrade metrics
                var renewalPatterns = userUpgradePatterns
                    .Where(p => (string)p.GetType().GetProperty("UpgradePattern").GetValue(p) == "BasicToBasic" ||
                                (string)p.GetType().GetProperty("UpgradePattern").GetValue(p) == "PremiumToPremium")
                    .ToList();

                var upgradePatterns = userUpgradePatterns
                    .Where(p => (string)p.GetType().GetProperty("UpgradePattern").GetValue(p) == "BasicToPremium")
                    .ToList();

                double? averageTimeBetweenRenewals = renewalPatterns.Any()
                    ? renewalPatterns.Average(p => (double)p.GetType().GetProperty("DaysBetweenSubscriptions").GetValue(p))
                    : null;

                double? averageTimeBetweenUpgrades = upgradePatterns.Any()
                    ? upgradePatterns.Average(p => (double)p.GetType().GetProperty("DaysBetweenSubscriptions").GetValue(p))
                    : null;
                // Include overall summary data
                var summary = new
                {
                    StartDate = start,
                    EndDate = end,
                    TotalTransactions = detailedTransactions.Count,
                    TotalRevenue = detailedTransactions.Sum(p => p.Amount),
                    CandidateRevenue = detailedTransactions.Where(p => p.PaymentType == "CandidateSubscription").Sum(p => p.Amount),
                    CompanyRevenue = detailedTransactions.Where(p => p.PaymentType == "CompanySubscription").Sum(p => p.Amount),
                    CandidateTransactions = detailedTransactions.Count(p => p.PaymentType == "CandidateSubscription"),
                    CompanyTransactions = detailedTransactions.Count(p => p.PaymentType == "CompanySubscription"),
                    UniqueUsers = userRevenue.Count,
                    UniqueCandidates = userRevenue.Count(u => u.AccountType == "Candidate"),
                    UniqueCompanies = userRevenue.Count(u => u.AccountType == "Company"),
                    AverageTransactionValue = detailedTransactions.Count > 0 ? detailedTransactions.Average(p => p.Amount) : 0,
                    MedianTransactionValue = detailedTransactions.Count > 0 ? GetMedian(detailedTransactions.Select(p => p.Amount)) : 0,
                    TopPaymentProvider = detailedTransactions
                        .GroupBy(t => t.PaymentProvider)
                        .OrderByDescending(g => g.Count())
                        .Select(g => new { Provider = g.Key, Count = g.Count() })
                        .FirstOrDefault()
                };

                // Create the combined export result
                return Ok(new
                {
                    // Revenue data
                    Summary = summary,
                    PackageStatistics = new
                    {
                        CandidatePackages = candidatePackageStats,
                        CompanyPackages = companyPackageStats
                    },
                    MonthlyRevenue = monthlyRevenue,
                    DailyRevenue = dailyRevenue,
                    UserRevenue = userRevenue,
                    Transactions = detailedTransactions,

                    // Package upgrade data
                    UpgradeStatistics = new
                    {
                        CandidateStatistics = new
                        {
                            TotalPackagePayments = candidatePayments.Count,
                            BasicPackagePayments = candidateUpgrades.BasicCount,
                            PremiumPackagePayments = candidateUpgrades.PremiumCount,
                            BasicToPremiumUpgrades = candidateUpgrades.BasicToPremiumCount,
                            BasicToBasicRenewals = candidateUpgrades.BasicToBasicCount,
                            PremiumToPremiumRenewals = candidateUpgrades.PremiumToPremiumCount,
                            UpgradeRate = candidateUpgrades.UpgradeRate,
                            RenewalRate = candidateUpgrades.RenewalRate,
                            ReturnRate = candidateReturnRate.ReturnRate,
                            NewUsers = candidateReturnRate.NewUsers,
                            ReturningUsers = candidateReturnRate.ReturningUsers,
                            PackageDistribution = candidateUpgrades.PackageDistribution
                        },
                        CompanyStatistics = new
                        {
                            TotalPackagePayments = companyPayments.Count,
                            BasicPackagePayments = companyUpgrades.BasicCount,
                            PremiumPackagePayments = companyUpgrades.PremiumCount,
                            BasicToPremiumUpgrades = companyUpgrades.BasicToPremiumCount,
                            BasicToBasicRenewals = companyUpgrades.BasicToBasicCount,
                            PremiumToPremiumRenewals = companyUpgrades.PremiumToPremiumCount,
                            UpgradeRate = companyUpgrades.UpgradeRate,
                            RenewalRate = companyUpgrades.RenewalRate,
                            ReturnRate = companyReturnRate.ReturnRate,
                            NewUsers = companyReturnRate.NewUsers,
                            ReturningUsers = companyReturnRate.ReturningUsers,
                            PackageDistribution = companyUpgrades.PackageDistribution
                        },
                        CombinedStatistics = new
                        {
                            TotalPackagePayments = candidatePayments.Count + companyPayments.Count,
                            TotalBasicPackagePayments = candidateUpgrades.BasicCount + companyUpgrades.BasicCount,
                            TotalPremiumPackagePayments = candidateUpgrades.PremiumCount + companyUpgrades.PremiumCount,
                            TotalUpgrades = candidateUpgrades.BasicToPremiumCount + companyUpgrades.BasicToPremiumCount,
                            TotalSamePackageRenewals = candidateUpgrades.BasicToBasicCount + candidateUpgrades.PremiumToPremiumCount +
                                                     companyUpgrades.BasicToBasicCount + companyUpgrades.PremiumToPremiumCount,
                            OverallUpgradeRate = CalculateOverallRate(
                                candidateUpgrades.BasicToPremiumCount + companyUpgrades.BasicToPremiumCount,
                                candidateUpgrades.BasicCount + companyUpgrades.BasicCount),
                            OverallRenewalRate = CalculateOverallRate(
                                candidateUpgrades.BasicToBasicCount + candidateUpgrades.PremiumToPremiumCount +
                                companyUpgrades.BasicToBasicCount + companyUpgrades.PremiumToPremiumCount,
                                candidateUpgrades.BasicCount + candidateUpgrades.PremiumCount +
                                companyUpgrades.BasicCount + companyUpgrades.PremiumCount),
                            OverallReturnRate = CalculateOverallRate(
                                candidateReturnRate.ReturningUsers + companyReturnRate.ReturningUsers,
                                candidateReturnRate.NewUsers + candidateReturnRate.ReturningUsers +
                                companyReturnRate.NewUsers + companyReturnRate.ReturningUsers)
                        },
                        UpgradeMetrics = new
                        {
                            AverageTimeBetweenRenewals = averageTimeBetweenRenewals,
                            AverageTimeBetweenUpgrades = averageTimeBetweenUpgrades
                        },
                        UserUpgradePatterns = userUpgradePatterns
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting revenue and package upgrade data");
                return StatusCode(500, new { Error = "Error exporting revenue and package upgrade data", Message = ex.Message });
            }
        }

        // Helper method to calculate user upgrade patterns
        // Helper method to calculate user upgrade patterns
        private List<object> GetUserUpgradePatterns(
            List<IGrouping<int, Payment>> userPaymentsGroups,
            List<SubscriptionType> candidatePackages,
            List<CompanySubscriptionType> companyPackages,
            List<object> paymentsWithUsers, // Thay đổi kiểu dữ liệu từ List<dynamic> sang List<object>
            DateTime start,
            DateTime end)
        {
            var userUpgradePatterns = new List<object>();

            // Identify basic and premium package IDs
            var candidateBasicPackageIds = candidatePackages
                .Where(p => p.PackageType == SubscriptionPackageType.Basic)
                .Select(p => p.SubscriptionTypeId)
                .ToList();

            var candidatePremiumPackageIds = candidatePackages
                .Where(p => p.PackageType == SubscriptionPackageType.Premium)
                .Select(p => p.SubscriptionTypeId)
                .ToList();

            var companyBasicPackageIds = companyPackages
                .Where(p => p.PackageType == CompanySubscriptionPackageType.Basic)
                .Select(p => p.CompanySubscriptionTypeId)
                .ToList();

            var companyPremiumPackageIds = companyPackages
                .Where(p => p.PackageType == CompanySubscriptionPackageType.Premium)
                .Select(p => p.CompanySubscriptionTypeId)
                .ToList();

            // Process each user's payment history
            foreach (var userGroup in userPaymentsGroups)
            {
                var userId = userGroup.Key;

                // Sửa cách truy cập dữ liệu từ danh sách anonymous type bằng dynamic casting
                dynamic userPaymentInfo = null;
                foreach (var paymentWithUser in paymentsWithUsers)
                {
                    dynamic item = paymentWithUser;
                    if (item.User.UserId == userId)
                    {
                        userPaymentInfo = item;
                        break;
                    }
                }

                if (userPaymentInfo == null) continue;

                dynamic user = userPaymentInfo.User;

                // Process candidate subscriptions
                var candidateSubscriptions = userGroup
                    .Where(p => string.IsNullOrEmpty(p.PaymentType) || p.PaymentType != "CompanySubscription")
                    .OrderBy(p => p.CreatedAt)
                    .ToList();

                if (candidateSubscriptions.Count >= 2)
                {
                    for (int i = 0; i < candidateSubscriptions.Count - 1; i++)
                    {
                        bool firstIsBasic = candidateBasicPackageIds.Contains(candidateSubscriptions[i].SubscriptionTypeId);
                        bool secondIsBasic = candidateBasicPackageIds.Contains(candidateSubscriptions[i + 1].SubscriptionTypeId);
                        bool firstIsPremium = candidatePremiumPackageIds.Contains(candidateSubscriptions[i].SubscriptionTypeId);
                        bool secondIsPremium = candidatePremiumPackageIds.Contains(candidateSubscriptions[i + 1].SubscriptionTypeId);

                        string upgradePattern = "Unknown";
                        if (firstIsBasic && secondIsPremium)
                            upgradePattern = "BasicToPremium";
                        else if (firstIsBasic && secondIsBasic)
                            upgradePattern = "BasicToBasic";
                        else if (firstIsPremium && secondIsPremium)
                            upgradePattern = "PremiumToPremium";
                        else if (firstIsPremium && secondIsBasic)
                            upgradePattern = "PremiumToBasic";

                        // Add to patterns if second payment is within range
                        if (candidateSubscriptions[i + 1].CreatedAt >= start && candidateSubscriptions[i + 1].CreatedAt <= end)
                        {
                            var firstPackage = candidatePackages.FirstOrDefault(p => p.SubscriptionTypeId == candidateSubscriptions[i].SubscriptionTypeId);
                            var secondPackage = candidatePackages.FirstOrDefault(p => p.SubscriptionTypeId == candidateSubscriptions[i + 1].SubscriptionTypeId);

                            userUpgradePatterns.Add(new
                            {
                                UserId = userId,
                                UserName = user.FullName,
                                Email = user.Email,
                                AccountType = "Candidate",
                                UpgradePattern = upgradePattern,
                                FirstSubscriptionDate = candidateSubscriptions[i].CreatedAt,
                                FirstPackageName = firstPackage?.Name ?? "Unknown",
                                FirstPackageType = firstPackage?.PackageType.ToString() ?? "Unknown",
                                SecondSubscriptionDate = candidateSubscriptions[i + 1].CreatedAt,
                                SecondPackageName = secondPackage?.Name ?? "Unknown",
                                SecondPackageType = secondPackage?.PackageType.ToString() ?? "Unknown",
                                DaysBetweenSubscriptions = (candidateSubscriptions[i + 1].CreatedAt - candidateSubscriptions[i].CreatedAt).TotalDays,
                                FirstTransactionId = candidateSubscriptions[i].PaymentId,
                                SecondTransactionId = candidateSubscriptions[i + 1].PaymentId
                            });

                            // Stop after finding the first pattern within range
                            break;
                        }
                    }
                }

                // Process company subscriptions
                var companySubscriptions = userGroup
                    .Where(p => p.PaymentType == "CompanySubscription")
                    .OrderBy(p => p.CreatedAt)
                    .ToList();

                if (companySubscriptions.Count >= 2)
                {
                    for (int i = 0; i < companySubscriptions.Count - 1; i++)
                    {
                        bool firstIsBasic = companyBasicPackageIds.Contains(companySubscriptions[i].SubscriptionTypeId);
                        bool secondIsBasic = companyBasicPackageIds.Contains(companySubscriptions[i + 1].SubscriptionTypeId);
                        bool firstIsPremium = companyPremiumPackageIds.Contains(companySubscriptions[i].SubscriptionTypeId);
                        bool secondIsPremium = companyPremiumPackageIds.Contains(companySubscriptions[i + 1].SubscriptionTypeId);

                        string upgradePattern = "Unknown";
                        if (firstIsBasic && secondIsPremium)
                            upgradePattern = "BasicToPremium";
                        else if (firstIsBasic && secondIsBasic)
                            upgradePattern = "BasicToBasic";
                        else if (firstIsPremium && secondIsPremium)
                            upgradePattern = "PremiumToPremium";
                        else if (firstIsPremium && secondIsBasic)
                            upgradePattern = "PremiumToBasic";

                        // Add to patterns if second payment is within range
                        if (companySubscriptions[i + 1].CreatedAt >= start && companySubscriptions[i + 1].CreatedAt <= end)
                        {
                            var firstPackage = companyPackages.FirstOrDefault(p => p.CompanySubscriptionTypeId == companySubscriptions[i].SubscriptionTypeId);
                            var secondPackage = companyPackages.FirstOrDefault(p => p.CompanySubscriptionTypeId == companySubscriptions[i + 1].SubscriptionTypeId);

                            string companyName = user.CompanyProfile != null ? user.CompanyProfile.CompanyName : user.FullName;

                            userUpgradePatterns.Add(new
                            {
                                UserId = userId,
                                UserName = companyName,
                                Email = user.Email,
                                AccountType = "Company",
                                UpgradePattern = upgradePattern,
                                FirstSubscriptionDate = companySubscriptions[i].CreatedAt,
                                FirstPackageName = firstPackage?.Name ?? "Unknown",
                                FirstPackageType = firstPackage?.PackageType.ToString() ?? "Unknown",
                                SecondSubscriptionDate = companySubscriptions[i + 1].CreatedAt,
                                SecondPackageName = secondPackage?.Name ?? "Unknown",
                                SecondPackageType = secondPackage?.PackageType.ToString() ?? "Unknown",
                                DaysBetweenSubscriptions = (companySubscriptions[i + 1].CreatedAt - companySubscriptions[i].CreatedAt).TotalDays,
                                FirstTransactionId = companySubscriptions[i].PaymentId,
                                SecondTransactionId = companySubscriptions[i + 1].PaymentId
                            });

                            // Stop after finding the first pattern within range
                            break;
                        }
                    }
                }
            }

            return userUpgradePatterns;
        }

        // Helper method to calculate median value
        private decimal GetMedian(IEnumerable<decimal> values)
        {
            var sortedValues = values.OrderBy(v => v).ToList();
            int count = sortedValues.Count;

            if (count == 0)
                return 0;

            if (count % 2 == 0)
                return (sortedValues[count / 2 - 1] + sortedValues[count / 2]) / 2;
            else
                return sortedValues[count / 2];
        }

        [HttpGet("package-upgrades")]
        public async Task<IActionResult> GetPackageUpgradeStatistics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                // Set default date range if not provided (last 30 days)
                var end = endDate ?? DateTime.UtcNow;
                var start = startDate ?? end.AddDays(-30);

                // Get all completed payments within the date range
                var completedPayments = await _context.Payments
                    .Where(p => p.Status == PaymentStatus.Completed &&
                                p.CreatedAt >= start &&
                                p.CreatedAt <= end)
                    .Include(p => p.User)
                    .ToListAsync();

                // Get candidate payments
                var candidatePayments = completedPayments
                    .Where(p => string.IsNullOrEmpty(p.PaymentType) || p.PaymentType != "CompanySubscription")
                    .ToList();

                // Get company payments
                var companyPayments = completedPayments
                    .Where(p => p.PaymentType == "CompanySubscription")
                    .ToList();

                // Get candidate subscription types (packages)
                var candidatePackages = await _context.SubscriptionTypes.ToListAsync();

                // Get company subscription types (packages)
                var companyPackages = await _context.CompanySubscriptionTypes.ToListAsync();

                // Get all users who made payments
                var userIds = completedPayments.Select(p => p.UserId).Distinct().ToList();

                // Get all payments for these users (even outside the date range) to determine upgrade patterns
                var allUserPayments = await _context.Payments
                    .Where(p => p.Status == PaymentStatus.Completed && userIds.Contains(p.UserId))
                    .OrderBy(p => p.UserId)
                    .ThenBy(p => p.CreatedAt)
                    .ToListAsync();

                // Group by users to analyze package upgrade patterns
                var userPaymentsGroups = allUserPayments.GroupBy(p => p.UserId).ToList();

                // Calculate upgrade statistics for candidates
                var candidateUpgrades = CalculateCandidatePackageUpgrades(
                    userPaymentsGroups,
                    candidatePayments,
                    candidatePackages,
                    start,
                    end);

                // Calculate upgrade statistics for companies
                var companyUpgrades = CalculateCompanyPackageUpgrades(
                    userPaymentsGroups,
                    companyPayments,
                    companyPackages,
                    start,
                    end);

                // Calculate return rate statistics
                var candidateReturnRate = CalculateReturnRate(candidatePayments);
                var companyReturnRate = CalculateReturnRate(companyPayments);

                return Ok(new
                {
                    StartDate = start,
                    EndDate = end,
                    CandidateStatistics = new
                    {
                        TotalPackagePayments = candidatePayments.Count,
                        BasicPackagePayments = candidateUpgrades.BasicCount,
                        PremiumPackagePayments = candidateUpgrades.PremiumCount,
                        BasicToPremiumUpgrades = candidateUpgrades.BasicToPremiumCount,
                        BasicToBasicRenewals = candidateUpgrades.BasicToBasicCount,
                        PremiumToPremiumRenewals = candidateUpgrades.PremiumToPremiumCount,
                        UpgradeRate = candidateUpgrades.UpgradeRate,
                        RenewalRate = candidateUpgrades.RenewalRate,
                        ReturnRate = candidateReturnRate.ReturnRate,
                        NewUsers = candidateReturnRate.NewUsers,
                        ReturningUsers = candidateReturnRate.ReturningUsers,
                        PackageDistribution = candidateUpgrades.PackageDistribution
                    },
                    CompanyStatistics = new
                    {
                        TotalPackagePayments = companyPayments.Count,
                        BasicPackagePayments = companyUpgrades.BasicCount,
                        PremiumPackagePayments = companyUpgrades.PremiumCount,
                        BasicToPremiumUpgrades = companyUpgrades.BasicToPremiumCount,
                        BasicToBasicRenewals = companyUpgrades.BasicToBasicCount,
                        PremiumToPremiumRenewals = companyUpgrades.PremiumToPremiumCount,
                        UpgradeRate = companyUpgrades.UpgradeRate,
                        RenewalRate = companyUpgrades.RenewalRate,
                        ReturnRate = companyReturnRate.ReturnRate,
                        NewUsers = companyReturnRate.NewUsers,
                        ReturningUsers = companyReturnRate.ReturningUsers,
                        PackageDistribution = companyUpgrades.PackageDistribution
                    },
                    CombinedStatistics = new
                    {
                        TotalPackagePayments = candidatePayments.Count + companyPayments.Count,
                        TotalBasicPackagePayments = candidateUpgrades.BasicCount + companyUpgrades.BasicCount,
                        TotalPremiumPackagePayments = candidateUpgrades.PremiumCount + companyUpgrades.PremiumCount,
                        TotalUpgrades = candidateUpgrades.BasicToPremiumCount + companyUpgrades.BasicToPremiumCount,
                        TotalSamePackageRenewals = candidateUpgrades.BasicToBasicCount + candidateUpgrades.PremiumToPremiumCount +
                                                 companyUpgrades.BasicToBasicCount + companyUpgrades.PremiumToPremiumCount,
                        OverallUpgradeRate = CalculateOverallRate(
                            candidateUpgrades.BasicToPremiumCount + companyUpgrades.BasicToPremiumCount,
                            candidateUpgrades.BasicCount + companyUpgrades.BasicCount),
                        OverallRenewalRate = CalculateOverallRate(
                            candidateUpgrades.BasicToBasicCount + candidateUpgrades.PremiumToPremiumCount +
                            companyUpgrades.BasicToBasicCount + companyUpgrades.PremiumToPremiumCount,
                            (candidateUpgrades.BasicCount + candidateUpgrades.PremiumCount +
                            companyUpgrades.BasicCount + companyUpgrades.PremiumCount)),
                        OverallReturnRate = CalculateOverallRate(
                            candidateReturnRate.ReturningUsers + companyReturnRate.ReturningUsers,
                            candidateReturnRate.NewUsers + candidateReturnRate.ReturningUsers +
                            companyReturnRate.NewUsers + companyReturnRate.ReturningUsers)
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting package upgrade statistics");
                return StatusCode(500, new { Error = "Error retrieving package upgrade statistics", Message = ex.Message });
            }
        }

        // Helper method to calculate package upgrades for candidates
        private (int BasicCount, int PremiumCount, int BasicToPremiumCount, int BasicToBasicCount, int PremiumToPremiumCount, double UpgradeRate, double RenewalRate, object PackageDistribution)
            CalculateCandidatePackageUpgrades(
            List<IGrouping<int, Payment>> userPaymentsGroups,
            List<Payment> filteredPayments,
            List<SubscriptionType> packages,
            DateTime startDate,
            DateTime endDate)
        {
            int basicCount = 0;
            int premiumCount = 0;
            int basicToPremiumCount = 0;
            int basicToBasicCount = 0;
            int premiumToPremiumCount = 0;
            var packageTypes = new Dictionary<string, int>();

            // Get package IDs
            var basicPackageIds = new List<int>();
            var premiumPackageIds = new List<int>();

            foreach (var package in packages)
            {
                var packageType = package.PackageType;
                var packageId = package.SubscriptionTypeId;
                var packageName = package.Name;

                // Count packages by type
                if (!packageTypes.ContainsKey(packageName))
                {
                    packageTypes[packageName] = 0;
                }

                // Add to the correct list
                if (packageType == SubscriptionPackageType.Basic)
                {
                    basicPackageIds.Add(packageId);
                }
                else if (packageType == SubscriptionPackageType.Premium)
                {
                    premiumPackageIds.Add(packageId);
                }
            }

            // Count payments by package type
            foreach (var payment in filteredPayments.Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate))
            {
                // Get package info
                var package = packages.FirstOrDefault(p => p.SubscriptionTypeId == payment.SubscriptionTypeId);

                if (package != null)
                {
                    var packageName = package.Name;
                    packageTypes[packageName] = packageTypes.ContainsKey(packageName)
                        ? packageTypes[packageName] + 1
                        : 1;
                }

                if (basicPackageIds.Contains(payment.SubscriptionTypeId))
                {
                    basicCount++;
                }
                else if (premiumPackageIds.Contains(payment.SubscriptionTypeId))
                {
                    premiumCount++;
                }
            }

            // Count users who upgraded from Basic to Premium or renewed same package
            foreach (var userGroup in userPaymentsGroups)
            {
                // Filter payments for candidate subscriptions only
                var candidatePayments = userGroup.Where(p =>
                    string.IsNullOrEmpty(p.PaymentType) ||
                    p.PaymentType != "CompanySubscription").ToList();

                if (candidatePayments.Count < 2)
                    continue;

                // Get user's payments ordered by date
                var orderedPayments = candidatePayments
                    .OrderBy(p => p.CreatedAt)
                    .ToList();

                if (orderedPayments.Count >= 2)
                {
                    // Check for upgrade/renewal patterns
                    for (int i = 0; i < orderedPayments.Count - 1; i++)
                    {
                        // Only consider payments in the date range
                        if (orderedPayments[i + 1].CreatedAt >= startDate && orderedPayments[i + 1].CreatedAt <= endDate)
                        {
                            bool firstIsBasic = basicPackageIds.Contains(orderedPayments[i].SubscriptionTypeId);
                            bool secondIsBasic = basicPackageIds.Contains(orderedPayments[i + 1].SubscriptionTypeId);
                            bool firstIsPremium = premiumPackageIds.Contains(orderedPayments[i].SubscriptionTypeId);
                            bool secondIsPremium = premiumPackageIds.Contains(orderedPayments[i + 1].SubscriptionTypeId);

                            // Check Basic to Premium upgrade
                            if (firstIsBasic && secondIsPremium)
                            {
                                basicToPremiumCount++;
                                break; // Count only once per user for upgrade
                            }
                            // Check Basic to Basic renewal/update
                            else if (firstIsBasic && secondIsBasic)
                            {
                                basicToBasicCount++;
                                break; // Count only once per user for renewal
                            }
                            // Check Premium to Premium renewal/update
                            else if (firstIsPremium && secondIsPremium)
                            {
                                premiumToPremiumCount++;
                                break; // Count only once per user for renewal
                            }
                        }
                    }
                }
            }

            // Calculate upgrade rate (percentage of Basic users who upgraded to Premium)
            double upgradeRate = basicCount > 0
                ? Math.Round((double)basicToPremiumCount * 100 / basicCount, 2)
                : 0;

            // Calculate renewal rate (percentage of users who renewed same package type)
            int totalUsersWithMultiplePayments = basicToBasicCount + premiumToPremiumCount + basicToPremiumCount;
            int samePackageRenewals = basicToBasicCount + premiumToPremiumCount;
            double renewalRate = totalUsersWithMultiplePayments > 0
                ? Math.Round((double)samePackageRenewals * 100 / totalUsersWithMultiplePayments, 2)
                : 0;

            // Create package distribution object
            var packageDistribution = packageTypes
                .Where(kvp => kvp.Key != "Free")
                .Select(kvp => new { PackageName = kvp.Key, Count = kvp.Value })
                .OrderByDescending(x => x.Count)
                .ToList();

            return (basicCount, premiumCount, basicToPremiumCount, basicToBasicCount, premiumToPremiumCount, upgradeRate, renewalRate, packageDistribution);
        }

        // Helper method to calculate package upgrades for companies
        private (int BasicCount, int PremiumCount, int BasicToPremiumCount, int BasicToBasicCount, int PremiumToPremiumCount, double UpgradeRate, double RenewalRate, object PackageDistribution)
            CalculateCompanyPackageUpgrades(
            List<IGrouping<int, Payment>> userPaymentsGroups,
            List<Payment> filteredPayments,
            List<CompanySubscriptionType> packages,
            DateTime startDate,
            DateTime endDate)
        {
            int basicCount = 0;
            int premiumCount = 0;
            int basicToPremiumCount = 0;
            int basicToBasicCount = 0;
            int premiumToPremiumCount = 0;
            var packageTypes = new Dictionary<string, int>();

            // Get package IDs
            var basicPackageIds = new List<int>();
            var premiumPackageIds = new List<int>();

            foreach (var package in packages)
            {
                var packageType = package.PackageType;
                var packageId = package.CompanySubscriptionTypeId;
                var packageName = package.Name;

                // Count packages by type
                if (!packageTypes.ContainsKey(packageName))
                {
                    packageTypes[packageName] = 0;
                }

                // Add to the correct list
                if (packageType == CompanySubscriptionPackageType.Basic)
                {
                    basicPackageIds.Add(packageId);
                }
                else if (packageType == CompanySubscriptionPackageType.Premium)
                {
                    premiumPackageIds.Add(packageId);
                }
            }

            // Count payments by package type
            foreach (var payment in filteredPayments.Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate))
            {
                // Get package info
                var package = packages.FirstOrDefault(p => p.CompanySubscriptionTypeId == payment.SubscriptionTypeId);

                if (package != null)
                {
                    var packageName = package.Name;
                    packageTypes[packageName] = packageTypes.ContainsKey(packageName)
                        ? packageTypes[packageName] + 1
                        : 1;
                }

                if (basicPackageIds.Contains(payment.SubscriptionTypeId))
                {
                    basicCount++;
                }
                else if (premiumPackageIds.Contains(payment.SubscriptionTypeId))
                {
                    premiumCount++;
                }
            }

            // Count users who upgraded from Basic to Premium or renewed same package
            foreach (var userGroup in userPaymentsGroups)
            {
                // Filter payments for company subscriptions only
                var companyPayments = userGroup.Where(p =>
                    p.PaymentType == "CompanySubscription").ToList();

                if (companyPayments.Count < 2)
                    continue;

                // Get user's payments ordered by date
                var orderedPayments = companyPayments
                    .OrderBy(p => p.CreatedAt)
                    .ToList();

                if (orderedPayments.Count >= 2)
                {
                    // Check for upgrade/renewal patterns
                    for (int i = 0; i < orderedPayments.Count - 1; i++)
                    {
                        // Only consider payments in the date range
                        if (orderedPayments[i + 1].CreatedAt >= startDate && orderedPayments[i + 1].CreatedAt <= endDate)
                        {
                            bool firstIsBasic = basicPackageIds.Contains(orderedPayments[i].SubscriptionTypeId);
                            bool secondIsBasic = basicPackageIds.Contains(orderedPayments[i + 1].SubscriptionTypeId);
                            bool firstIsPremium = premiumPackageIds.Contains(orderedPayments[i].SubscriptionTypeId);
                            bool secondIsPremium = premiumPackageIds.Contains(orderedPayments[i + 1].SubscriptionTypeId);

                            // Check Basic to Premium upgrade
                            if (firstIsBasic && secondIsPremium)
                            {
                                basicToPremiumCount++;
                                break; // Count only once per user for upgrade
                            }
                            // Check Basic to Basic renewal/update
                            else if (firstIsBasic && secondIsBasic)
                            {
                                basicToBasicCount++;
                                break; // Count only once per user for renewal
                            }
                            // Check Premium to Premium renewal/update
                            else if (firstIsPremium && secondIsPremium)
                            {
                                premiumToPremiumCount++;
                                break; // Count only once per user for renewal
                            }
                        }
                    }
                }
            }

            // Calculate upgrade rate (percentage of Basic users who upgraded to Premium)
            double upgradeRate = basicCount > 0
                ? Math.Round((double)basicToPremiumCount * 100 / basicCount, 2)
                : 0;

            // Calculate renewal rate (percentage of users who renewed same package type)
            int totalUsersWithMultiplePayments = basicToBasicCount + premiumToPremiumCount + basicToPremiumCount;
            int samePackageRenewals = basicToBasicCount + premiumToPremiumCount;
            double renewalRate = totalUsersWithMultiplePayments > 0
                ? Math.Round((double)samePackageRenewals * 100 / totalUsersWithMultiplePayments, 2)
                : 0;

            // Create package distribution object
            var packageDistribution = packageTypes
                .Where(kvp => kvp.Key != "Free")
                .Select(kvp => new { PackageName = kvp.Key, Count = kvp.Value })
                .OrderByDescending(x => x.Count)
                .ToList();

            return (basicCount, premiumCount, basicToPremiumCount, basicToBasicCount, premiumToPremiumCount, upgradeRate, renewalRate, packageDistribution);
        }

        // Helper method to calculate return rate
        private (int NewUsers, int ReturningUsers, double ReturnRate) CalculateReturnRate(List<Payment> payments)
        {
            // Group by user to find returning users
            var userGroups = payments
                .GroupBy(p => p.UserId)
                .ToList();

            // Count users with single transaction (new) vs multiple transactions (returning)
            int newUsers = userGroups.Count(g => g.Count() == 1);
            int returningUsers = userGroups.Count(g => g.Count() > 1);
            int totalUsers = newUsers + returningUsers;

            // Calculate return rate
            double returnRate = totalUsers > 0
                ? Math.Round((double)returningUsers * 100 / totalUsers, 2)
                : 0;

            return (newUsers, returningUsers, returnRate);
        }

        // Helper method to calculate overall rate
        private double CalculateOverallRate(int numerator, int denominator)
        {
            return denominator > 0
                ? Math.Round((double)numerator * 100 / denominator, 2)
                : 0;
        }
    }
}