using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace JOB_FINDER_API.Services
{
    public class GeminiAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GeminiAuthService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetAccessToken()
        {
            try
            {
                var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("Google", "access_token");
                if (string.IsNullOrEmpty(accessToken))
                {
                    Console.WriteLine("No Google access token found, triggering Google authentication.");
                    await _httpContextAccessor.HttpContext.ChallengeAsync("Google");
                    return null;
                }
                Console.WriteLine($"Google Access Token: {accessToken}");
                return accessToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting Google access token: {ex.Message}");
                return null;
            }
        }
    }
}