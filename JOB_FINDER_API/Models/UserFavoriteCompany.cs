using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class UserFavoriteCompany
    {  
        public int UserId { get; set; }
        public int CompanyProfileId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public User? User { get; set; }
        [JsonIgnore]
        public CompanyProfile? CompanyProfile { get; set; }
    }
}