using System;

namespace JOB_FINDER_API.Models
{
    public class CandidateToCompanyRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyProfileDescription { get; set; }
        public string? Location { get; set; }
        public string? TeamSize { get; set; }
        public string? Website { get; set; }
        public string? Contact { get; set; }
        public int IndustryId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}