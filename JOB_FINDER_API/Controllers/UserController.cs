
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.Requests;
using JOB_FINDER_API.Services;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly JobFinderDbContext _dbContext;

        public UserController(JobFinderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _dbContext.Users
                .Include(u => u.Role)
                .Include(u => u.CompanyProfile)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var isCompany = user.Role?.RoleName == "Company";

            return Ok(new
            {
                Id = user.UserId,
                user.FullName,
                user.Email,
                user.Phone,
                user.Image,
                urlCompanyLogo = isCompany ? user.CompanyProfile?.UrlCompanyLogo : null,
                Role = user.Role.RoleName,
                user.IsActive,
                user.CreatedAt,
                user.UpdatedAt
            });
        }

     
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _dbContext.Users
                .Include(u => u.Role)
                .Include(u => u.CompanyProfile)
                .ToListAsync();

            var result = users.Select(u => {
                var isCompany = u.Role?.RoleName == "Company";

                return new
                {
                    Id = u.UserId,
                    u.FullName,
                    u.Email,
                    u.Phone,
                    u.Image,
                    urlCompanyLogo = isCompany ? u.CompanyProfile?.UrlCompanyLogo : null,
                    Role = u.Role?.RoleName,
                    u.IsActive,
                    u.CreatedAt,
                    u.UpdatedAt
                };
            });

            return Ok(result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromForm] UpdateUserRequest request, IFormFile? imageFile, [FromServices] CloudinaryService cloudinaryService)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
                return NotFound("User not found.");

            // Kiểm tra và cập nhật email nếu thay đổi
            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
            {
                var emailInUse = await _dbContext.Users.AnyAsync(u => u.Email == request.Email && u.UserId != id);
                if (emailInUse)
                    return BadRequest("Email is already in use by another user.");
                user.Email = request.Email;
            }

            // Cập nhật FullName
            if (!string.IsNullOrWhiteSpace(request.FullName))
                user.FullName = request.FullName;

            // Cập nhật Phone
            if (!string.IsNullOrWhiteSpace(request.Phone))
                user.Phone = request.Phone;

            // Cập nhật Role nếu có
            /*if (request.RoleId.HasValue)
            {
                var roleExists = await _dbContext.Roles.AnyAsync(r => r.RoleId == request.RoleId.Value);
                if (!roleExists)
                    return BadRequest("Invalid RoleId.");
                user.RoleId = request.RoleId.Value;
            }*/

            // Cập nhật Image nếu có file upload
            if (imageFile != null)
            {
                var imageUrl = await cloudinaryService.UploadImageAsync(imageFile);
                user.Image = imageUrl;
            }

            user.UpdatedAt = DateTime.UtcNow;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                user.UserId,
                user.FullName,
                user.Email,
                user.Phone,
                user.Image,
                //user.RoleId,
                user.UpdatedAt
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return Ok("User deleted successfully.");
        }

        [HttpPut("{id}/lock")]
        public async Task<IActionResult> LockUser(int id)
        {
            var user = await _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
                return NotFound("User not found.");

            if (user.Role.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Cannot lock an admin account.");

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return Ok("User has been locked.");
        }

        [HttpPut("{id}/unlock")]
        public async Task<IActionResult> UnlockUser(int id)
        {
            var user = await _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
                return NotFound("User not found.");

            if (user.Role.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Cannot unlock an admin account.");

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return Ok("User has been unlocked.");
        }


        [HttpPut("full/{id}")]
        public async Task<IActionResult> PutUserFull(int id, [FromForm] UpdateUserFullRequest request, IFormFile? imageFile, [FromServices] CloudinaryService cloudinaryService)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
                return NotFound("User not found.");

            // Xử lý roleId
            if (request.RoleId.HasValue)
            {
                var roleExists = await _dbContext.Roles.AnyAsync(r => r.RoleId == request.RoleId.Value);
                if (!roleExists)
                    return BadRequest("Invalid RoleId.");
                user.RoleId = request.RoleId.Value;
            }

            if (!string.IsNullOrWhiteSpace(request.FullName) && request.FullName != "string")
                user.FullName = request.FullName;

            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != "string")
            {
                var emailInUse = await _dbContext.Users.AnyAsync(u => u.Email == request.Email && u.UserId != id);
                if (emailInUse)
                    return BadRequest("Email is already in use by another user.");
                user.Email = request.Email;
            }

            if (!string.IsNullOrWhiteSpace(request.Phone) && request.Phone != "string")
                user.Phone = request.Phone;

            if (!string.IsNullOrWhiteSpace(request.Password) && request.Password != "string")
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

            if (imageFile != null)
            {
                var imageUrl = await cloudinaryService.UploadImageAsync(imageFile);
                user.Image = imageUrl;
            }

            user.UpdatedAt = DateTime.UtcNow;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return Ok("User fully updated successfully.");
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddUser([FromForm] CreateUserRequest request, IFormFile? imageFile, [FromServices] CloudinaryService cloudinaryService)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and Password are required.");

            var emailExists = await _dbContext.Users.AnyAsync(u => u.Email == request.Email);
            if (emailExists)
                return BadRequest("Email is already in use.");

            string? imageUrl = null;
            if (imageFile != null)
            {
                imageUrl = await cloudinaryService.UploadImageAsync(imageFile);
            }

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = request.RoleId,
                IsActive = true,
                Image = imageUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, new
            {
                user.UserId,
                user.FullName,
                user.Email,
                user.Phone,
                user.Image,
                user.IsActive,
                user.CreatedAt,
                user.UpdatedAt
            });
        }

    }
}