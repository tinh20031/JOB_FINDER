
using FirebaseAdmin.Auth;
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
using static Google.Apis.Auth.OAuth2.Web.AuthorizationCodeWebApp;

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


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Validate email format
                if (!IsValidEmail(request.Email))
                {
                    return BadRequest("Định dạng email không hợp lệ. Vui lòng cung cấp địa chỉ email chính xác.");
                }

                // Check if email already exists
                if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email))
                {
                    return BadRequest("Email này đã được sử dụng.");
                }

                var userRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == "Candidate");
                if (userRole == null)
                {
                    return StatusCode(500, "Không tìm thấy vai trò mặc định.");
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
                    Console.WriteLine($"Không thể gửi email xác thực: {ex.Message}");
                }

                return Ok(new
                {
                    message = "Đăng ký thành công. Vui lòng kiểm tra email để xác thực tài khoản.",
                    userId = user.Id,
                    email = user.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi đăng ký: {ex.Message}");
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
        //            return BadRequest("Định dạng email không hợp lệ. Vui lòng cung cấp địa chỉ email chính xác.");
        //        }

        //        var user = await _dbContext.Users
        //            .Include(u => u.Role)
        //            .Include(u => u.CompanyProfile)
        //            .FirstOrDefaultAsync(u => u.Email == request.Email);

        //        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        //        {
        //            return Unauthorized("Thông tin đăng nhập không hợp lệ.");
        //        }

        //        if (!user.IsActive)
        //        {
        //            return Forbid("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ hỗ trợ.");
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
        //                    Console.WriteLine($"Không thể gửi email xác thực: {ex.Message}");
        //                }
        //            }

        //            return BadRequest(new
        //            {
        //                message = "Email chưa được xác thực. Vui lòng kiểm tra hộp thư để xác thực tài khoản trước khi đăng nhập.",
        //                requiresVerification = true,
        //                userId = user.Id,
        //                email = user.Email
        //            });
        //        }

        //        if (user.Role == null)
        //        {
        //            return StatusCode(500, "Không tìm thấy vai trò người dùng.");
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
        //        return StatusCode(500, $"Lỗi đăng nhập: {ex.Message}");
        //    }
        //}

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!IsValidEmail(request.Email))
                {
                    return BadRequest("Định dạng email không hợp lệ.");
                }

                var user = await _dbContext.Users
                    .Include(u => u.Role)
                    .Include(u => u.CompanyProfile)
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                {
                    return Unauthorized("Thông tin đăng nhập không hợp lệ.");
                }

                if (!user.IsActive)
                {
                    return Forbid("Tài khoản bị khóa.");
                }

                if (!user.IsEmailVerified)
                {
                    if (user.EmailVerificationCodeExpiry == null || user.EmailVerificationCodeExpiry < DateTime.UtcNow)
                    {
                        user.EmailVerificationCode = GenerateVerificationCode();
                        user.EmailVerificationCodeExpiry = DateTime.UtcNow.AddHours(24);
                        await _dbContext.SaveChangesAsync();
                        _emailService.SendVerificationEmail(user.Email, user.EmailVerificationCode);
                    }
                    return BadRequest(new { message = "Email chưa xác thực.", requiresVerification = true, userId = user.Id, email = user.Email });
                }

                if (user.Role == null)
                {
                    return StatusCode(500, "Vai trò không tìm thấy.");
                }

                if (string.IsNullOrEmpty(user.FirebaseUid))
                {
                    user.FirebaseUid = Guid.NewGuid().ToString(); 
                    await _dbContext.SaveChangesAsync();
                }

                var customToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(user.FirebaseUid);
                var token = GenerateJwtToken(user);

                string? companyName = user.CompanyProfile?.CompanyName;
                string? urlCompanyLogo = user.CompanyProfile?.UrlCompanyLogo;

                return Ok(new
                {
                    Token = token,
                    FirebaseToken = customToken,
                    Role = user.Role.RoleName,
                    User = new { user.Id, user.FullName, user.Email, user.Phone, user.RoleId, user.Image, RoleName = user.Role.RoleName, CompanyName = companyName, UrlCompanyLogo = urlCompanyLogo }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi đăng nhập: {ex.Message}");
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
                    return BadRequest("Email và mã xác thực không được để trống.");
                }

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                {
                    return NotFound("Không tìm thấy người dùng.");
                }

                if (user.IsEmailVerified)
                {
                    return Ok("Email đã được xác thực trước đó.");
                }

                if (user.EmailVerificationCode != request.VerificationCode ||
                    user.EmailVerificationCodeExpiry == null ||
                    user.EmailVerificationCodeExpiry < DateTime.UtcNow)
                {
                    return BadRequest("Mã xác thực không hợp lệ hoặc đã hết hạn.");
                }

                // Mark email as verified
                user.IsEmailVerified = true;
                user.EmailVerificationCode = "VERIFIED"; // Use placeholder since we can't set null
                user.EmailVerificationCodeExpiry = null;
                user.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                return Ok("Xác thực email thành công. Bạn có thể đăng nhập ngay bây giờ.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi xác thực email: {ex.Message}");
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
                    return BadRequest("Định dạng email không hợp lệ.");
                }

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                {
                    return NotFound("Không tìm thấy người dùng.");
                }

                if (user.IsEmailVerified)
                {
                    return Ok("Email đã được xác thực trước đó.");
                }

                // Generate new verification code
                user.EmailVerificationCode = GenerateVerificationCode();
                user.EmailVerificationCodeExpiry = DateTime.UtcNow.AddHours(24);
                await _dbContext.SaveChangesAsync();

                // Send verification email
                try
                {
                    _emailService.SendVerificationEmail(user.Email, user.EmailVerificationCode);
                    return Ok("Mã xác thực đã được gửi lại đến email của bạn. Vui lòng kiểm tra hộp thư.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Không thể gửi email xác thực: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi gửi lại mã xác thực: {ex.Message}");
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
                    FirebaseUid = firebaseUid, // Gán rõ ràng
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsEmailVerified = true,
                    EmailVerificationCode = "VERIFIED",
                    EmailVerificationCodeExpiry = null
                };
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                // Reload user từ cơ sở dữ liệu để đảm bảo đồng bộ
                user = await _dbContext.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == email);
                Console.WriteLine($"Debug - Reloaded Firebase UID: {user.FirebaseUid}");
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








        //production
        //[HttpGet("google-response")]
        //public async Task<IActionResult> GoogleResponse()
        //{
        //    var authenticateResult = await HttpContext.AuthenticateAsync("External");
        //    if (!authenticateResult.Succeeded)
        //    {
        //        return Redirect($"https://job-finder-fe.vercel.app/auth/error?message={Uri.EscapeDataString("Authentication failed")}");
        //    }

        //    var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
        //    var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;

        //    if (string.IsNullOrEmpty(email))
        //    {
        //        return Redirect($"https://job-finder-fe.vercel.app/auth/error?message={Uri.EscapeDataString("Email not provided")}");
        //    }

        //    var user = await _dbContext.Users
        //        .Include(u => u.Role)
        //        .FirstOrDefaultAsync(u => u.Email == email);

        //    if (user == null)
        //    {
        //        var candidateRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == "Candidate");
        //        if (candidateRole == null)
        //        {
        //            return Redirect($"https://job-finder-fe.vercel.app/auth/error?message={Uri.EscapeDataString("User role not found")}");
        //        }

        //        // Tạo Firebase UID mới hoặc sử dụng email làm identifier tạm thời
        //        string firebaseUid = Guid.NewGuid().ToString(); // Tạo UID tạm thời, sau này có thể đồng bộ với Firebase

        //        user = new User
        //        {
        //            FullName = name,
        //            Email = email,
        //            RoleId = candidateRole.RoleId,
        //            FirebaseUid = firebaseUid, // Gán UID tạm thời
        //            CreatedAt = DateTime.UtcNow,
        //            UpdatedAt = DateTime.UtcNow,
        //            IsActive = true
        //        };
        //        _dbContext.Users.Add(user);
        //        await _dbContext.SaveChangesAsync();

        //        var candidateProfile = new CandidateProfile
        //        {
        //            UserId = user.Id ?? 0
        //        };
        //        _dbContext.CandidateProfiles.Add(candidateProfile);
        //        await _dbContext.SaveChangesAsync();

        //        user = await _dbContext.Users
        //            .Include(u => u.Role)
        //            .FirstOrDefaultAsync(u => u.Email == email);
        //    }

        //    // Tạo custom token mà không cần id_token
        //    var firebaseAuth = FirebaseAuth.DefaultInstance;
        //    string customToken = await firebaseAuth.CreateCustomTokenAsync(user.FirebaseUid); // Sử dụng UID đã gán

        //    var token = GenerateJwtToken(user);
        //    await HttpContext.SignOutAsync("External");

        //    return Redirect($"https://job-finder-fe.vercel.app/auth/callback?token={Uri.EscapeDataString(token)}&role={Uri.EscapeDataString(user.Role.RoleName)}&firebaseToken={Uri.EscapeDataString(customToken)}");
        //}


        //local 
        //[HttpGet("google-response")]
        //public async Task<IActionResult> GoogleResponse()
        //{
        //    var authenticateResult = await HttpContext.AuthenticateAsync("External");
        //    if (!authenticateResult.Succeeded)
        //    {
        //        return Redirect($"http://localhost:3000//auth/error?message={Uri.EscapeDataString("Authentication failed")}");
        //    }

        //    var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
        //    var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;

        //    if (string.IsNullOrEmpty(email))
        //    {
        //        return Redirect($"http://localhost:3000/auth/error?message={Uri.EscapeDataString("Email not provided")}");
        //    }

        //    var user = await _dbContext.Users
        //        .Include(u => u.Role)
        //        .FirstOrDefaultAsync(u => u.Email == email);

        //    if (user == null)
        //    {
        //        var candidateRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == "Candidate");
        //        if (candidateRole == null)
        //        {
        //            return Redirect($"http://localhost:3000/auth/error?message={Uri.EscapeDataString("User role not found")}");
        //        }

        //        // Tạo Firebase UID mới hoặc sử dụng email làm identifier tạm thời
        //        string firebaseUid = Guid.NewGuid().ToString(); // Tạo UID tạm thời, sau này có thể đồng bộ với Firebase

        //        user = new User
        //        {
        //            FullName = name,
        //            Email = email,
        //            RoleId = candidateRole.RoleId,
        //            FirebaseUid = firebaseUid, // Gán UID tạm thời
        //            CreatedAt = DateTime.UtcNow,
        //            UpdatedAt = DateTime.UtcNow,
        //            IsActive = true
        //        };
        //        _dbContext.Users.Add(user);
        //        await _dbContext.SaveChangesAsync();

        //        var candidateProfile = new CandidateProfile
        //        {
        //            UserId = user.Id ?? 0
        //        };
        //        _dbContext.CandidateProfiles.Add(candidateProfile);
        //        await _dbContext.SaveChangesAsync();

        //        user = await _dbContext.Users
        //            .Include(u => u.Role)
        //            .FirstOrDefaultAsync(u => u.Email == email);
        //    }

        //    // Tạo custom token mà không cần id_token
        //    var firebaseAuth = FirebaseAuth.DefaultInstance;
        //    string customToken = await firebaseAuth.CreateCustomTokenAsync(user.FirebaseUid); // Sử dụng UID đã gán

        //    var token = GenerateJwtToken(user);
        //    await HttpContext.SignOutAsync("External");

        //    return Redirect($"http://localhost:3000/auth/callback?token={Uri.EscapeDataString(token)}&role={Uri.EscapeDataString(user.Role.RoleName)}&firebaseToken={Uri.EscapeDataString(customToken)}");
        //}


    }
}

