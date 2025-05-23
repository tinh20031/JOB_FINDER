
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
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(new
            {
                Id = user.Id,
                user.FullName,
                user.Email,
                user.Phone,
                user.Image,
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
                .Select(u => new
                {
                    Id = u.Id,

                    u.FullName,
                    u.Email,
                    u.Phone,
                    u.Image,
                    Role = u.Role.RoleName,
                    u.IsActive,
                    u.CreatedAt,
                    u.UpdatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.FullName = request.FullName ?? user.FullName;
            user.Email = request.Email ?? user.Email;
            user.Phone = request.Phone ?? user.Phone;
            if (request.RoleId.HasValue)
            {
                user.RoleId = request.RoleId.Value;
            }
            user.UpdatedAt = DateTime.UtcNow;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return Ok("User updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

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
                .FirstOrDefaultAsync(u => u.Id == id);

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
                .FirstOrDefaultAsync(u => u.Id == id);

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
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
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
                var emailInUse = await _dbContext.Users.AnyAsync(u => u.Email == request.Email && u.Id != id);
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

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new
            {
                user.Id,
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

