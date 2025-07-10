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
using Microsoft.Extensions.DependencyInjection;

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
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SemanticMatchingService(IOptions<GeminiConfig> geminiConfig, IHttpClientFactory httpClientFactory,
            ILogger<SemanticMatchingService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _geminiConfig = geminiConfig.Value ?? throw new ArgumentNullException(nameof(geminiConfig));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;

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
                            new { text = $"Analyze the following text and determine its primary field (e.g., software development, content marketing). Extract into fields: 'Description:', 'Skills:', 'Experience:' (detailed work experience, e.g., years and roles), 'Education:' focusing ONLY on IT-specific keywords (e.g., Java, RESTful API, software engineering) and minimizing non-IT terms (e.g., marketing, design) unless contextually relevant to IT (e.g., UI/UX). Summarize in {targetLanguage}. Respond ONLY in the format:\nField: [domain]\nDescription: [content]\nSkills: [content]\nExperience: [content]\nEducation: [content]\nSummary: [content]\n\n{text}" }
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

        public async Task<(bool Success, string ErrorMessage, float[] Vector)> PreprocessAndGenerateEmbeddingAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("PreprocessAndGenerateEmbeddingAsync called with empty or whitespace text");
                return (false, "Input text is empty", new float[0]);
            }

            var itKeywords = new HashSet<string> { "java", "python", "c#", "dotnet", "android", "rest", "api", "testing", "sql", "ui", "ux", "development", "software", "engineering", "react", "next.js", "javascript" };
            var nonItKeywords = new HashSet<string> { "marketing", "seo", "social", "design" };

            var words = text.ToLower().Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            var weightedText = string.Join(" ", words.Select(word =>
            {
                if (itKeywords.Contains(word)) return word + " *1.0";
                if (nonItKeywords.Contains(word)) return word + " *0.1";
                return word + " *0.5";
            }));

            if (string.IsNullOrWhiteSpace(weightedText))
            {
                _logger.LogWarning("No processable content found in text after weighting");
                return (false, "No processable content", new float[0]);
            }

            var modelName = "models/text-embedding-004";
            var preprocessedText = weightedText;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                var existingEmbedding = await context.Embeddings
                    .FirstOrDefaultAsync(e => e.Text == preprocessedText && e.Model == modelName);
                if (existingEmbedding != null)
                {
                    _logger.LogInformation("Reusing existing embedding for text: {Text}", preprocessedText);
                    return (true, string.Empty, existingEmbedding.Vector);
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
                        await _authenticatedClient.PostAsync(_geminiConfig.EmbeddingEndpoint, jsonContent));

                    _logger.LogDebug("Embedding API Response: Status={StatusCode}, Reason={ReasonPhrase}", response.StatusCode, response.ReasonPhrase);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError("Embedding API error: {ErrorContent}", errorContent);
                        return (false, $"API Error: {errorContent}", new float[0]);
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
                    context.Embeddings.Add(embeddingEntity);
                    await context.SaveChangesAsync();
                    _logger.LogInformation("New embedding saved for text: {Text}", preprocessedText);

                    return (true, string.Empty, embeddingArray);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Embedding generation error");
                    return (false, $"Embedding generation failed: {ex.Message}", new float[0]);
                }
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
            var tasks = new List<Task<(bool, string, float[])>>();

            for (int i = 0; i < nonNullTexts.Length; i++)
            {
                tasks.Add(PreprocessAndGenerateEmbeddingAsync(nonNullTexts[i]));
            }

            var results = await Task.WhenAll(tasks);
            for (int i = 0; i < results.Length; i++)
            {
                var (success, error, vector) = results[i];
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

        private async Task<(bool Success, string ErrorMessage, float[][] Vectors)> GenerateVectorsForCVCriteria(CV cv)
        {
            try
            {
                var (success, error, cvData, summary) = await ExtractCvDataAsync(cv);
                if (!success)
                {
                    _logger.LogError("Failed to extract CV data: {Error}", error);
                    return (false, error, new float[0][]);
                }

                var texts = new[]
                {
                    cvData.Description,
                    cvData.Skills,
                    cvData.Experience,
                    cvData.Education
                };

                var embeddingsResult = await PreprocessAndGenerateEmbeddingsAsync(texts);
                if (!embeddingsResult.Success)
                {
                    return (false, embeddingsResult.ErrorMessage, new float[0][]);
                }

                return (true, string.Empty, embeddingsResult.Vectors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenerateVectorsForCVCriteria");
                return (false, $"Failed to generate CV vectors: {ex.Message}", new float[0][]);
            }
        }

        private float CalculateDynamicThreshold(float[][] vectors)
        {
            if (vectors == null || !vectors.Any() || vectors[0].Length == 0) return 0.3f;
            var variances = vectors.Select(v => v.Select(x => x * x).Average()).ToArray();
            return Math.Min(0.7f, Math.Max(0.3f, variances.Average() * 0.5f));
        }

        public float CalculateCosineSimilarity(float[] vector1, float[] vector2)
        {
            if (vector1.Length != vector2.Length || vector1.Length == 0) return 0f;
            float dotProduct = vector1.Zip(vector2, (a, b) => a * b).Sum();
            float magnitude1 = (float)Math.Sqrt(vector1.Sum(x => x * x));
            float magnitude2 = (float)Math.Sqrt(vector2.Sum(x => x * x));
            return magnitude1 * magnitude2 == 0 ? 0 : dotProduct / (magnitude1 * magnitude2);
        }

        public async Task<(bool Success, string ErrorMessage, float FinalSimilarity, float SimilarityDescription, float SimilaritySkills, float SimilarityExperience, float SimilarityEducation, string GeminiReasoning)> CalculateTotalSimilarity(Job job, CV cv, string cvSummary, string jobSummary)
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

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                try
                {
                    var jobVectorsResult = await GenerateVectorsForCriteria(job);
                    if (!jobVectorsResult.Success)
                    {
                        _logger.LogError("Failed to generate job vectors: {Error}", jobVectorsResult.ErrorMessage);
                        return (false, jobVectorsResult.ErrorMessage, 0f, 0f, 0f, 0f, 0f, string.Empty);
                    }

                    var cvVectorsResult = await GenerateVectorsForCVCriteria(cv);
                    if (!cvVectorsResult.Success)
                    {
                        _logger.LogError("Failed to generate CV vectors: {Error}", cvVectorsResult.ErrorMessage);
                        return (false, cvVectorsResult.ErrorMessage, 0f, 0f, 0f, 0f, 0f, string.Empty);
                    }

                    float dynamicThreshold = CalculateDynamicThreshold(jobVectorsResult.Vectors);

                    var similarities = new[]
                    {
                        CalculateSimilarityWithContext(jobVectorsResult.Vectors[0], cvVectorsResult.Vectors[0], job.DescriptionWeight, dynamicThreshold),
                        CalculateSimilarityWithContext(jobVectorsResult.Vectors[1], cvVectorsResult.Vectors[1], job.SkillsWeight, dynamicThreshold),
                        CalculateSimilarityWithContext(jobVectorsResult.Vectors[2], cvVectorsResult.Vectors[2], job.ExperienceWeight, dynamicThreshold),
                        CalculateSimilarityWithContext(jobVectorsResult.Vectors[3], cvVectorsResult.Vectors[3], job.EducationWeight, dynamicThreshold)
                    };

                    float totalSimilarity = similarities.Sum(s => s);
                    _logger.LogInformation("Initial Similarity Scores for Job {JobId}: Description={Description:F2}, Skills={Skills:F2}, Experience={Experience:F2}, Education={Education:F2}, Total={Total:F2}",
                        job.JobId, similarities[0], similarities[1], similarities[2], similarities[3], totalSimilarity);

                    float finalSimilarity = totalSimilarity;
                    string geminiReasoning = string.Empty;

                    if (totalSimilarity >= _geminiConfig.SimilarityThreshold)
                    {
                        _logger.LogInformation("Total similarity {TotalSimilarity:F2} exceeds threshold {_SimilarityThreshold:F2}, calling Gemini for validation", totalSimilarity, _geminiConfig.SimilarityThreshold);
                        var prompt = $"Analyze the following CV and job description to determine their similarity. Assign a similarity score (0 to 1) based on how well the CV matches the job requirements, focusing on skills, experience, and education. Provide detailed reasoning for the score.\n\nCV: {cv.FullCvJson}\nJob Description: {job.Description}\n\nResponse format:\nSimilarity Score: [0-1]\nReasoning: [Detailed explanation]";
                        var requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
                        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                        var response = await _retryPolicy.ExecuteAsync(async () => await _authenticatedClient.PostAsync(_geminiConfig.ChatEndpoint, jsonContent));
                        if (!response.IsSuccessStatusCode)
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            _logger.LogError("Gemini API error: {ErrorContent}", errorContent);
                        }
                        else
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                            var text = jsonResponse.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
                            var scoreLine = text.Split('\n').FirstOrDefault(l => l.StartsWith("Similarity Score:"));
                            var reasoningLine = text.Split('\n').FirstOrDefault(l => l.StartsWith("Reasoning:"));

                            finalSimilarity = float.TryParse(scoreLine?.Substring("Similarity Score:".Length).Trim(), out float s) ? s : totalSimilarity;
                            geminiReasoning = reasoningLine?.Substring("Reasoning:".Length).Trim() ?? "No reasoning provided";
                            _logger.LogInformation("Gemini adjusted similarity to {FinalSimilarity:F2} with reasoning: {Reasoning}", finalSimilarity, geminiReasoning);
                        }
                    }

                    return (true, string.Empty, finalSimilarity, similarities[0], similarities[1], similarities[2], similarities[3], geminiReasoning);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error in CalculateTotalSimilarity for Job {JobId}: {Error}", job?.JobId, ex.Message);
                    return (false, $"Similarity calculation failed: {ex.Message}", 0f, 0f, 0f, 0f, 0f, string.Empty);
                }
            }
        }

        private float CalculateSimilarityWithContext(float[] jobVector, float[] cvVector, float weight, float threshold)
        {
            if (jobVector == null || cvVector == null || jobVector.Length == 0 || cvVector.Length == 0 || jobVector.Length != cvVector.Length)
            {
                _logger.LogWarning("Invalid vectors detected, returning 0 similarity.");
                return 0f;
            }

            float rawSimilarity = CalculateCosineSimilarity(jobVector, cvVector);
            return rawSimilarity > threshold ? rawSimilarity * weight : 0f;
        }
    }


}

