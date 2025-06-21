using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class Education
    {
        public int EducationId { get; set; }
      
        public int CandidateProfileId { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? School { get; set; } = string.Empty;
        public string? Degree { get; set; } = string.Empty;
        public string? Major { get; set; } = string.Empty;
        public bool? IsStudying { get; set; } = false;
        public DateTime? MonthStart { get; set; }   
        public DateTime? YearStart { get; set; }    
        public DateTime? MonthEnd { get; set; } 
        public DateTime? YearEnd { get; set; }  
        public string? Detail { get; set; } = string.Empty;
       

      
        [JsonIgnore]
        public CandidateProfile? CandidateProfile { get; set; }
    }
}