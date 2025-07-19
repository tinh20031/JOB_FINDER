using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace JOB_FINDER_API.Models.Subscription
{
    public enum PaymentStatus
    {
        Pending = 0,
        Completed = 1,
        Failed = 2,
        Refunded = 3,
        Cancelled = 4
    }

    public enum PaymentMethod
    {
        CreditCard = 0,
        MoMo = 1,
        BankTransfer = 2,
        PayPal = 3,
        PayOS = 4,
        Other = 5
    }

    public class SubscriptionTransaction
    {
        [Key]
        public string TransactionId { get; set; } = Guid.NewGuid().ToString();
        public int UserId { get; set; }
        public int? PlanId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public PaymentMethod PaymentMethod { get; set; }
        public string PaymentReference { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public User User { get; set; } = null!;

        [JsonIgnore]
        public SubscriptionPlan? Plan { get; set; }
    }
}