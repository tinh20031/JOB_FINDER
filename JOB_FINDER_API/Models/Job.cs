using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class Job
    {
        public enum JobStatus
        {
            pending,
            active,
            inactive
        }
        public int JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Education { get; set; } = string.Empty;
        public string YourSkill { get; set; } = string.Empty;
        public string YourExperience { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public int? MinSalary { get; set; }
        public int? MaxSalary { get; set; }
        public bool IsSalaryNegotiable { get; set; }
        public int IndustryId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int LevelId { get; set; }
        public int JobTypeId { get; set; }
        public int ExperienceLevelId { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public JobStatus Status { get; set; } = JobStatus.pending;
        public string ProvinceName { get; set; } = string.Empty;
        public string AddressDetail { get; set; } = string.Empty;
        public bool DeactivatedByAdmin { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public User? Company { get; set; }
        [JsonIgnore]
        public Industry? Industry { get; set; }
        [JsonIgnore]
        public Level? Level { get; set; }
        [JsonIgnore]
        public JobType? JobType { get; set; }
        [JsonIgnore]
        public ExperienceLevel? ExperienceLevel { get; set; }
        [JsonIgnore]
        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
        [JsonIgnore]
        public ICollection<Application> Applications { get; set; } = new List<Application>();
        [JsonIgnore]
        public ICollection<UserFavoriteJob> FavoritedByUsers { get; set; } = new List<UserFavoriteJob>();

        public bool IsExpired()
        {
            return DateTime.UtcNow > TimeEnd;
        }

        
        public bool CanCompanyEditContent()
        {
            // Không cho phép edit nếu bị admin lock
            if (DeactivatedByAdmin)
                return false;

            // Được phép nếu job đang pending, active hoặc inactive (chưa hết hạn)
            return (Status == JobStatus.pending ||
                    Status == JobStatus.active ||
                    Status == JobStatus.inactive) && !IsExpired();
        }

        public bool CanCompanyChangeStatus(JobStatus newStatus)
        {
            if (IsExpired() || DeactivatedByAdmin)
                return false;

            // Chỉ được chuyển giữa active <-> inactive trong thời gian còn hiệu lực
            if ((Status == JobStatus.active && newStatus == JobStatus.inactive) ||
                (Status == JobStatus.inactive && newStatus == JobStatus.active))
                return true;

            return false;
        }

        public bool CanAdminChangeStatus(JobStatus newStatus)
        {
            // Admin được chuyển từ pending sang active/inactive hoặc unlock job bị lock
            if (Status == JobStatus.pending && (newStatus == JobStatus.active || newStatus == JobStatus.inactive))
                return true;

            if (DeactivatedByAdmin && Status == JobStatus.inactive && newStatus == JobStatus.active)
                return true;

            return false;
        }

        public void AutoExpireIfNeeded()
        {
            if (Status == JobStatus.active && IsExpired())
            {
                Status = JobStatus.inactive;
            }
        }
    }

}
