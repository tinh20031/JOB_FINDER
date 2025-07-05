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
            IUserService userService,
            IMemoryCache cache)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _emailService = new EmailService(configuration);
            _userService = userService;
            _cache = cache;
        }

        // Helper: Validate email format
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            string pattern = @"^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        // Đăng ký tài khoản
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!IsValidEmail(request.Email))
                return BadRequest("Email không hợp lệ.");

            if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("Email đã được sử dụng.");

            var userRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == "Candidate");
            if (userRole == null)
                return StatusCode(500, "Không tìm thấy vai trò mặc định.");

            string verificationCode = GenerateVerificationCode();

            // Tạo user trên Firebase
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
                return StatusCode(500, $"Không thể tạo người dùng trên Firebase: {ex.Message}");
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

            _dbContext.CandidateProfiles.Add(new CandidateProfile { UserId = user.UserId ?? 0 });
            await _dbContext.SaveChangesAsync();

            try
            {
                _emailService.SendVerificationEmail(user.Email, verificationCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Không gửi được email xác thực: {ex.Message}");
            }

            return Ok(new
            {
                message = "Đăng ký thành công. Vui lòng kiểm tra email để xác thực tài khoản.",
                userId = user.UserId,
                firebaseUid = user.FirebaseUid,
                email = user.Email
            });
        }

        // Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!IsValidEmail(request.Email))
                return BadRequest("Email không hợp lệ.");

            var user = await _dbContext.Users
                .Include(u => u.Role)
                .Include(u => u.CompanyProfile)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return Unauthorized("Thông tin đăng nhập không đúng.");

            if (!user.IsActive)
                return Forbid("Tài khoản đã bị khóa.");

            if (!user.IsEmailVerified)
            {
                if (user.EmailVerificationCodeExpiry == null || user.EmailVerificationCodeExpiry < DateTime.UtcNow)
                {
                    user.EmailVerificationCode = GenerateVerificationCode();
                    user.EmailVerificationCodeExpiry = DateTime.UtcNow.AddHours(24);
                    await _dbContext.SaveChangesAsync();

                    try
                    {
                        _emailService.SendVerificationEmail(user.Email, user.EmailVerificationCode);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Không gửi được email xác thực: {ex.Message}");
                    }
                }

                return BadRequest(new
                {
                    message = "Email chưa xác thực. Vui lòng kiểm tra email.",
                    requiresVerification = true,
                    userId = user.UserId,
                    email = user.Email
                });
            }

            if (user.Role == null)
                return StatusCode(500, "Không tìm thấy vai trò người dùng.");

            var token = GenerateJwtToken(user);

            string? companyName = user.CompanyProfile?.CompanyName;
            string? urlCompanyLogo = user.CompanyProfile?.UrlCompanyLogo;

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

        // Sinh mã xác thực
        private string GenerateVerificationCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        // Xác thực email
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.VerificationCode))
                return BadRequest("Email và mã xác thực không được để trống.");

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return NotFound("Không tìm thấy người dùng.");

            if (user.IsEmailVerified)
                return Ok("Email đã được xác thực.");

            if (user.EmailVerificationCode != request.VerificationCode ||
                user.EmailVerificationCodeExpiry == null ||
                user.EmailVerificationCodeExpiry < DateTime.UtcNow)
                return BadRequest("Mã xác thực không đúng hoặc đã hết hạn.");

            user.IsEmailVerified = true;
            user.EmailVerificationCode = "VERIFIED";
            user.EmailVerificationCodeExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return Ok("Xác thực email thành công. Bạn có thể đăng nhập.");
        }

        // Gửi lại mã xác thực email
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequest request)
        {
            if (!IsValidEmail(request.Email))
                return BadRequest("Email không hợp lệ.");

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return NotFound("Không tìm thấy người dùng.");

            if (user.IsEmailVerified)
                return Ok("Email đã được xác thực.");

            user.EmailVerificationCode = GenerateVerificationCode();
            user.EmailVerificationCodeExpiry = DateTime.UtcNow.AddHours(24);
            await _dbContext.SaveChangesAsync();

            try
            {
                _emailService.SendVerificationEmail(user.Email, user.EmailVerificationCode);
                return Ok("Đã gửi lại mã xác thực qua email.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Không gửi được email xác thực: {ex.Message}");
            }
        }

        // Đăng xuất
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Đăng xuất thành công. Vui lòng xóa token ở phía client." });
        }

        // Đổi mật khẩu (yêu cầu đăng nhập)
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest("Vui lòng nhập đầy đủ mật khẩu cũ và mới.");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Không tìm thấy người dùng.");

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password))
                return BadRequest("Mật khẩu hiện tại không đúng.");

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return Ok("Đổi mật khẩu thành công.");
        }

        // Quên mật khẩu - Bước 1: Gửi mã xác nhận
        [HttpPost("forgot-password/request")]
        public async Task<IActionResult> RequestForgotPassword([FromBody] ForgotPasswordRequestDto dto)
        {
            var user = await _userService.GetByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("Email không tồn tại.");

            var code = GenerateVerificationCode();
            _cache.Set($"fp_{dto.Email}", code, TimeSpan.FromMinutes(10));
            _emailService.SendEmail(dto.Email, "Mã xác nhận quên mật khẩu", $"Mã xác nhận của bạn là: {code}", false);
            return Ok("Đã gửi mã xác nhận về email.");
        }

        // Quên mật khẩu - Bước 2: Xác thực mã
        [HttpPost("forgot-password/verify")]
        public IActionResult VerifyForgotPassword([FromBody] ForgotPasswordVerifyDto dto)
        {
            if (_cache.TryGetValue($"fp_{dto.Email}", out string code) && code == dto.Code)
                return Ok("Mã xác nhận hợp lệ.");
            return BadRequest("Mã xác nhận không đúng hoặc đã hết hạn.");
        }

        // Quên mật khẩu - Bước 3: Đặt lại mật khẩu mới
        [HttpPost("forgot-password/reset")]
        public async Task<IActionResult> ResetForgotPassword([FromBody] ForgotPasswordResetDto dto)
        {
            if (_cache.TryGetValue($"fp_{dto.Email}", out string code) && code == dto.Code)
            {
                var user = await _userService.GetByEmailAsync(dto.Email);
                if (user == null)
                    return BadRequest("Email không tồn tại.");

                await _userService.UpdatePasswordAsync(user, dto.NewPassword);
                _cache.Remove($"fp_{dto.Email}");
                return Ok("Đặt lại mật khẩu thành công.");
            }
            return BadRequest("Mã xác nhận không đúng hoặc đã hết hạn.");
        }
        [HttpPost("forgot-password/resend-verification")]
        public async Task<IActionResult> ResendForgotPasswordVerification([FromBody] ForgotPasswordResendVerificationDto dto)
        {
            if (!IsValidEmail(dto.Email))
                return BadRequest("Email không hợp lệ.");

            var user = await _userService.GetByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("Email không tồn tại.");

            // ⏳ Kiểm tra giới hạn thời gian gửi lại
            string throttleKey = $"fp_throttle_{dto.Email}";
            if (_cache.TryGetValue(throttleKey, out _))
            {
                return BadRequest("Bạn vừa yêu cầu mã xác nhận. Vui lòng đợi 60 giây trước khi gửi lại.");
            }

            // ✅ Nếu được phép, tiếp tục gửi lại mã
            var code = GenerateVerificationCode();
            _cache.Set($"fp_{dto.Email}", code, TimeSpan.FromMinutes(10));

            // ⏱️ Đặt giới hạn gửi lại 60 giây
            _cache.Set(throttleKey, true, TimeSpan.FromSeconds(60));

            try
            {
                _emailService.SendEmail(dto.Email, "Mã xác nhận quên mật khẩu (Gửi lại)", $"Mã xác nhận mới của bạn là: {code}", false);
                return Ok("Đã gửi lại mã xác nhận về email.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Không gửi được email: {ex.Message}");
            }
        }


        // Helper: Sinh JWT token
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
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
    }
}

