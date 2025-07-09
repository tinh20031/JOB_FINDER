using Google.Apis.Auth.OAuth2;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JOB_FINDER_API.Models.Services
{
    public class SemanticMatchingService
    {
        private readonly ILogger<SemanticMatchingService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;
        private readonly GeminiConfig _geminiConfig;
        private HttpClient _authenticatedClient;
        private string _accessToken;
        private DateTime _tokenExpiration;
        private readonly Dictionary<string, (string CleanedText, string[] WeightedTerms)> _preprocessCache = new();

        public SemanticMatchingService(
            ILogger<SemanticMatchingService> logger,
            IServiceScopeFactory serviceScopeFactory,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _geminiConfig = configuration.GetSection("Gemini").Get<GeminiConfig>() ?? throw new ArgumentNullException(nameof(_geminiConfig));

            _retryPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(r => r.StatusCode >= System.Net.HttpStatusCode.InternalServerError || r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (result, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning("Retry {RetryCount} after {TimeSpan}s due to error: {Error}", retryCount, timeSpan.TotalSeconds, result.Exception?.Message ?? result.Result.ReasonPhrase);
                    });

            if (string.IsNullOrEmpty(_geminiConfig.ChatEndpoint) || string.IsNullOrEmpty(_geminiConfig.EmbeddingEndpoint))
            {
                throw new ArgumentException("ChatEndpoint and EmbeddingEndpoint must be configured in appsettings.json");
            }

            if (string.IsNullOrEmpty(_geminiConfig.ServiceAccountKeyPath))
            {
                throw new ArgumentException("ServiceAccountKeyPath must be configured in appsettings.json");
            }

            _authenticatedClient = _httpClientFactory.CreateClient();
            InitializeToken().Wait();
        }

        private async Task InitializeToken()
        {
            _accessToken = await GetAccessTokenFromServiceAccount();
            if (string.IsNullOrEmpty(_accessToken))
            {
                throw new ArgumentException("Failed to retrieve AccessToken from Service Account.");
            }
            _authenticatedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            _tokenExpiration = DateTime.UtcNow.AddHours(1);
        }

        private async Task<string> GetAccessTokenFromServiceAccount()
        {
            try
            {
                var credential = GoogleCredential.FromFile(_geminiConfig.ServiceAccountKeyPath)
                    .CreateScoped(new[] { "https://www.googleapis.com/auth/generative-language", "https://www.googleapis.com/auth/cloud-language" });
                var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
                _logger.LogInformation("Successfully retrieved access token from Service Account.");
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve access token from Service Account.");
                return null;
            }
        }

        private async Task EnsureValidToken()
        {
            if (DateTime.UtcNow >= _tokenExpiration.AddMinutes(-5))
            {
                lock (_authenticatedClient)
                {
                    if (DateTime.UtcNow >= _tokenExpiration.AddMinutes(-5))
                    {
                        _logger.LogInformation("Refreshing access token due to impending expiration.");
                        InitializeToken().Wait();
                    }
                }
            }
        }

        private async Task<(bool Success, string CleanedText, string[] WeightedTerms)> PreprocessTextWithGeminiAsync(string text, string originalText = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("Empty text provided to PreprocessTextWithGeminiAsync");
                return (false, string.Empty, Array.Empty<string>());
            }

            // Kiểm tra cache
            if (_preprocessCache.TryGetValue(text, out var cachedResult))
            {
                _logger.LogInformation("Reusing cached preprocessing result for text: Length={Length}", text.Length);
                return (true, cachedResult.CleanedText, cachedResult.WeightedTerms);
            }

            string cleanedText = text;
            string[] weightedTerms = Array.Empty<string>();

            if (text.Length < 100) // Văn bản ngắn, sử dụng trọng số thấp
            {
                _logger.LogInformation("Short text detected (Length={Length}), assigning default weights", text.Length);
                weightedTerms = text.Split(' ').Select(s => $"{s} *0.5").ToArray();
                var result = (true, text, weightedTerms);
                _preprocessCache[text] = (text, weightedTerms);
                return result;
            }

            try
            {
                await EnsureValidToken();
                var requestBody = new
                {
                    contents = new[] { new { parts = new[] { new { text = $"Preprocess this text for semantic analysis in a job matching context:\n{text}" } } } }
                };
                var requestJsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await _retryPolicy.ExecuteAsync(async () => await _authenticatedClient.PostAsync(_geminiConfig.ChatEndpoint, requestJsonContent));
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseJson = JsonSerializer.Deserialize<JsonNode>(responseContent);
                var responseText = responseJson["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                if (string.IsNullOrWhiteSpace(responseText))
                {
                    _logger.LogWarning("Empty response from Gemini for text preprocessing");
                    return (false, text, Array.Empty<string>());
                }

                var responseLines = responseText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in responseLines)
                {
                    string trimmedLine = line.Trim();
                    if (trimmedLine.StartsWith("Cleaned Text:", StringComparison.OrdinalIgnoreCase))
                    {
                        cleanedText = trimmedLine.Substring("Cleaned Text:".Length).Trim();
                    }
                    else if (trimmedLine.StartsWith("Weighted Terms:", StringComparison.OrdinalIgnoreCase))
                    {
                        weightedTerms = trimmedLine.Substring("Weighted Terms:".Length).Trim().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray();
                    }
                }

                _logger.LogInformation("Preprocessed text with Gemini: CleanedTextLength={Length}, WeightedTermsCount={Count}",
                    cleanedText.Length, weightedTerms.Length);

                // Lưu vào cache
                _preprocessCache[text] = (cleanedText, weightedTerms);
                return (true, cleanedText, weightedTerms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preprocessing text with Gemini");
                return (false, text, Array.Empty<string>());
            }
        }

        public async Task<(bool Success, string ErrorMessage, float[] Vector)> PreprocessAndGenerateEmbeddingAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("PreprocessAndGenerateEmbeddingAsync called with empty or whitespace text");
                return (false, "Input text is empty", new float[0]);
            }

            await EnsureValidToken();

            var (success, cleanedText, weightedTerms) = await PreprocessTextWithGeminiAsync(text);
            if (!success)
            {
                _logger.LogWarning("Failed to preprocess text: {Error}", cleanedText);
                return (false, cleanedText, new float[0]);
            }

            var words = cleanedText.ToLower().Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 1 && w.Length < 30 && Regex.IsMatch(w, @"^[a-z0-9#+]+$"))
                .ToArray();

            var preprocessedText = string.Join(" ", words.Select(word =>
                Array.Exists(weightedTerms, wt => wt.Contains(word, StringComparison.OrdinalIgnoreCase)) ? $"{word} *1.0" : $"{word} *0.5"));

            if (string.IsNullOrWhiteSpace(preprocessedText))
            {
                _logger.LogWarning("No processable content found in text after weighting");
                return (false, "No processable content", new float[0]);
            }

            var modelName = "models/text-embedding-004";

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                var existingEmbedding = await context.Embeddings
                    .FirstOrDefaultAsync(e => e.Text == preprocessedText && e.Model == modelName);
                if (existingEmbedding != null && existingEmbedding.CreatedAt > DateTime.UtcNow.AddDays(-7) &&
                    existingEmbedding.Vector.All(v => !float.IsNaN(v) && !float.IsInfinity(v)))
                {
                    _logger.LogInformation("Reusing existing embedding for text: {Text}", preprocessedText);
                    return (true, string.Empty, existingEmbedding.Vector);
                }

                var embeddingRequestBody = new
                {
                    model = modelName,
                    content = new { parts = new[] { new { text = preprocessedText } } }
                };

                var embeddingJsonContent = new StringContent(JsonSerializer.Serialize(embeddingRequestBody), Encoding.UTF8, "application/json");
                var embeddingResponse = await _retryPolicy.ExecuteAsync(async () =>
                    await _authenticatedClient.PostAsync(_geminiConfig.EmbeddingEndpoint, embeddingJsonContent));

                if (!embeddingResponse.IsSuccessStatusCode)
                {
                    var errorContent = await embeddingResponse.Content.ReadAsStringAsync();
                    _logger.LogError("Embedding API error: {ErrorContent}", errorContent);
                    return (false, $"API Error: {errorContent}", new float[0]);
                }

                var embeddingResponseContent = await embeddingResponse.Content.ReadAsStringAsync();
                try
                {
                    var geminiResult = JsonSerializer.Deserialize<JsonElement>(embeddingResponseContent);
                    var embeddingArray = geminiResult.GetProperty("embedding").GetProperty("values").EnumerateArray()
                        .Select(e => e.GetSingle()).ToArray();

                    if (embeddingArray.Any(v => float.IsNaN(v) || float.IsInfinity(v)))
                    {
                        _logger.LogWarning("Invalid embedding values detected for text: {Text}", preprocessedText);
                        return (false, "Invalid embedding values", new float[0]);
                    }

                    var embeddingEntity = new Embedding
                    {
                        Text = preprocessedText,
                        Model = modelName,
                        Vector = embeddingArray,
                        CreatedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.AddDays(7)
                    };
                    context.Embeddings.Add(embeddingEntity);
                    await context.SaveChangesAsync();
                    _logger.LogInformation("New embedding saved for text: {Text}", preprocessedText);

                    return (true, string.Empty, embeddingArray);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Failed to parse embedding response: {ResponseContent}", embeddingResponseContent);
                    return (false, "Invalid embedding response format", new float[0]);
                }
            }
        }

        public async Task<(bool Success, string ErrorMessage, float[][] Vectors)> GenerateVectorsForCriteria(Job job, string jobText, bool summarize = false)
        {
            if (job == null || string.IsNullOrWhiteSpace(jobText))
            {
                _logger.LogWarning("GenerateVectorsForCriteria called with null job or empty jobText");
                return (false, "Invalid job or jobText", new float[4][]);
            }

            _logger.LogInformation("Extracting job content - Description: {Description}, YourSkill: {YourSkill}, YourExperience: {YourExperience}, Education: {Education}",
                job.Description, job.YourSkill, job.YourExperience, job.Education);

            var texts = new[] { job.Description, job.YourSkill, job.YourExperience, job.Education };
            var vectors = new float[4][] { new float[0], new float[0], new float[0], new float[0] };

            string processedJobText = jobText;
            if (summarize && jobText.Length > 500)
            {
                var (summarizeSuccess, summary, error) = await SummarizeAndTranslate(jobText, "en");
                if (summarizeSuccess)
                {
                    processedJobText = summary;
                    _logger.LogInformation("Summarized job text: Length={Length}", processedJobText.Length);
                }
                else
                {
                    _logger.LogWarning("Failed to summarize job text: {Error}, using original text", error);
                }
            }

            var tasks = texts.Select(async (text, i) =>
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    var (vectorSuccess, vectorError, vector) = await PreprocessAndGenerateEmbeddingAsync(text);
                    return (Index: i, Success: vectorSuccess, Vector: vector, Error: vectorError);
                }
                return (Index: i, Success: false, Vector: new float[0], Error: "Empty text");
            }).ToArray();

            var results = await Task.WhenAll(tasks);
            foreach (var result in results)
            {
                if (result.Success && result.Vector != null && result.Vector.Length > 0)
                {
                    vectors[result.Index] = result.Vector;
                    _logger.LogInformation("Generated vector for job criteria {Index}: Length={Length}", result.Index, result.Vector.Length);
                }
                else
                {
                    vectors[result.Index] = new float[0];
                    _logger.LogWarning("Failed to generate vector for job criteria {Index}: {Error}", result.Index, result.Error);
                }
            }

            if (vectors.All(v => v.Length == 0))
            {
                _logger.LogError("All job vectors are empty for Job {JobId}", job.JobId);
                return (false, "Failed to generate any job vectors", vectors);
            }

            return (true, string.Empty, vectors);
        }

        public async Task<(bool Success, string ErrorMessage, float[][] Vectors)> GenerateVectorsForCVCriteria(CV cv, string cvText, bool summarize = false)
        {
            if (cv == null || string.IsNullOrWhiteSpace(cvText))
            {
                _logger.LogWarning("GenerateVectorsForCVCriteria called with null CV or empty cvText");
                return (false, "Invalid CV or cvText", new float[4][]);
            }

            var fullCvText = cv.FullCvJson;
            var (extractSuccess, extractError, cvData, _) = await ExtractCvDataAsync(cv, fullCvText);
            if (!extractSuccess)
            {
                _logger.LogError("Failed to extract CV data: {Error}", extractError);
                return (false, extractError, new float[4][]);
            }

            var texts = new[]
            {
                cvData.Description ?? "No description",
                cvData.Skills != null && cvData.Skills.Any() ? string.Join(" ", cvData.Skills) : "No skills",
                cvData.Experience ?? "No experience",
                cvData.Education ?? "No education"
            };
            _logger.LogInformation("Extracted CV criteria - Description: {Description}, Skills: {Skills}, Experience: {Experience}, Education: {Education}",
                texts[0], string.Join(", ", cvData.Skills ?? new List<string> { "No skills" }), texts[2], texts[3]);

            var vectors = new float[4][] { new float[0], new float[0], new float[0], new float[0] };

            string processedCvText = cvText;
            if (summarize && cvText.Length > 500)
            {
                var (success, summary, error) = await SummarizeAndTranslate(cvText, "en");
                if (success)
                {
                    processedCvText = summary;
                    _logger.LogInformation("Summarized CV text: Length={Length}", processedCvText.Length);
                }
                else
                {
                    _logger.LogWarning("Failed to summarize CV text: {Error}, using original text", error);
                }
            }

            var tasks = texts.Select(async (text, i) =>
            {
                if (!string.IsNullOrWhiteSpace(text) && !text.Contains("No "))
                {
                    var (vectorSuccess, vectorError, vector) = await PreprocessAndGenerateEmbeddingAsync(text);
                    return (Index: i, Success: vectorSuccess, Vector: vector, Error: vectorError);
                }
                return (Index: i, Success: false, Vector: new float[0], Error: "Empty or invalid text");
            }).ToArray();

            var results = await Task.WhenAll(tasks);
            foreach (var result in results)
            {
                if (result.Success && result.Vector != null && result.Vector.Length > 0)
                {
                    vectors[result.Index] = result.Vector;
                    _logger.LogInformation("Generated vector for CV criteria {Index}: Length={Length}", result.Index, result.Vector.Length);
                }
                else
                {
                    vectors[result.Index] = new float[0];
                    _logger.LogWarning("Failed to generate vector for CV criteria {Index}: {Error}", result.Index, result.Error);
                }
            }

            if (vectors.All(v => v.Length == 0))
            {
                _logger.LogError("All CV vectors are empty for CV");
                return (false, "Failed to generate any CV vectors", vectors);
            }

            return (true, string.Empty, vectors);
        }

        private async Task<(bool Success, string ErrorMessage, float[][] Vectors)> GenerateBatchEmbeddingsAsync(string[] texts)
        {
            if (texts == null || !texts.Any())
            {
                return (false, "No texts provided for batch embedding", new float[0][]);
            }

            await EnsureValidToken();

            var modelName = "models/text-embedding-004";
            var requestBody = new
            {
                model = modelName,
                content = new { parts = texts.Select(t => new { text = t }).ToArray() }
            };

            var embeddingJsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var embeddingResponse = await _retryPolicy.ExecuteAsync(async () =>
                await _authenticatedClient.PostAsync(_geminiConfig.EmbeddingEndpoint, embeddingJsonContent));

            if (!embeddingResponse.IsSuccessStatusCode)
            {
                var errorContent = await embeddingResponse.Content.ReadAsStringAsync();
                _logger.LogError("Batch embedding API error: {ErrorContent}", errorContent);
                return (false, $"API Error: {errorContent}", new float[0][]);
            }

            var embeddingResponseContent = await embeddingResponse.Content.ReadAsStringAsync();
            try
            {
                var geminiResult = JsonSerializer.Deserialize<JsonElement>(embeddingResponseContent);
                var embeddings = new List<float[]>();

                if (geminiResult.TryGetProperty("embedding", out var embeddingElement))
                {
                    if (embeddingElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in embeddingElement.EnumerateArray())
                        {
                            if (item.TryGetProperty("values", out var values))
                            {
                                var embeddingArray = values.EnumerateArray().Select(v => v.GetSingle()).ToArray();
                                embeddings.Add(embeddingArray);
                            }
                        }
                    }
                    else if (embeddingElement.ValueKind == JsonValueKind.Object)
                    {
                        if (embeddingElement.TryGetProperty("values", out var values))
                        {
                            var embeddingArray = values.EnumerateArray().Select(v => v.GetSingle()).ToArray();
                            embeddings.Add(embeddingArray);
                        }
                    }
                }

                if (!embeddings.Any() || embeddings.Any(v => v.Any(x => float.IsNaN(x) || float.IsInfinity(x))))
                {
                    _logger.LogWarning("Invalid or empty embedding values detected in batch");
                    return (false, "Invalid embedding values", new float[0][]);
                }

                return (true, string.Empty, embeddings.ToArray());
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse batch embedding response: {ResponseContent}", embeddingResponseContent);
                return (false, "Invalid embedding response format", new float[0][]);
            }
        }

        private float CalculateSimilarityWithContext(float[] jobVector, float[] cvVector, float weight)
        {
            if (jobVector == null || cvVector == null || jobVector.Length == 0 || cvVector.Length == 0)
            {
                _logger.LogWarning("Invalid vectors detected in CalculateSimilarityWithContext");
                return 0f;
            }

            float cosineSimilarity = CalculateCosineSimilarity(jobVector, cvVector);
            _logger.LogInformation("Cosine similarity calculated: {CosineSimilarity:F4}, Weight: {Weight:F4}", cosineSimilarity, weight);

            float result = cosineSimilarity * weight;
            _logger.LogInformation("Similarity score: {Result:F4}", result);
            return result;
        }

        private float CalculateCosineSimilarity(float[] vectorA, float[] vectorB)
        {
            if (vectorA.Length != vectorB.Length)
                return 0f;

            float dotProduct = 0f;
            float normA = 0f;
            float normB = 0f;
            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
                normA += vectorA[i] * vectorA[i];
                normB += vectorB[i] * vectorB[i];
            }

            normA = (float)Math.Sqrt(normA);
            normB = (float)Math.Sqrt(normB);
            return normA * normB > 0 ? dotProduct / (normA * normB) : 0f;
        }

        public async Task<(bool Success, string ErrorMessage, float FinalSimilarity, float SimilarityDescription, float SimilaritySkills, float SimilarityExperience, float SimilarityEducation, string GeminiReasoning)> CalculateTotalSimilarity(Job job, CV cv, string cvSummary, string jobSummary)
        {
            if (job == null || cv == null)
            {
                _logger.LogWarning("CalculateTotalSimilarity called with null job or CV");
                return (false, "Invalid job or CV", 0f, 0f, 0f, 0f, 0f, string.Empty);
            }

            _logger.LogInformation("Extracting job content - Description: {Description}, YourSkill: {YourSkill}, YourExperience: {YourExperience}, Education: {Education}",
                job.Description, job.YourSkill, job.YourExperience, job.Education);
            _logger.LogInformation("Extracting CV content from FullCvJson: {FullCvJson}", cv.FullCvJson);

            float descriptionWeight = job.DescriptionWeight;
            float skillsWeight = job.SkillsWeight;
            float experienceWeight = job.ExperienceWeight;
            float educationWeight = job.EducationWeight;
            float totalWeight = descriptionWeight + skillsWeight + experienceWeight + educationWeight;

            if (Math.Abs(totalWeight - 1.0f) > 0.0001f || descriptionWeight < 0 || skillsWeight < 0 || experienceWeight < 0 || educationWeight < 0)
            {
                _logger.LogWarning("Invalid weights for Job {JobId}, total weight {TotalWeight:F2} is not 1.0 or contains negative values, normalizing", job.JobId, totalWeight);
                if (totalWeight <= 0)
                {
                    _logger.LogWarning("Total weight is zero or negative for Job {JobId}, using default weights", job.JobId);
                    descriptionWeight = 0.4f;
                    skillsWeight = 0.3f;
                    experienceWeight = 0.2f;
                    educationWeight = 0.1f;
                }
                else
                {
                    descriptionWeight /= totalWeight;
                    skillsWeight /= totalWeight;
                    experienceWeight /= totalWeight;
                    educationWeight /= totalWeight;
                }
                _logger.LogInformation("Normalized weights for Job {JobId}: Description={0:F2}, Skills={1:F2}, Experience={2:F2}, Education={3:F2}",
                    job.JobId, descriptionWeight, skillsWeight, experienceWeight, educationWeight);
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                try
                {
                    string jobText = $"{job.Description}\n{job.YourSkill}\n{job.YourExperience}\n{job.Education}";
                    var (jobVectorsSuccess, jobVectorsError, jobVectors) = await GenerateVectorsForCriteria(job, jobText, summarize: true);
                    if (!jobVectorsSuccess)
                    {
                        _logger.LogError("Failed to generate job vectors: {Error}", jobVectorsError);
                        return (false, jobVectorsError, 0f, 0f, 0f, 0f, 0f, string.Empty);
                    }

                    var (cvVectorsSuccess, cvVectorsError, cvVectors) = await GenerateVectorsForCVCriteria(cv, cv.FullCvJson, summarize: true);
                    if (!cvVectorsSuccess)
                    {
                        _logger.LogError("Failed to generate CV vectors: {Error}", cvVectorsError);
                        return (false, cvVectorsError, 0f, 0f, 0f, 0f, 0f, string.Empty);
                    }

                    if (jobVectors.Length != 4 || cvVectors.Length != 4)
                    {
                        _logger.LogError("Invalid vector array length: JobVectors={JobLength}, CVVectors={CVLength}", jobVectors.Length, cvVectors.Length);
                        return (false, "Invalid vector array length", 0f, 0f, 0f, 0f, 0f, string.Empty);
                    }

                    var similarities = new float[4];
                    for (int i = 0; i < 4; i++)
                    {
                        similarities[i] = CalculateSimilarityWithContext(
                            jobVectors[i],
                            cvVectors[i],
                            i switch
                            {
                                0 => descriptionWeight,
                                1 => skillsWeight,
                                2 => experienceWeight,
                                3 => educationWeight,
                                _ => 0f
                            }
                        );
                    }

                    float similarityDescription = Math.Clamp(similarities[0], 0f, 1f);
                    float similaritySkills = Math.Clamp(similarities[1], 0f, 1f);
                    float similarityExperience = Math.Clamp(similarities[2], 0f, 1f);
                    float similarityEducation = Math.Clamp(similarities[3], 0f, 1f);

                    float totalSimilarity = similarityDescription + similaritySkills + similarityExperience + similarityEducation;
                    totalSimilarity = Math.Clamp(totalSimilarity, 0f, 1f);

                    _logger.LogInformation("Initial Similarity Scores for Job {JobId}: Description={Description:F2}, Skills={Skills:F2}, Experience={Experience:F2}, Education={Education:F2}, Total={Total:F2}",
                        job.JobId, similarityDescription, similaritySkills, similarityExperience, similarityEducation, totalSimilarity);

                    float finalSimilarity = totalSimilarity;
                    string geminiReasoning = string.Empty;

                    // Luôn gọi Gemini để điều chỉnh điểm
                    _logger.LogInformation("Calling Gemini to adjust similarity score for Job {JobId}", job.JobId);
                    var prompt = $@"Analyze the following CV and job description to determine their similarity. Assign a similarity score (0 to 1) based on how well the CV matches the job requirements, focusing on skills, experience, and education. Provide detailed reasoning for the score.
CV: {cv.FullCvJson}
Job Description: {jobText}
Initial Similarity: {totalSimilarity:F2}
Response format:
Similarity Score: [0-1]
Reasoning: [Detailed explanation]";

                    var requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
                    var requestJsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                    var response = await _retryPolicy.ExecuteAsync(async () => await _authenticatedClient.PostAsync(_geminiConfig.ChatEndpoint, requestJsonContent));
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError("Gemini API error: {ErrorContent}", await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                            var text = jsonResponse.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
                            text = Regex.Replace(text, @"^```json\n|\n```$", "");
                            var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                            var scoreLine = lines.FirstOrDefault(l => l.StartsWith("Similarity Score:", StringComparison.OrdinalIgnoreCase));
                            var reasoningLine = lines.FirstOrDefault(l => l.StartsWith("Reasoning:", StringComparison.OrdinalIgnoreCase));

                            if (scoreLine != null && float.TryParse(scoreLine.Substring("Similarity Score:".Length).Trim(), out float s))
                            {
                                finalSimilarity = Math.Clamp(s, 0f, 1f);
                                geminiReasoning = reasoningLine?.Substring("Reasoning:".Length).Trim() ?? "No reasoning provided";
                                _logger.LogInformation("Gemini adjusted similarity to {FinalSimilarity:F2} with reasoning: {Reasoning}", finalSimilarity, geminiReasoning);
                            }
                            else
                            {
                                _logger.LogWarning("Invalid similarity score format from Gemini: {ScoreLine}", scoreLine);
                            }
                        }
                        catch (JsonException ex)
                        {
                            _logger.LogError(ex, "Failed to parse similarity response: {ResponseContent}", responseContent);
                        }
                    }

                    return (true, string.Empty, finalSimilarity, similarityDescription, similaritySkills, similarityExperience, similarityEducation, geminiReasoning);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in CalculateTotalSimilarity for Job {JobId}: {Error}", job?.JobId, ex.Message);
                    return (false, $"Similarity calculation failed: {ex.Message}", 0f, 0f, 0f, 0f, 0f, string.Empty);
                }
            }
        }

        public async Task<(bool Success, string SummaryText, string ErrorMessage)> SummarizeAndTranslate(string text, string targetLanguage)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("SummarizeAndTranslate called with empty or whitespace text");
                return (false, string.Empty, "Input text is empty");
            }

            if (text.Contains("TranslatedText") && !string.IsNullOrWhiteSpace(JsonSerializer.Deserialize<JsonElement>(text).GetProperty("TranslatedText").GetString()))
            {
                _logger.LogInformation("Reusing existing translated text: Length={Length}", text.Length);
                return (true, text, string.Empty);
            }

            await EnsureValidToken();

            try
            {
                var prompt = $@"Summarize the following text in 50-100 words and translate it to {targetLanguage}. Provide only the summarized text in the response.
Text:
{text}";

                var requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
                var requestJsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await _retryPolicy.ExecuteAsync(async () => await _authenticatedClient.PostAsync(_geminiConfig.ChatEndpoint, requestJsonContent));

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Gemini API error during summarization: {ErrorContent}", errorContent);
                    return (false, string.Empty, $"Failed to summarize text: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                try
                {
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    var summaryText = jsonResponse.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
                    summaryText = Regex.Replace(summaryText, @"^```json\n|\n```$", "");
                    if (string.IsNullOrWhiteSpace(summaryText))
                    {
                        _logger.LogWarning("Empty summary text returned from Gemini");
                        return (false, string.Empty, "Empty summary text returned");
                    }
                    return (true, summaryText, string.Empty);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Failed to parse summarization response: {ResponseContent}", responseContent);
                    return (false, string.Empty, $"Summarization failed: Invalid JSON format - {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error summarizing text");
                return (false, string.Empty, $"Summarization failed: {ex.Message}");
            }
        }

        public async Task<(bool Success, string ErrorMessage, CVData CvData, string CvSummary)> ExtractCvDataAsync(CV cv, string extractedText = null)
        {
            if (cv == null && string.IsNullOrWhiteSpace(extractedText))
            {
                _logger.LogWarning("ExtractCvDataAsync called with null CV and empty extractedText");
                return (false, "No CV or text provided", null, string.Empty);
            }

            string textToProcess = extractedText ?? (cv?.FullCvJson ?? "");

            try
            {
                if (string.IsNullOrWhiteSpace(textToProcess))
                {
                    _logger.LogWarning("No CV text available for processing");
                    return (false, "No CV text available for processing", new CVData(), string.Empty);
                }

                textToProcess = PreprocessText(textToProcess);

                var prompt = $@"Extract the following fields from the provided CV text: Description, Skills (up to 4 key skills), Experience, Education. Return only the extracted data in JSON format.
If any field cannot be extracted, use a default value: Description='No description', Skills=['No skills'], Experience='No experience', Education='No education'.
Text:
{textToProcess}";

                var requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
                var requestJsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await _retryPolicy.ExecuteAsync(async () => await _authenticatedClient.PostAsync(_geminiConfig.ChatEndpoint, requestJsonContent));

                var cvData = new CVData();
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                        var text = jsonResponse.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
                        text = Regex.Replace(text, @"^```json\n|\n```$", "").Trim();
                        cvData = JsonSerializer.Deserialize<CVData>(text) ?? new CVData();
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Failed to parse Gemini CV extraction response: {ResponseContent}", responseContent);
                    }
                }
                else
                {
                    _logger.LogWarning("Gemini CV extraction failed: {Error}", await response.Content.ReadAsStringAsync());
                }

                if (string.IsNullOrEmpty(cvData.Description)) cvData.Description = "No description";
                if (cvData.Skills == null || !cvData.Skills.Any()) cvData.Skills = new List<string> { "No skills" };
                if (string.IsNullOrEmpty(cvData.Experience)) cvData.Experience = "No experience";
                if (string.IsNullOrEmpty(cvData.Education)) cvData.Education = "No education";

                return (true, string.Empty, cvData, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting CV data");
                return (true, $"CV extraction failed: {ex.Message}", new CVData
                {
                    Description = "No description",
                    Skills = new List<string> { "No skills" },
                    Experience = "No experience",
                    Education = "No education"
                }, string.Empty);
            }
        }

        private string PreprocessText(string text)
        {
            text = Regex.Replace(text, @"\\u[0-9A-Fa-f]{4}", "");
            text = Regex.Replace(text, @"[\uF000-\uF0FF?]", "");
            text = Regex.Replace(text, @"<[^>]+>|[\r\n]+", " ");
            text = Regex.Replace(text, @"CONTACT.*?(?=\w+|$)", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\s+", " ").Trim();
            return text;
        }

        private bool IsValidJson(string text)
        {
            try
            {
                JsonSerializer.Deserialize<JsonElement>(text);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        private bool IsValidCvData(CVData cvData)
        {
            return cvData != null &&
                   !string.IsNullOrEmpty(cvData.Description) && !cvData.Description.Contains("No description") &&
                   (cvData.Skills != null && cvData.Skills.Any() && !cvData.Skills.Contains("No skills")) &&
                   !string.IsNullOrEmpty(cvData.Experience) && !cvData.Experience.Contains("No experience") &&
                   !string.IsNullOrEmpty(cvData.Education) && !cvData.Education.Contains("No education");
        }

        private CVData FallbackExtractCvData(string text)
        {
            return new CVData
            {
                Description = ExtractDescriptionFallback(text),
                Skills = ExtractSkillsFallback(text),
                Experience = ExtractExperienceFallback(text),
                Education = ExtractEducationFallback(text)
            };
        }

        private string ExtractDescriptionFallback(string text)
        {
            var sentences = Regex.Split(text, @"(?<=[\.!\?])\s+").Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            var description = string.Join(" ", sentences.Take(3));
            return description.Length > 500 ? description.Substring(0, 500) : description;
        }

        private List<string> ExtractSkillsFallback(string text)
        {
            var skills = new List<string>();
            var skillPatterns = new[] { @"(Skills|Kỹ năng)\s*[:\s]*(.*?)(?=\n|$)", @"(experienced|proficient|thành thạo)\s*(.*?)(?=\n|$)" };
            foreach (var pattern in skillPatterns)
            {
                var matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    var skillText = match.Groups.Count > 1 ? match.Groups[1].Value : "";
                    skills.AddRange(skillText.Split(new[] { ',', ' ', '-' }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(s => s.Length > 2 && Regex.IsMatch(s, @"^[a-zA-ZÀ-ỹ0-9\s]+$"))
                        .Distinct());
                }
            }
            return skills.Any() ? skills.Take(4).ToList() : new List<string> { "No skills" };
        }

        private string ExtractExperienceFallback(string text)
        {
            var experienceLines = new List<string>();
            var experiencePatterns = new[] {
                @"(Tháng \d+ năm \d{4} đến Tháng \d+ năm \d{4})\s*\((\d+ năm, \d+ tháng|\d+ năm|\d+ tháng)\)\s*(.*?)(?=\n|$)",
                @"(\d{1,2}\s*years?\s*experience|\d{4}\s*-\s*\d{4})\s*(.*?)(?=\n|$)"
            };
            foreach (var pattern in experiencePatterns)
            {
                var matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    var duration = match.Groups.Count > 1 ? match.Groups[1].Value.Trim() : "";
                    var roleCompany = match.Groups.Count > 2 ? match.Groups[2].Value.Trim() : "";
                    experienceLines.Add($"{roleCompany} {duration}".Trim());
                }
            }
            return string.Join("; ", experienceLines.Distinct().Take(5));
        }

        private string ExtractEducationFallback(string text)
        {
            var educationLines = new List<string>();
            var educationPatterns = new[] {
                @"(Education|Hồ sơ năng lực)\s*[:\s]*(.*?)(?=\n|$)",
                @"(University|Đại học|Bachelor|Cử nhân|Master|PhD)\s*(.*?)(?=\n|$)"
            };
            foreach (var pattern in educationPatterns)
            {
                var matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    var educationText = match.Groups.Count > 1 ? match.Groups[1].Value.Trim() : "";
                    educationLines.Add(educationText);
                }
            }
            return string.Join(", ", educationLines.Distinct().Take(5));
        }
    }









}
