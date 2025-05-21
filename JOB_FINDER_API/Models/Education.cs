using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class Education
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string School { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string MonthStart { get; set; } = string.Empty;
        public string YearStart { get; set; } = string.Empty;
        public string MonthEnd { get; set; } = string.Empty;
        public string YearEnd { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; } = null!;
    }
}