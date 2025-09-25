namespace JOB_FINDER_API.Models.Requests
{
    public class JobDraftRequest
    {
        public int CompanyId { get; set; }
        public int? IndustryId { get; set; }
        public int? LevelId { get; set; }
        public int? JobTypeId { get; set; }
        public int Quantity { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Education { get; set; }
        public string? YourSkill { get; set; }
        public string? YourExperience { get; set; }
        public string? ProvinceName { get; set; }
        public string? AddressDetail { get; set; }
        public bool IsSalaryNegotiable { get; set; }
        public int? MinSalary { get; set; }
        public int? MaxSalary { get; set; }
        public float? DescriptionWeight { get; set; }
        public float? SkillsWeight { get; set; }
        public float? ExperienceWeight { get; set; }
        public float? EducationWeight { get; set; }
    }
}