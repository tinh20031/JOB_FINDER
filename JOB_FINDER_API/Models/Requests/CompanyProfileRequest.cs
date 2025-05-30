namespace JOB_FINDER_API.Models.Requests
{
    public class CompanyProfileRequest
    {
        public int UserId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyProfileDescription { get; set; }
        public string? Location { get; set; }
        public string? TeamSize { get; set; }
        public string? Website { get; set; }
        public string? Contact { get; set; }
        public int IndustryId { get; set; }
    }
}
