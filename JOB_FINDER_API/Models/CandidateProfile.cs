using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class CandidateProfile
    {
        public int UserId { get; set; } // Khóa chính và khóa ngoại tham chiếu đến User.Id
        public string Gender { get; set; } = string.Empty;
        public DateTime? Dob { get; set; }
        public string Address { get; set; } = string.Empty;
       
        public string? Language { get; set; }

        [JsonIgnore]
        public User User { get; set; } = null!;
    }
}