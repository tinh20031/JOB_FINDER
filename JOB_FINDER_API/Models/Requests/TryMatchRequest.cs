namespace JOB_FINDER_API.Models.Requests
{
    public class TryMatchRequest
    {
        public int JobId { get; set; }
        public int? CvId { get; set; } 
        public IFormFile CvFile { get; set; } 
    }
}
