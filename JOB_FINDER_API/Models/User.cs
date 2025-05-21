using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int RoleId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public Role Role { get; set; } = null!;
        [JsonIgnore]
        public CandidateProfile? CandidateProfile { get; set; }
        [JsonIgnore]
        public CompanyProfile? CompanyProfile { get; set; }
        [JsonIgnore]
        public ICollection<Job> PostedJobs { get; set; } = new List<Job>();
        [JsonIgnore]
        public ICollection<Application> Applications { get; set; } = new List<Application>();
        [JsonIgnore]
        public ICollection<UserFavoriteJob> FavoriteJobs { get; set; } = new List<UserFavoriteJob>();
        [JsonIgnore]
        public ICollection<Experience> Experiences { get; set; } = new List<Experience>();
        [JsonIgnore]
        public ICollection<Education> Educations { get; set; } = new List<Education>();
        [JsonIgnore]
        public ICollection<CV> CVs { get; set; } = new List<CV>();
        [JsonIgnore]
        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        [JsonIgnore]
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        [JsonIgnore]
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    }
}