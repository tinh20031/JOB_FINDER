using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class HighlightProject
    {
        public int HighlightProjectId { get; set; }
      
        public int CandidateProfileId { get; set; }
        public string? ProjectName { get; set; }= string.Empty;
        public bool IsWorking { get; set; } = false;
        public DateTime? MonthStart { get; set; }   
        public DateTime? YearStart { get; set; }    
        public DateTime? MonthEnd { get; set; } 
        public DateTime? YearEnd { get; set; }  
        public string? ProjectDescription { get; set; } = string.Empty;
        public string? Technologies { get; set; } = string.Empty;
        public string? Responsibilities { get; set; } = string.Empty;
        public string? TeamSize { get; set; } = string.Empty;
        public string? Achievements { get; set; } = string.Empty;
        public string? ProjectLink { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public CandidateProfile? CandidateProfile { get; set; }
    }
}
