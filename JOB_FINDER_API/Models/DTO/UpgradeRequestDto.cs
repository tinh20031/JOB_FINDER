namespace JOB_FINDER_API.Models.DTO
{
    public class UpgradeRequestDto
    {
        public int CandidateToCompanyRequestId { get; set; }
        public int UserId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyProfileDescription { get; set; }
        public string? Location { get; set; }
        public string? TeamSize { get; set; }
        public string? Website { get; set; }
        public string? Contact { get; set; }
        public int IndustryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public RequestStatus Status { get; set; }

        
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Image { get; set; }

     
        public string? IndustryName { get; set; }
    }
}
