using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using Polly;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JOB_FINDER_API.Data;
using Microsoft.EntityFrameworkCore;

namespace JOB_FINDER_API.Models.Services
{
    public class SemanticMatchingService
    {
        private readonly GeminiConfig _geminiConfig;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAsyncPolicy _retryPolicy;
        private readonly ILogger<SemanticMatchingService> _logger;
        private HttpClient _authenticatedClient;
        private string _accessToken;
        private readonly JobFinderDbContext _context;

        public SemanticMatchingService(IOptions<GeminiConfig> geminiConfig, IHttpClientFactory httpClientFactory,
            ILogger<SemanticMatchingService> logger, JobFinderDbContext context)
        {
            _geminiConfig = geminiConfig.Value ?? throw new ArgumentNullException(nameof(geminiConfig));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger;
            _context = context;
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning("Retry {RetryCount} after {TimeSpan}s due to error: {Error}", retryCount, timeSpan.TotalSeconds, exception.Message);
                    });

            if (string.IsNullOrEmpty(_geminiConfig.ChatEndpoint) || string.IsNullOrEmpty(_geminiConfig.EmbeddingEndpoint))
            {
                throw new ArgumentException("ChatEndpoint and EmbeddingEndpoint must be configured in appsettings.json");
            }

            _authenticatedClient = _httpClientFactory.CreateClient();
            _accessToken = GetAccessTokenFromServiceAccount().Result;
            if (string.IsNullOrEmpty(_accessToken))
            {
                throw new ArgumentException("Failed to retrieve AccessToken from Service Account.");
            }
            _authenticatedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }

        private async Task<string> GetAccessTokenFromServiceAccount()
        {
            if (!string.IsNullOrEmpty(_accessToken)) return _accessToken;

            var credential = GoogleCredential.FromFile(_geminiConfig.ServiceAccountKeyPath)
                .CreateScoped(new[] { "https://www.googleapis.com/auth/generative-language", "https://www.googleapis.com/auth/cloud-language" });
            var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            return token;
        }

        public async Task<(bool Success, string TranslatedText, CVData CVData)> TranslateText(string text, string targetLanguage = "en")
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("TranslateText called with empty or whitespace text");
                return (false, "Input text is empty", new CVData());
            }

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = $"Detect the language of the following text and translate it into {targetLanguage}. Response format: Translated text: ...\n\n{text}" }
                        }
                    }
                }
            };

            try
            {
                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                _logger.LogDebug("Translate Request Body: {RequestBody}", JsonSerializer.Serialize(requestBody));
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _authenticatedClient.PostAsync(_geminiConfig.ChatEndpoint, jsonContent));

                _logger.LogDebug("Translate API Response: Status={StatusCode}, Reason={ReasonPhrase}", response.StatusCode, response.ReasonPhrase);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Gemini API error during translation: {ErrorContent}", errorContent);
                    return (false, $"Unable to translate: {errorContent}", new CVData());
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                var translatedText = jsonResponse.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? text;

                var translatedLines = translatedText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                string finalTranslatedText = translatedLines.FirstOrDefault(l => l.Trim().StartsWith("Translated text:"))?.Substring("Translated text:".Length).Trim() ?? text;

                return (true, finalTranslatedText, new CVData());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during translation");
                return (false, $"Translation error: {ex.Message}", new CVData());
            }
        }

        public async Task<(bool Success, string Summary, CVData CVData)> SummarizeAndTranslate(string text, string targetLanguage = "en")
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("SummarizeAndTranslate called with empty or whitespace text");
                return (false, "Input text is empty", new CVData());
            }

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = $"Analyze the following text and determine its primary field (e.g., software development, content marketing). Assign an IT-relevance score (0 to 1) indicating how closely it relates to Information Technology (1 = fully IT-related, 0 = not IT-related). Extract into fields: 'Description:', 'Skills:', 'Experience:' (detailed work experience, e.g., years and roles), 'Education:' focusing ONLY on IT-specific keywords (e.g., Java, RESTful API, software engineering) and minimizing non-IT terms (e.g., marketing, design) unless contextually relevant to IT (e.g., UI/UX). Summarize in {targetLanguage}. Respond ONLY in the format:\nField: [domain]\nIT-Relevance: [score]\nDescription: [content]\nSkills: [content]\nExperience: [content]\nEducation: [content]\nSummary: [content]\n\n{text}" }
                        }
                    }
                }
            };

            try
            {
                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                _logger.LogDebug("Summarize Request Body: {RequestBody}", JsonSerializer.Serialize(requestBody));
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _authenticatedClient.PostAsync(_geminiConfig.ChatEndpoint, jsonContent));

                _logger.LogDebug("Summarize API Response: Status={StatusCode}, Reason={ReasonPhrase}", response.StatusCode, response.ReasonPhrase);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Gemini API error during summarization: {ErrorContent}", errorContent);
                    return (false, $"Unable to summarize: {errorContent}", new CVData());
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                var summaryText = jsonResponse.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? "Unable to summarize";

                _logger.LogDebug("Summarize Response: {SummaryText}", summaryText);
                var cvData = ParseGeminiResponseForCVData(summaryText);
                return (true, summaryText, cvData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during summarization and translation");
                return (false, $"Summarization error: {ex.Message}", new CVData());
            }
        }

        private CVData ParseGeminiResponseForCVData(string summary)
        {
            var cvData = new CVData();
            var lines = summary.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            string currentField = null;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (trimmedLine.StartsWith("Field:", StringComparison.OrdinalIgnoreCase))
                {
                    cvData.Field = trimmedLine.Substring("Field:".Length).Trim();
                }
                else if (trimmedLine.StartsWith("IT-Relevance:", StringComparison.OrdinalIgnoreCase))
                {
                    cvData.ITRelevance = float.TryParse(trimmedLine.Substring("IT-Relevance:".Length).Trim(), out float score) ? score : 1.0f;
                }
                else if (trimmedLine.StartsWith("Description:", StringComparison.OrdinalIgnoreCase))
                {
                    currentField = "Description";
                    cvData.Description = trimmedLine.Substring("Description:".Length).Trim();
                }
                else if (trimmedLine.StartsWith("Skills:", StringComparison.OrdinalIgnoreCase))
                {
                    currentField = "Skills";
                    cvData.Skills = trimmedLine.Substring("Skills:".Length).Trim();
                }
                else if (trimmedLine.StartsWith("Experience:", StringComparison.OrdinalIgnoreCase))
                {
                    currentField = "Experience";
                    cvData.Experience = trimmedLine.Substring("Experience:".Length).Trim();
                }
                else if (trimmedLine.StartsWith("Education:", StringComparison.OrdinalIgnoreCase))
                {
                    currentField = "Education";
                    cvData.Education = trimmedLine.Substring("Education:".Length).Trim();
                }
                else if (trimmedLine.StartsWith("Summary:", StringComparison.OrdinalIgnoreCase))
                {
                    currentField = "Summary";
                    cvData.Summary = trimmedLine.Substring("Summary:".Length).Trim();
                }
                else if (currentField != null)
                {
                    switch (currentField)
                    {
                        case "Description": cvData.Description += " " + trimmedLine; break;
                        case "Skills": cvData.Skills += " " + trimmedLine; break;
                        case "Experience": cvData.Experience += " " + trimmedLine; break;
                        case "Education": cvData.Education += " " + trimmedLine; break;
                        case "Summary": cvData.Summary += " " + trimmedLine; break;
                    }
                }
            }

            cvData.Field = string.IsNullOrEmpty(cvData.Field) ? "Unknown" : cvData.Field;
            cvData.ITRelevance = cvData.ITRelevance > 0 ? cvData.ITRelevance : 1.0f;
            cvData.Description = string.IsNullOrEmpty(cvData.Description) ? "No description provided." : cvData.Description;
            cvData.Skills = string.IsNullOrEmpty(cvData.Skills) ? "No skills provided." : cvData.Skills;
            cvData.Experience = string.IsNullOrEmpty(cvData.Experience) ? "No experience provided." : cvData.Experience;
            cvData.Education = string.IsNullOrEmpty(cvData.Education) ? "No education provided." : cvData.Education;
            cvData.Summary = string.IsNullOrEmpty(cvData.Summary) ? "No summary provided." : cvData.Summary;

            return cvData;
        }

        public async Task<(bool Success, string ErrorMessage, CVData CVData, string Summary)> ExtractCvDataAsync(CV cv, string extractedText = null)
        {
            try
            {
                CVData cvData = new CVData();
                string cvSummary = string.Empty;

                if (!string.IsNullOrWhiteSpace(extractedText))
                {
                    var (summarySuccess, summaryText, summaryCvData) = await SummarizeAndTranslate(extractedText, "en");
                    if (summarySuccess && IsValidCvData(summaryCvData))
                    {
                        cvData = summaryCvData;
                        cvSummary = summaryText;
                        _logger.LogInformation("Extracted valid CV data from new text");
                    }
                    else
                    {
                        _logger.LogWarning("Invalid CV data from new text, using fallback");
                        cvData.Description = extractedText.Length > 500 ? extractedText.Substring(0, 500) : extractedText;
                        cvSummary = cvData.Description;
                    }
                }
                else if (cv != null && !string.IsNullOrEmpty(cv.FullCvJson))
                {
                    var jsonContent = JsonSerializer.Deserialize<JsonElement>(cv.FullCvJson);
                    if (jsonContent.TryGetProperty("CVData", out var cvDataElement))
                    {
                        cvData = JsonSerializer.Deserialize<CVData>(cvDataElement.GetRawText()) ?? new CVData();
                        cvSummary = jsonContent.TryGetProperty("Summary", out var summaryElement) ? summaryElement.GetString() ?? string.Empty : string.Empty;
                    }

                    if (!IsValidCvData(cvData))
                    {
                        var rawText = jsonContent.TryGetProperty("Text", out var textElement) ? textElement.GetString() ?? string.Empty : string.Empty;
                        if (!string.IsNullOrWhiteSpace(rawText))
                        {
                            var (summarySuccess, summaryText, summaryCvData) = await SummarizeAndTranslate(rawText, "en");
                            if (summarySuccess)
                            {
                                cvData = summaryCvData;
                                cvSummary = summaryText;
                                _logger.LogInformation("Extracted valid CV data from FullCvJson after summarization");
                            }
                            else
                            {
                                cvData.Description = rawText.Length > 500 ? rawText.Substring(0, 500) : rawText;
                                cvSummary = cvData.Description;
                                _logger.LogWarning("Summarization failed for FullCvJson, using fallback");
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No processable CV data found");
                    return (false, "No processable CV data", new CVData(), string.Empty);
                }

                return (true, string.Empty, cvData, cvSummary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting CV data");
                return (false, $"Failed to extract CV data: {ex.Message}", new CVData(), string.Empty);
            }
        }

        private bool IsValidCvData(CVData cvData)
        {
            return !string.IsNullOrEmpty(cvData.Description) && !cvData.Description.Contains("No description") &&
                   !string.IsNullOrEmpty(cvData.Skills) && !cvData.Skills.Contains("No skills") &&
                   !string.IsNullOrEmpty(cvData.Experience) && !cvData.Experience.Contains("No experience") &&
                   !string.IsNullOrEmpty(cvData.Education) && !cvData.Education.Contains("No education");
        }

        public async Task<(bool Success, string ErrorMessage, float[] Vector, float ITRelevance)> PreprocessAndGenerateEmbeddingAsync(string text, float itRelevance)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("PreprocessAndGenerateEmbeddingAsync called with empty or whitespace text");
                return (false, "Input text is empty", new float[0], itRelevance);
            }

            var itKeywords = new HashSet<string> { "java", "python", "c#", "dotnet", "android", "rest", "api", "testing", "sql", "ui", "ux", "development", "software", "engineering" };
            var nonItKeywords = new HashSet<string> { "marketing", "seo", "social", "design" };

            var words = text.ToLower().Split();
            var weightedText = string.Join(" ", words.Select(word =>
            {
                if (itKeywords.Contains(word)) return word + " ";
                if (nonItKeywords.Contains(word)) return word + $" *{Math.Max(0.1f, itRelevance * 0.5f)}";
                return word + " *0.5";
            }));

            if (string.IsNullOrWhiteSpace(weightedText))
            {
                _logger.LogWarning("No processable content found in text after weighting");
                return (false, "No processable content", new float[0], itRelevance);
            }

            var modelName = "models/text-embedding-004";
            var preprocessedText = weightedText;
            var existingEmbedding = await _context.Embeddings
                .FirstOrDefaultAsync(e => e.Text == preprocessedText && e.Model == modelName);
            if (existingEmbedding != null)
            {
                _logger.LogInformation("Reusing existing embedding for text: {Text}", preprocessedText);
                return (true, string.Empty, existingEmbedding.Vector, itRelevance);
            }

            try
            {
                var requestBody = new
                {
                    model = modelName,
                    content = new { parts = new[] { new { text = preprocessedText } } }
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                _logger.LogDebug("Embedding Request Body: {RequestBody}", JsonSerializer.Serialize(requestBody));
                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _authenticatedClient.PostAsync("https://generativelanguage.googleapis.com/v1beta/models/text-embedding-004:embedContent", jsonContent));

                _logger.LogDebug("Embedding API Response: Status={StatusCode}, Reason={ReasonPhrase}", response.StatusCode, response.ReasonPhrase);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Embedding API error: {ErrorContent}", errorContent);
                    return (false, $"API Error: {errorContent}", new float[0], itRelevance);
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var geminiResult = JsonSerializer.Deserialize<JsonElement>(responseContent);
                var embeddingArray = geminiResult.GetProperty("embedding").GetProperty("values").EnumerateArray()
                    .Select(e => e.GetSingle()).ToArray();

                var embeddingEntity = new Embedding
                {
                    Text = preprocessedText,
                    Model = modelName,
                    Vector = embeddingArray,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Embeddings.Add(embeddingEntity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("New embedding saved for text: {Text}", preprocessedText);

                return (true, string.Empty, embeddingArray, itRelevance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Embedding generation error");
                return (false, $"Embedding generation failed: {ex.Message}", new float[0], itRelevance);
            }
        }

        public async Task<(bool Success, string ErrorMessage, float[][] Vectors)> PreprocessAndGenerateEmbeddingsAsync(string[] texts)
        {
            if (texts == null || !texts.Any())
            {
                _logger.LogWarning("PreprocessAndGenerateEmbeddingsAsync called with empty or null texts");
                return (false, "Input texts are empty or missing", new float[0][]);
            }

            var nonNullTexts = texts.Select(t => t ?? string.Empty).ToArray();
            var embeddings = new float[nonNullTexts.Length][];

            for (int i = 0; i < nonNullTexts.Length; i++)
            {
                var (success, error, vector, itRelevance) = await PreprocessAndGenerateEmbeddingAsync(nonNullTexts[i], 1.0f);
                if (!success)
                {
                    _logger.LogError("Failed to generate embedding for text at index {Index}: {Error}", i, error);
                    return (false, error, new float[0][]);
                }
                embeddings[i] = vector;
            }

            return (true, string.Empty, embeddings);
        }

        private async Task<(bool Success, string ErrorMessage, float[][] Vectors)> GenerateVectorsForCriteria(Job job)
        {
            var texts = new[]
            {
                job.Description ?? string.Empty,
                job.YourSkill ?? string.Empty,
                job.YourExperience ?? string.Empty,
                job.Education ?? string.Empty
            };
            return await PreprocessAndGenerateEmbeddingsAsync(texts);
        }

        private async Task<(bool Success, string ErrorMessage, float[][] Vectors, float ITRelevance)> GenerateVectorsForCVCriteria(CV cv)
        {
            try
            {
                var (success, error, cvData, _) = await ExtractCvDataAsync(cv);
                if (!success)
                {
                    _logger.LogError("Failed to extract CV data: {Error}", error);
                    return (false, error, new float[0][], 1.0f);
                }

                var texts = new[]
                {
                    cvData.Description,
                    cvData.Skills,
                    cvData.Experience,
                    cvData.Education
                };

                _logger.LogDebug("CV texts for embedding: {Texts}", string.Join(" | ", texts.Select(t => t.Length > 50 ? t.Substring(0, 50) + "..." : t)));
                var embeddingsResult = await PreprocessAndGenerateEmbeddingsAsync(texts);
                if (!embeddingsResult.Success)
                {
                    return (false, embeddingsResult.ErrorMessage, new float[0][], cvData.ITRelevance);
                }

                return (true, string.Empty, embeddingsResult.Vectors, cvData.ITRelevance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenerateVectorsForCVCriteria");
                return (false, $"Failed to generate CV vectors: {ex.Message}", new float[0][], 1.0f);
            }
        }

        private float[] CombineVectors(float[][] vectors)
        {
            if (vectors == null || vectors.Length == 0 || vectors[0].Length == 0)
                return new float[0];

            int dimension = vectors[0].Length;
            var combined = new float[dimension];
            for (int i = 0; i < dimension; i++)
            {
                combined[i] = vectors.Average(v => v[i]);
            }
            return combined;
        }

        public float CalculateCosineSimilarity(float[] vector1, float[] vector2)
        {
            if (vector1.Length != vector2.Length || vector1.Length == 0) return 0f;
            float dotProduct = vector1.Zip(vector2, (a, b) => a * b).Sum();
            float magnitude1 = (float)Math.Sqrt(vector1.Sum(x => x * x));
            float magnitude2 = (float)Math.Sqrt(vector2.Sum(x => x * x));
            return magnitude1 * magnitude2 == 0 ? 0 : dotProduct / (magnitude1 * magnitude2);
        }

        public async Task<(bool Success, string ErrorMessage, float TotalSimilarity, float SimilarityDescription, float SimilaritySkills, float SimilarityExperience, float SimilarityEducation)> CalculateTotalSimilarity(Job job, CV cv, string cvSummary, string jobSummary)
        {
            float totalWeight = job.DescriptionWeight + job.SkillsWeight + job.ExperienceWeight + job.EducationWeight;
            if (Math.Abs(totalWeight - 1.0f) > 0.0001f)
            {
                _logger.LogWarning("Invalid weights for Job {JobId}, total weight {TotalWeight:F2} is not 1.0, using default weights", job.JobId, totalWeight);
                job.DescriptionWeight = 0.4f;
                job.SkillsWeight = 0.3f;
                job.ExperienceWeight = 0.2f;
                job.EducationWeight = 0.1f;
            }

            try
            {
                var jobVectorsResult = await GenerateVectorsForCriteria(job);
                if (!jobVectorsResult.Success)
                {
                    _logger.LogError("Failed to generate job vectors: {Error}", jobVectorsResult.ErrorMessage);
                    return (false, jobVectorsResult.ErrorMessage, 0f, 0f, 0f, 0f, 0f);
                }

                var cvVectorsResult = await GenerateVectorsForCVCriteria(cv);
                if (!cvVectorsResult.Success)
                {
                    _logger.LogError("Failed to generate CV vectors: {Error}", cvVectorsResult.ErrorMessage);
                    return (false, cvVectorsResult.ErrorMessage, 0f, 0f, 0f, 0f, 0f);
                }

                float itRelevance = cvVectorsResult.ITRelevance;
                if (itRelevance < 0.1)
                {
                    _logger.LogInformation("CV for Job {JobId} has low IT relevance ({IT-Relevance:F2}), similarity will be minimal", job.JobId, itRelevance);
                }

                float rawSimilarityDescription = cvVectorsResult.Vectors.Length > 0 ? CalculateCosineSimilarity(jobVectorsResult.Vectors[0], cvVectorsResult.Vectors[0]) : 0f;
                float similarityDescription = rawSimilarityDescription > 0.3 ? rawSimilarityDescription * job.DescriptionWeight * Math.Max(0.1f, itRelevance) : 0;

                float rawSimilaritySkills = cvVectorsResult.Vectors.Length > 1 ? CalculateCosineSimilarity(jobVectorsResult.Vectors[1], cvVectorsResult.Vectors[1]) : 0f;
                float similaritySkills = rawSimilaritySkills > 0.3 ? rawSimilaritySkills * job.SkillsWeight * Math.Max(0.1f, itRelevance) : 0;

                float rawSimilarityExperience = cvVectorsResult.Vectors.Length > 2 ? CalculateCosineSimilarity(jobVectorsResult.Vectors[2], cvVectorsResult.Vectors[2]) : 0f;
                float similarityExperience = rawSimilarityExperience > 0.3 ? rawSimilarityExperience * job.ExperienceWeight * Math.Max(0.1f, itRelevance) : 0;

                float rawSimilarityEducation = cvVectorsResult.Vectors.Length > 3 ? CalculateCosineSimilarity(jobVectorsResult.Vectors[3], cvVectorsResult.Vectors[3]) : 0f;
                float similarityEducation = rawSimilarityEducation > 0.3 ? rawSimilarityEducation * job.EducationWeight * Math.Max(0.1f, itRelevance) : 0;

                float totalSimilarity = similarityDescription + similaritySkills + similarityExperience + similarityEducation;
                _logger.LogInformation("Similarity Score for Job {JobId}: IT-Relevance={IT-Relevance:F2}, Description={Description:F2}, Skills={Skills:F2}, Experience={Experience:F2}, Education={Education:F2}, Total={Total:F2}",
                    job.JobId, itRelevance, similarityDescription, similaritySkills, similarityExperience, similarityEducation, totalSimilarity);

                return (true, string.Empty, totalSimilarity, similarityDescription, similaritySkills, similarityExperience, similarityEducation);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in CalculateTotalSimilarity for Job {JobId}: {Error}", job.JobId, ex.Message);
                return (false, $"Similarity calculation failed: {ex.Message}", 0f, 0f, 0f, 0f, 0f);
            }
        }
    }



  
}