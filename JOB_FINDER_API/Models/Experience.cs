using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class Experience
    {
        public int Id { get; set; }
        public string ExperienceName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public User User { get; set; } = null!;
        [JsonIgnore]
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}