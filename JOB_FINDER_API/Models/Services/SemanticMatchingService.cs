using Google.Apis.Auth.OAuth2;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _cache;
        private HttpClient _authenticatedClient;
        private string _accessToken;
        private DateTime _tokenExpiration;
        private static readonly SemaphoreSlim _tokenLock = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim _rateLimitSemaphore = new SemaphoreSlim(10, 10);
        private const string TokenCacheKey = "GeminiAccessToken";
        public SemanticMatchingService(
              ILogger<SemanticMatchingService> logger,
              IServiceScopeFactory serviceScopeFactory,
              IConfiguration configuration,
              IHttpClientFactory httpClientFactory,
              IMemoryCache cache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
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
            InitializeToken().GetAwaiter().GetResult();
        }

        private async Task InitializeToken()
        {
            _accessToken = await GetAccessTokenFromServiceAccount();
            if (string.IsNullOrEmpty(_accessToken))
            {
                _logger.LogError("Failed to retrieve access token from Service Account.");
                throw new InvalidOperationException("Failed to retrieve AccessToken from Service Account.");
            }
            _authenticatedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            _tokenExpiration = DateTime.UtcNow.AddHours(1);
            _cache.Set(TokenCacheKey, (_accessToken, _tokenExpiration), TimeSpan.FromHours(1));
            _logger.LogInformation("Access Token initialized: {TokenPrefix}, Expires: {Expiration}", _accessToken.Substring(0, 10) + "...", _tokenExpiration);
        }

        private async Task<string> GetAccessTokenFromServiceAccount()
        {
            await _rateLimitSemaphore.WaitAsync();
            try
            {
                var retryPolicy = Policy<string>
                    .Handle<Exception>()
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (result, timeSpan, retryCount, context) =>
                        {
                            _logger.LogWarning("Retry {RetryCount} after {TimeSpan}s due to error: {Error}",
                                retryCount, timeSpan.TotalSeconds, result.Exception?.Message ?? "Unknown error");
                        });

                return await retryPolicy.ExecuteAsync(async () =>
                {
                    var credential = GoogleCredential.FromFile(_geminiConfig.ServiceAccountKeyPath)
                        .CreateScoped(new[] { "https://www.googleapis.com/auth/generative-language", "https://www.googleapis.com/auth/cloud-language" });
                    var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
                    _logger.LogInformation("Successfully retrieved access token from Service Account.");
                    return token;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve access token from Service Account.");
                return null;
            }
            finally
            {
            
                _ = Task.Delay(TimeSpan.FromSeconds(6)).ContinueWith(_ => _rateLimitSemaphore.Release());
            }
        }
        private async Task EnsureValidToken()
        {
            if (_cache.TryGetValue(TokenCacheKey, out (string Token, DateTime Expiration) cachedToken) &&
                DateTime.UtcNow < cachedToken.Expiration.AddMinutes(-5))
            {
                _accessToken = cachedToken.Token;
                _tokenExpiration = cachedToken.Expiration;
                _authenticatedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                _logger.LogInformation("Using cached token: {TokenPrefix}, Expires: {Expiration}", _accessToken.Substring(0, 10) + "...", _tokenExpiration);
                return;
            }

            await _tokenLock.WaitAsync();
            try
            {
                if (!_cache.TryGetValue(TokenCacheKey, out cachedToken) ||
                    DateTime.UtcNow >= cachedToken.Expiration.AddMinutes(-5))
                {
                    _logger.LogInformation("Refreshing access token due to impending expiration.");
                    await InitializeToken();
                }
            }
            finally
            {
                _tokenLock.Release();
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
                if (string.IsNullOrEmpty(_accessToken))
                {
                    _logger.LogError("Access token is null or empty");
                    return "en";
                }

                var prompt = $@"Detect the primary language of the following text. Return only the language code (e.g., 'en' for English, 'vi' for Vietnamese).
Text:
{text}";

                var requestBody = new
                {
                    contents = new[] { new { parts = new[] { new { text = prompt } } } },
                    generationConfig = new
                    {
                        temperature = 0.0f,
                        topP = 1.0f,
                        topK = 1,
                        candidateCount = 1
                    }
                };

                var requestJsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await _retryPolicy.ExecuteAsync(async () => await _authenticatedClient.PostAsync(_geminiConfig.ChatEndpoint, requestJsonContent));

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to detect language: {Error}", errorContent);
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

        public async Task<(bool Success, string CleanedText, string ContextAnalysis)> PreprocessTextWithGeminiAsync(string text, string originalText = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("Empty text provided to PreprocessTextWithGeminiAsync");
                return (false, string.Empty, string.Empty);
            }

            var normalizedText = NormalizeText(text);
            _logger.LogInformation("Preprocessing text with Gemini API: Length={Length}", normalizedText.Length);

            string cleanedText = normalizedText;
            string contextAnalysis = string.Empty;

            int minTextLength = _configuration.GetValue<int>("Gemini:MinTextLengthForAnalysis", 100);
            if (normalizedText.Length < minTextLength)
            {
                _logger.LogInformation("Short text detected (Length={Length}), skipping detailed analysis", normalizedText.Length);
                contextAnalysis = "Short text, no deep context analysis";
                return (true, cleanedText, contextAnalysis);
            }

            try
            {
                await EnsureValidToken();
                if (string.IsNullOrEmpty(_accessToken))
                {
                    _logger.LogError("Access token is null or empty");
                    return (false, normalizedText, string.Empty);
                }

                var prompt = $@"Preprocess this text for semantic analysis in a job matching context (supporting multiple languages, e.g., English, Vietnamese):
- Clean the text by removing irrelevant details (e.g., contact info, formatting) based on configurable rules.
- Analyze context: identify technical skill proficiency levels (e.g., Java (senior), Python (junior)), soft skills (e.g., teamwork, communication), experience type (e.g., project-based, theoretical), education, and relevance to job matching.
Text:
{normalizedText}

Response format:
Cleaned Text: [cleaned text]
Context Analysis: [e.g., 'Technical Skills: Java (senior), Python (junior); Soft Skills: teamwork (high), communication (medium); Experience: 2 years project-based; Education: Bachelor in IT; Relevance: highly relevant to software development roles']
";

                var requestBody = new
                {
                    contents = new[] { new { parts = new[] { new { text = prompt } } } },
                    generationConfig = new
                    {
                        temperature = 0.0f,
                        topP = 1.0f,
                        topK = 1,
                        candidateCount = 1
                    }
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
                    return (false, normalizedText, string.Empty);
                }

                var responseLines = responseText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in responseLines)
                {
                    string trimmedLine = line.Trim();
                    if (trimmedLine.StartsWith("Cleaned Text:", StringComparison.OrdinalIgnoreCase))
                        cleanedText = trimmedLine.Substring("Cleaned Text:".Length).Trim();
                    else if (trimmedLine.StartsWith("Context Analysis:", StringComparison.OrdinalIgnoreCase))
                        contextAnalysis = trimmedLine.Substring("Context Analysis:".Length).Trim();
                }

                return (true, cleanedText, contextAnalysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preprocessing text with Gemini");
                return (false, normalizedText, string.Empty);
            }
        }


        public async Task<(bool Success, string Summary, CVData CVData)> SummarizeAndTranslate(string text, string targetLanguage = "en")
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("SummarizeAndTranslate called with empty or whitespace text");
                return (false, "Input text is empty", new CVData());
            }

            try
            {
                await EnsureValidToken();
                if (string.IsNullOrEmpty(_accessToken))
                {
                    _logger.LogError("Access token is null or empty");
                    return (false, "Invalid access token", new CVData());
                }

                var requestBody = new
                {
                    contents = new[]
                    {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = $@"Analyze the following CV text and extract the following fields in a job matching context:
- Description: A brief summary of the candidate's profile or objective (50-100 words).
- Skills: A list of all relevant skills (technical and soft skills, no limit on number of skills, comma-separated).
- Experience: A summary of relevant work experience, including years and roles (50-100 words).
- Education: A summary of educational background, focusing on IT-specific keywords (e.g., Java, RESTful API, software engineering) and minimizing non-IT terms unless contextually relevant (50-100 words).
Respond ONLY in the format:
Description: [content]
Skills: [content]
Experience: [content]
Education: [content]

CV text:
{text}"
                        }
                    }
                }
            },
                    generationConfig = new
                    {
                        temperature = 0.0f,
                        topP = 1.0f,
                        topK = 1,
                        candidateCount = 1
                    }
                };

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
                var responseText = jsonResponse.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? "Unable to summarize";

                _logger.LogDebug("Summarize Response: {SummaryText}", responseText);
                var cvData = ParseGeminiResponseForCVData(responseText);
                return (true, responseText, cvData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during summarization and translation");
                return (false, $"Summarization error: {ex.Message}", new CVData());
            }
        }


        public async Task<(bool Success, string Error, CVData CVData)> ExtractCvDataAsync(CV cv, string extractedText = null)
        {
            try
            {
                string inputText = extractedText;
                if (inputText == null && cv != null)
                {
                    if (!IsValidJson(cv.FullCvJson))
                    {
                        _logger.LogWarning("Invalid JSON format in FullCvJson for CVId {CVId}: {FullCvJson}", cv.CVId, cv.FullCvJson.Length > 100 ? cv.FullCvJson.Substring(0, 100) + "..." : cv.FullCvJson);
                        return (false, "Invalid JSON format in FullCvJson", null);
                    }

                    var jsonElement = JsonSerializer.Deserialize<JsonElement>(cv.FullCvJson);
                    inputText = jsonElement.TryGetProperty("Text", out var textProperty)
                        ? textProperty.GetString()
                        : throw new JsonException($"Text property not found in FullCvJson for CVId {cv.CVId}");
                }

                if (string.IsNullOrWhiteSpace(inputText))
                {
                    _logger.LogWarning("Input text for CV extraction is empty or null for CVId {CVId}", cv?.CVId);
                    return (false, "Input text is empty or null", null);
                }

                var (success, summaryText, cvData) = await SummarizeAndTranslate(inputText, "en");
                if (!success)
                {
                    _logger.LogError("Failed to extract CV data for CVId {CVId}: {Error}", cv?.CVId, summaryText);
                    return (false, summaryText, null);
                }

                if (!IsValidCvData(cvData))
                {
                    _logger.LogWarning("Invalid CV data extracted for CVId {CVId}, falling back to default extraction", cv?.CVId);
                    cvData = FallbackExtractCvData(inputText);
                }

                return (true, null, cvData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to extract CV data for CVId {CVId}: {Message}", cv?.CVId, ex.Message);
                return (false, ex.Message, null);
            }
        }
        private CVData ParseGeminiResponseForCVData(string responseText)
        {
            var cvData = new CVData();
            var lines = responseText.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (trimmedLine.StartsWith("Description:", StringComparison.OrdinalIgnoreCase))
                    cvData.Description = trimmedLine.Substring("Description:".Length).Trim();
                else if (trimmedLine.StartsWith("Skills:", StringComparison.OrdinalIgnoreCase))
                    cvData.Skills = trimmedLine.Substring("Skills:".Length).Trim().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                else if (trimmedLine.StartsWith("Experience:", StringComparison.OrdinalIgnoreCase))
                    cvData.Experience = trimmedLine.Substring("Experience:".Length).Trim();
                else if (trimmedLine.StartsWith("Education:", StringComparison.OrdinalIgnoreCase))
                    cvData.Education = trimmedLine.Substring("Education:".Length).Trim();
            }

            return cvData;
        }
        public async Task<(bool Success, string ErrorMessage, float[] Vector)> PreprocessAndGenerateEmbeddingAsync(string text, int? jobId = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("PreprocessAndGenerateEmbeddingAsync called with empty or whitespace text");
                return (false, "Input text is empty", new float[0]);
            }

            var normalizedText = NormalizeText(text);
            await EnsureValidToken();
            if (string.IsNullOrEmpty(_accessToken))
            {
                _logger.LogError("Access token is null or empty");
                return (false, "Invalid access token", new float[0]);
            }

            var (success, cleanedText, contextAnalysis) = await PreprocessTextWithGeminiAsync(normalizedText);
            if (!success)
            {
                _logger.LogWarning("Failed to preprocess text: {Error}", cleanedText);
                return (false, cleanedText, new float[0]);
            }

            var words = cleanedText.ToLower().Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 1 && w.Length < 30 && Regex.IsMatch(w, @"^[a-z0-9#+]+$"))
                .ToArray();

            var preprocessedText = string.Join(" ", words);

            if (string.IsNullOrWhiteSpace(preprocessedText))
            {
                _logger.LogWarning("No processable content found in text after preprocessing");
                return (false, "No processable content", new float[0]);
            }

            var modelName = "models/text-embedding-004";

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                try
                {
                    Embedding existingEmbedding = null;
                    {
                        var embeddings = await context.Embeddings
                            .Where(e => e.Text == preprocessedText && e.Model == modelName && e.CreatedAt > DateTime.UtcNow.AddDays(-7))
                            .ToListAsync();
                        existingEmbedding = embeddings.FirstOrDefault(e => e.Vector != null && e.Vector.All(v => !float.IsNaN(v) && !float.IsInfinity(v)));

                        if (existingEmbedding != null)
                        {
                            _logger.LogInformation("Reusing existing embedding for text: {Text}", preprocessedText);
                            return (true, string.Empty, existingEmbedding.Vector);
                        }
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
                            ExpiresAt = DateTime.UtcNow.AddDays(7),
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
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing embedding for text: {Text}", preprocessedText);
                    return (false, $"Processing error: {ex.Message}", new float[0]);
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
                processedJobText = summarizeSuccess ? summary : jobText;
                _logger.LogInformation("Summarized job text: Length={Length}", processedJobText.Length);
            }

            var (preprocessSuccess, cleanedJobText, /*weightedTerms,*/ contextAnalysis) = await PreprocessTextWithGeminiAsync(processedJobText);
            if (preprocessSuccess)
            {
                jobContext = contextAnalysis;
            }
            else
            {
                _logger.LogWarning("Preprocess failed for job text, using original text as fallback");
                cleanedJobText = processedJobText;
            }

            var tasks = texts.Select(async (text, i) =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            var existingVector = await context.Embeddings
                                .Where(e => e.Text == text && e.Model == "models/text-embedding-004" && e.CreatedAt > DateTime.UtcNow.AddDays(-7))
                                .Select(e => e.Vector)
                                .FirstOrDefaultAsync();

                            if (existingVector != null && existingVector.Length > 0)
                            {
                                _logger.LogInformation("Reusing existing vector for Criteria Index: {Index}", i);
                                return (Index: i, Success: true, Vector: existingVector, Error: string.Empty);
                            }

                            var inputText = string.IsNullOrEmpty(cleanedJobText) ? text : cleanedJobText;
                            var (vectorSuccess, vectorError, vector) = await PreprocessAndGenerateEmbeddingAsync(inputText);
                            return (Index: i, Success: vectorSuccess, Vector: vector, Error: vectorError);
                        }
                        return (Index: i, Success: false, Vector: new float[0], Error: "Empty text");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error generating vector for job criteria {Index}", i);
                        return (Index: i, Success: false, Vector: new float[0], Error: ex.Message);
                    }
                }
            }).ToArray();

            var results = await Task.WhenAll(tasks);
            foreach (var result in results)
            {
                vectors[result.Index] = result.Success && result.Vector != null && result.Vector.Length > 0 ? result.Vector : new float[0];
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to generate vector for job criteria {Index}: {Error}", result.Index, result.Error);
                }
            }

            return (vectors.Any(v => v.Length > 0), vectors.All(v => v.Length == 0) ? "Your CV is not relative with job" : string.Empty, vectors, jobContext);
        }


        public async Task<(bool Success, string ErrorMessage, float[][] Vectors, string CvContext)> GenerateVectorsForCVCriteria(CV cv, string fullCvJson)
        {
            if (cv == null || string.IsNullOrWhiteSpace(fullCvJson))
            {
                _logger.LogWarning("GenerateVectorsForCVCriteria called with null CV or empty FullCvJson");
                return (false, "Invalid CV or FullCvJson", new float[4][], string.Empty);
            }

            string cvText;
            try
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(fullCvJson);
                cvText = jsonElement.TryGetProperty("Text", out var textProperty)
                    ? textProperty.GetString()
                    : throw new JsonException("Text property not found in FullCvJson");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse FullCvJson for CVId {CVId}", cv.CVId);
                return (false, $"Invalid FullCvJson format: {ex.Message}", new float[4][], string.Empty);
            }

            if (string.IsNullOrWhiteSpace(cvText))
            {
                _logger.LogWarning("Text field in FullCvJson is empty for CVId {CVId}", cv.CVId);
                return (false, "Text field in FullCvJson is empty", new float[4][], string.Empty);
            }

            var (extractSuccess, extractError, cvData) = await ExtractCvDataAsync(cv, cvText);
            if (!extractSuccess)
            {
                _logger.LogError("Failed to extract CV data for CVId {CVId}: {Error}", cv.CVId, extractError);
                return (false, extractError, new float[4][], string.Empty);
            }

            var texts = new[]
            {
                cvData.Description ?? "No description",
                cvData.Skills.Any() ? string.Join(" ", cvData.Skills) : "No skills",
                cvData.Experience ?? "No experience",
                cvData.Education ?? "No education"
            };
            _logger.LogInformation("Extracted CV criteria - Description: {Description}, Skills: {Skills}, Experience: {Experience}, Education: {Education}",
                texts[0], texts[1], texts[2], texts[3]);

            var vectors = new float[4][] { new float[0], new float[0], new float[0], new float[0] };

            var (preprocessSuccess, cleanedCvText, /*weightedTerms,*/ contextAnalysis) = await PreprocessTextWithGeminiAsync(cvText);
            if (!preprocessSuccess)
            {
                _logger.LogWarning("Preprocess failed for CV text, using original text for CVId {CVId}", cv.CVId);
                cleanedCvText = cvText;
            }

            var tasks = texts.Select(async (text, i) =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(text) && !text.Contains("No "))
                        {
                            var (vectorSuccess, vectorError, vector) = await PreprocessAndGenerateEmbeddingAsync(
                                string.IsNullOrEmpty(cleanedCvText) ? text : cleanedCvText);
                            return (Index: i, Success: vectorSuccess, Vector: vector, Error: vectorError);
                        }
                        else
                        {
                            var (vectorSuccess, vectorError, vector) = await PreprocessAndGenerateEmbeddingAsync(cvText);
                            return (Index: i, Success: vectorSuccess, Vector: vector, Error: vectorError);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error generating vector for CV criteria {Index} for CVId {CVId}", i, cv.CVId);
                        return (Index: i, Success: false, Vector: new float[0], Error: ex.Message);
                    }
                }
            }).ToArray();

            var results = await Task.WhenAll(tasks);
            foreach (var result in results)
            {
                vectors[result.Index] = result.Success && result.Vector != null && result.Vector.Length > 0 ? result.Vector : new float[0];
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to generate vector for CV criteria {Index} for CVId {CVId}: {Error}", result.Index, cv.CVId, result.Error);
                }
            }

            return (vectors.Any(v => v.Length > 0), vectors.All(v => v.Length == 0) ? "All vectors empty" : string.Empty, vectors, contextAnalysis);
        }
        public void ClearPreprocessCache() { _logger.LogInformation("Preprocess cache clearing is no longer needed as caching is disabled."); }
        public async Task<(bool Success, string ErrorMessage, float[][] Vectors)> GenerateBatchEmbeddingsAsync(string[] texts)
        {
            if (texts == null || !texts.Any())
            {
                return (false, "No texts provided for batch embedding", new float[0][]);
            }

            await EnsureValidToken();
            if (string.IsNullOrEmpty(_accessToken))
            {
                _logger.LogError("Access token is null or empty");
                return (false, "Invalid access token", new float[0][]);
            }

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
        public async Task<List<string>> GenerateImprovementSuggestions(
            Job job,
            CV cv,
            float similarityDescription,
            float similaritySkills,
            float similarityExperience,
            float similarityEducation,
            float descriptionMaxScore,
            float skillsMaxScore,
            float experienceMaxScore,
            float educationMaxScore)
        {
            var suggestions = new List<string>();


            const float descriptionThreshold = 0.4f;
            const float skillsThreshold = 0.5f;
            const float experienceThreshold = 0.4f;
            const float educationThreshold = 0.6f;
            const float totalThreshold = 0.5f;


            float normalizedDescription = descriptionMaxScore > 0 ? similarityDescription / descriptionMaxScore : 0f;
            float normalizedSkills = skillsMaxScore > 0 ? similaritySkills / skillsMaxScore : 0f;
            float normalizedExperience = experienceMaxScore > 0 ? similarityExperience / experienceMaxScore : 0f;
            float normalizedEducation = educationMaxScore > 0 ? similarityEducation / educationMaxScore : 0f;
            float totalSimilarity = (descriptionMaxScore + skillsMaxScore + experienceMaxScore + educationMaxScore) > 0
                ? (similarityDescription + similaritySkills + similarityExperience + similarityEducation) /
                  (descriptionMaxScore + skillsMaxScore + experienceMaxScore + educationMaxScore)
                : 0f;


            if (normalizedDescription < descriptionThreshold)
            {
                suggestions.Add("Revise your **description** to better align with the job's requirements.");
            }


            if (normalizedSkills < skillsThreshold)
            {
                suggestions.Add("Enhance your **skills** section to better match the job's technical demands.");
            }


            if (normalizedExperience < experienceThreshold)
            {
                suggestions.Add("Strengthen your **experience** section to highlight relevant roles or projects.");
            }


            if (normalizedEducation < educationThreshold)
            {
                suggestions.Add("Update your **education** details to better reflect the job's qualifications.");
            }


            if (totalSimilarity < totalThreshold)
            {
                suggestions.Add("Tailor your **CV** to improve overall alignment with the job's requirements.");
            }


            return suggestions.Any() ? suggestions : new List<string> { "Your **CV** is **well-aligned** with the job requirements. No major changes needed!" };
        }
        public async Task<(bool Success, string ErrorMessage, float FinalSimilarity, float SimilarityDescription, float SimilaritySkills, float SimilarityExperience, float SimilarityEducation, float DescriptionMaxScore, float SkillsMaxScore, float ExperienceMaxScore, float EducationMaxScore, string GeminiReasoning)> CalculateTotalSimilarity(Job job, CV cv)
        {
            if (job == null || cv == null)
            {
                _logger.LogWarning("CalculateTotalSimilarity called with null job or CV");
                return (false, "Invalid job or CV", 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, string.Empty);
            }

            _logger.LogInformation("Extracting job content - Description: {Description}, YourSkill: {YourSkill}, YourExperience: {YourExperience}, Education: {Education}",
                job.Description, job.YourSkill, job.YourExperience, job.Education);
            _logger.LogInformation("Extracting CV content from FullCvJson: {FullCvJson}", cv.FullCvJson);


            float descriptionWeight = job.DescriptionWeight;
            float skillsWeight = job.SkillsWeight;
            float experienceWeight = job.ExperienceWeight;
            float educationWeight = job.EducationWeight;


            float totalWeight = descriptionWeight + skillsWeight + experienceWeight + educationWeight;
            if (Math.Abs(totalWeight) < 0.0001f)
            {
                _logger.LogWarning("Total weight is zero for Job {JobId}", job.JobId);
                return (false, "Total weight cannot be zero.", 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, "Invalid weights");
            }


            float descriptionMaxScore = descriptionWeight * 100f;
            float skillsMaxScore = skillsWeight * 100f;
            float experienceMaxScore = experienceWeight * 100f;
            float educationMaxScore = educationWeight * 100f;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                try
                {
                    string jobText = $"{job.Description}\n{job.YourSkill}\n{job.YourExperience}\n{job.Education}";
                    var (jobVectorsSuccess, jobVectorsError, jobVectors, jobContext) = await GenerateVectorsForCriteria(job, jobText, summarize: false);
                    if (!jobVectorsSuccess)
                    {
                        _logger.LogError("Failed to generate job vectors: {Error}", jobVectorsError);
                        return (false, jobVectorsError, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, jobVectorsError);
                    }

                    var (cvVectorsSuccess, cvVectorsError, cvVectors, cvContext) = await GenerateVectorsForCVCriteria(cv, cv.FullCvJson);
                    if (!cvVectorsSuccess)
                    {
                        _logger.LogError("Failed to generate CV vectors: {Error}", cvVectorsError);
                        return (false, cvVectorsError, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, cvVectorsError);
                    }

                    if (jobVectors.Length != 4 || cvVectors.Length != 4)
                    {
                        _logger.LogError("Invalid vector array length: JobVectors={JobLength}, CVVectors={CVLength}", jobVectors.Length, cvVectors.Length);
                        return (false, "Invalid vector array length", 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, "Invalid vector array length");
                    }


                    float similarityDescription = CalculateCosineSimilarity(jobVectors[0], cvVectors[0]) * descriptionMaxScore;
                    float similaritySkills = CalculateCosineSimilarity(jobVectors[1], cvVectors[1]) * skillsMaxScore;
                    float similarityExperience = CalculateCosineSimilarity(jobVectors[2], cvVectors[2]) * experienceMaxScore;
                    float similarityEducation = CalculateCosineSimilarity(jobVectors[3], cvVectors[3]) * educationMaxScore;


                    float finalSimilarity = similarityDescription + similaritySkills + similarityExperience + similarityEducation;


                    similarityDescription = Math.Clamp(similarityDescription, 0f, descriptionMaxScore);
                    similaritySkills = Math.Clamp(similaritySkills, 0f, skillsMaxScore);
                    similarityExperience = Math.Clamp(similarityExperience, 0f, experienceMaxScore);
                    similarityEducation = Math.Clamp(similarityEducation, 0f, educationMaxScore);
                    finalSimilarity = Math.Clamp(finalSimilarity, 0f, 100f);

                    string geminiReasoning = $"Similarity calculated based on vector cosine similarity. Scores: Description {similarityDescription:F1}/{descriptionMaxScore:F1}, Skills {similaritySkills:F1}/{skillsMaxScore:F1}, Experience {similarityExperience:F1}/{experienceMaxScore:F1}, Education {similarityEducation:F1}/{educationMaxScore:F1}, Total {finalSimilarity:F1}/100";

                    _logger.LogInformation("Similarity Scores for Job {JobId}: Description={Description:F1}/{DescriptionMax:F1}, Skills={Skills:F1}/{SkillsMax:F1}, Experience={Experience:F1}/{ExperienceMax:F1}, Education={Education:F1}/{EducationMax:F1}, Total={Total:F1}/100",
                        job.JobId, similarityDescription, descriptionMaxScore, similaritySkills, skillsMaxScore, similarityExperience, experienceMaxScore, similarityEducation, educationMaxScore, finalSimilarity);

                    return (true, string.Empty, finalSimilarity, similarityDescription, similaritySkills, similarityExperience, similarityEducation, descriptionMaxScore, skillsMaxScore, experienceMaxScore, educationMaxScore, geminiReasoning);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in CalculateTotalSimilarity for Job {JobId}: {Error}", job?.JobId, ex.Message);
                    return (false, $"Similarity calculation failed: {ex.Message}", 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, ex.Message);
                }
            }
        }

        private bool IsValidJson(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("Input JSON is null or empty");
                return false;
            }

            try
            {
                JsonSerializer.Deserialize<JsonElement>(text);
                return true;
            }
            catch (JsonException ex)
            {
                _logger.LogWarning("Invalid JSON format detected: {ErrorMessage}. Input: {Text}", ex.Message, text.Length > 100 ? text.Substring(0, 100) + "..." : text);
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
        private string NormalizeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            return Regex.Replace(text.Trim().ToLower(), @"\s+", " ");
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

