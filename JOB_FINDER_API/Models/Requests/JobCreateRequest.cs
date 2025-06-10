using static JOB_FINDER_API.Models.Job;

namespace JOB_FINDER_API.Models.Requests
{
    public class JobCreateRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int CompanyId { get; set; }
        public int Salary { get; set; }
        public int IndustryId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int LevelId { get; set; }
        public int JobTypeId { get; set; }
        public int ExperienceLevelId { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; } 
        public JobStatus Status { get; set; } = JobStatus.pending;
        public string ProvinceName { get; set; }
        public string AddressDetail { get; set; }
    }
}
