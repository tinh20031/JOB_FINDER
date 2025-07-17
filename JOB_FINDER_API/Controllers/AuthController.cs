using FirebaseAdmin.Auth;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.DTO;
using JOB_FINDER_API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using static Google.Apis.Auth.OAuth2.Web.AuthorizationCodeWebApp;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JobFinderDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly IUserService _userService;
        private readonly IMemoryCache _cache;

        public AuthController(
            JobFinderDbContext dbContext,
            IConfiguration configuration,
            EmailService emailService,
            IUserService userService,
            IMemoryCache cache)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _emailService = emailService;
            _userService = userService;
            _cache = cache;
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

        // bản gốc dùng dc 
        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        //{
        //    try
        //    {
        //        // Validate email format
        //        if (!IsValidEmail(request.Email))
        //        {
        //            return BadRequest("Invalid email format. Please provide a valid email address.");
        //        }

        //        // Check if email already exists
        //        if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email))
        //        {
        //            return BadRequest("This email has already been used.");
        //        }

        //        var userRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == "Candidate");
        //        if (userRole == null)
        //        {
        //            return StatusCode(500, "Default role not found.");
        //        }

        //        // Generate verification code
        //        string verificationCode = GenerateVerificationCode();

        //        var user = new User
        //        {
        //            FullName = request.FullName,
        //            Email = request.Email,
        //            Phone = request.Phone,
        //            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
        //            RoleId = userRole.RoleId,
        //            Image = request.Image,
        //            CreatedAt = DateTime.UtcNow,
        //            UpdatedAt = DateTime.UtcNow,
        //            IsActive = true,
        //            IsEmailVerified = false,
        //            EmailVerificationCode = verificationCode,
        //            EmailVerificationCodeExpiry = DateTime.UtcNow.AddHours(24)
        //        };

        //        _dbContext.Users.Add(user);
        //        await _dbContext.SaveChangesAsync(); // user.Id được cập nhật tự động

        //        var candidateProfile = new CandidateProfile
        //        {
        //            UserId = user.UserId ?? 0
        //        };
        //        _dbContext.CandidateProfiles.Add(candidateProfile);
        //        await _dbContext.SaveChangesAsync();

        //        // Send verification email
        //        try
        //        {
        //            _emailService.SendVerificationEmail(user.Email, verificationCode);
        //        }
        //        catch (Exception ex)
        //        {
        //            // Log email sending error but continue registration process
        //            Console.WriteLine($"Unable to send verification email: {ex.Message}");
        //        }

        //        return Ok(new
        //        {
        //            message = "Registration successful. Please check your email to verify your account.",
        //            userId = user.UserId,
        //            email = user.Email
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Registration error: {ex.Message}");
        //    }
        //}
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!IsValidEmail(request.Email))
                {
                    return BadRequest("Invalid email format. Please provide a valid email address.");
                }

                if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email))
                {
                    return BadRequest("This email has already been used.");
                }

                var userRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == "Candidate");
                if (userRole == null)
                {
                    return StatusCode(500, "Default role not found.");
                }

                string verificationCode = GenerateVerificationCode();

                // Tạo người dùng trong Firebase
                string firebaseUid;
                try
                {
                    var userRecordArgs = new UserRecordArgs
                    {
                        Email = request.Email,
                        Password = request.Password,
                        DisplayName = request.FullName,
                        EmailVerified = false
                    };
                    var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);
                    firebaseUid = userRecord.Uid;
                }
                catch (FirebaseAuthException ex)
                {
                    return StatusCode(500, $"Failed to create user in Firebase: {ex.Message}");
                }

                var user = new User
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    Phone = request.Phone,
                    Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    RoleId = userRole.RoleId,
                    Image = request.Image,
                    FirebaseUid = firebaseUid,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsEmailVerified = false,
                    EmailVerificationCode = verificationCode,
                    EmailVerificationCodeExpiry = DateTime.UtcNow.AddHours(24)
                };

                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                var candidateProfile = new CandidateProfile
                {
                    UserId = user.UserId ?? throw new InvalidOperationException("User ID is null after save.")
                };
                _dbContext.CandidateProfiles.Add(candidateProfile);
                await _dbContext.SaveChangesAsync();

                try
                {
                    _emailService.SendVerificationEmail(user.Email, verificationCode);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to send verification email: {ex.Message}");
                }

                return Ok(new
                {
                    message = "Registration successful. Please check your email to verify your account.",
                    userId = user.UserId,
                    firebaseUid = user.FirebaseUid,
                    email = user.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Registration error: {ex.Message}");
            }
        }

        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginRequest request)
        //{
        //    try
        //    {
        //        // Validate email format
        //        if (!IsValidEmail(request.Email))
        //        {
        //            return BadRequest("Invalid email format. Please provide a valid email address.");
        //        }

        //        var user = await _dbContext.Users
        //            .Include(u => u.Role)
        //            .Include(u => u.CompanyProfile)
        //            .FirstOrDefaultAsync(u => u.Email == request.Email);

        //        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        //        {
        //            return Unauthorized("Invalid login credentials.");
        //        }

        //        if (!user.IsActive)
        //        {
        //            return Forbid("Your account has been locked. Please contact support.");
        //        }

        //        // Email verification check
        //        if (!user.IsEmailVerified)
        //        {
        //            // Regenerate verification code if needed
        //            if (user.EmailVerificationCodeExpiry == null || user.EmailVerificationCodeExpiry < DateTime.UtcNow)
        //            {
        //                user.EmailVerificationCode = GenerateVerificationCode();
        //                user.EmailVerificationCodeExpiry = DateTime.UtcNow.AddHours(24);
        //                await _dbContext.SaveChangesAsync();

        //                // Send new verification code
        //                try
        //                {
        //                    _emailService.SendVerificationEmail(user.Email, user.EmailVerificationCode);
        //                }
        //                catch (Exception ex)
        //                {
        //                    // Log but continue
        //                    Console.WriteLine($"Unable to send verification email: {ex.Message}");
        //                }
        //            }

        //            return BadRequest(new
        //            {
        //                message = "Email has not been verified. Please check your inbox to verify your account before logging in.",
        //                requiresVerification = true,
        //                userId = user.Id,
        //                email = user.Email
        //            });
        //        }

        //        if (user.Role == null)
        //        {
        //            return StatusCode(500, "User role not found.");
        //        }

        //        var token = GenerateJwtToken(user);

        //        // Lấy thông tin công ty nếu có
        //        string? companyName = null;
        //        string? urlCompanyLogo = null;
        //        if (user.CompanyProfile != null)
        //        {
        //            companyName = user.CompanyProfile.CompanyName;
        //            urlCompanyLogo = user.CompanyProfile.UrlCompanyLogo;
        //        }

        //        return Ok(new
        //        {
        //            Token = token,
        //            Role = user.Role.RoleName,
        //            User = new
        //            {
        //                user.Id,
        //                user.FullName,
        //                user.Email,
        //                user.Phone,
        //                user.RoleId,
        //                user.Image,
        //                RoleName = user.Role.RoleName,
        //                CompanyName = companyName,
        //                UrlCompanyLogo = urlCompanyLogo
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Login error: {ex.Message}");
        //    }
        //}

        //bản gốc dùng được k đuộc xóa 
        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginRequest request)
        //{
        //    try
        //    {
        //        // Validate email format
        //        if (!IsValidEmail(request.Email))
        //        {
        //            return BadRequest("Invalid email format. Please provide a valid email address.");
        //        }

        //        var user = await _dbContext.Users
        //            .Include(u => u.Role)
        //            .Include(u => u.CompanyProfile)
        //            .FirstOrDefaultAsync(u => u.Email == request.Email);

        //        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        //        {
        //            return Unauthorized("Invalid login credentials.");
        //        }

        //        if (!user.IsActive)
        //        {
        //            return Forbid("Your account has been locked. Please contact support.");
        //        }

        //        // Email verification check
        //        if (!user.IsEmailVerified)
        //        {
        //            // Regenerate verification code if needed
        //            if (user.EmailVerificationCodeExpiry == null || user.EmailVerificationCodeExpiry < DateTime.UtcNow)
        //            {
        //                user.EmailVerificationCode = GenerateVerificationCode();
        //                user.EmailVerificationCodeExpiry = DateTime.UtcNow.AddHours(24);
        //                await _dbContext.SaveChangesAsync();

        //                // Send new verification code
        //                try
        //                {
        //                    _emailService.SendVerificationEmail(user.Email, user.EmailVerificationCode);
        //                }
        //                catch (Exception ex)
        //                {
        //                    // Log but continue
        //                    Console.WriteLine($"Unable to send verification email: {ex.Message}");
        //                }
        //            }

        //            return BadRequest(new
        //            {
        //                message = "Email has not been verified. Please check your inbox to verify your account before logging in.",
        //                requiresVerification = true,
        //                userId = user.UserId,
        //                email = user.Email
        //            });
        //        }

        //        if (user.Role == null)
        //        {
        //            return StatusCode(500, "User role not found.");
        //        }

        //        var token = GenerateJwtToken(user);

        //        // Lấy thông tin công ty nếu có
        //        string? companyName = null;
        //        string? urlCompanyLogo = null;
        //        if (user.CompanyProfile != null)
        //        {
        //            companyName = user.CompanyProfile.CompanyName;
        //            urlCompanyLogo = user.CompanyProfile.UrlCompanyLogo;
        //        }

        //        return Ok(new
        //        {
        //            Token = token,
        //            Role = user.Role.RoleName,
        //            User = new
        //            {
        //                user.UserId,
        //                user.FullName,
        //                user.Email,
        //                user.Phone,
        //                user.RoleId,
        //                user.Image,
        //                RoleName = user.Role.RoleName,
        //                CompanyName = companyName,
        //                UrlCompanyLogo = urlCompanyLogo
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Login error: {ex.Message}");
        //    }
        //}

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
                    return Unauthorized("Invalid login credentials.");
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
                        message = "Email has not been verified. Please check your inbox to verify your account before logging in.",
                        requiresVerification = true,
                        userId = user.UserId,
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
                        user.UserId,
                        user.FullName,
                        user.Email,
                        user.Phone,
                        user.RoleId,
                        user.Image,
                        RoleName = user.Role.RoleName,
                        CompanyName = companyName,
                        UrlCompanyLogo = urlCompanyLogo,
                        FirebaseUid = user.FirebaseUid
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
                    return BadRequest("Email and verification code must not be empty.");
                }

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                if (user.IsEmailVerified)
                {
                    return Ok("Email has already been verified.");
                }

                if (user.EmailVerificationCode != request.VerificationCode ||
                    user.EmailVerificationCodeExpiry == null ||
                    user.EmailVerificationCodeExpiry < DateTime.UtcNow)
                {
                    return BadRequest("Invalid or expired verification code.");
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
                return StatusCode(500, $"Email verification error: {ex.Message}");
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
                    return Ok("Email has already been verified.");
                }

                // Generate new verification code
                user.EmailVerificationCode = GenerateVerificationCode();
                user.EmailVerificationCodeExpiry = DateTime.UtcNow.AddHours(24);
                await _dbContext.SaveChangesAsync();

                // Send verification email
                try
                {
                    _emailService.SendVerificationEmail(user.Email, user.EmailVerificationCode);
                    return Ok("A new verification code has been sent to your email. Please check your inbox.");
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
                 new Claim("nameid", user.UserId.ToString()),
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
        //bản gốc dùng được 
        //[HttpGet("google-response")]
        //public async Task<IActionResult> GoogleResponse()
        //{
        //    var authenticateResult = await HttpContext.AuthenticateAsync("External");
        //    if (!authenticateResult.Succeeded)
        //    {
        //        return Redirect($"http://localhost:3000/auth/error?message={Uri.EscapeDataString("Authentication failed")}");
        //    }

        //    var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
        //    var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;

        //    if (string.IsNullOrEmpty(email))
        //    {
        //        return Redirect($"http://localhost:3000/auth/error?message={Uri.EscapeDataString("Email not provided")}");
        //    }

        //    // Check if user exists
        //    var user = await _dbContext.Users
        //        .Include(u => u.Role)
        //        .FirstOrDefaultAsync(u => u.Email == email);

        //    if (user == null)
        //    {
        //        // Create new user with Candidate role
        //        var candidateRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == "Candidate");
        //        if (candidateRole == null)
        //        {
        //            return Redirect($"http://localhost:3000/auth/error?message={Uri.EscapeDataString("User role not found")}");
        //        }

        //        user = new User
        //        {
        //            FullName = name,
        //            Email = email,
        //            RoleId = candidateRole.RoleId,
        //            CreatedAt = DateTime.UtcNow,
        //            UpdatedAt = DateTime.UtcNow,
        //            IsActive = true,
        //            IsEmailVerified = true,
        //            EmailVerificationCode = "VERIFIED",
        //            EmailVerificationCodeExpiry = null
        //        };
        //        _dbContext.Users.Add(user);
        //        await _dbContext.SaveChangesAsync();

        //        // Create CandidateProfile for new user
        //        var candidateProfile = new CandidateProfile
        //        {
        //            UserId = user.Id ?? 0
        //        };
        //        _dbContext.CandidateProfiles.Add(candidateProfile);
        //        await _dbContext.SaveChangesAsync();

        //        // Refresh user to include the role
        //        user = await _dbContext.Users
        //            .Include(u => u.Role)
        //            .FirstOrDefaultAsync(u => u.Email == email);
        //    }

        //    // Generate JWT token
        //    var token = GenerateJwtToken(user);

        //    // Sign out of the temporary External cookie
        //    await HttpContext.SignOutAsync("External");

        //    // Redirect to frontend with token and role
        //    return Redirect($"http://localhost:3000/auth/callback?token={Uri.EscapeDataString(token)}&role={Uri.EscapeDataString(user.Role.RoleName)}");
        //}

        //final local
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("External");
            if (!authenticateResult.Succeeded)
            {
                return Redirect($"http://localhost:3000/auth/error?message={Uri.EscapeDataString("Authentication failed: " + authenticateResult.Failure?.Message)}");
            }

            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;
            var googleIdToken = authenticateResult.Properties.GetTokenValue("id_token");

            Console.WriteLine($"Debug - Email: {email}");
            Console.WriteLine($"Debug - Name: {name}");
            Console.WriteLine($"Debug - Google ID Token: {googleIdToken ?? "null"}");

            if (string.IsNullOrEmpty(email))
            {
                return Redirect($"http://localhost:3000/auth/error?message={Uri.EscapeDataString("Email not provided")}");
            }

            var user = await _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);

            var firebaseAuth = FirebaseAuth.DefaultInstance; // Khai báo một lần duy nhất

            if (user == null)
            {
                var candidateRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == "Candidate");
                if (candidateRole == null)
                {
                    return Redirect($"http://localhost:3000/auth/error?message={Uri.EscapeDataString("User role not found")}");
                }

                // Thử lấy Firebase UID từ id_token nếu có
                string firebaseUid = null;
                if (!string.IsNullOrEmpty(googleIdToken))
                {
                    try
                    {
                        var decodedToken = await firebaseAuth.VerifyIdTokenAsync(googleIdToken);
                        firebaseUid = decodedToken.Uid;
                        Console.WriteLine($"Debug - Firebase UID from id_token: {firebaseUid}");
                    }
                    catch (FirebaseAuthException ex)
                    {
                        Console.WriteLine($"Debug - Error verifying id_token: {ex.Message} (Error Code: {ex.ErrorCode})");
                        firebaseUid = Guid.NewGuid().ToString();
                        Console.WriteLine($"Debug - Fallback Firebase UID: {firebaseUid}");
                    }
                }
                else
                {
                    firebaseUid = Guid.NewGuid().ToString();
                    Console.WriteLine($"Debug - No id_token, generated Firebase UID: {firebaseUid}");
                }

                // Tạo người dùng với FirebaseUid đã gán
                user = new User
                {
                    FullName = name,
                    Email = email,
                    RoleId = candidateRole.RoleId,
                    FirebaseUid = firebaseUid, // Lưu FirebaseUid
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsEmailVerified = true,
                    EmailVerificationCode = "VERIFIED",
                    EmailVerificationCodeExpiry = null
                };
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                // Tạo CandidateProfile cho người dùng mới
                var candidateProfile = new CandidateProfile
                {
                    UserId = user.UserId ?? 0
                };
                _dbContext.CandidateProfiles.Add(candidateProfile);
                await _dbContext.SaveChangesAsync();

                // Refresh user to include the role
                user = await _dbContext.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == email);
                Console.WriteLine($"Debug - Reloaded Firebase UID: {user.FirebaseUid}");
                Console.WriteLine($"Debug - Created User ID: {user.UserId}");
            }
            else if (string.IsNullOrEmpty(user.FirebaseUid))
            {
                // Nếu user đã tồn tại nhưng FirebaseUid rỗng, gán giá trị mới
                user.FirebaseUid = Guid.NewGuid().ToString();
                await _dbContext.SaveChangesAsync();
                await _dbContext.Entry(user).ReloadAsync(); // Đồng bộ lại từ DB
                Console.WriteLine($"Debug - Updated Firebase UID for existing user: {user.FirebaseUid}");
            }

            // Kiểm tra và tạo custom token
            if (string.IsNullOrEmpty(user.FirebaseUid))
            {
                Console.WriteLine("Debug - Critical Error: FirebaseUid is still null or empty after assignment.");
                return Redirect($"http://localhost:3000/auth/error?message={Uri.EscapeDataString("Internal error: Unable to generate Firebase UID")}");
            }

            string customToken = await firebaseAuth.CreateCustomTokenAsync(user.FirebaseUid);

            var token = GenerateJwtToken(user);
            await HttpContext.SignOutAsync("External");

            return Redirect($"http://localhost:3000/auth/callback?token={Uri.EscapeDataString(token)}&role={Uri.EscapeDataString(user.Role.RoleName)}&firebaseToken={Uri.EscapeDataString(customToken)}");
        }

        //final production
        //[HttpGet("google-response")]
        //public async Task<IActionResult> GoogleResponse()
        //{
        //    var authenticateResult = await HttpContext.AuthenticateAsync("External");
        //    if (!authenticateResult.Succeeded)
        //    {
        //        return Redirect($"https://job-finder-fe.vercel.app/auth/error?message={Uri.EscapeDataString("Authentication failed: " + authenticateResult.Failure?.Message)}");
        //    }

        //    var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
        //    var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;
        //    var googleIdToken = authenticateResult.Properties.GetTokenValue("id_token");

        //    Console.WriteLine($"Debug - Email: {email}");
        //    Console.WriteLine($"Debug - Name: {name}");
        //    Console.WriteLine($"Debug - Google ID Token: {googleIdToken ?? "null"}");

        //    if (string.IsNullOrEmpty(email))
        //    {
        //        return Redirect($"https://job-finder-fe.vercel.app/auth/error?message={Uri.EscapeDataString("Email not provided")}");
        //    }

        //    var user = await _dbContext.Users
        //        .Include(u => u.Role)
        //        .FirstOrDefaultAsync(u => u.Email == email);

        //    var firebaseAuth = FirebaseAuth.DefaultInstance; // Khai báo một lần duy nhất

        //    if (user == null)
        //    {
        //        var candidateRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == "Candidate");
        //        if (candidateRole == null)
        //        {
        //            return Redirect($"https://job-finder-fe.vercel.app/auth/error?message={Uri.EscapeDataString("User role not found")}");
        //        }

        //        // Thử lấy Firebase UID từ id_token nếu có
        //        string firebaseUid = null;
        //        if (!string.IsNullOrEmpty(googleIdToken))
        //        {
        //            try
        //            {
        //                var decodedToken = await firebaseAuth.VerifyIdTokenAsync(googleIdToken);
        //                firebaseUid = decodedToken.Uid;
        //                Console.WriteLine($"Debug - Firebase UID from id_token: {firebaseUid}");
        //            }
        //            catch (FirebaseAuthException ex)
        //            {
        //                Console.WriteLine($"Debug - Error verifying id_token: {ex.Message} (Error Code: {ex.ErrorCode})");
        //                firebaseUid = Guid.NewGuid().ToString();
        //                Console.WriteLine($"Debug - Fallback Firebase UID: {firebaseUid}");
        //            }
        //        }
        //        else
        //        {
        //            firebaseUid = Guid.NewGuid().ToString();
        //            Console.WriteLine($"Debug - No id_token, generated Firebase UID: {firebaseUid}");
        //        }

        //        // Tạo người dùng với FirebaseUid đã gán
        //        user = new User
        //        {
        //            FullName = name,
        //            Email = email,
        //            RoleId = candidateRole.RoleId,
        //            FirebaseUid = firebaseUid, // Lưu FirebaseUid
        //            CreatedAt = DateTime.UtcNow,
        //            UpdatedAt = DateTime.UtcNow,
        //            IsActive = true,
        //            IsEmailVerified = true,
        //            EmailVerificationCode = "VERIFIED",
        //            EmailVerificationCodeExpiry = null
        //        };
        //        _dbContext.Users.Add(user);
        //        await _dbContext.SaveChangesAsync();

        //        // Tạo CandidateProfile cho người dùng mới
        //        var candidateProfile = new CandidateProfile
        //        {
        //            UserId = user.UserId ?? 0
        //        };
        //        _dbContext.CandidateProfiles.Add(candidateProfile);
        //        await _dbContext.SaveChangesAsync();

        //        // Refresh user to include the role
        //        user = await _dbContext.Users
        //            .Include(u => u.Role)
        //            .FirstOrDefaultAsync(u => u.Email == email);
        //        Console.WriteLine($"Debug - Reloaded Firebase UID: {user.FirebaseUid}");
        //        Console.WriteLine($"Debug - Created User ID: {user.UserId}");
        //    }
        //    else if (string.IsNullOrEmpty(user.FirebaseUid))
        //    {
        //        // Nếu user đã tồn tại nhưng FirebaseUid rỗng, gán giá trị mới
        //        user.FirebaseUid = Guid.NewGuid().ToString();
        //        await _dbContext.SaveChangesAsync();
        //        await _dbContext.Entry(user).ReloadAsync(); // Đồng bộ lại từ DB
        //        Console.WriteLine($"Debug - Updated Firebase UID for existing user: {user.FirebaseUid}");
        //    }

        //    // Kiểm tra và tạo custom token
        //    if (string.IsNullOrEmpty(user.FirebaseUid))
        //    {
        //        Console.WriteLine("Debug - Critical Error: FirebaseUid is still null or empty after assignment.");
        //        return Redirect($"https://job-finder-fe.vercel.app/auth/error?message={Uri.EscapeDataString("Internal error: Unable to generate Firebase UID")}");
        //    }

        //    string customToken = await firebaseAuth.CreateCustomTokenAsync(user.FirebaseUid);

        //    var token = GenerateJwtToken(user);
        //    await HttpContext.SignOutAsync("External");

        //    return Redirect($"https://job-finder-fe.vercel.app/auth/callback?token={Uri.EscapeDataString(token)}&role={Uri.EscapeDataString(user.Role.RoleName)}&firebaseToken={Uri.EscapeDataString(customToken)}");
        //}

        [HttpPost("forgot-password/request")]
        public async Task<IActionResult> RequestForgotPassword([FromBody] ForgotPasswordRequestDto dto)
        {
            var user = await _userService.GetByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("Email does not exist.");

            var code = GenerateVerificationCode();
            _cache.Set($"fp_{dto.Email}", code, TimeSpan.FromMinutes(10));

            string subject = "Password Reset Request";
            string body = $@"
<html>
  <body style='font-family: Arial, sans-serif; background: #f6f6f6; padding: 30px;'>
    <div style='max-width: 600px; margin: auto; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px #eee; padding: 32px;'>
      <h2 style='color: #2d8cf0;'>Password Reset Request</h2>
      <p>Hello,</p>
      <p>We received a request to reset your password for the Job Finder account associated with this email. To proceed, please use the verification code below:</p>
      <div style='margin: 24px 0; text-align: center;'>
        <h1 style='font-size: 32px; letter-spacing: 5px; color: #2d8cf0; border: 2px dashed #2d8cf0; display: inline-block; padding: 10px 20px; border-radius: 5px;'>{code}</h1>
      </div>
      <p>The verification code is valid for 10 minutes. If you did not request this code, please ignore this email or contact support.</p>
      <p>Best regards,<br>Job Finder Team</p>
    </div>
  </body>
</html>";

            _emailService.SendEmail(dto.Email, subject, body, true);
            return Ok("The verification code has been sent to your email.");
        }

        [HttpPost("forgot-password/verify")]
        public IActionResult VerifyForgotPassword([FromBody] ForgotPasswordVerifyDto dto)
        {
            if (_cache.TryGetValue($"fp_{dto.Email}", out string code) && code == dto.Code)
                return Ok("The verification code is valid.");
            return BadRequest("The verification code is incorrect or has expired.");
        }

        [HttpPost("forgot-password/reset")]
        public async Task<IActionResult> ResetForgotPassword([FromBody] ForgotPasswordResetDto dto)
        {
            if (_cache.TryGetValue($"fp_{dto.Email}", out string code) && code == dto.Code)
            {
                var user = await _userService.GetByEmailAsync(dto.Email);
                if (user == null)
                    return BadRequest("Email does not exist.");

                await _userService.UpdatePasswordAsync(user, dto.NewPassword);
                _cache.Remove($"fp_{dto.Email}");

                string subject = "Password Reset Successful";
                string body = $@"
<html>
  <body style='font-family: Arial, sans-serif; background: #f6f6f6; padding: 30px;'>
    <div style='max-width: 600px; margin: auto; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px #eee; padding: 32px;'>
      <h2 style='color: #2d8cf0;'>Password Reset Successful</h2>
      <p>Hello,</p>
      <p>Your password for the Job Finder account associated with this email has been successfully reset. You can now log in with your new password.</p>
      <p>If you did not request this change, please contact support immediately.</p>
      <p>Best regards,<br>Job Finder Team</p>
    </div>
  </body>
</html>";

                _emailService.SendEmail(dto.Email, subject, body, true);
                return Ok("Password reset successful.");
            }
            return BadRequest("The verification code is incorrect or has expired.");
        }

        [HttpPost("forgot-password/resend-verification")]
        public async Task<IActionResult> ResendForgotPasswordVerification([FromBody] ForgotPasswordResendVerificationDto dto)
        {
            if (!IsValidEmail(dto.Email))
                return BadRequest("Invalid email address.");

            var user = await _userService.GetByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("Email does not exist.");

            string throttleKey = $"fp_throttle_{dto.Email}";
            if (_cache.TryGetValue(throttleKey, out _))
            {
                return BadRequest("You have recently requested a verification code. Please wait 60 seconds before requesting again.");
            }

            var code = GenerateVerificationCode();
            _cache.Set($"fp_{dto.Email}", code, TimeSpan.FromMinutes(10));
            _cache.Set(throttleKey, true, TimeSpan.FromSeconds(60));

            string subject = "Password Reset Verification Code (Resend)";
            string body = $@"
<html>
  <body style='font-family: Arial, sans-serif; background: #f6f6f6; padding: 30px;'>
    <div style='max-width: 600px; margin: auto; background: #fff; border-radius: 8px; box-shadow: 0 2px 8px #eee; padding: 32px;'>
      <h2 style='color: #2d8cf0;'>Password Reset Verification Code (Resend)</h2>
      <p>Hello,</p>
      <p>We have resent a verification code to reset your password for the Job Finder account associated with this email. Please use the code below:</p>
      <div style='margin: 24px 0; text-align: center;'>
        <h1 style='font-size: 32px; letter-spacing: 5px; color: #2d8cf0; border: 2px dashed #2d8cf0; display: inline-block; padding: 10px 20px; border-radius: 5px;'>{code}</h1>
      </div>
      <p>The verification code is valid for 10 minutes. If you did not request this code, please ignore this email or contact support.</p>
      <p>Best regards,<br>Job Finder Team</p>
    </div>
  </body>
</html>";

            try
            {
                _emailService.SendEmail(dto.Email, subject, body, true);
                return Ok("The verification code has been resent to your email.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to send email: {ex.Message}");
            }
        }
    }
}





