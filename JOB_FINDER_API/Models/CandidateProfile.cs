using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class CandidateProfile
    {
        public int CandidateProfileId { get; set; } 
        public int UserId { get; set; } 
        public string? Gender { get; set; } = string.Empty;
        public DateTime? Dob { get; set; }
        public string? JobTitle { get; set; }
       
        public string? Address { get; set; } = string.Empty;
        public string? Province { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
         
        public string? PersonalLink { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        [JsonIgnore]
        public User? User { get; set; }

        [JsonIgnore]
        public List<Skill> Skills { get; set; } = new List<Skill>(); // List<Skill>
        [JsonIgnore]
      
        public List<Education> Educations { get; set; } = new List<Education>();
        [JsonIgnore]
        public List<AboutMe> AboutMes { get; set; } = new List<AboutMe>();
        [JsonIgnore]
        public List<WorkExperience> WorkExperiences { get; set; } = new List<WorkExperience>();
        [JsonIgnore]
       
        public List<Award> Awards { get; set; } = new List<Award>(); // List<Award>
        [JsonIgnore]
        public List<HighlightProject> HighlightProjects { get; set; } = new List<HighlightProject>();

        [JsonIgnore]
        public List<Certificate> Certificates { get; set; } = new List<Certificate>(); 
        [JsonIgnore]
        public List<ForeignLanguage> ForeginLanguages { get; set; } = new List<ForeignLanguage>(); // List<ForeginLanguage>
    }
}