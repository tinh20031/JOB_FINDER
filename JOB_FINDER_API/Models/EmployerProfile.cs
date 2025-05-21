using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class EmployerProfile
    {
        public int UserId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyProfile { get; set; }
        public string? Location { get; set; }
        public string? UrlCompanyLogo { get; set; }
        public string? ImageLogoLgr { get; set; }
        public string? TeamSize { get; set; }
        public bool IsVerified { get; set; } = false; // Thêm cờ xác minh
        public string? Website { get; set; }
        public string? Contact { get; set; }

        [JsonIgnore]
        public User User { get; set; } = null!;
    }
}