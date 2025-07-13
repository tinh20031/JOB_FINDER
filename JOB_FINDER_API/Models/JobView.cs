using System;
using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class JobView
    {
        public int JobViewId { get; set; }
        public int JobId { get; set; }
        public int? UserId { get; set; }  // Nullable for anonymous views
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public Job? Job { get; set; }

        [JsonIgnore]
        public User? User { get; set; }
    }
}