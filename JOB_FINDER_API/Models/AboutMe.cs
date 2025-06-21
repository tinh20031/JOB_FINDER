using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class AboutMe
    {
        public int AboutMeId { get; set; }
    
        public int CandidateProfileId { get; set; }
        public string? AboutMeDescription { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public CandidateProfile? CandidateProfile { get; set; }
    }

}
