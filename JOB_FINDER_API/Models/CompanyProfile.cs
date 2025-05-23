using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class CompanyProfile
    {
        public int UserId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyProfileDescription { get; set; }
        public string? Location { get; set; }
        public string? UrlCompanyLogo { get; set; }
        public string? ImageLogoLgr { get; set; }
        public string? TeamSize { get; set; }
        public bool IsVerified { get; set; } = false;
        public string? Website { get; set; }
        public string? Contact { get; set; }

        public int IndustryId { get; set; }
        [JsonIgnore]
        public Industry Industry { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        [JsonIgnore]
        public User? User { get; set; }
    }
}