namespace JOB_FINDER_API.Models
{
    public class Embedding
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Model { get; set; }
        public float[] Vector { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
