using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.Requests;
using JOB_FINDER_API.Models.Services;
using JOB_FINDER_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UglyToad.PdfPig;

namespace JOB_FINDER_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CVController : ControllerBase
    {
        private readonly JobFinderDbContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly SemanticMatchingService _semanticMatchingService;
        private readonly ILogger<CVController> _logger;

        public CVController(
            JobFinderDbContext context,
            CloudinaryService cloudinaryService,
            SemanticMatchingService semanticMatchingService,
            ILogger<CVController> logger)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
            _semanticMatchingService = semanticMatchingService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.CVs.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _context.CVs.FindAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [Authorize]
        [HttpGet("my-cvs")]
        public async Task<IActionResult> GetMyCVs()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user ID.");
            var cvs = await _context.CVs
                .Where(c => c.UserId == userId && c.Type == CvType.Upload)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return Ok(cvs);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateCVRequest request)
        {

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId) || userId != request.UserId)
            {
                _logger.LogWarning("Unauthorized attempt to create CV for UserId {UserId} by {ClaimUserId}", request.UserId, userIdStr);
                return Unauthorized("You can only create a CV for your own account.");
            }

            var cvCount = await _context.CVs.CountAsync(c => c.UserId == userId && c.Type == CvType.Upload);
            if (cvCount >= 5)
            {
                return BadRequest("You can only upload a maximum of 5 CVs. Please delete old CVs to upload new ones.");
            }

            if (request.File == null || request.File.Length == 0)
            {
                _logger.LogWarning("No file selected for CV creation by UserId {UserId}", userId);
                return BadRequest("No file selected.");
            }


            if (!request.File.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) &&
                !Path.GetExtension(request.File.FileName).ToLower().EndsWith(".pdf"))
            {
                _logger.LogWarning("Invalid file format for CV by UserId {UserId}. Only PDF is allowed.", userId);
                return BadRequest("Only PDF files are allowed.");
            }


            var fileUrl = await _cloudinaryService.UploadCvAsync(request.File);
            if (string.IsNullOrEmpty(fileUrl))
            {
                _logger.LogError("Failed to upload CV to Cloudinary for UserId {UserId}", userId);
                return StatusCode(500, "Failed to upload CV to Cloudinary.");
            }

            string extractedText = string.Empty;
            string cvSummary = string.Empty;
            CVData cvData = new CVData();
            try
            {
                using (var stream = request.File.OpenReadStream())
                using (var pdfDocument = PdfDocument.Open(stream))
                {
                    foreach (var page in pdfDocument.GetPages())
                    {
                        extractedText += page.Text + "\n";
                    }
                }
                extractedText = CleanExtractedText(extractedText);

                if (string.IsNullOrWhiteSpace(extractedText))
                {
                    _logger.LogWarning("CV content is empty or unreadable for UserId {UserId}", userId);
                    return BadRequest("CV content is empty or unreadable.");
                }


                var (success, extractError, extractedCvData, extractedSummary) = await _semanticMatchingService.ExtractCvDataAsync(null, extractedText);
                if (!success)
                {
                    _logger.LogError("Failed to extract CV data for UserId {UserId}: {Error}", userId, extractError);
                    return BadRequest($"Failed to extract CV data: {extractError}");
                }

                cvData = extractedCvData;
                cvSummary = extractedSummary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PDF extraction failed for UserId {UserId}", userId);
                return BadRequest($"PDF extraction failed: {ex.Message}");
            }


            var jsonOptions = new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            var fullCvJson = JsonSerializer.Serialize(new
            {
                Text = extractedText,
                TranslatedText = string.Empty,
                Summary = cvSummary,
                CVData = cvData
            }, jsonOptions);


            var cv = new CV
            {
                UserId = request.UserId,
                FileUrl = fileUrl,
                FullCvJson = fullCvJson,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Type = CvType.Upload
            };

            _context.CVs.Add(cv);
            await _context.SaveChangesAsync();
            _logger.LogInformation("CV created successfully for UserId {UserId}, CVId {CVId}", userId, cv.CVId);

            return CreatedAtAction(nameof(Get), new { id = cv.CVId }, cv);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CV model)
        {

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId) || userId != model.UserId)
            {
                _logger.LogWarning("Unauthorized attempt to update CV {CVId} by UserId {ClaimUserId}", id, userIdStr);
                return Unauthorized("You can only update your own CV.");
            }

            if (id != model.CVId)
            {
                _logger.LogWarning("Invalid CVId {CVId} for update by UserId {UserId}", id, userId);
                return BadRequest("Invalid CV ID.");
            }

            _context.Entry(model).State = EntityState.Modified;
            model.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _logger.LogInformation("CV {CVId} updated successfully for UserId {UserId}", id, userId);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cv = await _context.CVs.FindAsync(id);
            if (cv == null)
            {
                _logger.LogWarning("CV {CVId} not found for deletion", id);
                return NotFound();
            }

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId) || userId != cv.UserId)
            {
                _logger.LogWarning("Unauthorized attempt to delete CV {CVId} by UserId {ClaimUserId}", id, userIdStr);
                return Unauthorized("You can only delete your own CV.");
            }


            var relatedApplications = _context.Applications.Where(a => a.CvId == id);
            _context.Applications.RemoveRange(relatedApplications);

            _context.CVs.Remove(cv);
            await _context.SaveChangesAsync();
            _logger.LogInformation("CV {CVId} deleted successfully for UserId {UserId}", id, userId);

            return NoContent();
        }

        private string CleanExtractedText(string text)
        {
            text = Regex.Replace(text, @"(Page|Trang)\s*\d+\s*(of|\/|-)\s*\d+\s*(-?\s*©.*)?", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"©\s*[^\n]+", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\n{2,}", "\n");
            text = Regex.Replace(text, @"\s{2,}", " ");
            text = Regex.Replace(text, @"[\t\r]+", " ");
            return text.Trim();
        }
    }
}

