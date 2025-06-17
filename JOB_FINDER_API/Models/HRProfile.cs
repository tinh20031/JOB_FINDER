using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class HRProfile
    {
        public int UserId { get; set; } 
        public int EmployerId { get; set; } 
        public string? Position { get; set; }
        public string? ProfileImage { get; set; }

        [JsonIgnore]
        public User User { get; set; } = null!;
        [JsonIgnore]
        public User Employer { get; set; } = null!; 
    }
}