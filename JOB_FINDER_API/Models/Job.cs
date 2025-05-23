using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class Job
    {
        public enum JobStatus
        {
            Submitted, // Đã gửi
            Posted,    // Đã đăng
            Rejected,  // Từ chối
            Closed     // Đã đóng
        }

        public int JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public int Salary { get; set; }
        public int IndustryId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int LevelId { get; set; }
        public int JobTypeId { get; set; }
        public int ExperienceLevelId { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public JobStatus Status { get; set; } = JobStatus.Submitted;
        public string? ImageJob { get; set; }
        public string ProvinceName { get; set; } = string.Empty; 
        public string AddressDetail { get; set; } = string.Empty; 

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
    }
}
