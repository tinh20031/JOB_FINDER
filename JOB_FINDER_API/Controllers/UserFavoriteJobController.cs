using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using Microsoft.AspNetCore.Authorization; // Add this
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JOB_FINDER_API.Models.DTO;
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
                .Where(f => f.UserId == userId && f.Job.Status != Job.JobStatus.pending && f.Job.Status != Job.JobStatus.inactive)
                .Include(f => f.Job)
                .ToListAsync();

            return Ok(favorites);
        }

        [HttpGet("{userId}/{jobId}")]
        public async Task<IActionResult> IsFavorited(int userId, int jobId)
        {
            var favorite = await _context.UserFavoriteJobs
                .Include(f => f.Job)
                .FirstOrDefaultAsync(f => f.UserId == userId && f.JobId == jobId);

            bool isFavorite = favorite != null && favorite.Job != null
                && favorite.Job.Status != Job.JobStatus.pending
                && favorite.Job.Status != Job.JobStatus.inactive;

            return Ok(new { isFavorite });
        }

        [HttpPost]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> AddFavorite([FromBody] UserFavoriteJobCreateDto model)
        {
            var exists = await _context.UserFavoriteJobs.FindAsync(model.UserId, model.JobId);
            if (exists != null)
                return Conflict("Job is already in favorites.");

            var favorite = new UserFavoriteJob
            {
                UserId = model.UserId,
                JobId = model.JobId
            };

            _context.UserFavoriteJobs.Add(favorite);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(IsFavorited), new { userId = model.UserId, jobId = model.JobId }, favorite);
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