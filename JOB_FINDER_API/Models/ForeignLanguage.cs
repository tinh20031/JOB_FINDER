using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class ForeignLanguage
    {
        public int ForeignLanguageId { get; set; }
     
        public int CandidateProfileId { get; set; }
        public string? LanguageName { get; set; } = string.Empty;
        public string? LanguageLevel { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public CandidateProfile? CandidateProfile { get; set; }
    }
}
