using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class Certificate
    {
        public int CertificateId { get; set; }
       
        public int CandidateProfileId { get; set; }
        public string CertificateName { get; set; } = string.Empty;
        public string? Organization { get; set; } = string.Empty;
        public DateTime? Month { get; set; } 
        public DateTime? Year { get; set; } 
        public string? CertificateUrl { get; set; } = string.Empty;
        public string? CertificateDescription { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        [JsonIgnore]
        public CandidateProfile? CandidateProfile { get; set; }
    }
}
