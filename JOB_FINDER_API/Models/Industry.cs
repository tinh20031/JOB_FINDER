namespace JOB_FINDER_API.Models
{
    public class Industry
    {
        public int IndustryId { get; set; }
        public string IndustryName { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }

}
