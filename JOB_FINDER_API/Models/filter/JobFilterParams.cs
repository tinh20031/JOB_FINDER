namespace JOB_FINDER_API.Models.filter
{
    public class JobFilterParams
    {
        public string? Title { get; set; }
        public int? IndustryId { get; set; }
        public int? LevelId { get; set; }
        public int? JobTypeId { get; set; }
        public int? Quantity { get; set; }
        public int? MinSalary { get; set; }
        public int? MaxSalary { get; set; }
        public string? ProvinceName { get; set; }
        public string? Status { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public List<int>? SkillIds { get; set; }
        public string? SkillName { get; set; }
    }
}
