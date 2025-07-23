using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public enum PaymentStatus
    {
        Pending = 1,
        Completed = 2,
        Failed = 3,
        Refunded = 4,
        Cancelled = 5
    }

    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        public int UserId { get; set; }

        public int SubscriptionTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string TransactionCode { get; set; }

        [Required]
        public string PaymentProvider { get; set; } = "PayOS";

        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; }

        public string? PaymentResponse { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public User User { get; set; }

        [JsonIgnore]
        public SubscriptionType SubscriptionType { get; set; }
    }
}