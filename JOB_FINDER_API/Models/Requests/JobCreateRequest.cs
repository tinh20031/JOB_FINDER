using System.ComponentModel.DataAnnotations;
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
        public int Quantity { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public Job.JobStatus Status { get; set; } = Job.JobStatus.pending;
        public string ProvinceName { get; set; }
        public string AddressDetail { get; set; }
        public List<SkillInput>? skillInputs { get; set; }
        [Required(ErrorMessage = "DescriptionWeight là bắt buộc.")]
        [Range(0, 100, ErrorMessage = "DescriptionWeight phải từ 0 đến 100.")]
        [SumToHundred]
        public float DescriptionWeight { get; set; }

        [Required(ErrorMessage = "SkillsWeight là bắt buộc.")]
        [Range(0, 100, ErrorMessage = "SkillsWeight phải từ 0 đến 100.")]
        [SumToHundred]
        public float SkillsWeight { get; set; }

        [Required(ErrorMessage = "ExperienceWeight là bắt buộc.")]
        [Range(0, 100, ErrorMessage = "ExperienceWeight phải từ 0 đến 100.")]
        [SumToHundred]
        public float ExperienceWeight { get; set; }

        [Required(ErrorMessage = "EducationWeight là bắt buộc.")]
        [Range(0, 100, ErrorMessage = "EducationWeight phải từ 0 đến 100.")]
        [SumToHundred]
        public float EducationWeight { get; set; }
        public class SkillInput
        {
            public int? SkillId { get; set; }
            public string? SkillName { get; set; }
        }
        public class SumToHundredAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (JobCreateRequest)validationContext.ObjectInstance;
                float totalWeight = model.DescriptionWeight + model.SkillsWeight + model.ExperienceWeight + model.EducationWeight;
                if (Math.Abs(totalWeight - 100.0f) > 0.1f) // Dung sai 0.1 để tránh lỗi làm tròn
                {
                    return new ValidationResult("Tổng tỷ lệ của DescriptionWeight, SkillsWeight, ExperienceWeight, và EducationWeight phải bằng 100%.");
                }
                return ValidationResult.Success;
            }
        }
    }
}

