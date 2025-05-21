using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class Job
    {
        public int JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int EmployerId { get; set; } // Khóa ngoại tham chiếu đến Users.Id (Employer)
        public int Salary { get; set; }
        public int IndustryId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int LevelId { get; set; }
        public int JobTypeId { get; set; }
        public int ExperienceId { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ImageJob { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public User Employer { get; set; } = null!;
        [JsonIgnore]
        public Industry Industry { get; set; } = null!;
        [JsonIgnore]
        public Level Level { get; set; } = null!;
        [JsonIgnore]
        public JobType JobType { get; set; } = null!;
        [JsonIgnore]
        public Experience Experience { get; set; } = null!;
        [JsonIgnore]
        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
        [JsonIgnore]
        public ICollection<Application> Applications { get; set; } = new List<Application>();
        [JsonIgnore]
        public ICollection<UserFavoriteJob> FavoritedByUsers { get; set; } = new List<UserFavoriteJob>();
    }
}
>>>>>>> 6ad28f2efa29a5890e3b04abf9707bcf7b91cfca
