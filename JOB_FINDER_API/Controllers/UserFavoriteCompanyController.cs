using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.DTO;
using JOB_FINDER_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserFavoriteCompanyController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        private readonly EmailService _emailService;

        public UserFavoriteCompanyController(JobFinderDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: api/UserFavoriteCompany/user/5
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetFavoritesByUser(int userId)
        {
            var favorites = await _context.UserFavoriteCompanies
                .Where(f => f.UserId == userId)
                .Include(f => f.Company)
                    .ThenInclude(u => u.CompanyProfile)
                .Select(f => new
                {
                    f.CompanyId
               
                })
                .ToListAsync();

            return Ok(favorites);
        }

        // GET: api/UserFavoriteCompany/{userId}/{companyId}
        [HttpGet("{userId}/{companyId}")]
        public async Task<IActionResult> IsFavorited(int userId, int companyId)
        {
            var exists = await _context.UserFavoriteCompanies
                .AnyAsync(f => f.UserId == userId && f.CompanyId == companyId);

            return Ok(new { isFavorite = exists });
        }

        // POST: api/UserFavoriteCompany
        [HttpPost]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> AddFavorite([FromBody] UserFavoriteCompanyCreateDto model)
        {
            var exists = await _context.UserFavoriteCompanies
                .FirstOrDefaultAsync(f => f.UserId == model.UserId && f.CompanyId == model.CompanyId);

            if (exists != null)
                return Conflict("Company is already in favorites.");

            var user = await _context.Users.FindAsync(model.UserId);
            var company = await _context.Users.FindAsync(model.CompanyId);

            if (user == null || company == null)
                return NotFound("User or Company not found.");

            var favorite = new UserFavoriteCompany
            {
                UserId = model.UserId,
                CompanyId = model.CompanyId,
                User = user,
                Company = company
            };

            _context.UserFavoriteCompanies.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok(favorite);
        }

        // DELETE: api/UserFavoriteCompany/{userId}/{companyId}
        [HttpDelete("{userId}/{companyId}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> RemoveFavorite(int userId, int companyId)
        {
            var favorite = await _context.UserFavoriteCompanies
                .FirstOrDefaultAsync(f => f.UserId == userId && f.CompanyId == companyId);

            if (favorite == null)
                return NotFound("Favorite not found.");

            _context.UserFavoriteCompanies.Remove(favorite);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Hàm này chỉ dùng nội bộ, không phải endpoint API
        [NonAction]
        public async Task NotifyFavoriteUsersOfNewJob(int companyId, Job job)
        {
            var favoriteUsers = await _context.UserFavoriteCompanies
                .Where(f => f.CompanyId == companyId)
                .Select(f => f.User)
                .ToListAsync();

            foreach (var user in favoriteUsers)
            {
                if (!string.IsNullOrEmpty(user?.Email))
                {
                    _emailService.SendEmail(
                        user.Email,
                        "Thông báo việc làm mới từ công ty yêu thích",
                        $"Công ty bạn yêu thích vừa đăng việc mới: {job.JobId}"
                    );
                }
            }
        }
    }
}