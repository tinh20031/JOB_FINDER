using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Company")]
    public class CompanySubscriptionController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CompanySubscriptionController> _logger;
        private readonly PayOS _payOS;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CompanySubscriptionController(
            JobFinderDbContext context,
            IConfiguration configuration,
            ILogger<CompanySubscriptionController> logger,
            PayOS payOS,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _payOS = payOS;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("packages")]
        public async Task<IActionResult> GetSubscriptionPackages()
        {
            try
            {
                // First check if we have any subscription types in the database
                var dbPackages = await _context.CompanySubscriptionTypes
                    .Where(s => s.IsActive)
                    .ToListAsync();

                // If no packages found in database, create default ones
                if (!dbPackages.Any())
                {
                    _logger.LogInformation("No company subscription packages found in database. Returning default packages.");

                    // Return default packages
                    return Ok(new List<object>
                    {
                        new
                        {
                            CompanySubscriptionTypeId = 1,
                            PackageType = CompanySubscriptionPackageType.Free,
                            Name = "Free",
                            Description = "Free tier with basic features",
                            Price = 0m,
                            JobPostLimit = 2,
                            CvMatchLimit = 5,
                            TrendingJobLimit = 0, // Free tier has no trending jobs
                            DurationInDays = 30,
                            Features = new string[]
                            {
                                "Post up to 2 jobs",
                                "View top 5 CV matches per job"
                            }
                        },
                        new
                        {
                            CompanySubscriptionTypeId = 2,
                            PackageType = CompanySubscriptionPackageType.Basic,
                            Name = "Basic",
                            Description = "Basic tier with extended features",
                            Price = 500000m,
                            JobPostLimit = 10,
                            CvMatchLimit = 10,
                            TrendingJobLimit = 5, // Basic tier gets 5 trending jobs
                            DurationInDays = 30,
                            Features = new string[]
                            {
                                "Post up to 10 jobs",
                                "View top 10 CV matches per job",
                                "Priority job listings",
                                "Up to 5 trending job posts"
                            }
                        },
                        new
                        {
                            CompanySubscriptionTypeId = 3,
                            PackageType = CompanySubscriptionPackageType.Premium,
                            Name = "Premium",
                            Description = "Premium tier with unlimited features",
                            Price = 1500000m,
                            JobPostLimit = int.MaxValue,
                            CvMatchLimit = int.MaxValue,
                            TrendingJobLimit = 10, // Premium tier gets 10 trending jobs
                            DurationInDays = 30,
                            Features = new string[]
                            {
                                "Unlimited job posts",
                                "View all CV matches",
                                "Featured listings",
                                "Premium company badge",
                                "Advanced analytics",
                                "Up to 10 trending job posts"
                            }
                        }
                    });
                }

                // If packages exist in database, map them to the response model
                var packages = dbPackages.Select(s => new
                {
                    s.CompanySubscriptionTypeId,
                    s.PackageType,
                    s.Name,
                    s.Description,
                    s.Price,
                    s.JobPostLimit,
                    s.CvMatchLimit,
                    s.TrendingJobLimit,
                    s.DurationInDays,
                    Features = GetPackageFeatures(s)
                }).ToList();

                return Ok(packages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company subscription packages: {ErrorMessage}", ex.Message);
                return StatusCode(500, "Error retrieving subscription packages");
            }
        }

        private string[] GetPackageFeatures(CompanySubscriptionType packageType)
        {
            var features = new List<string>();

            switch (packageType.PackageType)
            {
                case CompanySubscriptionPackageType.Free:
                    features.Add($"Post up to {packageType.JobPostLimit} jobs");
                    features.Add($"View top {packageType.CvMatchLimit} CV matches per job");
                    features.Add("No trending jobs");
                    break;

                case CompanySubscriptionPackageType.Basic:
                    features.Add($"Post up to {packageType.JobPostLimit} jobs");
                    features.Add($"View top {packageType.CvMatchLimit} CV matches per job");
                    features.Add("Priority job listings");
                    features.Add($"Up to {packageType.TrendingJobLimit} trending job posts");
                    break;

                case CompanySubscriptionPackageType.Premium:
                    features.Add("Unlimited job posts");
                    features.Add("View all CV matches");
                    features.Add("Featured listings");
                    features.Add("Premium company badge");
                    features.Add("Advanced analytics");
                    features.Add($"Up to {packageType.TrendingJobLimit} trending job posts");
                    break;
            }

            return features.ToArray();
        }

        [HttpGet("payment-status/{orderCode}")]
        public async Task<IActionResult> CheckPaymentStatus(string orderCode)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID");

            // Format the order code with COMPSUB- prefix if needed
            string formattedOrderCode = orderCode;
            if (!orderCode.StartsWith("COMPSUB-"))
            {
                formattedOrderCode = $"COMPSUB-{orderCode}";
            }

            _logger.LogInformation($"Checking payment status for company order: {formattedOrderCode}, UserId: {userId}");

            var payment = await _context.Payments
                .Where(p => (p.TransactionCode == formattedOrderCode || p.TransactionCode == orderCode) &&
                      (p.UserId == userId || User.IsInRole("Admin")) &&
                      p.PaymentType == "CompanySubscription")
                .FirstOrDefaultAsync();

            if (payment == null)
            {
                _logger.LogWarning($"Company payment not found for order: {formattedOrderCode}");
                return NotFound($"Payment not found for order code: {orderCode}");
            }

            try
            {
                // Extract numeric part from order code
                string numericPart = payment.TransactionCode.Replace("COMPSUB-", "");
                if (!long.TryParse(numericPart, out long numericOrderCode))
                {
                    _logger.LogWarning($"Invalid order code format: {payment.TransactionCode}");
                    return BadRequest("Invalid order code format");
                }

                // Check payment status from PayOS API
                PaymentLinkInformation paymentInfo = await _payOS.getPaymentLinkInformation(numericOrderCode);
                _logger.LogInformation($"PayOS status for {payment.TransactionCode}: {paymentInfo.status}");

                // Update payment status if needed
                if (paymentInfo.status == "PAID" && payment.Status != PaymentStatus.Completed)
                {
                    // Update payment status
                    payment.Status = PaymentStatus.Completed;
                    payment.UpdatedAt = DateTime.UtcNow;

                    // Get subscription package
                    var subscriptionType = await _context.CompanySubscriptionTypes
                        .FindAsync(payment.SubscriptionTypeId);

                    if (subscriptionType == null)
                    {
                        _logger.LogError($"Company subscription type not found: {payment.SubscriptionTypeId}");
                        return BadRequest($"Company subscription type not found: {payment.SubscriptionTypeId}");
                    }

                    // Check if company has an active subscription
                    var existingSubscription = await _context.CompanySubscriptions
                        .Where(s => s.UserId == payment.UserId && s.IsActive && s.EndDate > DateTime.UtcNow)
                        .Include(s => s.SubscriptionType)
                        .FirstOrDefaultAsync();

                    if (existingSubscription != null)
                    {
                        // Get current subscription type
                        var currentSubscriptionType = existingSubscription.SubscriptionType;
                        
                        // Only upgrade subscription type if the new one is higher tier, otherwise keep current tier
                        bool shouldUpgradeSubscriptionType = subscriptionType.PackageType > currentSubscriptionType.PackageType;
                        
                        if (shouldUpgradeSubscriptionType)
                        {
                            // Upgrade to higher tier subscription
                            existingSubscription.CompanySubscriptionTypeId = payment.SubscriptionTypeId;
                            _logger.LogInformation($"Upgraded company subscription type from {currentSubscriptionType.PackageType} to {subscriptionType.PackageType} for user {payment.UserId}");
                        }
                        else
                        {
                            _logger.LogInformation($"Keeping current company subscription type {currentSubscriptionType.PackageType} (higher or equal to purchased {subscriptionType.PackageType}) for user {payment.UserId}");
                        }
                        
                        // Always extend duration and add benefits regardless of tier
                        existingSubscription.EndDate = existingSubscription.EndDate.AddDays(subscriptionType.DurationInDays);
                        existingSubscription.RemainingJobPosts += subscriptionType.JobPostLimit;
                        existingSubscription.RemainingTrendingJobPosts += subscriptionType.TrendingJobLimit;
                        existingSubscription.UpdatedAt = DateTime.UtcNow;
                        
                        _logger.LogInformation($"Extended company subscription for user {payment.UserId} until {existingSubscription.EndDate}, " +
                            $"added {subscriptionType.JobPostLimit} job posts and {subscriptionType.TrendingJobLimit} trending job posts");
                    }
                    else
                    {
                        // Create new subscription
                        var subscription = new CompanySubscription
                        {
                            UserId = payment.UserId,
                            CompanySubscriptionTypeId = payment.SubscriptionTypeId,
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddDays(subscriptionType.DurationInDays),
                            IsActive = true,
                            RemainingJobPosts = subscriptionType.JobPostLimit,
                            RemainingTrendingJobPosts = subscriptionType.TrendingJobLimit,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        _context.CompanySubscriptions.Add(subscription);
                        _logger.LogInformation($"Created new company subscription for user {payment.UserId} until {subscription.EndDate}, " +
                            $"with {subscription.RemainingJobPosts} job posts and {subscription.RemainingTrendingJobPosts} trending job posts");
                    }

                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    Success = true,
                    OrderCode = payment.TransactionCode,
                    Status = payment.Status.ToString(),
                    Amount = payment.Amount,
                    PayOsStatus = paymentInfo.status,
                    CreatedAt = payment.CreatedAt,
                    UpdatedAt = payment.UpdatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking company payment status: {ex.Message}");
                return StatusCode(500, "Error checking payment status");
            }
        }

        // Helper method to process successful company payments
        private async Task ProcessCompanyPayment(Payment payment)
        {
            _logger.LogInformation($"Processing successful company payment for order: {payment.TransactionCode}");

            // Update payment status
            payment.Status = PaymentStatus.Completed;
            payment.UpdatedAt = DateTime.UtcNow;

            try
            {
                // Get subscription package
                var subscriptionType = await _context.CompanySubscriptionTypes
                    .FindAsync(payment.SubscriptionTypeId);

                if (subscriptionType == null)
                {
                    _logger.LogError($"Company subscription type not found: {payment.SubscriptionTypeId}");
                    throw new Exception($"Company subscription type not found: {payment.SubscriptionTypeId}");
                }

                // Check if company has an active subscription
                var existingSubscription = await _context.CompanySubscriptions
                    .Where(s => s.UserId == payment.UserId && s.IsActive && s.EndDate > DateTime.UtcNow)
                    .Include(s => s.SubscriptionType)
                    .FirstOrDefaultAsync();

                if (existingSubscription != null)
                {
                    // Get current subscription type
                    var currentSubscriptionType = existingSubscription.SubscriptionType;
                    
                    // Only upgrade subscription type if the new one is higher tier, otherwise keep current tier
                    bool shouldUpgradeSubscriptionType = subscriptionType.PackageType > currentSubscriptionType.PackageType;
                    
                    if (shouldUpgradeSubscriptionType)
                    {
                        // Upgrade to higher tier subscription
                        existingSubscription.CompanySubscriptionTypeId = payment.SubscriptionTypeId;
                        _logger.LogInformation($"Upgraded company subscription type from {currentSubscriptionType.PackageType} to {subscriptionType.PackageType} for user {payment.UserId}");
                    }
                    else
                    {
                        _logger.LogInformation($"Keeping current company subscription type {currentSubscriptionType.PackageType} (higher or equal to purchased {subscriptionType.PackageType}) for user {payment.UserId}");
                    }
                    
                    // Always extend duration and add benefits regardless of tier
                    existingSubscription.EndDate = existingSubscription.EndDate.AddDays(subscriptionType.DurationInDays);
                    existingSubscription.RemainingJobPosts += subscriptionType.JobPostLimit;
                    existingSubscription.RemainingTrendingJobPosts += subscriptionType.TrendingJobLimit;
                    existingSubscription.UpdatedAt = DateTime.UtcNow;
                    
                    _logger.LogInformation($"Extended company subscription for user {payment.UserId} until {existingSubscription.EndDate}, " +
                        $"added {subscriptionType.JobPostLimit} job posts and {subscriptionType.TrendingJobLimit} trending job posts");
                }
                else
                {
                    // Create new subscription
                    var subscription = new CompanySubscription
                    {
                        UserId = payment.UserId,
                        CompanySubscriptionTypeId = payment.SubscriptionTypeId,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddDays(subscriptionType.DurationInDays),
                        IsActive = true,
                        RemainingJobPosts = subscriptionType.JobPostLimit,
                        RemainingTrendingJobPosts = subscriptionType.TrendingJobLimit,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.CompanySubscriptions.Add(subscription);
                    _logger.LogInformation($"Created new company subscription for user {payment.UserId} until {subscription.EndDate}, " +
                        $"with {subscription.RemainingJobPosts} job posts and {subscription.RemainingTrendingJobPosts} trending job posts");
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Company payment processed successfully for order: {payment.TransactionCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing company payment: {ex.Message}");
                throw;
            }
        }

        [HttpGet("my-subscription")]
        public async Task<IActionResult> GetMySubscription()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID");

            try
            {
                // Check for active company subscription
                var subscription = await _context.CompanySubscriptions
                    .Where(s => s.UserId == userId && s.IsActive && s.EndDate > DateTime.UtcNow)
                    .Include(s => s.SubscriptionType)
                    .OrderByDescending(s => s.EndDate)
                    .FirstOrDefaultAsync();

                // Get current job count
                var activeJobCount = await _context.Jobs
                    .CountAsync(j => j.CompanyId == userId &&
                                (j.Status == Job.JobStatus.active || j.Status == Job.JobStatus.pending));

                // Get trending job count
                var activeTrendingJobCount = await _context.Jobs
                    .CountAsync(j => j.CompanyId == userId &&
                                j.IsTrending &&
                                (j.Status == Job.JobStatus.active || j.Status == Job.JobStatus.pending));

                if (subscription == null)
                {
                    // Get the free subscription type for default values
                    var freeSubscription = await _context.CompanySubscriptionTypes
                        .FirstOrDefaultAsync(s => s.PackageType == CompanySubscriptionPackageType.Free);

                    int jobPostLimit = freeSubscription?.JobPostLimit ?? 2;
                    int cvMatchLimit = freeSubscription?.CvMatchLimit ?? 5;
                    int trendingJobLimit = freeSubscription?.TrendingJobLimit ?? 0; // Free tier has no trending jobs
                    int remainingJobPosts = Math.Max(0, jobPostLimit - activeJobCount);

                    return Ok(new
                    {
                        IsSubscribed = false,
                        CurrentTier = "Free",
                        JobPostLimit = jobPostLimit,
                        CvMatchLimit = cvMatchLimit,
                        TrendingJobLimit = trendingJobLimit,
                        ActiveJobCount = activeJobCount,
                        ActiveTrendingJobCount = activeTrendingJobCount,
                        RemainingJobPosts = remainingJobPosts,
                        RemainingTrendingJobPosts = 0,
                        Message = $"You are on the Free tier. Upgrade to post more jobs, see more CV matches, and create trending jobs."
                    });
                }

                // Calculate remaining job posts
                int remaining = Math.Max(0, subscription.RemainingJobPosts);
                int remainingTrending = Math.Max(0, subscription.RemainingTrendingJobPosts);

                return Ok(new
                {
                    IsSubscribed = true,
                    Subscription = new
                    {
                        subscription.CompanySubscriptionId,
                        PackageName = subscription.SubscriptionType.Name,
                        subscription.SubscriptionType.Description,
                        subscription.StartDate,
                        subscription.EndDate,
                        subscription.RemainingJobPosts,
                        subscription.RemainingTrendingJobPosts,
                        ActiveJobCount = activeJobCount,
                        ActiveTrendingJobCount = activeTrendingJobCount,
                        DaysRemaining = Math.Max(0, (subscription.EndDate - DateTime.UtcNow).Days),
                        JobPostLimit = subscription.SubscriptionType.JobPostLimit,
                        CvMatchLimit = subscription.SubscriptionType.CvMatchLimit,
                        TrendingJobLimit = subscription.SubscriptionType.TrendingJobLimit
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting company subscription for user ID {UserId}", userId);
                return StatusCode(500, "Error retrieving subscription information");
            }
        }

        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromBody] CompanyPaymentRequest request)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID");

            var package = await _context.CompanySubscriptionTypes
                .FirstOrDefaultAsync(s => s.CompanySubscriptionTypeId == request.SubscriptionTypeId);

            if (package == null)
                return NotFound("Subscription package not found");

            if (!package.IsActive)
                return BadRequest("This subscription package is no longer available");

            // Get user info
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return NotFound("User not found");

            try
            {
                // Create order code with timestamp
                long numericOrderCode = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                string orderCodeStr = $"COMPSUB-{numericOrderCode}";

                // Get base URL for the API
                var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
                var frontendBaseUrl = _configuration["AppSettings:BaseUrl"];

                // Create item for PayOS
                var item = new ItemData(
                    $"Company Package {package.Name}",
                    1,
                    (int)package.Price
                );
                List<ItemData> items = new List<ItemData> { item };

                // Create payment data
                var shortDesc = $"Company Package {package.Name}";
                if (shortDesc.Length > 25)
                {
                    shortDesc = shortDesc.Substring(0, 25);
                }

                // Log the transaction code we're using
                _logger.LogInformation($"Creating company payment with transaction code: {orderCodeStr}");

                // Create payment data with clear company type parameter
                var paymentData = new PaymentData(
                    numericOrderCode,
                    (int)package.Price,
                    shortDesc,
                    items,
                    $"{baseUrl}/api/payment/cancel/{orderCodeStr}",
                    $"{baseUrl}/others/payment-success?orderCode={orderCodeStr}&type=company"
                );

                // Call PayOS API to create payment link
                CreatePaymentResult paymentResult = await _payOS.createPaymentLink(paymentData);

                // Record the pending payment
                var payment = new Payment
                {
                    UserId = userId,
                    SubscriptionTypeId = package.CompanySubscriptionTypeId,
                    TransactionCode = orderCodeStr,
                    Amount = package.Price,
                    Status = PaymentStatus.Pending,
                    PaymentResponse = $"{{\"checkoutUrl\":\"{paymentResult.checkoutUrl}\",\"qrCode\":\"{paymentResult.qrCode}\",\"numericOrderCode\":{numericOrderCode}}}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    PaymentType = "CompanySubscription" // Add this field to distinguish from candidate payments
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Company payment created and saved to database with ID {payment.PaymentId} and TransactionCode {payment.TransactionCode}");

                return Ok(new
                {
                    Success = true,
                    CheckoutUrl = paymentResult.checkoutUrl,
                    QrCode = paymentResult.qrCode,
                    OrderCode = orderCodeStr,
                    NumericOrderCode = numericOrderCode,
                    Amount = package.Price,
                    PackageName = package.Name,
                    Description = shortDesc
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating company payment: {ex.Message}");
                return StatusCode(500, "Error processing payment request");
            }
        }

        // Cancel handling - use the same approach as PaymentController
        [HttpGet("cancel/{orderCode}")]
        public async Task<IActionResult> CancelWithOrderCode(string orderCode)
        {
            try
            {
                // Verify if order exists
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.TransactionCode == orderCode && p.PaymentType == "CompanySubscription");

                if (payment == null)
                {
                    _logger.LogWarning($"Company payment not found for order: {orderCode}");
                    return Redirect($"{_configuration["AppSettings:BaseUrl"]}/others/payment-cancelled?error=payment-not-found&type=company");
                }

                // Only update status if payment is still pending
                if (payment.Status == PaymentStatus.Pending)
                {
                    // Extract numeric part from order code
                    string numericPart = orderCode.Replace("COMPSUB-", "");
                    if (long.TryParse(numericPart, out long numericOrderCode))
                    {
                        try
                        {
                            // Call PayOS API to cancel payment
                            await _payOS.cancelPaymentLink(numericOrderCode);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error cancelling company payment with PayOS: {ex.Message}");
                            // Continue updating payment status even if PayOS API fails
                        }
                    }

                    // Update payment status in database
                    payment.Status = PaymentStatus.Cancelled;
                    payment.UpdatedAt = DateTime.UtcNow;
                    payment.PaymentResponse += $"\nCancelled at {DateTime.UtcNow}";

                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Company payment cancelled successfully for order: {orderCode}");
                }
                else
                {
                    _logger.LogWarning($"Cannot cancel company payment with status: {payment.Status} for order: {orderCode}");
                }

                // Redirect to the cancelled payment page
                return Redirect($"{_configuration["AppSettings:BaseUrl"]}/others/payment-cancelled?orderCode={orderCode}&type=company");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing company payment cancellation: {ex.Message}");
                return Redirect($"{_configuration["AppSettings:BaseUrl"]}/others/payment-cancelled?error=processing-error&orderCode={orderCode}&type=company");
            }
        }

        [HttpGet("cancel")]
        public IActionResult Cancel()
        {
            // Redirect user to the cancelled payment page
            return Redirect($"{_configuration["AppSettings:BaseUrl"]}/others/payment-cancelled?type=company");
        }
    }

    public class CompanyPaymentRequest
    {
        public int SubscriptionTypeId { get; set; }
    }
}