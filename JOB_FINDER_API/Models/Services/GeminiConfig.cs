namespace JOB_FINDER_API.Models.Services
{
    public class GeminiConfig
    {
        public string ApiKey { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ChatModel { get; set; }
        public string EmbeddingModel { get; set; }
        public string ChatEndpoint { get; set; }
        public string EmbeddingEndpoint { get; set; }
        public string RedirectUri { get; set; }
        public string ServiceAccountKeyPath { get; set; }
    }
}