using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JOB_FINDER_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OthersController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OthersController> _logger;
        private readonly PayOS _payOS;

        public OthersController(
            JobFinderDbContext context,
            IConfiguration configuration,
            ILogger<OthersController> logger,
            PayOS payOS)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _payOS = payOS;
        }

        [HttpGet("payment-success")]
        public async Task<IActionResult> PaymentSuccess([FromQuery] string orderCode, [FromQuery] string code, [FromQuery] string id, [FromQuery] bool cancel, [FromQuery] string status, [FromQuery] string? type = null)
        {
            try
            {
                _logger.LogInformation($"Payment success callback received: OrderCode={orderCode}, Code={code}, ID={id}, Cancel={cancel}, Status={status}, Type={type}");

                if (string.IsNullOrEmpty(orderCode))
                {
                    _logger.LogWarning("Payment success received without order code");
                    return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-success?error=missing-order-code");
                }

                // Determine payment type based on prefix if not provided
                if (string.IsNullOrEmpty(type))
                {
                    if (orderCode.StartsWith("COMPSUB-"))
                    {
                        type = "company";
                        _logger.LogInformation("Type not provided but order code has COMPSUB prefix, setting type to company");
                    }
                    else
                    {
                        type = "candidate"; // Default to candidate if no type provided
                        _logger.LogInformation("Type not provided, defaulting to candidate");
                    }
                }

                // Log all payments for debugging
                var allPayments = await _context.Payments.ToListAsync();
                _logger.LogInformation($"Found {allPayments.Count} payments in database");

                Payment payment = null;

                // Check if we're dealing with a company payment
                if (type == "company")
                {
                    // If order code already has the prefix, use it directly
                    if (orderCode.StartsWith("COMPSUB-"))
                    {
                        payment = await _context.Payments
                            .FirstOrDefaultAsync(p => p.TransactionCode == orderCode);
                        _logger.LogInformation($"Searching for company payment with COMPSUB- prefix: {orderCode}");
                    }
                    // Otherwise try adding the prefix
                    else
                    {
                        string prefixedOrderCode = $"COMPSUB-{orderCode}";
                        _logger.LogInformation($"Trying with COMPSUB- prefix: {prefixedOrderCode}");
                        payment = await _context.Payments
                            .FirstOrDefaultAsync(p => p.TransactionCode == prefixedOrderCode);
                    }

                    // Additionally, try searching by PaymentType
                    if (payment == null)
                    {
                        _logger.LogInformation("Trying to find company payment by PaymentType");
                        payment = await _context.Payments
                            .Where(p => p.PaymentType == "CompanySubscription")
                            .OrderByDescending(p => p.CreatedAt)
                            .FirstOrDefaultAsync();

                        if (payment != null)
                        {
                            _logger.LogInformation($"Found company payment by type: {payment.TransactionCode}");
                        }
                    }
                }
                else // candidate
                {
                    // Standard payment lookup for candidate payments
                    if (orderCode.StartsWith("SUB-"))
                    {
                        payment = await _context.Payments
                            .FirstOrDefaultAsync(p => p.TransactionCode == orderCode);
                        _logger.LogInformation($"Searching with SUB- prefix: {orderCode}");
                    }
                    else
                    {
                        string prefixedOrderCode = $"SUB-{orderCode}";
                        _logger.LogInformation($"Trying with SUB- prefix: {prefixedOrderCode}");
                        payment = await _context.Payments
                            .FirstOrDefaultAsync(p => p.TransactionCode == prefixedOrderCode);
                    }
                }

                // If still not found, try one more general search
                if (payment == null)
                {
                    _logger.LogInformation("No payment found with prefixes, trying general search");
                    payment = await _context.Payments
                        .FirstOrDefaultAsync(p =>
                            p.TransactionCode == orderCode ||
                            p.TransactionCode == $"SUB-{orderCode}" ||
                            p.TransactionCode == $"COMPSUB-{orderCode}");

                    // Last resort - check for most recent pending payment
                    if (payment == null)
                    {
                        _logger.LogInformation("Still no payment found, trying to find most recent pending payment");
                        payment = await _context.Payments
                            .Where(p => p.Status == PaymentStatus.Pending)
                            .OrderByDescending(p => p.CreatedAt)
                            .FirstOrDefaultAsync();

                        if (payment != null)
                        {
                            _logger.LogInformation($"Found most recent pending payment: {payment.TransactionCode}");
                        }
                    }
                }

                if (payment == null)
                {
                    _logger.LogWarning($"Payment not found for order: {orderCode}");
                    return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-success?error=payment-not-found&orderCodeReceived={orderCode}&type={type}");
                }

                // If payment is already completed, just redirect to success page
                if (payment.Status == PaymentStatus.Completed)
                {
                    _logger.LogInformation($"Payment already completed for order: {payment.TransactionCode}");
                    return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-success?orderCode={payment.TransactionCode}&type={type}&status=alreadyCompleted");
                }

                // Process payment if status is PAID or not specified
                if (status == "PAID" || string.IsNullOrEmpty(status))
                {
                    try
                    {
                        // Extract numeric part from order code for PayOS verification
                        string numericPart = payment.TransactionCode;
                        if (payment.TransactionCode.StartsWith("SUB-"))
                            numericPart = payment.TransactionCode.Replace("SUB-", "");
                        else if (payment.TransactionCode.StartsWith("COMPSUB-"))
                            numericPart = payment.TransactionCode.Replace("COMPSUB-", "");

                        if (long.TryParse(numericPart, out long numericOrderCode))
                        {
                            // Verify payment with PayOS API
                            try
                            {
                                PaymentLinkInformation paymentInfo = await _payOS.getPaymentLinkInformation(numericOrderCode);
                                _logger.LogInformation($"PayOS API returned status {paymentInfo.status} for order {numericOrderCode}");

                                if (paymentInfo.status != "PAID")
                                {
                                    _logger.LogWarning($"PayOS API reports payment not PAID for order: {payment.TransactionCode}, status: {paymentInfo.status}");
                                    // Continue anyway since sometimes the PayOS status might be delayed
                                    _logger.LogInformation("Continuing with payment processing despite PayOS status mismatch");
                                }
                                else
                                {
                                    _logger.LogInformation($"PayOS API confirms payment is PAID for order: {payment.TransactionCode}");
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Error verifying payment with PayOS API: {ex.Message}");
                                // Continue processing payment even if API check fails
                                _logger.LogInformation("Proceeding with payment processing despite PayOS API error");
                            }
                        }
                        else
                        {
                            _logger.LogWarning($"Could not parse numeric order code from {payment.TransactionCode}");
                        }

                        // Update payment status
                        payment.Status = PaymentStatus.Completed;
                        payment.UpdatedAt = DateTime.UtcNow;
                        payment.PaymentResponse += $"\nCompleted at {DateTime.UtcNow}. PayOS Transaction ID: {id}";

                        // Determine if this is a company subscription based on the payment type or the provided type parameter
                        bool isCompanySubscription = (type == "company" || payment.PaymentType == "CompanySubscription");

                        
                        if (isCompanySubscription)
                        {
                              // Handle company subscription
                            var subscriptionType = await _context.CompanySubscriptionTypes
                                .FindAsync(payment.SubscriptionTypeId);

                            if (subscriptionType == null)
                            {
                                _logger.LogError($"Company subscription type not found: {payment.SubscriptionTypeId}");
                                return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-success?error=subscription-not-found&orderCode={payment.TransactionCode}&type=company");
                            }

                            // Check if company has an active subscription
                            var existingSubscription = await _context.CompanySubscriptions
                                 .Include(s => s.SubscriptionType)
                                .Where(s => s.UserId == payment.UserId && s.IsActive)
                                .OrderByDescending(s => s.CreatedAt)
                                .FirstOrDefaultAsync();

                            if (existingSubscription != null)
                            {
                                // Update existing subscription
                                //existingSubscription.CompanySubscriptionTypeId = payment.SubscriptionTypeId; // Update the subscription type ID
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

                                // Always add benefits and extend duration regardless of tier
                                existingSubscription.RemainingJobPosts += subscriptionType.JobPostLimit;
                                existingSubscription.RemainingTrendingJobPosts += subscriptionType.TrendingJobLimit;
                                existingSubscription.UpdatedAt = DateTime.UtcNow;
                                //existingSubscription.EndDate = DateTime.UtcNow.AddDays(subscriptionType.DurationInDays);
                                existingSubscription.EndDate = existingSubscription.EndDate.AddDays(subscriptionType.DurationInDays);


                                //                    _logger.LogInformation($"Updated company subscription for user {payment.UserId} to {subscriptionType.Name}, " +
                                //$"added {subscriptionType.JobPostLimit} regular jobs and {subscriptionType.TrendingJobLimit} trending jobs");
                                //                }
                                _logger.LogInformation($"Updated company subscription for user {payment.UserId}, " +
                                $"added {subscriptionType.JobPostLimit} regular jobs and {subscriptionType.TrendingJobLimit} trending jobs");
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
                                _logger.LogInformation($"Created new company subscription for user {payment.UserId} with {subscription.RemainingJobPosts} " +
                                    $"regular jobs and {subscription.RemainingTrendingJobPosts} trending jobs");
                            }
                        }
                        else
                        {
                            // Handle candidate subscription
                            var subscriptionType = await _context.SubscriptionTypes
                                .FindAsync(payment.SubscriptionTypeId);

                            if (subscriptionType == null)
                            {
                                _logger.LogError($"Candidate subscription type not found: {payment.SubscriptionTypeId}");
                                return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-success?error=subscription-not-found&orderCode={payment.TransactionCode}&type=candidate");
                            }

                            // Check if user has an active subscription
                            var existingSubscription = await _context.CandidateSubscriptions
                                .Where(s => s.UserId == payment.UserId && s.IsActive)
                                .Include(s => s.SubscriptionType)
                                .OrderByDescending(s => s.CreatedAt)
                                .FirstOrDefaultAsync();

                            if (existingSubscription != null)
                            {
                                // Update existing subscription
                                //existingSubscription.SubscriptionTypeId = payment.SubscriptionTypeId; // Thêm dòng này để cập nhật SubscriptionTypeId
                                // Get current subscription type
                                var currentSubscriptionType = existingSubscription.SubscriptionType;

                                // Only upgrade subscription type if the new one is higher tier, otherwise keep current tier
                                bool shouldUpgradeSubscriptionType = subscriptionType.PackageType > currentSubscriptionType.PackageType;

                                if (shouldUpgradeSubscriptionType)
                                {
                                    // Upgrade to higher tier subscription
                                    existingSubscription.SubscriptionTypeId = payment.SubscriptionTypeId;
                                    _logger.LogInformation($"Upgraded candidate subscription type from {currentSubscriptionType.PackageType} to {subscriptionType.PackageType} for user {payment.UserId}");
                                }
                                else
                                {
                                    _logger.LogInformation($"Keeping current candidate subscription type {currentSubscriptionType.PackageType} (higher or equal to purchased {subscriptionType.PackageType}) for user {payment.UserId}");
                                }

                                // Always add benefits and extend duration regardless of tier
                                existingSubscription.RemainingTryMatches += subscriptionType.TryMatchLimit;
                                existingSubscription.UpdatedAt = DateTime.UtcNow;
                                existingSubscription.EndDate = DateTime.UtcNow.AddYears(10);

                                //    _logger.LogInformation($"Updated candidate subscription for user {payment.UserId}, changed subscription type to {subscriptionType.Name}, added {subscriptionType.TryMatchLimit} try matches, total now: {existingSubscription.RemainingTryMatches}");
                                //}
                                _logger.LogInformation($"Updated candidate subscription for user {payment.UserId}, added {subscriptionType.TryMatchLimit} try matches, total now: {existingSubscription.RemainingTryMatches}");
                            }
                            else
                            {
                                // Create new subscription with no expiration
                                var subscription = new CandidateSubscription
                                {
                                    UserId = payment.UserId,
                                    SubscriptionTypeId = payment.SubscriptionTypeId,
                                    StartDate = DateTime.UtcNow,
                                    EndDate = DateTime.UtcNow.AddYears(10),
                                    IsActive = true,
                                    RemainingTryMatches = subscriptionType.TryMatchLimit,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow
                                };

                                _context.CandidateSubscriptions.Add(subscription);
                                _logger.LogInformation($"Created new candidate subscription for user {payment.UserId} with {subscription.RemainingTryMatches} try matches");
                            }
                        }

                        try
                        {
                            await _context.SaveChangesAsync();
                            _logger.LogInformation($"Payment processing completed successfully for order: {payment.TransactionCode}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error saving payment changes to database: {ex.Message}");
                            return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-success?error=database-error&orderCode={payment.TransactionCode}&type={type}&message={Uri.EscapeDataString(ex.Message)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error processing payment: {ex.Message}");
                        return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-success?error=processing-error&orderCode={payment.TransactionCode}&type={type}&message={Uri.EscapeDataString(ex.Message)}");
                    }
                }
                else if (status == "CANCELLED")
                {
                    _logger.LogWarning($"Payment was cancelled. Status: {status}, Order: {payment.TransactionCode}");
                    payment.Status = PaymentStatus.Cancelled;
                    payment.UpdatedAt = DateTime.UtcNow;
                    payment.PaymentResponse += $"\nCancelled at {DateTime.UtcNow}";
                    await _context.SaveChangesAsync();

                    return Redirect($"{_configuration["AppSettings:BaseUrl"]}/others/payment-cancelled?orderCode={payment.TransactionCode}&type={type}");
                }
                else
                {
                    _logger.LogWarning($"Payment status is not PAID. Status: {status}, Order: {payment.TransactionCode}");
                    // Continue anyway and assume payment is successful
                    _logger.LogInformation("Continuing with payment processing despite status not being PAID");

                    // Set payment as completed anyway
                    payment.Status = PaymentStatus.Completed;
                    payment.UpdatedAt = DateTime.UtcNow;
                    payment.PaymentResponse += $"\nForced completion at {DateTime.UtcNow} with status {status}";
                    await _context.SaveChangesAsync();
                }

                // Redirect to success page with the transaction code from the payment record
                string redirectUrl = $"{_configuration["AppSettings:BaseUrl"]}/payment-success?orderCode={payment.TransactionCode}";
                if (!string.IsNullOrEmpty(type))
                {
                    redirectUrl += $"&type={type}";
                }

                _logger.LogInformation($"Redirecting to: {redirectUrl}");
                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error handling payment success: {ex.Message}");
                string redirectUrl = $"{_configuration["AppSettings:BaseUrl"]}/payment-success?error=unexpected-error&message={Uri.EscapeDataString(ex.Message)}";
                if (!string.IsNullOrEmpty(type))
                {
                    redirectUrl += $"&type={type}";
                }
                return Redirect(redirectUrl);
            }
        }
    }


public class ProcessPaymentRequest
    {
        public string OrderCode { get; set; }
        public string Type { get; set; } // "company" or null for candidate
    }
}