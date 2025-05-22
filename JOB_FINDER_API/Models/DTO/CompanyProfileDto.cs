namespace JOB_FINDER_API.Models.DTO
{
    public class CompanyProfileDto
    {
        public int UserId { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyProfileDescription { get; set; }
        public string? Location { get; set; }
        public string? UrlCompanyLogo { get; set; }
        public string? ImageLogoLgr { get; set; }
        public string? TeamSize { get; set; }
        public string? Website { get; set; }
        public string? Contact { get; set; }
        public int IndustryId { get; set; }
        public string? IndustryName { get; set; }
    }
}