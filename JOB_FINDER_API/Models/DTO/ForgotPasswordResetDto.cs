namespace JOB_FINDER_API.Models.DTO
{
    public class ForgotPasswordResetDto
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }
    }
}
