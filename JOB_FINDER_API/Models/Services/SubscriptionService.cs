using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models.Subscription;
using Microsoft.EntityFrameworkCore;

namespace JOB_FINDER_API.Services
{
    public class SubscriptionService
    {
        private readonly JobFinderDbContext _context;

        public SubscriptionService(JobFinderDbContext context)
        {
            _context = context;
        }

        public async Task<CandidateSubscription?> GetActiveSubscription(int userId)
        {
            return await _context.CandidateSubscriptions
                .Include(s => s.Plan)
                .Where(s => s.UserId == userId && s.IsActive && s.EndDate > DateTime.UtcNow)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasActiveSubscription(int userId)
        {
            var subscription = await GetActiveSubscription(userId);
            return subscription != null;
        }

        public async Task<SubscriptionPlan?> GetPlanById(int planId)
        {
            return await _context.SubscriptionPlans
                .FirstOrDefaultAsync(p => p.PlanId == planId && p.IsActive);
        }

        public async Task<List<SubscriptionPlan>> GetAllActivePlans()
        {
            return await _context.SubscriptionPlans
                .Where(p => p.IsActive)
                .OrderBy(p => p.Price)
                .ToListAsync();
        }

        public async Task<CandidateSubscription?> ActivateFreeSubscription(int userId)
        {
            var freePlan = await _context.SubscriptionPlans
                .FirstOrDefaultAsync(p => p.Type == SubscriptionType.Free);

            if (freePlan == null)
                return null;

            // Kiểm tra xem người dùng đã có gói đăng ký hoạt động hay chưa
            var existingSubscription = await GetActiveSubscription(userId);
            if (existingSubscription != null)
                return existingSubscription;

            var subscription = new CandidateSubscription
            {
                UserId = userId,
                PlanId = freePlan.PlanId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.MaxValue, // Gói Free không hết hạn
                IsActive = true,
                TryMatchingUsed = 0,
                CVDownloaded = 0,
                TransactionId = "FREE-" + Guid.NewGuid().ToString(),
                AmountPaid = 0
            };

            _context.CandidateSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            return subscription;
        }

        public async Task<CandidateSubscription> ActivatePaidSubscription(int userId, int planId, string transactionId)
        {
            var plan = await GetPlanById(planId);
            if (plan == null)
                throw new Exception("Gói đăng ký không hợp lệ");

            var transaction = await _context.SubscriptionTransactions
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            if (transaction == null || transaction.Status != PaymentStatus.Completed)
                throw new Exception("Thanh toán chưa được hoàn thành");

            // Hủy kích hoạt các gói đăng ký hiện có
            var existingSubscriptions = await _context.CandidateSubscriptions
                .Where(s => s.UserId == userId && s.IsActive)
                .ToListAsync();

            foreach (var sub in existingSubscriptions)
            {
                sub.IsActive = false;
                sub.UpdatedAt = DateTime.UtcNow;
            }

            // Tạo gói đăng ký mới
            var subscription = new CandidateSubscription
            {
                UserId = userId,
                PlanId = planId,
                StartDate = DateTime.UtcNow,
                EndDate = plan.DurationDays > 0
                    ? DateTime.UtcNow.AddDays(plan.DurationDays)
                    : DateTime.MaxValue,
                IsActive = true,
                TryMatchingUsed = 0,
                CVDownloaded = 0,
                TransactionId = transactionId,
                AmountPaid = plan.Price
            };

            _context.CandidateSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            return subscription;
        }

        public async Task<bool> CanUseMatching(int userId)
        {
            var subscription = await GetActiveSubscription(userId);

            // Người dùng Free sẽ có giới hạn mặc định nếu không có gói đăng ký
            if (subscription == null)
            {
                var freePlan = await _context.SubscriptionPlans
                    .FirstOrDefaultAsync(p => p.Type == SubscriptionType.Free);

                return freePlan != null && (freePlan.TryMatchingLimit == -1 || 0 < freePlan.TryMatchingLimit);
            }

            return subscription.Plan.TryMatchingLimit == -1 ||
                   subscription.TryMatchingUsed < subscription.Plan.TryMatchingLimit;
        }

        public async Task<bool> CanDownloadCV(int userId)
        {
            var subscription = await GetActiveSubscription(userId);

            if (subscription == null)
            {
                var freePlan = await _context.SubscriptionPlans
                    .FirstOrDefaultAsync(p => p.Type == SubscriptionType.Free);

                return freePlan != null && (freePlan.CVDownloadLimit == -1 || 0 < freePlan.CVDownloadLimit);
            }

            return subscription.Plan.CVDownloadLimit == -1 ||
                   subscription.CVDownloaded < subscription.Plan.CVDownloadLimit;
        }

        public async Task<bool> CanRemoveWatermark(int userId)
        {
            var subscription = await GetActiveSubscription(userId);

            // Người dùng Free không thể xóa watermark
            if (subscription == null)
                return false;

            return subscription.Plan.AllowWatermarkRemoval;
        }

        public async Task<bool> IncrementMatchingUsed(int userId)
        {
            var subscription = await GetActiveSubscription(userId);
            if (subscription == null)
            {
                await ActivateFreeSubscription(userId);
                subscription = await GetActiveSubscription(userId);
            }

            if (subscription != null &&
                (subscription.Plan.TryMatchingLimit == -1 || subscription.TryMatchingUsed < subscription.Plan.TryMatchingLimit))
            {
                subscription.TryMatchingUsed++;
                subscription.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }


        public async Task<bool> IncrementCVDownloaded(int userId)
        {
            var subscription = await GetActiveSubscription(userId);
            if (subscription == null)
            {
                await ActivateFreeSubscription(userId);
                subscription = await GetActiveSubscription(userId);
            }

            if (subscription != null &&
                (subscription.Plan.CVDownloadLimit == -1 || subscription.CVDownloaded < subscription.Plan.CVDownloadLimit))
            {
                subscription.CVDownloaded++;
                subscription.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}