using System.Text.Json.Serialization;

namespace JOB_FINDER_API.Models
{
    public class CandidateSkill
    {
        public int CandidateSkillId { get; set; } 
        public int UserId { get; set; }
        public int SkillId { get; set; }

        [JsonIgnore]
        public User? User { get; set; }
        [JsonIgnore]
        public Skill? Skill { get; set; }
    }
}
