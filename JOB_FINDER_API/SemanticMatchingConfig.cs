using System;

namespace JOB_FINDER_API.Models.Services
{
    public class SemanticMatchingConfig
    {
        public static class Defaults
        {
            // Scoring consistency settings
            public const bool FORCE_TRYMATH_RECALCULATION = true;
            public const bool ENABLE_ENHANCED_LOGGING = true;
            public const int EMBEDDING_CACHE_DAYS = 7;
            
            // Performance optimization settings
            public const bool ENABLE_PREPROCESSING_CACHE = true;
            public const int MAX_CONCURRENT_API_CALLS = 3;
            public const int PREPROCESSING_CACHE_HOURS = 1;
            public const bool SMART_CACHE_INVALIDATION = true;
            
            // Text processing settings
            public const int MIN_TEXT_LENGTH_FOR_ANALYSIS = 100;
            public const int SUMMARY_LENGTH_THRESHOLD = 500;
            
            // Similarity calculation settings
            public const float DESCRIPTION_THRESHOLD = 0.4f;
            public const float SKILLS_THRESHOLD = 0.5f;
            public const float EXPERIENCE_THRESHOLD = 0.4f;
            public const float EDUCATION_THRESHOLD = 0.6f;
            public const float TOTAL_THRESHOLD = 0.5f;
        }

        public class ProcessingMode
        {
            public const string TRY_MATCH = "TryMatch";
            public const string APPLICATION = "Application";
            public const string ADMIN_REVIEW = "AdminReview";
        }
    }
}