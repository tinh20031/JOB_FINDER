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
    public class PaymentController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentController> _logger;
        private readonly PayOS _payOS;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentController(
            JobFinderDbContext context,
            IConfiguration configuration,
            ILogger<PaymentController> logger,
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
            var packages = await _context.SubscriptionTypes
                .Where(s => s.IsActive)
                .Select(s => new
                {
                    s.SubscriptionTypeId,
                    s.PackageType,
                    s.Name,
                    s.Description,
                    s.Price,
                    s.TryMatchLimit,
                    s.DurationInDays
                })
                .ToListAsync();

            return Ok(packages);
        }

        /*[Authorize]
        [HttpGet("my-subscription")]
        public async Task<IActionResult> GetMySubscription()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID");

            var subscription = await _context.CandidateSubscriptions
                .Where(s => s.UserId == userId && s.IsActive && s.EndDate > DateTime.UtcNow)
                .Include(s => s.SubscriptionType)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync();

            if (subscription == null)
            {
                // Get the free subscription type to show what they could have
                var freeSubscription = await _context.SubscriptionTypes
                    .FirstOrDefaultAsync(s => s.PackageType == SubscriptionPackageType.Free);

                return Ok(new
                {
                    IsSubscribed = false,
                    FreePackage = freeSubscription != null ? new
                    {
                        freeSubscription.Name,
                        freeSubscription.Description,
                        freeSubscription.TryMatchLimit
                    } : null
                });
            }

            return Ok(new
            {
                IsSubscribed = true,
                Subscription = new
                {
                    subscription.CandidateSubscriptionId,
                    PackageName = subscription.SubscriptionType.Name,
                    subscription.SubscriptionType.Description,
                    subscription.StartDate,
                    subscription.EndDate,
                    subscription.RemainingTryMatches,
                    DaysRemaining = (subscription.EndDate - DateTime.UtcNow).Days
                }
            });
        }*/
        [Authorize]
        [HttpGet("my-subscription")]
        public async Task<IActionResult> GetMySubscription()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID");

            var subscription = await _context.CandidateSubscriptions
                .Where(s => s.UserId == userId && s.IsActive) 
                .Include(s => s.SubscriptionType)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (subscription == null)
            {
                // Get the free subscription type
                var freeSubscription = await _context.SubscriptionTypes
                    .FirstOrDefaultAsync(s => s.PackageType == SubscriptionPackageType.Free);

                // Check if user has used their free try-match
                var tryMatchCount = await _context.TryMatchRecords
                    .Where(r => r.UserId == userId)
                    .CountAsync();

                int remainingFreeMatches = tryMatchCount == 0 ? 1 : 0;

                return Ok(new
                {
                    IsSubscribed = false,
                    FreePackage = freeSubscription != null ? new
                    {
                        freeSubscription.Name,
                        freeSubscription.Description,
                        freeSubscription.TryMatchLimit,
                        RemainingFreeMatches = remainingFreeMatches 
                    } : null
                });
            }

            return Ok(new
            {
                IsSubscribed = true,
                Subscription = new
                {
                    subscription.CandidateSubscriptionId,
                    PackageName = subscription.SubscriptionType.Name,
                    subscription.SubscriptionType.Description,
                    subscription.StartDate,
                    subscription.RemainingTryMatches,
                    // No longer showing DaysRemaining since we've removed the time limitation
                }
            });
        }

        [Authorize]
        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID");

            var package = await _context.SubscriptionTypes
                .FirstOrDefaultAsync(s => s.SubscriptionTypeId == request.SubscriptionTypeId);

            if (package == null)
                return NotFound("Subscription package not found");

            if (!package.IsActive)
                return BadRequest("This subscription package is no longer available");

            // Get user info for the payment
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return NotFound("User not found");

            try
            {
                // Tạo orderCode là một số nguyên theo yêu cầu của PayOS
                long numericOrderCode = DateTimeOffset.Now.ToUnixTimeMilliseconds(); // Sử dụng timestamp
                                                                                     // Cũng tạo một mã đơn hàng dạng chuỗi để lưu trong database
                string orderCodeStr = $"SUB-{numericOrderCode}";

                // Get the current request's base URL
                var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";

                // Create item for PayOS
                var item = new ItemData(
                    $"Package {package.Name}",
                    1,
                    (int)package.Price
                );
                List<ItemData> items = new List<ItemData> { item };

                // Create payment data với orderCode là một số nguyên
                // FIX: Limit description to maximum 25 characters
                var shortDesc = $"Package {package.Name}";
                if (shortDesc.Length > 25)
                {
                    shortDesc = shortDesc.Substring(0, 25);
                }

                // Đảm bảo URL chuyển hướng sử dụng định dạng đúng
                var paymentData = new PaymentData(
                    numericOrderCode,
                    (int)package.Price,
                    shortDesc,
                    items,
                    $"{baseUrl}/api/payment/cancel/{orderCodeStr}",
                    $"{baseUrl}/others/payment-success?orderCode={orderCodeStr}"
                );

                // Ghi log để debug
                _logger.LogInformation($"Creating payment with success URL: {baseUrl}/others/payment-success?orderCode={orderCodeStr}");

                // Call PayOS API to create payment link
                CreatePaymentResult paymentResult = await _payOS.createPaymentLink(paymentData);

                // Record the pending payment (lưu mã dạng chuỗi vào database)
                var payment = new Payment
                {
                    UserId = userId,
                    SubscriptionTypeId = package.SubscriptionTypeId,
                    TransactionCode = orderCodeStr, // Lưu mã dạng chuỗi
                    Amount = package.Price,
                    Status = PaymentStatus.Pending,
                    PaymentResponse = $"{{\"checkoutUrl\":\"{paymentResult.checkoutUrl}\",\"qrCode\":\"{paymentResult.qrCode}\",\"numericOrderCode\":{numericOrderCode}}}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // Return the checkout URL and QR code
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
                _logger.LogError(ex, $"Error creating payment: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, "Error processing payment request");
            }
        }


        // New endpoint to handle success redirect from PayOS
        [HttpGet("success")]
        public async Task<IActionResult> PaymentSuccess([FromQuery] string orderCode)
        {
            try
            {
                if (string.IsNullOrEmpty(orderCode))
                {
                    return BadRequest("Order code is required");
                }

                _logger.LogInformation($"Payment success redirect received for order: {orderCode}");

                // Find payment in database
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.TransactionCode == orderCode);

                if (payment == null)
                {
                    _logger.LogWarning($"Payment not found for order: {orderCode}");
                    return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment/error?message=payment-not-found");
                }

                // Verify payment status with PayOS
                try
                {
                    // Extract numeric part from order code
                    string numericPart = orderCode.Replace("SUB-", "");
                    if (!long.TryParse(numericPart, out long numericOrderCode))
                    {
                        _logger.LogWarning($"Invalid order code format: {orderCode}");
                        return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment/error?message=invalid-order-code");
                    }

                    // Get payment info from PayOS API
                    PaymentLinkInformation paymentInfo = await _payOS.getPaymentLinkInformation(numericOrderCode);

                    if (paymentInfo.status == "PAID")
                    {
                        if (payment.Status != PaymentStatus.Completed)
                        {
                            // Process the payment if not already completed
                            await ProcessSuccessfulPayment(payment);
                        }

                        // Redirect to the success page with order details
                        return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment/success?orderCode={orderCode}");
                    }
                    else
                    {
                        _logger.LogWarning($"Payment is not paid. Status: {paymentInfo.status}, Order: {orderCode}");
                        return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment/error?message=payment-not-paid&status={paymentInfo.status}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error checking payment status with PayOS: {ex.Message}");

                    // If PayOS API fails, check our own payment status
                    if (payment.Status == PaymentStatus.Completed)
                    {
                        return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment/success?orderCode={orderCode}");
                    }
                    else
                    {
                        return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment/error?message=status-check-failed");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing payment success: {ex.Message}");
                return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment/error?message=processing-error");
            }
        }

        /*[HttpPost("webhook")]
        public async Task<IActionResult> PayOsWebhook([FromBody] WebhookType webhookData)
        {
            try
            {
                // Verify webhook data with PayOS
                WebhookData data = _payOS.verifyPaymentWebhookData(webhookData);

                if (data == null)
                {
                    _logger.LogWarning("Invalid webhook data");
                    return Unauthorized("Invalid webhook data");
                }

                // Convert numeric order code to string for database lookup
                string orderCodeStr = $"SUB-{data.orderCode}";

                // Find the payment by order code/transaction code
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.TransactionCode == orderCodeStr);

                if (payment == null)
                {
                    _logger.LogWarning($"Payment not found for order: {orderCodeStr}");
                    return NotFound("Payment not found");
                }

                if (payment.Status == PaymentStatus.Completed)
                {
                    // Payment already processed
                    return Ok(new { message = "Payment already processed" });
                }

                // Process the payment - update status and create/update subscription
                await ProcessSuccessfulPayment(payment);

                return Ok(new { message = "Webhook processed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing webhook: {ex.Message}");
                return StatusCode(500, "Error processing webhook");
            }
        }

        [Authorize]
        [HttpGet("payment-status/{orderCode}")]
        public async Task<IActionResult> CheckPaymentStatus(string orderCode)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID");

            var payment = await _context.Payments
                .Where(p => p.TransactionCode == orderCode && p.UserId == userId)
                .FirstOrDefaultAsync();

            if (payment == null)
                return NotFound("Payment not found");

            try
            {
                // Extract numeric part from order code
                string numericPart = orderCode.Replace("SUB-", "");
                if (!long.TryParse(numericPart, out long numericOrderCode))
                {
                    return BadRequest("Invalid order code format");
                }

                // Check payment status from PayOS API
                PaymentLinkInformation paymentInfo = await _payOS.getPaymentLinkInformation(numericOrderCode);

                // Update payment status if needed
                if (paymentInfo.status == "PAID" && payment.Status != PaymentStatus.Completed)
                {
                    // Process the payment
                    await ProcessSuccessfulPayment(payment);
                }

                return Ok(new
                {
                    Success = true,
                    OrderCode = orderCode,
                    Status = payment.Status.ToString(),
                    Amount = payment.Amount,
                    PayOsStatus = paymentInfo.status,
                    CreatedAt = payment.CreatedAt,
                    UpdatedAt = payment.UpdatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking payment status: {ex.Message}");
                return StatusCode(500, "Error checking payment status");
            }
        }*/
        // Only updating the relevant methods in PaymentController.cs

        [HttpPost("webhook")]
        public async Task<IActionResult> PayOsWebhook([FromBody] WebhookType webhookData)
        {
            try
            {
                // Verify webhook data with PayOS
                WebhookData data = _payOS.verifyPaymentWebhookData(webhookData);

                if (data == null)
                {
                    _logger.LogWarning("Invalid webhook data");
                    return Unauthorized("Invalid webhook data");
                }

                // Chuyển đổi orderCode từ số sang chuỗi để tìm trong database
                string orderCodeStr = $"SUB-{data.orderCode}";

                // Find the payment by order code/transaction code
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.TransactionCode == orderCodeStr); // So sánh với chuỗi

                if (payment == null)
                {
                    _logger.LogWarning($"Payment not found for order: {orderCodeStr}");
                    return NotFound("Payment not found");
                }

                if (payment.Status == PaymentStatus.Completed)
                {
                    // Payment already processed
                    return Ok(new { message = "Payment already processed" });
                }

                // Update payment status
                payment.Status = PaymentStatus.Completed;
                payment.UpdatedAt = DateTime.UtcNow;

                // Get subscription package
                var subscriptionType = await _context.SubscriptionTypes
                    .FindAsync(payment.SubscriptionTypeId);

                if (subscriptionType == null)
                {
                    _logger.LogError($"Subscription type not found: {payment.SubscriptionTypeId}");
                    return StatusCode(500, "Subscription type not found");
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
                    existingSubscription.EndDate = DateTime.UtcNow.AddYears(10); // Far future date
                }
                else
                {
                    // Create new subscription with no practical expiration
                    var subscription = new CandidateSubscription
                    {
                        UserId = payment.UserId,
                        SubscriptionTypeId = payment.SubscriptionTypeId,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddYears(10), // Far future date
                        IsActive = true,
                        RemainingTryMatches = subscriptionType.TryMatchLimit,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.CandidateSubscriptions.Add(subscription);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Payment processed successfully for order: {orderCodeStr}");

                return Ok(new { message = "Webhook processed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing webhook: {ex.Message}");
                return StatusCode(500, "Error processing webhook");
            }
        }

        [Authorize]
        [HttpGet("payment-status/{orderCode}")]
        public async Task<IActionResult> CheckPaymentStatus(string orderCode)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID");

            var payment = await _context.Payments
                .Where(p => p.TransactionCode == orderCode && p.UserId == userId)
                .FirstOrDefaultAsync();

            if (payment == null)
                return NotFound("Payment not found");

            try
            {
                // Trích xuất số từ chuỗi orderCode
                string numericPart = orderCode.Replace("SUB-", "");
                if (!long.TryParse(numericPart, out long numericOrderCode))
                {
                    return BadRequest("Invalid order code format");
                }

                // Check payment status from PayOS API sử dụng mã số
                PaymentLinkInformation paymentInfo = await _payOS.getPaymentLinkInformation(numericOrderCode);

                // Update payment status if needed
                if (paymentInfo.status == "PAID" && payment.Status != PaymentStatus.Completed)
                {
                    payment.Status = PaymentStatus.Completed;
                    payment.UpdatedAt = DateTime.UtcNow;

                    // Get subscription package
                    var subscriptionType = await _context.SubscriptionTypes
                        .FindAsync(payment.SubscriptionTypeId);

                    if (subscriptionType != null)
                    {
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

                            // Set EndDate to a far future date to ensure it doesn't expire
                            existingSubscription.EndDate = DateTime.UtcNow.AddYears(10);
                        }
                        else
                        {
                            // Create new subscription with no practical expiration
                            var subscription = new CandidateSubscription
                            {
                                UserId = payment.UserId,
                                SubscriptionTypeId = payment.SubscriptionTypeId,
                                StartDate = DateTime.UtcNow,
                                EndDate = DateTime.UtcNow.AddYears(10), // Far future date
                                IsActive = true,
                                RemainingTryMatches = subscriptionType.TryMatchLimit,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };

                            _context.CandidateSubscriptions.Add(subscription);
                        }

                        await _context.SaveChangesAsync();
                    }
                }

                return Ok(new
                {
                    Success = true,
                    OrderCode = orderCode,
                    Status = payment.Status.ToString(),
                    Amount = payment.Amount,
                    PayOsStatus = paymentInfo.status,
                    CreatedAt = payment.CreatedAt,
                    UpdatedAt = payment.UpdatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking payment status: {ex.Message}");
                return StatusCode(500, "Error checking payment status");
            }
        }

        // Helper method to process successful payments - used by multiple endpoints
        private async Task ProcessSuccessfulPayment(Payment payment)
        {
            _logger.LogInformation($"Processing successful payment for order: {payment.TransactionCode}");

            // Update payment status
            payment.Status = PaymentStatus.Completed;
            payment.UpdatedAt = DateTime.UtcNow;

            // Get subscription package
            var subscriptionType = await _context.SubscriptionTypes
                .FindAsync(payment.SubscriptionTypeId);

            if (subscriptionType == null)
            {
                _logger.LogError($"Subscription type not found: {payment.SubscriptionTypeId}");
                throw new Exception($"Subscription type not found: {payment.SubscriptionTypeId}");
            }

            // Check if user has an active subscription
            var existingSubscription = await _context.CandidateSubscriptions
                .Where(s => s.UserId == payment.UserId && s.IsActive && s.EndDate > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (existingSubscription != null)
            {
                // Extend existing subscription
                existingSubscription.EndDate = existingSubscription.EndDate.AddDays(subscriptionType.DurationInDays);
                existingSubscription.RemainingTryMatches += subscriptionType.TryMatchLimit;
                existingSubscription.UpdatedAt = DateTime.UtcNow;
                _logger.LogInformation($"Extended subscription for user {payment.UserId} until {existingSubscription.EndDate}");
            }
            else
            {
                // Create new subscription
                var subscription = new CandidateSubscription
                {
                    UserId = payment.UserId,
                    SubscriptionTypeId = payment.SubscriptionTypeId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(subscriptionType.DurationInDays),
                    IsActive = true,
                    RemainingTryMatches = subscriptionType.TryMatchLimit,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.CandidateSubscriptions.Add(subscription);
                _logger.LogInformation($"Created new subscription for user {payment.UserId} until {subscription.EndDate}");
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Payment processed successfully for order: {payment.TransactionCode}");
        }

        [HttpGet("cancel")]
        public IActionResult Cancel()
        {
            // Redirect user to the cancelled payment page
            return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-cancelled");
        }

        [HttpGet("cancel/{orderCode}")]
        public async Task<IActionResult> CancelWithOrderCode(string orderCode)
        {
            try
            {
                // Verify if order exists
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.TransactionCode == orderCode);

                if (payment == null)
                {
                    _logger.LogWarning($"Payment not found for order: {orderCode}");
                    return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-cancelled?error=payment-not-found");
                }

                // Only update status if payment is still pending
                if (payment.Status == PaymentStatus.Pending)
                {
                    // Extract numeric part from order code
                    string numericPart = orderCode.Replace("SUB-", "");
                    if (long.TryParse(numericPart, out long numericOrderCode))
                    {
                        try
                        {
                            // Call PayOS API to cancel payment
                            await _payOS.cancelPaymentLink(numericOrderCode);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error cancelling payment with PayOS: {ex.Message}");
                            // Continue updating payment status even if PayOS API fails
                        }
                    }

                    // Update payment status in database
                    payment.Status = PaymentStatus.Cancelled;
                    payment.UpdatedAt = DateTime.UtcNow;
                    payment.PaymentResponse += $"\nCancelled at {DateTime.UtcNow}";

                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Payment cancelled successfully for order: {orderCode}");
                }
                else
                {
                    _logger.LogWarning($"Cannot cancel payment with status: {payment.Status} for order: {orderCode}");
                }

                // Redirect to the cancelled payment page
                return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-cancelled?orderCode={orderCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing payment cancellation: {ex.Message}");
                return Redirect($"{_configuration["AppSettings:BaseUrl"]}/payment-cancelled?error=processing-error&orderCode={orderCode}");
            }
        }
    }

    // Request and response models
    public class CreatePaymentRequest
    {
        public int SubscriptionTypeId { get; set; }
    }

    public class ConfirmWebhookRequest
    {
        public string WebhookUrl { get; set; }
    }
}