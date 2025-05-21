using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class JobSkill
    {
        public int JobId { get; set; }
        public int SkillId { get; set; }

        [JsonIgnore]
        public Job Job { get; set; } = null!;
        [JsonIgnore]
        public Skill Skill { get; set; } = null!;
    }
}