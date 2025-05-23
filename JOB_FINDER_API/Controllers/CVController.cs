/*using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CVController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        private readonly string _cvFolder = "UploadedCVs";

        public CVController(JobFinderDbContext context)
        {
            _context = context;
            if (!Directory.Exists(_cvFolder))
            {
                Directory.CreateDirectory(_cvFolder);
            }
        }
         
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.CVs.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _context.CVs.FindAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateCVRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No file selected.");

            var fileName = Path.GetFileName(request.File.FileName);
            var filePath = Path.Combine(_cvFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            var cv = new CV
            {
                UserId = request.UserId,
                FileUrl = filePath,
                FullCvJson = request.FullCvJson,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.CVs.Add(cv);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = cv.Id }, cv);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CV model)
        {
            if (id != model.Id) return BadRequest();
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.CVs.FindAsync(id);
            if (item == null) return NotFound();
            _context.CVs.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}*/
using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.Requests;
using JOB_FINDER_API.Services; // Add this for CloudinaryService
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CVController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        private readonly CloudinaryService _cloudinaryService;

        public CVController(JobFinderDbContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.CVs.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _context.CVs.FindAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateCVRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No file selected.");

            // Upload CV to Cloudinary
            var fileUrl = await _cloudinaryService.UploadCvAsync(request.File);
            if (string.IsNullOrEmpty(fileUrl))
                return StatusCode(500, "Failed to upload CV to Cloudinary.");

            var cv = new CV
            {
                UserId = request.UserId,
                FileUrl = fileUrl,
                FullCvJson = request.FullCvJson,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.CVs.Add(cv);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = cv.Id }, cv);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CV model)
        {
            if (id != model.Id) return BadRequest();
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cv = await _context.CVs.FindAsync(id);
            if (cv == null)
            {
                return NotFound();
            }

            // Remove related Applications first
            var relatedApplications = _context.Applications.Where(a => a.CvId == id);
            _context.Applications.RemoveRange(relatedApplications);

            _context.CVs.Remove(cv);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}