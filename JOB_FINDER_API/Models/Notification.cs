using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string? Link { get; set; }
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; } = false;
        public int TryMatchId { get; set; } 
        public string? Status { get; set; }
        public float? SimilarityScore { get; set; }
        public string? Suggestions { get; set; }
        public string? CvSummary { get; set; }
        public string? JobSummary { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public User? User { get; set; }
        public enum NotificationType
        {
            NewJob = 1,
            ApplicationStatus = 2,
            SystemNotification = 3,
            JobApproved = 4,
            JobRejected = 5,
            NewJobApplication = 6,
            CompanyFavorited = 7,      
            JobFavorited = 8,
                TryMatchUpdate = 9 
        }
    }
}