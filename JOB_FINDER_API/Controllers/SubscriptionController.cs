using System;
using System.Security.Claims;
using System.Threading.Tasks;
using JOB_FINDER_API.Models.Subscription;
using JOB_FINDER_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubscriptionController : ControllerBase
    {
        private readonly SubscriptionService _subscriptionService;
        private readonly PaymentService _paymentService;

        public SubscriptionController(
            SubscriptionService subscriptionService,
            PaymentService paymentService)
        {
            _subscriptionService = subscriptionService;
            _paymentService = paymentService;
        }

        [HttpGet("plans")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPlans()
        {
            var plans = await _subscriptionService.GetAllActivePlans();
            return Ok(plans);
        }

        [HttpGet("my-subscription")]
        public async Task<IActionResult> GetMySubscription()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var subscription = await _subscriptionService.GetActiveSubscription(userId);
            if (subscription == null)
            {
                // Tự động kích hoạt gói miễn phí nếu chưa có
                subscription = await _subscriptionService.ActivateFreeSubscription(userId);
            }

            return Ok(subscription);
        }

        [HttpGet("usage-limits")]
        public async Task<IActionResult> GetUsageLimits()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            var subscription = await _subscriptionService.GetActiveSubscription(userId);

            // Nếu không có gói đăng ký hoạt động, trả về giới hạn của gói miễn phí
            if (subscription == null)
            {
                return Ok(new
                {
                    CanUseMatching = await _subscriptionService.CanUseMatching(userId),
                    //CanUseCVFeedback = await _subscriptionService.CanUseCVFeedback(userId),
                    CanDownloadCV = await _subscriptionService.CanDownloadCV(userId),
                    CanRemoveWatermark = false,
                    PlanType = "Free",
                    RemainingDays = 0
                });
            }

            return Ok(new
            {
                CanUseMatching = subscription.Plan.TryMatchingLimit == -1 ||
                                 subscription.TryMatchingUsed < subscription.Plan.TryMatchingLimit,
                //CanUseCVFeedback = subscription.Plan.CVFeedbackLimit == -1 ||
                //                  subscription.CVFeedbackUsed < subscription.Plan.CVFeedbackLimit,
                CanDownloadCV = subscription.Plan.CVDownloadLimit == -1 ||
                               subscription.CVDownloaded < subscription.Plan.CVDownloadLimit,
                CanRemoveWatermark = subscription.Plan.AllowWatermarkRemoval,
                PlanType = subscription.Plan.Type.ToString(),
                RemainingDays = (int)Math.Ceiling((subscription.EndDate - DateTime.UtcNow).TotalDays),
                Usage = new
                {
                    MatchingUsed = subscription.TryMatchingUsed,
                    MatchingLimit = subscription.Plan.TryMatchingLimit,
                    //CVFeedbackLimit = subscription.Plan.CVFeedbackLimit,
                    CVDownloaded = subscription.CVDownloaded,
                    CVDownloadLimit = subscription.Plan.CVDownloadLimit
                }
            });
        }

        
        [HttpPost("purchase")]
        public async Task<IActionResult> PurchasePlan([FromBody] PurchasePlanRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);

            try
            {
                // Create payment transaction
                var transaction = await _paymentService.CreateTransaction(
                    userId, request.PlanId, request.PaymentMethod);

                // Process payment
                var paymentResult = await _paymentService.ProcessPayment(transaction);

                if (paymentResult.IsSuccess)
                {
                    // For successful payments, activate subscription immediately
                    var subscription = await _subscriptionService.ActivatePaidSubscription(
                        userId, request.PlanId, transaction.TransactionId);

                    return Ok(new
                    {
                        Success = true,
                        Message = "Gói đăng ký đã được kích hoạt thành công",
                        Subscription = subscription,
                        RedirectUrl = paymentResult.RedirectUrl
                    });
                }
                else
                {
                    // Payment needs further processing (redirect to payment gateway)
                    return Ok(new
                    {
                        Success = false,
                        Message = paymentResult.Message ?? "Cần xử lý thanh toán",
                        PaymentId = transaction.TransactionId,
                        RedirectUrl = paymentResult.RedirectUrl ?? ""
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }

    public class PurchasePlanRequest
    {
        public int PlanId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}