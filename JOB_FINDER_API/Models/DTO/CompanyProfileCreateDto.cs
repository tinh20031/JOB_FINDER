namespace JOB_FINDER_API.Models.DTO
{
    public class CompanyProfileCreateDto
    {
        public int UserId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyProfileDescription { get; set; }
        public string? Location { get; set; }
        public string? UrlCompanyLogo { get; set; }
        public string? ImageLogoLgr { get; set; }
        public string? TeamSize { get; set; }
        public bool IsVerified { get; set; } = false;
        public string? Website { get; set; }
        public string? Contact { get; set; }
        public int IndustryId { get; set; }
    }
}
