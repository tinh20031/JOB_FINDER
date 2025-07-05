using System.Threading.Tasks;
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using Microsoft.EntityFrameworkCore;

namespace JOB_FINDER_API.Services
{
    public class UserService : IUserService
    {
        private readonly JobFinderDbContext _dbContext;

        public UserService(JobFinderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task UpdatePasswordAsync(User user, string newPassword)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }
    }
}