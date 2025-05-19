using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class Skill
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    }

}
