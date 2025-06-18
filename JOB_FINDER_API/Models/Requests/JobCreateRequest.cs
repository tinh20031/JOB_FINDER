using static JOB_FINDER_API.Models.Job;

namespace JOB_FINDER_API.Models.Requests
{

    public class JobCreateRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Education { get; set; }
        public string YourSkill { get; set; }
        public string YourExperience { get; set; }
       
        public int CompanyId { get; set; }
        public int? MinSalary { get; set; }
        public int? MaxSalary { get; set; }
        public bool IsSalaryNegotiable { get; set; }
        public int IndustryId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int LevelId { get; set; }
        public int JobTypeId { get; set; }
        public int ExperienceLevelId { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public Job.JobStatus Status { get; set; } = Job.JobStatus.pending;
        public string ProvinceName { get; set; }
        public string AddressDetail { get; set; }
        public List<SkillInput>? skillInputs { get; set; }
    }
    public class SkillInput
    {
        public int? SkillId { get; set; }
        public string? SkillName { get; set; }
    }
   
    }

