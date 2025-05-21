using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class UserFavoriteJob
    {
        public int UserId { get; set; }
        public int JobId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public User User { get; set; } = null!;
        [JsonIgnore]
        public Job Job { get; set; } = null!;
    }
}