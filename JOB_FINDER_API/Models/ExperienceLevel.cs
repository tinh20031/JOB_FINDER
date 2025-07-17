using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class ExperienceLevel
    {
        public int ExperienceLevelid { get; set; }
        public string name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
