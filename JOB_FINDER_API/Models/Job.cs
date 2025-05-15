namespace JOB_FINDER_API.Models
{
    public class Job
    {
        public int JobId { get; set; } // Có thể đổi thành Guid nếu dùng UUID
        public int EmployerId { get; set; }
        public User Employer { get; set; } = null!;

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Salary { get; set; } = string.Empty;

        public int IndustryId { get; set; }
        public Industry Industry { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime ExpiryDate { get; set; }

        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    }

}
