namespace JOB_FINDER_API.Models
{
    using System.Text.Json.Serialization;

    public class TryMatchRecord
    {
        public int TryMatchId { get; set; }
        public int UserId { get; set; }
        public int JobId { get; set; }
        public int CvId { get; set; }
        public float SimilarityScore { get; set; }
        public float ITRelevance { get; set; }
        public string Suggestions { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CvSummary { get; set; }
        public string JobSummary { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        [JsonIgnore]
        public Job Job { get; set; }

        [JsonIgnore]
        public CV CV { get; set; }
    }
}

