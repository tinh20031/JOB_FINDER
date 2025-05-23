using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
//using JOB_FINDER_API.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;

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
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Image = request.Image
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _dbContext.Users
                .Include(u => u.Role)
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
                    RoleName = user.Role.RoleName
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

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
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
    }
}