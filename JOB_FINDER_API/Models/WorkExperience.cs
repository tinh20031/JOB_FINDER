using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class WorkExperience
    {
        public int WorkExperienceId { get; set; }
     
        public int CandidateProfileId { get; set; }
        public string? JobTitle { get; set; } = string.Empty;
        public string? CompanyName { get; set; } = string.Empty;
        public bool IsWorking { get; set; } = false;
        public DateTime? MonthStart { get; set; }   
        public DateTime? YearStart { get; set; }    
        public DateTime? MonthEnd { get; set; } 
        public DateTime? YearEnd { get; set; }  
        public string? WorkDescription { get; set; } = string.Empty;
        public string? Responsibilities { get; set; } = string.Empty;
        public string? Achievements { get; set; } = string.Empty;
        public string? Technologies { get; set; } = string.Empty;
        public string? ProjectName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public CandidateProfile? CandidateProfile { get; set; }
       
    }
}
