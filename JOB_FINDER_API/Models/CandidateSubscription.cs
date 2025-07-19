using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace JOB_FINDER_API.Models.Subscription
{
    public class CandidateSubscription
    {
        [Key]
        public int SubscriptionId { get; set; }
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public int TryMatchingUsed { get; set; } = 0;
        public int CVDownloaded { get; set; } = 0;
        public string TransactionId { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public User User { get; set; } = null!;

        [JsonIgnore]
        public SubscriptionPlan Plan { get; set; } = null!;

        public bool IsExpired()
        {
            return !IsActive || DateTime.UtcNow > EndDate;
        }
    }
}