using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserFavoriteJobController : ControllerBase
    {
        private readonly JobFinderDbContext _context;

        public UserFavoriteJobController(JobFinderDbContext context)
        {
            _context = context;
        }


        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetFavoritesByUser(int userId)
        {
            var favorites = await _context.UserFavoriteJobs
                .Where(f => f.UserId == userId)
                .Include(f => f.Job)
                .ToListAsync();

            return Ok(favorites);
        }

        
        [HttpGet("{userId}/{jobId}")]
        public async Task<IActionResult> IsFavorited(int userId, int jobId)
        {
            var exists = await _context.UserFavoriteJobs.FindAsync(userId, jobId);
            return Ok(new { isFavorite = exists != null });
        }


        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] UserFavoriteJob model)
        {
            var exists = await _context.UserFavoriteJobs.FindAsync(model.UserId, model.JobId);
            if (exists != null)
                return Conflict("Job is already in favorites.");

            _context.UserFavoriteJobs.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(IsFavorited), new { userId = model.UserId, jobId = model.JobId }, model);
        }

        
        [HttpDelete("{userId}/{jobId}")]
        public async Task<IActionResult> RemoveFavorite(int userId, int jobId)
        {
            var favorite = await _context.UserFavoriteJobs.FindAsync(userId, jobId);
            if (favorite == null)
                return NotFound("Favorite not found.");

            _context.UserFavoriteJobs.Remove(favorite);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
