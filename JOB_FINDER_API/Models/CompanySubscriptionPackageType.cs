using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public enum CompanySubscriptionPackageType
    {
        Free = 0,
        Basic = 1,
        Premium = 2
    }

    public class CompanySubscriptionType
    {
        [Key]
        public int CompanySubscriptionTypeId { get; set; }

        [Required]
        public CompanySubscriptionPackageType PackageType { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public int JobPostLimit { get; set; } // Number of jobs that can be posted

        public int CvMatchLimit { get; set; } // Number of CVs that can be viewed in matching

        public int DurationInDays { get; set; } // Subscription duration in days

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public ICollection<CompanySubscription> CompanySubscriptions { get; set; } = new List<CompanySubscription>();
    }
}