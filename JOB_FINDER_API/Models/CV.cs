using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class CV
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string? FullCvJson { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public User? User { get; set; }
        [JsonIgnore]
        public ICollection<Application> Applications { get; set; } = new List<Application>();

        public CVData GetCVData()
        {
            if (string.IsNullOrEmpty(FullCvJson)) return new CVData();
            return System.Text.Json.JsonSerializer.Deserialize<CVData>(FullCvJson) ?? new CVData();
        }
    }

    public class CVData
    {
        public string Description { get; set; } = string.Empty;
        public string Skills { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string Education { get; set; } = string.Empty;
    }
}