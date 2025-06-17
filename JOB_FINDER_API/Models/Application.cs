using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public enum ApplicationStatus
    {
        Pending,
        Interview,
        Rejected,
        Accepted    
    }

    public class Application
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int JobId { get; set; }
        public string? ResumeUrl { get; set; }
        public string? CoverLetter { get; set; }
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public int CvId { get; set; }
        public string? SnapshotCv { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public User? User { get; set; }
        [JsonIgnore]
        public Job? Job { get; set; }
        [JsonIgnore]
        public CV? CV { get; set; }
    }
}