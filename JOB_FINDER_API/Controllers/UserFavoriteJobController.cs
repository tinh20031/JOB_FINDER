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
        public UserFavoriteJobController(JobFinderDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.UserFavoriteJobs.ToListAsync());

        [HttpGet("{userId}/{jobId}")]
        public async Task<IActionResult> Get(int userId, int jobId)
        {
            var item = await _context.UserFavoriteJobs.FindAsync(userId, jobId);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserFavoriteJob model)
        {
            _context.UserFavoriteJobs.Add(model);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { userId = model.UserId, jobId = model.JobId }, model);
        }

        [HttpDelete("{userId}/{jobId}")]
        public async Task<IActionResult> Delete(int userId, int jobId)
        {
            var item = await _context.UserFavoriteJobs.FindAsync(userId, jobId);
            if (item == null) return NotFound();
            _context.UserFavoriteJobs.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}