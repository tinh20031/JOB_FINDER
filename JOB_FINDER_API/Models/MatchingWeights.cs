namespace JOB_FINDER_API.Models
{
    public class MatchingWeights
    {
        
            public float DescriptionWeight { get; set; } = 0.4f; 
            public float SkillsWeight { get; set; } = 0.3f;   
            public float ExperienceWeight { get; set; } = 0.2f;
            public float EducationWeight { get; set; } = 0.1f;  
        
    }
}
