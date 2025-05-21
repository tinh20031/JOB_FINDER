using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class HRProfile
    {
        public int UserId { get; set; } // Khóa chính và khóa ngoại tham chiếu đến User.Id (HR)
        public int EmployerId { get; set; } // Khóa ngoại tham chiếu đến User.Id (Employer)
        public string? Position { get; set; }
        public string? ProfileImage { get; set; }

        [JsonIgnore]
        public User User { get; set; } = null!; // Người dùng HR
        [JsonIgnore]
        public User Employer { get; set; } = null!; // Employer (Company) mà HR thuộc về
    }
}