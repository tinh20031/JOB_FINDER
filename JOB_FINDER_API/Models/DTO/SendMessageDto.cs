namespace JOB_FINDER_API.Models.DTO
{
    public class SendMessageDto
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int? RelatedJobId { get; set; }
        public string MessageText { get; set; } = string.Empty;
    }
}
