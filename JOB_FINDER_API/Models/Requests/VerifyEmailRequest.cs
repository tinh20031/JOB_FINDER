namespace JOB_FINDER_API.Models.DTO
{
    public class VerifyEmailRequest
    {
        public string Email { get; set; }
        public string VerificationCode { get; set; }
    }
}