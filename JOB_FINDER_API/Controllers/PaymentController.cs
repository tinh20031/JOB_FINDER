using JOB_FINDER_API.Models.Payment;
using JOB_FINDER_API.Models.Subscription;
using JOB_FINDER_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly SubscriptionService _subscriptionService;
        private readonly PayOSService _payOSService;

        public PaymentController(
            PaymentService paymentService,
            SubscriptionService subscriptionService,
            PayOSService payOSService)
        {
            _paymentService = paymentService;
            _subscriptionService = subscriptionService;
            _payOSService = payOSService;
        }

        [HttpGet("payos-return")]
        [AllowAnonymous]
        public async Task<IActionResult> PayOSReturn([FromQuery] string orderCode)
        {
            try
            {
                // Xác minh thanh toán với PayOS
                var paymentResult = await _paymentService.VerifyPayment(orderCode, "");

                if (paymentResult.IsSuccess)
                {
                    // Lấy chi tiết giao dịch
                    var transaction = await _paymentService.GetTransaction(orderCode);

                    if (transaction != null)
                    {
                        // Kích hoạt gói đăng ký
                        await _subscriptionService.ActivatePaidSubscription(
                            transaction.UserId,
                            transaction.PlanId ?? 0,
                            transaction.TransactionId);

                        // Chuyển hướng đến trang thành công
                        return Redirect($"/payment-success?transactionId={transaction.TransactionId}");
                    }
                }

                // Chuyển hướng đến trang thất bại
                return Redirect("/payment-failed");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("payos-cancel")]
        [AllowAnonymous]
        public IActionResult PayOSCancel([FromQuery] string orderCode)
        {
            // Chuyển hướng đến trang hủy
            return Redirect($"/payment-cancelled?orderCode={orderCode}");
        }

        [HttpPost("payos-webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> PayOSWebhook([FromBody] PayOSWebhookRequest webhookData)
        {
            try
            {
                // Xác minh chữ ký webhook
                if (!_payOSService.VerifyWebhookSignature(webhookData))
                {
                    return BadRequest(new { success = false, message = "Chữ ký không hợp lệ" });
                }

                if (webhookData?.Data == null)
                {
                    return BadRequest(new { success = false, message = "Dữ liệu webhook không hợp lệ" });
                }

                // Xử lý thanh toán dựa trên trạng thái
                if (webhookData.Data.Status.ToUpper() == "PAID")
                {
                    var paymentResult = await _paymentService.HandlePayOSWebhook(webhookData.Data.OrderCode);

                    if (paymentResult.IsSuccess)
                    {
                        // Lấy chi tiết giao dịch
                        var transaction = await _paymentService.GetTransaction(webhookData.Data.OrderCode);

                        if (transaction != null)
                        {
                            // Kích hoạt gói đăng ký
                            await _subscriptionService.ActivatePaidSubscription(
                                transaction.UserId,
                                transaction.PlanId ?? 0,
                                transaction.TransactionId);

                            return Ok(new { success = true });
                        }
                    }
                }

                return Ok(new { success = true, message = "Đã nhận webhook" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi webhook PayOS: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Lỗi server" });
            }
        }

        [HttpGet("check-status/{transactionId}")]
        [Authorize]
        public async Task<IActionResult> CheckPaymentStatus(string transactionId)
        {
            try
            {
                var transaction = await _paymentService.GetTransaction(transactionId);
                if (transaction == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy giao dịch" });
                }

                if (transaction.Status == PaymentStatus.Completed)
                {
                    return Ok(new { success = true, status = "completed" });
                }
                else if (transaction.Status == PaymentStatus.Failed || transaction.Status == PaymentStatus.Cancelled)
                {
                    return Ok(new { success = false, status = "failed" });
                }
                else
                {
                    // Kiểm tra lại trạng thái với PayOS nếu là thanh toán PayOS
                    if (transaction.PaymentMethod == PaymentMethod.PayOS)
                    {
                        bool isVerified = await _payOSService.VerifyPayment(transactionId);
                        if (isVerified)
                        {
                            transaction.Status = PaymentStatus.Completed;
                            await _paymentService.VerifyPayment(transactionId, "completed");

                            // Kích hoạt gói đăng ký
                            await _subscriptionService.ActivatePaidSubscription(
                                transaction.UserId,
                                transaction.PlanId ?? 0,
                                transaction.TransactionId);

                            return Ok(new { success = true, status = "completed" });
                        }
                    }

                    return Ok(new { success = false, status = "pending" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}