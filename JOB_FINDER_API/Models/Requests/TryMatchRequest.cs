namespace JOB_FINDER_API.Models.Requests
{
    public class TryMatchRequest
    {
        public int JobId { get; set; }
        public int? CvId { get; set; } // Chọn CV hiện có
        public IFormFile CvFile { get; set; } // Upload CV mới
    }
}
