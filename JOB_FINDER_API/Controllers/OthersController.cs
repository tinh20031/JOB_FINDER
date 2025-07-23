using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;
using System;
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
        public async Task<IActionResult> PaymentSuccess([FromQuery] string orderCode, [FromQuery] string code, [FromQuery] string id, [FromQuery] bool cancel, [FromQuery] string status)
        {
            try
            {
                _logger.LogInformation($"Payment success callback received: OrderCode={orderCode}, Code={code}, ID={id}, Cancel={cancel}, Status={status}");

                if (string.IsNullOrEmpty(orderCode))
                {
                    _logger.LogWarning("Payment success received without order code");
                    return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-error?error=missing-order-code");
                }

                // Try to find payment with exact order code first
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.TransactionCode == orderCode);

                // If not found, try with "SUB-" prefix
                if (payment == null && !orderCode.StartsWith("SUB-"))
                {
                    string prefixedOrderCode = $"SUB-{orderCode}";
                    _logger.LogInformation($"Payment not found with original order code, trying with prefix: {prefixedOrderCode}");
                    payment = await _context.Payments
                        .FirstOrDefaultAsync(p => p.TransactionCode == prefixedOrderCode);
                }

                if (payment == null)
                {
                    _logger.LogWarning($"Payment not found for order: {orderCode}");
                    return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-error?error=payment-not-found");
                }

                // If payment is already completed, just redirect to success page
                if (payment.Status == PaymentStatus.Completed)
                {
                    _logger.LogInformation($"Payment already completed for order: {orderCode}");
                    return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-success?orderCode={orderCode}");
                }

                // Process payment if status is PAID
                if (status == "PAID")
                {
                    try
                    {
                        // Extract numeric part from order code
                        string numericPart = orderCode.Replace("SUB-", "");
                        if (long.TryParse(numericPart, out long numericOrderCode))
                        {
                            // Verify payment with PayOS API
                            try
                            {
                                PaymentLinkInformation paymentInfo = await _payOS.getPaymentLinkInformation(numericOrderCode);
                                if (paymentInfo.status != "PAID")
                                {
                                    _logger.LogWarning($"PayOS API reports payment not PAID for order: {orderCode}, status: {paymentInfo.status}");
                                    return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-error?error=payment-status-mismatch&orderCode={orderCode}");
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Error verifying payment with PayOS API: {ex.Message}");
                                // Continue processing payment even if API check fails
                                _logger.LogInformation("Proceeding with payment processing based on callback parameters");
                            }
                        }

                        // Update payment status
                        payment.Status = PaymentStatus.Completed;
                        payment.UpdatedAt = DateTime.UtcNow;
                        payment.PaymentResponse += $"\nCompleted at {DateTime.UtcNow}. PayOS Transaction ID: {id}";

                        // Get subscription package
                        var subscriptionType = await _context.SubscriptionTypes
                            .FindAsync(payment.SubscriptionTypeId);

                        if (subscriptionType == null)
                        {
                            _logger.LogError($"Subscription type not found: {payment.SubscriptionTypeId}");
                            return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-error?error=subscription-not-found&orderCode={orderCode}");
                        }

                        // Check if user has an active subscription
                        var existingSubscription = await _context.CandidateSubscriptions
                            .Where(s => s.UserId == payment.UserId && s.IsActive)
                            .OrderByDescending(s => s.CreatedAt)
                            .FirstOrDefaultAsync();

                        if (existingSubscription != null)
                        {
                            // Update existing subscription - just add more TryMatches
                            existingSubscription.RemainingTryMatches += subscriptionType.TryMatchLimit;
                            existingSubscription.UpdatedAt = DateTime.UtcNow;

                            // Keep tracking EndDate for backward compatibility but don't use it for validation
                            // Set it to a far future date to ensure it doesn't expire
                            existingSubscription.EndDate = DateTime.UtcNow.AddYears(10);

                            _logger.LogInformation($"Updated subscription for user {payment.UserId}, added {subscriptionType.TryMatchLimit} try matches, total now: {existingSubscription.RemainingTryMatches}");
                        }
                        else
                        {
                            // Create new subscription with no expiration
                            var subscription = new CandidateSubscription
                            {
                                UserId = payment.UserId,
                                SubscriptionTypeId = payment.SubscriptionTypeId,
                                StartDate = DateTime.UtcNow,
                                // Set EndDate to a far future date to ensure it doesn't expire
                                EndDate = DateTime.UtcNow.AddYears(10),
                                IsActive = true,
                                RemainingTryMatches = subscriptionType.TryMatchLimit,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };

                            _context.CandidateSubscriptions.Add(subscription);
                            _logger.LogInformation($"Created new subscription for user {payment.UserId} with {subscription.RemainingTryMatches} try matches");
                        }

                        try
                        {
                            await _context.SaveChangesAsync();
                            _logger.LogInformation($"Payment processing completed successfully for order: {orderCode}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error saving payment changes to database: {ex.Message}");
                            return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-error?error=database-error&orderCode={orderCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error processing payment: {ex.Message}");
                        return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-error?error=processing-error&orderCode={orderCode}");
                    }
                }
                else
                {
                    _logger.LogWarning($"Payment status is not PAID. Status: {status}, Order: {orderCode}");
                    return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-error?error=payment-not-paid&status={status}&orderCode={orderCode}");
                }

                // Redirect to success page
                return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-success?orderCode={orderCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error handling payment success: {ex.Message}");
                return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-error?error=unexpected-error");
            }
        }
    }
}