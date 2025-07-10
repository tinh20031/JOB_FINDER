using System.Text.Json.Serialization;
using System.Text.Json;

namespace JOB_FINDER_API.Models
{
    public class CV
    {
        public int CVId { get; set; }
        public int UserId { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string? FullCvJson { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public CvType Type { get; set; } = CvType.Upload;

        [JsonIgnore]
        public User? User { get; set; }
        [JsonIgnore]
        public ICollection<Application> Applications { get; set; } = new List<Application>();
        [JsonIgnore]
        public ICollection<TryMatchRecord> TryMatchRecords { get; set; } = new List<TryMatchRecord>();


        public (string VietnameseText, string EnglishText) GetCvText()
        {
            try
            {
                if (string.IsNullOrEmpty(FullCvJson))
                {
                    Console.WriteLine("FullCvJson is null or empty");
                    return (string.Empty, string.Empty);
                }

                Console.WriteLine($"FullCvJson content: {FullCvJson}"); // Log để debug
                var jsonContent = JsonSerializer.Deserialize<JsonElement>(FullCvJson);

                // Trích xuất Text làm VietnameseText
                string vietnameseText = jsonContent.TryGetProperty("Text", out var textElement)
                    ? textElement.GetString() ?? string.Empty
                    : string.Empty;

                // Trích xuất TranslatedText làm EnglishText
                string englishText = jsonContent.TryGetProperty("TranslatedText", out var translatedTextElement)
                    ? translatedTextElement.GetString() ?? string.Empty
                    : string.Empty;

                return (vietnameseText, englishText);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing CV JSON: {ex.Message}");
                return (string.Empty, string.Empty);
            }
        }
    }

    public class CVData
    {
        public string Field { get; set; } = "Unknown";
        public float ITRelevance { get; set; } = 1.0f;
        public string Description { get; set; } = string.Empty;
        public string Skills { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string Education { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
    }
}