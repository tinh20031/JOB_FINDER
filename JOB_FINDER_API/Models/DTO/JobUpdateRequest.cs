using JOB_FINDER_API.Models.Requests;
using static JOB_FINDER_API.Models.Requests.JobCreateRequest;

namespace JOB_FINDER_API.Models.DTO
{
    public class JobUpdateRequest
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
        public int LevelId { get; set; }
        public int JobTypeId { get; set; }
        public int Quantity { get; set; }
        public string ProvinceName { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public string AddressDetail { get; set; }
    }
}
