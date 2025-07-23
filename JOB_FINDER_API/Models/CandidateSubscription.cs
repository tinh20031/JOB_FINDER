using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class CandidateSubscription
    {
        [Key]
        public int CandidateSubscriptionId { get; set; }

        public int UserId { get; set; }

        public int SubscriptionTypeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public int RemainingTryMatches { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public User User { get; set; }

        [JsonIgnore]
        public SubscriptionType SubscriptionType { get; set; }
    }
}