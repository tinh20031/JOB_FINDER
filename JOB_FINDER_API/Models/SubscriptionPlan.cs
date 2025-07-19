using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace JOB_FINDER_API.Models.Subscription
{
    public enum SubscriptionType
    {
        Free = 0,
        Basic = 1,
        Advanced = 2
    }

    public class SubscriptionPlan
    {
        [Key]
        public int PlanId { get; set; }
        public string Name { get; set; } = string.Empty;
        public SubscriptionType Type { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; } = 30; // Default 30 days
        public int TryMatchingLimit { get; set; }
        public int CVDownloadLimit { get; set; } // -1 means unlimited
        public bool AllowWatermarkRemoval { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public ICollection<CandidateSubscription> Subscriptions { get; set; } = new List<CandidateSubscription>();
    }
}