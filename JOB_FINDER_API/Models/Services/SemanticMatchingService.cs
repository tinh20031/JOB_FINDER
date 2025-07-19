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
        private readonly Dictionary<string, (string CleanedText, string[] WeightedTerms, string ContextAnalysis)> _preprocessCache = new();
        private readonly Dictionary<string, (string SummaryText, DateTime ExpiresAt)> _translationCache = new();

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

        public async Task<string> DetectLanguageAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("DetectLanguageAsync called with empty text");
                return "en"; // Default to English
            }

            try
            {
                await EnsureValidToken();
                var prompt = $@"Detect the primary language of the following text. Return only the language code (e.g., 'en' for English, 'vi' for Vietnamese).
Text:
{text}";

                var requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
                var requestJsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await _retryPolicy.ExecuteAsync(async () => await _authenticatedClient.PostAsync(_geminiConfig.ChatEndpoint, requestJsonContent));

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to detect language: {Error}", await response.Content.ReadAsStringAsync());
                    return "en"; // Default to English
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                var languageCode = jsonResponse.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString().Trim();
                return languageCode ?? "en";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error detecting language");
                return "en"; // Default to English
            }
        }

        public async Task<(bool Success, string CleanedText, string[] WeightedTerms, string ContextAnalysis)> PreprocessTextWithGeminiAsync(string text, string originalText = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("Empty text provided to PreprocessTextWithGeminiAsync");
                return (false, string.Empty, Array.Empty<string>(), string.Empty);
            }

            if (_preprocessCache.TryGetValue(text, out var cachedResult))
            {
                _logger.LogInformation("Reusing cached preprocessing result for text: Length={Length}", text.Length);
                return (true, cachedResult.CleanedText, cachedResult.WeightedTerms, cachedResult.ContextAnalysis);
            }

            int maxCacheSize = _configuration.GetValue<int>("Gemini:MaxCacheSize", 1000);
            if (_preprocessCache.Count >= maxCacheSize)
            {
                var oldestKey = _preprocessCache.OrderBy(x => x.Value.CleanedText.Length).First().Key;
                _preprocessCache.Remove(oldestKey);
                _logger.LogInformation("Removed oldest cache entry to maintain size limit: {MaxCacheSize}", maxCacheSize);
            }

            string cleanedText = text;
            string[] weightedTerms = Array.Empty<string>();
            string contextAnalysis = string.Empty;

            int minTextLength = _configuration.GetValue<int>("Gemini:MinTextLengthForAnalysis", 100);
            if (text.Length < minTextLength)
            {
                _logger.LogInformation("Short text detected (Length={Length}), assigning default weights", text.Length);
                weightedTerms = text.Split(' ').Select(s => $"{s} *0.5").ToArray();
                contextAnalysis = "Short text, no deep context analysis";
                _preprocessCache[text] = (cleanedText, weightedTerms, contextAnalysis);
                return (true, cleanedText, weightedTerms, contextAnalysis);
            }

            try
            {
                await EnsureValidToken();
                var prompt = $@"Preprocess this text for semantic analysis in a job matching context (supporting multiple languages, e.g., English, Vietnamese):
- Clean the text by removing irrelevant details (e.g., contact info, formatting) based on configurable rules.
- Identify key weighted terms with their importance (e.g., term *weight, weight from 0.5 to 1.0). Dynamically infer skills and suggest related terms or synonyms based on context (e.g., infer C# from .NET usage, PHP from web development mentions) without relying on a fixed synonym list.
- Analyze context: identify technical skill proficiency levels (e.g., Java (senior), Python (junior)), soft skills (e.g., teamwork, communication), experience type (e.g., project-based, theoretical), education, and relevance to job matching.
Text:
{text}

Response format:
Cleaned Text: [cleaned text]
Weighted Terms: [term1 *weight, term2 *weight, ...]
Context Analysis: [e.g., 'Technical Skills: Java (senior), Python (junior); Soft Skills: teamwork (high), communication (medium); Experience: 2 years project-based; Education: Bachelor in IT; Relevance: highly relevant to software development roles']
";

                var requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
                var requestJsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await _retryPolicy.ExecuteAsync(async () => await _authenticatedClient.PostAsync(_geminiConfig.ChatEndpoint, requestJsonContent));
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseJson = JsonSerializer.Deserialize<JsonNode>(responseContent);
                var responseText = responseJson["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                if (string.IsNullOrWhiteSpace(responseText))
                {
                    _logger.LogWarning("Empty response from Gemini for text preprocessing");
                    return (false, text, Array.Empty<string>(), string.Empty);
                }

                var responseLines = responseText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in responseLines)
                {
                    string trimmedLine = line.Trim();
                    if (trimmedLine.StartsWith("Cleaned Text:", StringComparison.OrdinalIgnoreCase))
                        cleanedText = trimmedLine.Substring("Cleaned Text:".Length).Trim();
                    else if (trimmedLine.StartsWith("Weighted Terms:", StringComparison.OrdinalIgnoreCase))
                        weightedTerms = trimmedLine.Substring("Weighted Terms:".Length).Trim().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray();
                    else if (trimmedLine.StartsWith("Context Analysis:", StringComparison.OrdinalIgnoreCase))
                        contextAnalysis = trimmedLine.Substring("Context Analysis:".Length).Trim();
                }

                // Boost weights for contextually relevant skills
                if (!string.IsNullOrEmpty(contextAnalysis) && weightedTerms.Any())
                {
                    var skillsInContext = Regex.Matches(contextAnalysis, @"Technical Skills:\s*(.*?)(?:;|\z)", RegexOptions.IgnoreCase)
                        .Cast<Match>()
                        .SelectMany(m => m.Groups[1].Value.Split(',').Select(s => s.Trim().Split(' ')[0]))
                        .ToArray();
                    weightedTerms = weightedTerms.Select(term =>
                    {
                        var skill = term.Split('*')[0].Trim();
                        return skillsInContext.Contains(skill) ? $"{skill} *1.0" : term;
                    }).ToArray();
                }

                _preprocessCache[text] = (cleanedText, weightedTerms, contextAnalysis);
                return (true, cleanedText, weightedTerms, contextAnalysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preprocessing text with Gemini");
                return (false, text, Array.Empty<string>(), string.Empty);
            }
        }

        public async Task<(bool Success, string SummaryText, string ErrorMessage)> SummarizeAndTranslate(string text, string targetLanguage)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("SummarizeAndTranslate called with empty text");
                return (false, string.Empty, "Input text is empty");
            }

            string cacheKey = $"{text}:{targetLanguage}";
            if (_translationCache.TryGetValue(cacheKey, out var cachedResult) && cachedResult.ExpiresAt > DateTime.UtcNow)
            {
                _logger.LogInformation("Reusing cached translation: Length={Length}, ExpiresAt={ExpiresAt}", cachedResult.SummaryText.Length, cachedResult.ExpiresAt);
                return (true, cachedResult.SummaryText, string.Empty);
            }

            int maxCacheSize = _configuration.GetValue<int>("Gemini:MaxCacheSize", 1000);
            if (_translationCache.Count >= maxCacheSize)
            {
                var oldestKey = _translationCache.OrderBy(x => x.Value.ExpiresAt).First().Key;
                _translationCache.Remove(oldestKey);
                _logger.LogInformation("Removed oldest cache entry to maintain size limit: {MaxCacheSize}", maxCacheSize);
            }

            try
            {
                await EnsureValidToken();
                int summaryLengthThreshold = _configuration.GetValue<int>("Gemini:SummaryLengthThreshold", 500);
                var prompt = text.Length > summaryLengthThreshold
                    ? $@"Summarize the following text in 50-100 words and translate it to {targetLanguage}. Provide only the summarized text in the response.
Text:
{text}"
                    : $@"Translate the following text to {targetLanguage}. Provide only the translated text in the response.
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

                    int cacheDays = _configuration.GetValue<int>("Gemini:TranslationCacheDays", 30);
                    _translationCache[cacheKey] = (summaryText, DateTime.UtcNow.AddDays(cacheDays));
                    _logger.LogInformation("Cached translation result: Length={Length}, ExpiresAt={ExpiresAt}", summaryText.Length, DateTime.UtcNow.AddDays(cacheDays));

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

                string detectedLanguage = await DetectLanguageAsync(textToProcess);
                if (detectedLanguage != "en")
                {
                    var (translateSuccess, translatedText, translateError) = await SummarizeAndTranslate(textToProcess, "en");
                    if (!translateSuccess)
                    {
                        _logger.LogWarning("Failed to translate CV text to English: {Error}, using original text", translateError);
                    }
                    else
                    {
                        _logger.LogInformation("Translated CV text to English: Length={Length}", translatedText.Length);
                        textToProcess = translatedText;
                    }
                }
                else
                {
                    _logger.LogInformation("CV text is already in English, skipping translation");
                }

                var prompt = $@"Extract the following fields from the provided CV text (supporting multiple languages, e.g., English, Vietnamese): Description, Skills (all relevant skills, no limit, infer skills dynamically from context), Experience (as a single string, joined with semicolons), Education. Use default values if any field cannot be extracted: Description='No description', Skills=['No skills'], Experience='No experience', Education='No education'.
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

                        var jsonElement = JsonSerializer.Deserialize<JsonElement>(text);
                        cvData.Description = jsonElement.GetProperty("Description").GetString() ?? "No description";
                        cvData.Skills = jsonElement.GetProperty("Skills").EnumerateArray().Select(e => e.GetString()).ToList() ?? new List<string> { "No skills" };
                        cvData.Education = jsonElement.GetProperty("Education").GetString() ?? "No education";

                        if (jsonElement.TryGetProperty("Experience", out var experienceProp))
                        {
                            if (experienceProp.ValueKind == JsonValueKind.Array)
                            {
                                var experienceList = experienceProp.EnumerateArray()
                                    .Select(item => item.ValueKind == JsonValueKind.String
                                        ? item.GetString()
                                        : $"{item.GetProperty("dates").GetString()}: {item.GetProperty("title").GetString()} at {item.GetProperty("company").GetString()}. {item.GetProperty("description").GetString()}")
                                    .Where(s => !string.IsNullOrEmpty(s))
                                    .ToList();
                                cvData.Experience = string.Join("; ", experienceList);
                            }
                            else
                            {
                                cvData.Experience = experienceProp.GetString() ?? "No experience";
                            }
                        }
                        else
                        {
                            cvData.Experience = "No experience";
                        }

                        if (string.IsNullOrEmpty(cvData.Experience))
                            cvData.Experience = "No experience";
                        if (cvData.Skills == null || !cvData.Skills.Any())
                            cvData.Skills = new List<string> { "No skills" };

                        _logger.LogInformation("Extracted CV data - Description: {Description}, Skills: {Skills}, Experience: {Experience}, Education: {Education}",
                            cvData.Description, string.Join(", ", cvData.Skills), cvData.Experience, cvData.Education);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Failed to parse Gemini CV extraction response: {ResponseContent}", responseContent);
                        cvData = FallbackExtractCvData(textToProcess);
                    }
                }
                else
                {
                    _logger.LogWarning("Gemini CV extraction failed: {Error}", await response.Content.ReadAsStringAsync());
                    cvData = FallbackExtractCvData(textToProcess);
                }

                return (true, string.Empty, cvData, textToProcess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting CV data");
                return (true, $"CV extraction failed: {ex.Message}", FallbackExtractCvData(textToProcess), string.Empty);
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

            var (success, cleanedText, weightedTerms, _) = await PreprocessTextWithGeminiAsync(text);
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

        public async Task<(bool Success, string ErrorMessage, float[][] Vectors, string JobContext)> GenerateVectorsForCriteria(Job job, string jobText, bool summarize = false)
        {
            if (job == null || string.IsNullOrWhiteSpace(jobText))
            {
                _logger.LogWarning("GenerateVectorsForCriteria called with null job or empty jobText");
                return (false, "Invalid job or jobText", new float[4][], string.Empty);
            }

            _logger.LogInformation("Extracting job content - Description: {Description}, YourSkill: {YourSkill}, YourExperience: {YourExperience}, Education: {Education}",
                job.Description, job.YourSkill, job.YourExperience, job.Education);

            var texts = new[] { job.Description, job.YourSkill, job.YourExperience, job.Education };
            var vectors = new float[4][] { new float[0], new float[0], new float[0], new float[0] };
            int summaryLengthThreshold = _configuration.GetValue<int>("Gemini:SummaryLengthThreshold", 500);

            string processedJobText = jobText;
            string jobContext = string.Empty;
            if (summarize && jobText.Length > summaryLengthThreshold)
            {
                var (summarizeSuccess, summary, error) = await SummarizeAndTranslate(jobText, "en");
                processedJobText = summarizeSuccess ? summary : jobText; // Sử dụng text gốc nếu summarize thất bại
                _logger.LogInformation("Summarized job text: Length={Length}", processedJobText.Length);
            }

            // Preprocess toàn bộ job text để đảm bảo tính nhất quán
            var (preprocessSuccess, cleanedJobText, weightedTerms, contextAnalysis) = await PreprocessTextWithGeminiAsync(processedJobText);
            if (preprocessSuccess)
            {
                jobContext = contextAnalysis;
            }
            else
            {
                _logger.LogWarning("Preprocess failed for job text, using original text");
            }

            var tasks = texts.Select(async (text, i) =>
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    // Sử dụng cùng cleanedJobText để đảm bảo preprocess giống nhau
                    var (vectorSuccess, vectorError, vector) = await PreprocessAndGenerateEmbeddingAsync(
                        string.IsNullOrEmpty(cleanedJobText) ? text : cleanedJobText);
                    return (Index: i, Success: vectorSuccess, Vector: vector, Error: vectorError);
                }
                return (Index: i, Success: false, Vector: new float[0], Error: "Empty text");
            }).ToArray();

            var results = await Task.WhenAll(tasks);
            foreach (var result in results)
            {
                vectors[result.Index] = result.Success && result.Vector != null && result.Vector.Length > 0 ? result.Vector : new float[0];
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to generate vector for job criteria {Index}: {Error}", result.Index, result.Error);
                    ClearPreprocessCache(); // Xóa cache khi vector hóa thất bại
                }
            }

            return (vectors.Any(v => v.Length > 0), vectors.All(v => v.Length == 0) ? "All vectors empty" : string.Empty, vectors, jobContext);
        }

        public async Task<(bool Success, string ErrorMessage, float[][] Vectors, string CvContext)> GenerateVectorsForCVCriteria(CV cv, string cvText, bool summarize = false)
        {
            if (cv == null || string.IsNullOrWhiteSpace(cvText))
            {
                _logger.LogWarning("GenerateVectorsForCVCriteria called with null CV or empty cvText");
                return (false, "Invalid CV or cvText", new float[4][], string.Empty);
            }

            var fullCvText = cv.FullCvJson;
            var (extractSuccess, extractError, cvData, cvSummary) = await ExtractCvDataAsync(cv, fullCvText);
            if (!extractSuccess)
            {
                _logger.LogError("Failed to extract CV data: {Error}", extractError);
                return (false, extractError, new float[4][], string.Empty);
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
            int summaryLengthThreshold = _configuration.GetValue<int>("Gemini:SummaryLengthThreshold", 500);

            string processedCvText = cvText;
            string cvContext = string.Empty;
            if (summarize && cvText.Length > summaryLengthThreshold)
            {
                var (success, summary, error) = await SummarizeAndTranslate(cvText, "en");
                processedCvText = success ? summary : cvText; // Sử dụng text gốc nếu summarize thất bại
                _logger.LogInformation("Summarized CV text: Length={Length}", processedCvText.Length);
            }

            // Preprocess toàn bộ cv text để đảm bảo tính nhất quán
            var (preprocessSuccess, cleanedCvText, weightedTerms, contextAnalysis) = await PreprocessTextWithGeminiAsync(processedCvText);
            if (preprocessSuccess)
            {
                cvContext = contextAnalysis;
            }
            else
            {
                _logger.LogWarning("Preprocess failed for CV text, using original text");
            }

            var tasks = texts.Select(async (text, i) =>
            {
                if (!string.IsNullOrWhiteSpace(text) && !text.Contains("No "))
                {
                    // Sử dụng cùng cleanedCvText để đảm bảo preprocess giống nhau
                    var (vectorSuccess, vectorError, vector) = await PreprocessAndGenerateEmbeddingAsync(
                        string.IsNullOrEmpty(cleanedCvText) ? text : cleanedCvText);
                    return (Index: i, Success: vectorSuccess, Vector: vector, Error: vectorError);
                }
                else
                {
                    string fallbackText = i switch
                    {
                        0 => cvData.Description ?? fullCvText,
                        1 => cvData.Skills != null && cvData.Skills.Any() ? string.Join(" ", cvData.Skills) : fullCvText,
                        2 => cvData.Experience ?? fullCvText,
                        3 => cvData.Education ?? fullCvText,
                        _ => fullCvText
                    };
                    var (vectorSuccess, vectorError, vector) = await PreprocessAndGenerateEmbeddingAsync(fallbackText);
                    return (Index: i, Success: vectorSuccess, Vector: vector, Error: vectorError);
                }
            }).ToArray();

            var results = await Task.WhenAll(tasks);
            foreach (var result in results)
            {
                vectors[result.Index] = result.Success && result.Vector != null && result.Vector.Length > 0 ? result.Vector : new float[0];
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to generate vector for CV criteria {Index}: {Error}", result.Index, result.Error);
                    ClearPreprocessCache(); // Xóa cache khi vector hóa thất bại
                }
            }

            return (vectors.Any(v => v.Length > 0), vectors.All(v => v.Length == 0) ? "All vectors empty" : string.Empty, vectors, cvContext);
        }
        public void ClearPreprocessCache()
        {
            lock (_preprocessCache)
            {
                _preprocessCache.Clear();
                _logger.LogInformation("Preprocess cache cleared.");
            }
        }
        public async Task<(bool Success, string ErrorMessage, float[][] Vectors)> GenerateBatchEmbeddingsAsync(string[] texts)
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

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                try
                {
                    string jobText = $"{job.Description}\n{job.YourSkill}\n{job.YourExperience}\n{job.Education}";
                    var (jobVectorsSuccess, jobVectorsError, jobVectors, jobContext) = await GenerateVectorsForCriteria(job, jobText, summarize: true);
                    if (!jobVectorsSuccess)
                    {
                        _logger.LogError("Failed to generate job vectors: {Error}", jobVectorsError);
                        return (false, jobVectorsError, 0f, 0f, 0f, 0f, 0f, string.Empty);
                    }

                    var (cvVectorsSuccess, cvVectorsError, cvVectors, cvContext) = await GenerateVectorsForCVCriteria(cv, cv.FullCvJson, summarize: true);
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
                    totalSimilarity = Math.Clamp(totalSimilarity / 4, 0f, 1f); // Trung bình các thành phần

                    _logger.LogInformation("Similarity Scores for Job {JobId}: Description={Description:F2}, Skills={Skills:F2}, Experience={Experience:F2}, Education={Education:F2}, Total={Total:F2}",
                        job.JobId, similarityDescription, similaritySkills, similarityExperience, similarityEducation, totalSimilarity);

                    // Không gọi Gemini nữa, sử dụng totalSimilarity đã tính
                    float finalSimilarity = totalSimilarity;
                    string geminiReasoning = "Similarity calculated based on vector cosine similarity with weighted criteria.";

                    return (true, string.Empty, finalSimilarity, similarityDescription, similaritySkills, similarityExperience, similarityEducation, geminiReasoning);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in CalculateTotalSimilarity for Job {JobId}: {Error}", job?.JobId, ex.Message);
                    return (false, $"Similarity calculation failed: {ex.Message}", 0f, 0f, 0f, 0f, 0f, string.Empty);
                }
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
            var skillPatterns = new[] {
                @"(Skills|Kỹ năng)\s*[:\s]*(.*?)(?=\n|$)",
                @"(experienced|proficient|thành thạo)\s*(.*?)(?=\n|$)"
            };
            foreach (var pattern in skillPatterns)
            {
                var matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    var skillText = match.Groups.Count > 1 ? match.Groups[2].Value : match.Groups[1].Value;
                    skills.AddRange(skillText.Split(new[] { ',', ' ', '-' }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(s => s.Length > 2 && Regex.IsMatch(s, @"^[a-zA-ZÀ-ỹ0-9\s]+$"))
                        .Distinct());
                }
            }
            return skills.Any() ? skills : new List<string> { "No skills" };
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