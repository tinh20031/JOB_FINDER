using System;
using System.Text.Json;
using System.Threading.Tasks;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models.Subscription;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace JOB_FINDER_API.Services
{
    public class PaymentService
    {
        private readonly JobFinderDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PayOSService _payOSService;
        private readonly ILogger<PaymentService> _logger;
        public PaymentService(
            JobFinderDbContext context,
            IConfiguration configuration,
            PayOSService payOSService,
            ILogger<PaymentService> logger)
        {
            _context = context;
            _configuration = configuration;
            _payOSService = payOSService;
            _logger = logger;
        }

        public async Task<PaymentResult> ProcessPayment(SubscriptionTransaction transaction)
        {
            _logger.LogInformation($"Processing payment for transaction {transaction.TransactionId}, method: {transaction.PaymentMethod}");

            // Get user info
            var user = await _context.Users.FindAsync(transaction.UserId);
            if (user == null)
            {
                _logger.LogWarning($"User not found for transaction {transaction.TransactionId}");
                return new PaymentResult
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy thông tin người dùng"
                };
            }

            // Get plan info
            var plan = await _context.SubscriptionPlans.FindAsync(transaction.PlanId);
            if (plan == null)
            {
                _logger.LogWarning($"Plan not found for transaction {transaction.TransactionId}");
                return new PaymentResult
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy gói đăng ký"
                };
            }

            switch (transaction.PaymentMethod)
            {
                case PaymentMethod.PayOS:
                    _logger.LogInformation($"Creating PayOS payment link for transaction {transaction.TransactionId}");
                    // Create payment link with PayOS
                    var payOSResponse = await _payOSService.CreatePaymentLink(
                        transaction.TransactionId,
                        transaction.Amount,
                        $"Thanh toán gói {plan.Name}",
                        user.FullName ?? "Người dùng",
                        user.Email ?? "unknown@example.com",
                        user.Phone ?? ""
                    );

                    _logger.LogInformation($"PayOS response for transaction {transaction.TransactionId}: {JsonSerializer.Serialize(payOSResponse)}");

                    if (payOSResponse != null && payOSResponse.Data != null)
                    {
                        // Update transaction with PayOS payment ID
                        transaction.PaymentReference = payOSResponse.Data.PaymentLinkId;
                        transaction.UpdatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();

                        _logger.LogInformation($"Generated PayOS payment link: {payOSResponse.Data.CheckoutUrl} for transaction {transaction.TransactionId}");

                        return new PaymentResult
                        {
                            IsSuccess = false, // Needs additional processing (redirection)
                            RedirectUrl = payOSResponse.Data.CheckoutUrl,
                            Message = "Chuyển hướng đến trang thanh toán PayOS"
                        };
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to create PayOS payment link for transaction {transaction.TransactionId}");
                        return new PaymentResult
                        {
                            IsSuccess = false,
                            Message = payOSResponse?.Description ?? "Không thể tạo link thanh toán PayOS"
                        };
                    }

                // Other payment methods remain unchanged
                case PaymentMethod.BankTransfer:
                    // For bank transfers, mark as pending and show instructions
                    transaction.Status = PaymentStatus.Pending;
                    transaction.PaymentReference = $"BT-{DateTime.UtcNow:yyyyMMddHHmmss}";
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Created bank transfer transaction {transaction.TransactionId}");

                    return new PaymentResult
                    {
                        IsSuccess = false,
                        RedirectUrl = $"/payment/bank-transfer?transactionId={transaction.TransactionId}"
                    };

                default:
                    // For demo: automatically approve payments for other methods
                    transaction.Status = PaymentStatus.Completed;
                    transaction.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Automatically approved transaction {transaction.TransactionId} for payment method {transaction.PaymentMethod}");

                    return new PaymentResult
                    {
                        IsSuccess = true,
                        RedirectUrl = $"/payment/success?transactionId={transaction.TransactionId}"
                    };
            }
        }

        public async Task<PaymentResult> VerifyPayment(string paymentId, string status)
        {
            var transaction = await _context.SubscriptionTransactions
                .FirstOrDefaultAsync(t => t.TransactionId == paymentId);

            if (transaction == null)
                return new PaymentResult { IsSuccess = false, Message = "Không tìm thấy giao dịch" };

            // Với thanh toán PayOS, xác minh qua API của họ
            if (transaction.PaymentMethod == PaymentMethod.PayOS)
            {
                bool isVerified = await _payOSService.VerifyPayment(transaction.TransactionId);

                if (isVerified)
                {
                    transaction.Status = PaymentStatus.Completed;
                    transaction.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    return new PaymentResult { IsSuccess = true };
                }
                else
                {
                    // Nếu trạng thái chỉ rõ là thất bại, cập nhật tương ứng
                    if (status.ToLower() == "failed" || status.ToLower() == "cancelled")
                    {
                        transaction.Status = PaymentStatus.Failed;
                        transaction.UpdatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }

                    return new PaymentResult { IsSuccess = false };
                }
            }

            // Với các phương thức thanh toán khác
            if (status.ToLower() == "success" || status.ToLower() == "completed")
            {
                transaction.Status = PaymentStatus.Completed;
                transaction.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new PaymentResult { IsSuccess = true };
            }
            else
            {
                transaction.Status = PaymentStatus.Failed;
                transaction.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new PaymentResult { IsSuccess = false };
            }
        }

        public async Task<PaymentResult> HandlePayOSWebhook(string orderCode)
        {
            var transaction = await _context.SubscriptionTransactions
                .FirstOrDefaultAsync(t => t.TransactionId == orderCode);

            if (transaction == null)
                return new PaymentResult { IsSuccess = false, Message = "Không tìm thấy giao dịch" };

            // Xác minh thanh toán với PayOS
            bool isVerified = await _payOSService.VerifyPayment(orderCode);

            if (isVerified)
            {
                transaction.Status = PaymentStatus.Completed;
                transaction.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new PaymentResult { IsSuccess = true };
            }

            return new PaymentResult { IsSuccess = false };
        }

        public async Task<SubscriptionTransaction> GetTransaction(string transactionId)
        {
            return await _context.SubscriptionTransactions
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }

        public async Task<SubscriptionTransaction> CreateTransaction(int userId, int planId, PaymentMethod paymentMethod)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(planId);
            if (plan == null)
                throw new Exception("Gói đăng ký không hợp lệ");

            var transaction = new SubscriptionTransaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                UserId = userId,
                PlanId = planId,
                Amount = plan.Price,
                Status = PaymentStatus.Pending,
                PaymentMethod = paymentMethod,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.SubscriptionTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }
    }

    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
        public string RedirectUrl { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}