using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class CV
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string? FullCvJson { get; set; } = string.Empty; // Đổi từ FULL_CV_JSON để thống nhất
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public User? User { get; set; }
        [JsonIgnore]
        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}