using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public enum SubscriptionPackageType
    {
        Free = 1,
        Basic = 2,
        Premium = 3
    }

    public class SubscriptionType
    {
        [Key]
        public int SubscriptionTypeId { get; set; }

        public SubscriptionPackageType PackageType { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public int TryMatchLimit { get; set; }

        public int DurationInDays { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public ICollection<CandidateSubscription> CandidateSubscriptions { get; set; } = new List<CandidateSubscription>();
    }
}