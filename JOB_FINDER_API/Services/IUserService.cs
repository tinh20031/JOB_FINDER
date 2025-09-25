using System.Threading.Tasks;
using JOB_FINDER_API.Models;

namespace JOB_FINDER_API.Services
{
    public interface IUserService
    {
        Task<User?> GetByEmailAsync(string email);
        Task UpdatePasswordAsync(User user, string newPassword);
        // Thêm các hàm khác nếu cần
    }
}