using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace JOB_FINDER_API.Models.Services
{
    public class BatchEmbeddingService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BatchEmbeddingService> _logger;
        private readonly SemaphoreSlim _semaphore;
        
        public BatchEmbeddingService(HttpClient httpClient, ILogger<BatchEmbeddingService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _semaphore = new SemaphoreSlim(3, 3); // Limit concurrent requests
        }
        
        public async Task<Dictionary<string, float[]>> BatchGenerateEmbeddingsAsync(List<string> texts)
        {
            var results = new ConcurrentDictionary<string, float[]>();
            var tasks = texts.Select(async text =>
            {
                await _semaphore.WaitAsync();
                try
                {
                    var embedding = await GenerateSingleEmbeddingAsync(text);
                    if (embedding != null)
                    {
                        results[text] = embedding;
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            });
            
            await Task.WhenAll(tasks);
            return new Dictionary<string, float[]>(results);
        }
        
        private async Task<float[]> GenerateSingleEmbeddingAsync(string text)
        {
            try
            {
                var requestBody = new
                {
                    model = "models/text-embedding-004",
                    content = new { parts = new[] { new { text } } }
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(requestBody), 
                    Encoding.UTF8, 
                    "application/json"
                );

                var response = await _httpClient.PostAsync(
                    "https://generativelanguage.googleapis.com/v1beta/models/text-embedding-004:embedContent", 
                    jsonContent
                );

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    
                    return jsonResponse.GetProperty("embedding")
                        .GetProperty("values")
                        .EnumerateArray()
                        .Select(e => e.GetSingle())
                        .ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate embedding for text: {Text}", text);
            }
            
            return null;
        }
    }
}