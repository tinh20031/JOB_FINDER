using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class Award
    {
        public int AwardId { get; set; }
     
        public int CandidateProfileId { get; set; }
        public string? AwardName { get; set; } = string.Empty;
        public string? AwardOrganization { get; set; } = string.Empty;
        public DateTime? Month { get; set; }
        public DateTime? Year { get; set; }
        public string? AwardDescription { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public CandidateProfile? CandidateProfile { get; set; }
    }
}
