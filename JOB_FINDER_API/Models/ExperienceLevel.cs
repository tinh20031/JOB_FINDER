using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class ExperienceLevel
    {
        public int id { get; set; }
        public string name { get; set; }
        [JsonIgnore]
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
