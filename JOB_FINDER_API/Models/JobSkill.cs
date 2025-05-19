using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class JobSkill
    {
        public int JobId { get; set; }
        public int SkillId { get; set; }

        // Navigation properties (optional for API payloads)
        [JsonIgnore]
        public Job? Job { get; set; }
        [JsonIgnore]
        public Skill? Skill { get; set; }
    }
}
