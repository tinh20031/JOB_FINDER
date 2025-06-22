namespace JOB_FINDER_API.Models.DTO
{
    public class UpdateCandidateProfileDto
    {
        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string? JobTitle { get; set; }
        public string? Address { get; set; }
        public string? Province { get; set; }
        public string? City { get; set; }
        public string? PersonalLink { get; set; }
        public string? FullName { get; set; } // Thêm trường này
      
    }
}