namespace JOB_FINDER_API.Models.DTO
{
    public class CreateCandidateProfileDto
    {
        public int UserId { get; set; }
        public string Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string JobTitle { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string? Language { get; set; }
        public List<int> SkillIds { get; set; } = new();
    }
}
