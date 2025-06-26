using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.DTO;
using JOB_FINDER_API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JobFinderDbContext _dbContext;
        private readonly IConfiguration _configuration;
private readonly EmailService _emailService = new EmailService(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

        public AuthController(JobFinderDbContext dbContext, IConfiguration configuration, EmailService emailService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _emailService = emailService;
        }

        // Helper method to validate email format
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // RFC 5322 compliant email regex pattern
            string pattern = @"^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        /*[HttpPost("register")]
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
        }*/
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Validate email format
                if (!IsValidEmail(request.Email))
                {
                    return BadRequest("Invalid email format. Please provide a valid email address.");
                }

                // Check if email already exists
                if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email))
                {
                    return BadRequest("This email is already in use.");
                }

                var userRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == "Candidate");
                if (userRole == null)
                {
                    return StatusCode(500, "No default role found.");
                }

                // Generate verification code
                string verificationCode = GenerateVerificationCode();

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
                    IsActive = true,
                    IsEmailVerified = false,
                    EmailVerificationCode = verificationCode,
                    EmailVerificationCodeExpiry = DateTime.UtcNow.AddHours(24)
                };

                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync(); // user.Id được cập nhật tự động

                var candidateProfile = new CandidateProfile
                {
                    UserId = user.Id ?? 0
                };
                _dbContext.CandidateProfiles.Add(candidateProfile);
                await _dbContext.SaveChangesAsync();

                // Send verification email
                try
                {
                    _emailService.SendVerificationEmail(user.Email, verificationCode);
                }
                catch (Exception ex)
                {
                    // Log email sending error but continue registration process
                    Console.WriteLine($"Unable to send verification email: {ex.Message}");
                }

                return Ok(new
                {
                    message = "Registration successful. Please check your email to verify your account.",
                    userId = user.Id,
                    email = user.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Registration error: {ex.Message}");
            }
        }

        /*[HttpPost("login")]
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
        }    */
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                // Validate email format
                if (!IsValidEmail(request.Email))
                {
                    return BadRequest("Invalid email format. Please provide a valid email address.");
                }

                var user = await _dbContext.Users
                    .Include(u => u.Role)
                    .Include(u => u.CompanyProfile)
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                {
                    return Unauthorized("Invalid login information.");
                }

                if (!user.IsActive)
                {
                    return Forbid("Your account has been locked. Please contact support.");
                }

                // Email verification check
                if (!user.IsEmailVerified)
                {
                    // Regenerate verification code if needed
                    if (user.EmailVerificationCodeExpiry == null || user.EmailVerificationCodeExpiry < DateTime.UtcNow)
                    {
                        user.EmailVerificationCode = GenerateVerificationCode();
                        user.EmailVerificationCodeExpiry = DateTime.UtcNow.AddHours(24);
                        await _dbContext.SaveChangesAsync();

                        // Send new verification code
                        try
                        {
                            _emailService.SendVerificationEmail(user.Email, user.EmailVerificationCode);
                        }
                        catch (Exception ex)
                        {
                            // Log but continue
                            Console.WriteLine($"Unable to send verification email: {ex.Message}");
                        }
                    }

                    return BadRequest(new
                    {
                        message = "Email is not verified. Please check your inbox to verify your account before logging in.",
                        requiresVerification = true,
                        userId = user.Id,
                        email = user.Email
                    });
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Login error: {ex.Message}");
            }
        }

        // Helper method to generate verification code
        private string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
        // Verify email endpoint
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.VerificationCode))
                {
                    return BadRequest("Email and verification code cannot be blank.");
                }

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                if (user.IsEmailVerified)
                {
                    return Ok("Email has been previously verified.");
                }

                if (user.EmailVerificationCode != request.VerificationCode ||
                    user.EmailVerificationCodeExpiry == null ||
                    user.EmailVerificationCodeExpiry < DateTime.UtcNow)
                {
                    return BadRequest("The verification code is invalid or has expired.");
                }

                // Mark email as verified
                user.IsEmailVerified = true;
                user.EmailVerificationCode = "VERIFIED"; // Use placeholder since we can't set null
                user.EmailVerificationCodeExpiry = null;
                user.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                return Ok("Email verification successful. You can log in now.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Email authentication error: {ex.Message}");
            }
        }
        // Resend verification email
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequest request)
        {
            try
            {
                if (!IsValidEmail(request.Email))
                {
                    return BadRequest("Invalid email format.");
                }

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                if (user.IsEmailVerified)
                {
                    return Ok("Email has been previously verified.");
                }

                // Generate new verification code
                user.EmailVerificationCode = GenerateVerificationCode();
                user.EmailVerificationCodeExpiry = DateTime.UtcNow.AddHours(24);
                await _dbContext.SaveChangesAsync();

                // Send verification email
                try
                {
                    _emailService.SendVerificationEmail(user.Email, user.EmailVerificationCode);
                    return Ok("The verification code has been re-sent to your email. Please check your inbox.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Unable to send verification email: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error resending verification code: {ex.Message}");
            }
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
                     //new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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
        /*[HttpGet("google-response")]
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
        }*/
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("External");
            if (!authenticateResult.Succeeded)
            {
                return Redirect($"http://localhost:3000/auth/error?message={Uri.EscapeDataString("Xác thực không thành công")}");
            }

            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return Redirect($"http://localhost:3000/auth/error?message={Uri.EscapeDataString("Email không được cung cấp")}");
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
                    return Redirect($"http://localhost:3000/auth/error?message={Uri.EscapeDataString("Không tìm thấy vai trò người dùng")}");
                }

                user = new User
                {
                    FullName = name,
                    Email = email,
                    RoleId = candidateRole.RoleId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsEmailVerified = true,
                    EmailVerificationCode = "VERIFIED_WITH_GOOGLE" // Khởi tạo trường này với một giá trị không phải null
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
            else
            {
                // Xử lý trường hợp email đã đăng ký nhưng chưa xác nhận
                if (!user.IsEmailVerified)
                {
                    // Tự động xác thực email vì người dùng đã đăng nhập qua Google
                    user.IsEmailVerified = true;
                    user.EmailVerificationCode = "VERIFIED_WITH_GOOGLE";
                    user.EmailVerificationCodeExpiry = null;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _dbContext.SaveChangesAsync();

                    // Tùy chọn: Gửi email thông báo cho người dùng
                    try
                    {
                        string subject = "Email của bạn đã được xác thực qua Google";
                        string body = $@"
<html>
  <body style='font-family: Arial, sans-serif; background: #f6f6f6; padding: 30px;'>
    <div style='max-width: 600px; margin: auto; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px #eee; padding: 32px;'>
      <h2 style='color: #2d8cf0;'>Xác thực email thành công</h2>
      <p>Chào bạn,</p>
      <p>Email của bạn đã được xác thực tự động thông qua đăng nhập Google.</p>
      <p>Bây giờ bạn có thể sử dụng đầy đủ tính năng của hệ thống Job Finder.</p>
      <p>Trân trọng,<br>Đội ngũ Job Finder</p>
    </div>
  </body>
</html>
";
                        _emailService.SendEmail(user.Email, subject, body, true);
                    }
                    catch (Exception ex)
                    {
                        // Ghi log lỗi nhưng vẫn tiếp tục xử lý
                        Console.WriteLine($"Không thể gửi email thông báo: {ex.Message}");
                    }
                }
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