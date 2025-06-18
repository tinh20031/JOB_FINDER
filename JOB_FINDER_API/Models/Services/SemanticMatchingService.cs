using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Polly;
using JOB_FINDER_API.Models;

namespace JOB_FINDER_API.Models.Services
{
    public class OpenAIConfig
    {
        public string ApiKey { get; set; }
        public string Model { get; set; }
        public string Endpoint { get; set; }
    }

    public class SemanticMatchingService
    {
        private readonly OpenAIConfig _openAIConfig;
        private readonly HttpClient _httpClient;
        private readonly IAsyncPolicy _retryPolicy;

        public SemanticMatchingService(IOptions<OpenAIConfig> openAIConfig)
        {
            _openAIConfig = openAIConfig.Value;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_openAIConfig.Endpoint)
            };
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAIConfig.ApiKey}");
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"Retry {retryCount} after {timeSpan.TotalSeconds}s due to {exception.Message}");
                    });
        }

        public async Task<(bool Success, string ErrorMessage, float[] Vector)> PreprocessAndGenerateEmbeddingAsync(string text)
        {
            if (string.IsNullOrEmpty(text))
                return (false, "Input text is empty", new float[0]);

            var result = await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    string preprocessedText = text.Trim().ToLower().Replace("\n", " ").Replace(".", " ");
                    var requestBody = new { inputs = preprocessedText };
                    var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync("", jsonContent);
                    Console.WriteLine($"API Response: Status={response.StatusCode}, Reason={response.ReasonPhrase}, Headers={string.Join(", ", response.Headers)}");

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorContent);
                        Console.WriteLine($"Error Response Body: {errorContent}");
                        return (false, $"API Error: {(response.StatusCode == System.Net.HttpStatusCode.NotFound ? "Model not found or URL invalid" : errorResponse?.Error ?? "Unknown error")}", new float[0]);
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var embeddingResponse = JsonSerializer.Deserialize<float[][]>(responseContent);
                    if (embeddingResponse == null || embeddingResponse.Length == 0)
                        return (false, "No embedding data returned", new float[0]);

                    return (true, null, embeddingResponse[0].Select(x => (float)x).ToArray());
                }
                catch (Exception ex)
                {
                    return (false, $"Embedding generation failed: {ex.Message}", new float[0]);
                }
            });

            return result;
        }

        public async Task<(bool Success, string ErrorMessage, float[][] Vectors)> PreprocessAndGenerateEmbeddingsAsync(string[] texts)
        {
            if (texts == null || !texts.Any())
                return (false, "Input texts are empty or missing", new float[0][]);

            var result = await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var preprocessedTexts = texts.Select(text => text?.Trim().ToLower().Replace("\n", " ").Replace(".", " ") ?? "").ToArray();
                    var requestBody = new { inputs = preprocessedTexts };
                    var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync("", jsonContent);
                    Console.WriteLine($"API Response: Status={response.StatusCode}, Reason={response.ReasonPhrase}, Endpoint={_openAIConfig.Endpoint}");

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorContent) ?? new ErrorResponse { Error = errorContent };
                        Console.WriteLine($"Error Response Body: {errorContent}");
                        return (false, $"API Error: {(response.StatusCode == System.Net.HttpStatusCode.NotFound ? "Model not found or URL invalid" : errorResponse.Error)}", new float[0][]);
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var embeddingResponse = JsonSerializer.Deserialize<float[][]>(responseContent);
                    if (embeddingResponse == null || embeddingResponse.Length == 0 || embeddingResponse.Length != texts.Length)
                        return (false, "No or invalid embedding data returned", new float[0][]);

                    return (true, null, embeddingResponse);
                }
                catch (JsonException ex)
                {
                    return (false, $"JSON parsing failed: {ex.Message}", new float[0][]);
                }
                catch (Exception ex)
                {
                    return (false, $"Embedding generation failed: {ex.Message}", new float[0][]);
                }
            });

            return result;
        }

        private async Task<(bool Success, string ErrorMessage, (float[] description, float[] skills, float[] experience, float[] education) Vectors)> GenerateVectors(Job job)
        {
            Console.WriteLine($"Job Data - Description: {job.Description?.Length ?? 0}, YourSkill: {job.YourSkill?.Length ?? 0}, " +
                              $"YourExperience: {job.YourExperience?.Length ?? 0}, Education: {job.Education?.Length ?? 0}");
            var texts = new[] { job.Description ?? string.Empty, job.YourSkill ?? string.Empty, job.YourExperience ?? string.Empty, job.Education ?? string.Empty };
            var embeddingsResult = await PreprocessAndGenerateEmbeddingsAsync(texts);
            if (!embeddingsResult.Success) return (false, embeddingsResult.ErrorMessage, (new float[0], new float[0], new float[0], new float[0]));

            return (true, null, (embeddingsResult.Vectors[0], embeddingsResult.Vectors[1], embeddingsResult.Vectors[2], embeddingsResult.Vectors[3]));
        }

        private async Task<(bool Success, string ErrorMessage, (float[] description, float[] skills, float[] experience, float[] education) Vectors)> GenerateVectorsForCV(CV cv)
        {
            var cvData = cv.GetCVData();
            Console.WriteLine($"CV Data - Description: {cvData.Description?.Length ?? 0}, Skills: {cvData.Skills?.Length ?? 0}, " +
                              $"Experience: {cvData.Experience?.Length ?? 0}, Education: {cvData.Education?.Length ?? 0}");
            var texts = new[] { cvData.Description, cvData.Skills, cvData.Experience, cvData.Education };
            var embeddingsResult = await PreprocessAndGenerateEmbeddingsAsync(texts);
            if (!embeddingsResult.Success) return (false, embeddingsResult.ErrorMessage, (new float[0], new float[0], new float[0], new float[0]));

            return (true, null, (embeddingsResult.Vectors[0], embeddingsResult.Vectors[1], embeddingsResult.Vectors[2], embeddingsResult.Vectors[3]));
        }

        public float CalculateCosineSimilarity(float[] vector1, float[] vector2)
        {
            if (vector1.Length != vector2.Length || vector1.Length == 0) return 0f;
            float dotProduct = vector1.Zip(vector2, (a, b) => a * b).Sum();
            float magnitude1 = (float)Math.Sqrt(vector1.Sum(x => x * x));
            float magnitude2 = (float)Math.Sqrt(vector2.Sum(x => x * x));
            return magnitude1 * magnitude2 == 0 ? 0 : dotProduct / (magnitude1 * magnitude2);
        }

        public async Task<(bool Success, string ErrorMessage, float TotalSimilarity)> CalculateTotalSimilarity(Job job, CV cv, MatchingWeights weights)
        {
            var jobVectors = await GenerateVectors(job);
            Console.WriteLine($"Job Vectors: Success={jobVectors.Success}, Error={jobVectors.ErrorMessage}");
            if (!jobVectors.Success) return (false, jobVectors.ErrorMessage, 0f);

            var cvVectors = await GenerateVectorsForCV(cv);
            Console.WriteLine($"CV Vectors: Success={cvVectors.Success}, Error={cvVectors.ErrorMessage}");
            if (!cvVectors.Success) return (false, cvVectors.ErrorMessage, 0f);

            try
            {
                float similarityDescription = CalculateCosineSimilarity(jobVectors.Vectors.description, cvVectors.Vectors.description);
                float similaritySkills = CalculateCosineSimilarity(jobVectors.Vectors.skills, cvVectors.Vectors.skills);
                float similarityExperience = CalculateCosineSimilarity(jobVectors.Vectors.experience, cvVectors.Vectors.experience);
                float similarityEducation = CalculateCosineSimilarity(jobVectors.Vectors.education, cvVectors.Vectors.education);

                float totalSimilarity = (weights.DescriptionWeight * similarityDescription) +
                                       (weights.SkillsWeight * similaritySkills) +
                                       (weights.ExperienceWeight * similarityExperience) +
                                       (weights.EducationWeight * similarityEducation);
                return (true, null, totalSimilarity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Similarity calculation failed: {ex.Message}");
                return (false, $"Similarity calculation failed: {ex.Message}", 0f);
            }
        }
    }

    public class EmbeddingResponse
    {
        public EmbeddingData[] Data { get; set; }
        public string Model { get; set; }
        public Usage Usage { get; set; }
    }

    public class EmbeddingData
    {
        public float[] Embedding { get; set; }
        public string Object { get; set; }
        public int Index { get; set; }
    }

    public class Usage
    {
        public int PromptTokens { get; set; }
        public int TotalTokens { get; set; }
    }

    public class ErrorResponse
    {
        public string Error { get; set; }
    }
}