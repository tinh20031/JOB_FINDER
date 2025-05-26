using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class Skill
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
        [JsonIgnore]
        public List<CandidateSkill>? CandidateSkills { get; set; }
    }
}