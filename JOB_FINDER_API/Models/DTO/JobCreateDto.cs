using System.ComponentModel.DataAnnotations;
using static JOB_FINDER_API.Models.Job;

namespace JOB_FINDER_API.Models.DTO
{
    public class JobCreateDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int CompanyId { get; set; }

        [Range(1, int.MaxValue)]
        public int Salary { get; set; }

        [Range(1, int.MaxValue)]
        public int IndustryId { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Range(1, int.MaxValue)]
        public int LevelId { get; set; }

        [Range(1, int.MaxValue)]
        public int JobTypeId { get; set; }

        [Range(1, int.MaxValue)]
        public int ExperienceLevelId { get; set; }

        [Required]
        public DateTime TimeStart { get; set; }

        [Required]
        public DateTime TimeEnd { get; set; }

        [Required]
        public JobStatus Status { get; set; } = JobStatus.pending;



        [Required]
        public string ProvinceName { get; set; } = string.Empty;

        [Required]
        public string AddressDetail { get; set; } = string.Empty;
    }
}
