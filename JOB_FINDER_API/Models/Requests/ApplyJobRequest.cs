namespace JOB_FINDER_API.Models.Requests
{
    public class ApplyJobRequest
    {
        public int JobId { get; set; }
        public string? CoverLetter { get; set; }

        public IFormFile? CvFile { get; set; }
    }
}
