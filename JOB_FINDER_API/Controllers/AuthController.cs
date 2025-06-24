using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JobFinderDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthController(JobFinderDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }


        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Email is already in use.");
            }

            var userRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == "Candidate");
            if (userRole == null)
            {
                return StatusCode(500, "Default role not found.");
            }

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = userRole.RoleId,
                Image = request.Image,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(); // user.Id sẽ được cập nhật tự động

            var candidateProfile = new CandidateProfile
            {
                UserId = user.Id ?? 0 
            };
            _dbContext.CandidateProfiles.Add(candidateProfile);
            await _dbContext.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _dbContext.Users   
                .Include(u => u.Role)
                .Include(u => u.CompanyProfile)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized("Invalid credentials.");
            }

            if (!user.IsActive)
            {
                return Forbid("Your account is locked. Please contact support.");
            }

            if (user.Role == null)
            {
                return StatusCode(500, "User role not found.");
            }

            var token = GenerateJwtToken(user);

            // Lấy thông tin công ty nếu có
            string? companyName = null;
            string? urlCompanyLogo = null;
            if (user.CompanyProfile != null)
            {
                companyName = user.CompanyProfile.CompanyName;
                urlCompanyLogo = user.CompanyProfile.UrlCompanyLogo;
            }

            return Ok(new
            {
                Token = token,
                Role = user.Role.RoleName,
                User = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.Phone,
                    user.RoleId,
                    user.Image,
                    RoleName = user.Role.RoleName,
                    CompanyName = companyName,
                    UrlCompanyLogo = urlCompanyLogo
                }
            });
        }                   

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // For JWT, logout is handled on the client by deleting the token.
            // Optionally, you can implement token blacklisting here if needed.
            return Ok(new { message = "Logged out successfully. Please remove the token on the client." });
        }
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest("Current and new password are required.");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password))
                return BadRequest("Current password is incorrect.");

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return Ok("Password changed successfully.");
        }
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim("nameid", user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.RoleName)
        }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                Audience = _configuration["Jwt:Audience"],
                Issuer = _configuration["Jwt:Issuer"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        // Update the login-google endpoint to use consistent casing
        [HttpGet("login-google")]
        public IActionResult LoginWithGoogle()
        {
            // Don't use Url.Action as it might not construct URLs correctly
            // Instead, use an absolute URL:
            var callbackUrl = $"{Request.Scheme}://{Request.Host}/api/auth/google-response/";

            var properties = new AuthenticationProperties
            {
                RedirectUri = callbackUrl,
                Items =
        {
            { ".xsrf", Guid.NewGuid().ToString() }
        }
            };

            return Challenge(properties, "Google");
        }

        // Make sure Google response endpoint matches exactly
        [HttpGet("google-response")]
        //[Route("google-response")]  // Add additional route for case-insensitive matching
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("External");
            if (!authenticateResult.Succeeded)
            {
                return Redirect($"http://localhost:3000/auth/error?message={Uri.EscapeDataString("Authentication failed")}");
            }

            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return Redirect($"http://localhost:3000/auth/error?message={Uri.EscapeDataString("Email not provided")}");
            }

            // Check if user exists
            var user = await _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                // Create new user with Candidate role
                var candidateRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == "Candidate");
                if (candidateRole == null)
                {
                    return Redirect($"http://localhost:3000/auth/error?message={Uri.EscapeDataString("User role not found")}");
                }

                user = new User
                {
                    FullName = name,
                    Email = email,
                    RoleId = candidateRole.RoleId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                // Create CandidateProfile for new user
                var candidateProfile = new CandidateProfile
                {
                    UserId = user.Id ?? 0
                };
                _dbContext.CandidateProfiles.Add(candidateProfile);
                await _dbContext.SaveChangesAsync();

                // Refresh user to include the role
                user = await _dbContext.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == email);
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            // Sign out of the temporary External cookie
            await HttpContext.SignOutAsync("External");

            // Redirect to frontend with token and role
            return Redirect($"http://localhost:3000/auth/callback?token={Uri.EscapeDataString(token)}&role={Uri.EscapeDataString(user.Role.RoleName)}");
        }

    }
}