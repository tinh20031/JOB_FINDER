using System.ComponentModel.DataAnnotations;

namespace JOB_FINDER_API.Models.DTO
{
    public class SendMessageDto
    {
        [Required(ErrorMessage = "SenderId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "SenderId must be a positive integer")]
        public int SenderId { get; set; }

        [Required(ErrorMessage = "ReceiverId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ReceiverId must be a positive integer")]
        public int ReceiverId { get; set; }

        public int? RelatedJobId { get; set; }

        [Required(ErrorMessage = "Message text is required")]
        [MaxLength(1000, ErrorMessage = "Message text cannot exceed 1000 characters")]
        public string MessageText { get; set; }
    }
}
