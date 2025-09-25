using System.ComponentModel.DataAnnotations.Schema;
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


        public string? VideoUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string? AboutMeDescription { get; set; }


        [JsonIgnore]
        public User? User { get; set; }

    
        [Column(TypeName = "nvarchar(max)")] 
        public string? SkillsJson { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? EducationsJson { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? WorkExperiencesJson { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? AwardsJson { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? HighlightProjectsJson { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? CertificatesJson { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? ForeignLanguagesJson { get; set; }

     
        [NotMapped]
        public List<SkillInfo> Skills
        {
            get => string.IsNullOrEmpty(SkillsJson)
                ? new List<SkillInfo>()
                : System.Text.Json.JsonSerializer.Deserialize<List<SkillInfo>>(SkillsJson);
            set => SkillsJson = System.Text.Json.JsonSerializer.Serialize(value);
        }

        [NotMapped]
        public List<EducationInfo> Educations
        {
            get => string.IsNullOrEmpty(EducationsJson)
                ? new List<EducationInfo>()
                : System.Text.Json.JsonSerializer.Deserialize<List<EducationInfo>>(EducationsJson);
            set => EducationsJson = System.Text.Json.JsonSerializer.Serialize(value);
        }

        [NotMapped]
        public List<WorkExperienceInfo> WorkExperiences
        {
            get => string.IsNullOrEmpty(WorkExperiencesJson)
                ? new List<WorkExperienceInfo>()
                : System.Text.Json.JsonSerializer.Deserialize<List<WorkExperienceInfo>>(WorkExperiencesJson);
            set => WorkExperiencesJson = System.Text.Json.JsonSerializer.Serialize(value);
        }

        [NotMapped]
        public List<AwardInfo> Awards
        {
            get => string.IsNullOrEmpty(AwardsJson)
                ? new List<AwardInfo>()
                : System.Text.Json.JsonSerializer.Deserialize<List<AwardInfo>>(AwardsJson);
            set => AwardsJson = System.Text.Json.JsonSerializer.Serialize(value);
        }

        [NotMapped]
        public List<HighlightProjectInfo> HighlightProjects
        {
            get => string.IsNullOrEmpty(HighlightProjectsJson)
                ? new List<HighlightProjectInfo>()
                : System.Text.Json.JsonSerializer.Deserialize<List<HighlightProjectInfo>>(HighlightProjectsJson);
            set => HighlightProjectsJson = System.Text.Json.JsonSerializer.Serialize(value);
        }

        [NotMapped]
        public List<CertificateInfo> Certificates
        {
            get => string.IsNullOrEmpty(CertificatesJson)
                ? new List<CertificateInfo>()
                : System.Text.Json.JsonSerializer.Deserialize<List<CertificateInfo>>(CertificatesJson);
            set => CertificatesJson = System.Text.Json.JsonSerializer.Serialize(value);
        }

        [NotMapped]
        public List<ForeignLanguageInfo> ForeignLanguages
        {
            get => string.IsNullOrEmpty(ForeignLanguagesJson)
                ? new List<ForeignLanguageInfo>()
                : System.Text.Json.JsonSerializer.Deserialize<List<ForeignLanguageInfo>>(ForeignLanguagesJson);
            set => ForeignLanguagesJson = System.Text.Json.JsonSerializer.Serialize(value);
        }
    }

    public class SkillInfo
    {
        public int Id { get; set; }
        public string? GroupName { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string? Experience { get; set; }
        public SkillType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class EducationInfo
    {
        public int Id { get; set; }
        public string? School { get; set; }
        public string? Degree { get; set; }
        public string? Major { get; set; }
        public bool? IsStudying { get; set; }
        public DateTime? MonthStart { get; set; }
        public DateTime? YearStart { get; set; }
        public DateTime? MonthEnd { get; set; }
        public DateTime? YearEnd { get; set; }
        public string? Detail { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class WorkExperienceInfo
    {
        public int Id { get; set; }
        public string? JobTitle { get; set; }
        public string? CompanyName { get; set; }
        public bool IsWorking { get; set; }
        public DateTime? MonthStart { get; set; }
        public DateTime? YearStart { get; set; }
        public DateTime? MonthEnd { get; set; }
        public DateTime? YearEnd { get; set; }
        public string? WorkDescription { get; set; }
        public string? Responsibilities { get; set; }
        public string? Achievements { get; set; }
        public string? Technologies { get; set; }
        public string? ProjectName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class AwardInfo
    {
        public int Id { get; set; }
        public string? AwardName { get; set; }
        public string? AwardOrganization { get; set; }
        public DateTime? Month { get; set; }
        public DateTime? Year { get; set; }
        public string? AwardDescription { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class HighlightProjectInfo
    {
        public int Id { get; set; }
        public string? ProjectName { get; set; }
        public bool IsWorking { get; set; }
        public DateTime? MonthStart { get; set; }
        public DateTime? YearStart { get; set; }
        public DateTime? MonthEnd { get; set; }
        public DateTime? YearEnd { get; set; }
        public string? ProjectDescription { get; set; }
        public string? Technologies { get; set; }
        public string? Responsibilities { get; set; }
        public string? TeamSize { get; set; }
        public string? Achievements { get; set; }
        public string? ProjectLink { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CertificateInfo
    {
        public int Id { get; set; }
        public string CertificateName { get; set; } = string.Empty;
        public string? Organization { get; set; }
        public DateTime? Month { get; set; }
        public DateTime? Year { get; set; }
        public string? CertificateUrl { get; set; }
        public string? CertificateDescription { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class ForeignLanguageInfo
    {
        public int Id { get; set; }
        public string? LanguageName { get; set; }
        public string? LanguageLevel { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum SkillType
    {
        Core,
        Soft
    }
}