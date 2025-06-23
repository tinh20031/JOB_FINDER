using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.DTO;
using Microsoft.AspNetCore.Authorization;
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


        /* [HttpPost("register")]
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

             // Tạo CandidateProfile với thông tin cơ bản
             var candidateProfile = new CandidateProfile
             {
                 UserId = user.Id ?? 0,

             };
             _dbContext.CandidateProfiles.Add(candidateProfile);
             await _dbContext.SaveChangesAsync();

             return Ok("User registered successfully.");
         }*/
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
            await _dbContext.SaveChangesAsync();

            // Tạo CandidateProfile
            var candidateProfile = new CandidateProfile
            {
                UserId = user.Id ?? 0
            };
            _dbContext.CandidateProfiles.Add(candidateProfile);
            await _dbContext.SaveChangesAsync();

            // Tạo các entity liên kết với CandidateProfile (mỗi entity 1 bản ghi rỗng)
            //_dbContext.AboutMes.Add(new AboutMe { CandidateProfileId = candidateProfile.CandidateProfileId });
            //_dbContext.Awards.Add(new Award { CandidateProfileId = candidateProfile.CandidateProfileId });
            //_dbContext.Certificates.Add(new Certificate { CandidateProfileId = candidateProfile.CandidateProfileId });
            //_dbContext.Educations.Add(new Education { CandidateProfileId = candidateProfile.CandidateProfileId });
            //_dbContext.ForeignLanguages.Add(new ForeignLanguage { CandidateProfileId = candidateProfile.CandidateProfileId });
            //_dbContext.HighlightProjects.Add(new HighlightProject { CandidateProfileId = candidateProfile.CandidateProfileId });
            //_dbContext.Skills.Add(new Skill { CandidateProfileId = candidateProfile.CandidateProfileId });
            //_dbContext.WorkExperiences.Add(new WorkExperience { CandidateProfileId = candidateProfile.CandidateProfileId });

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

            var userIdClaim = User.FindFirst(ClaimTypes.Name)?.Value;
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

    }
}