using JOB_FINDER_API.Models.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using System.Threading;

[Route("auth")]
[ApiController]
public class AuthGemeniController : ControllerBase
{
    [HttpGet("callback")]
    public async Task<IActionResult> OAuthCallback(string code, string state, [FromServices] IOptions<GeminiConfig> geminiConfig)
    {
        if (string.IsNullOrEmpty(code))
        {
            Console.WriteLine("Mã ủy quyền không hợp lệ hoặc không tồn tại.");
            return BadRequest("Mã ủy quyền không hợp lệ.");
        }

        Console.WriteLine($"Received code: {code}, state: {state}");

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = geminiConfig.Value.ClientId,
                ClientSecret = geminiConfig.Value.ClientSecret
            },
            Scopes = new[] { "https://www.googleapis.com/auth/cloud-platform" }
        });

        var token = await flow.ExchangeCodeForTokenAsync("", code, geminiConfig.Value.RedirectUri ?? "https://job-finder-kjt2.onrender.com/auth/callback", CancellationToken.None);

        Console.WriteLine($"Exchanged token - AccessToken: {token.AccessToken}");

        if (string.IsNullOrEmpty(token.AccessToken))
        {
            Console.WriteLine($"Token không hợp lệ - AccessToken: {token.AccessToken ?? "null"}");
            return StatusCode(500, "Token không được trả về từ Google.");
        }

        if (HttpContext.Session == null)
        {
            Console.WriteLine("Session is null. Ensure UseSession() is called in Program.cs.");
            return StatusCode(500, "Session not available.");
        }

        HttpContext.Session.SetString("AccessToken", token.AccessToken);
        HttpContext.Session.SetString("RefreshToken", token.RefreshToken ?? "");

        Console.WriteLine($"Token saved - AccessToken: {token.AccessToken}");
        return Ok(new { Success = true, Message = "Token saved successfully" });
    }
}