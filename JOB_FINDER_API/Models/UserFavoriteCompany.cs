using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class UserFavoriteCompany
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public User? User { get; set; }
        [JsonIgnore]
        public User? Company { get; set; }
    }
}