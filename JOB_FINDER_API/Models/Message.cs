using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string MessageText { get; set; } = null;
        public string? FileUrl { get; set; }
        public string? FileType { get; set; } 
        public string? FileName { get; set; } 
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public User Sender { get; set; } = null!;
        [JsonIgnore]
        public User Receiver { get; set; } = null!;
    }
}