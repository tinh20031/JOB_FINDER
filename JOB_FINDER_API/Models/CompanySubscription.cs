using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class CompanySubscription
    {
        [Key]
        public int CompanySubscriptionId { get; set; }

        public int UserId { get; set; } // Company ID (which is a User with company role)

        public int CompanySubscriptionTypeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public int RemainingJobPosts { get; set; } // Remaining job posts allowed
        public int RemainingTrendingJobPosts { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual User Company { get; set; }

        [JsonIgnore]
        [ForeignKey("CompanySubscriptionTypeId")]
        public virtual CompanySubscriptionType SubscriptionType { get; set; }
    }
}
