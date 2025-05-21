using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class Level
    {
        public int Id { get; set; }
        public string LevelName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}