using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Models.Background;
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
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CVController(
            JobFinderDbContext context,
            CloudinaryService cloudinaryService,
            SemanticMatchingService semanticMatchingService,
            ILogger<CVController> logger,
             IBackgroundTaskQueue taskQueue,
             IServiceScopeFactory serviceScopeFactory)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
            _semanticMatchingService = semanticMatchingService;
            _logger = logger;
            _taskQueue = taskQueue;
            _serviceScopeFactory = serviceScopeFactory;
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


        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateCVRequest request)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId) || userId != request.UserId)
            {
                _logger.LogWarning("Unauthorized attempt to create CV for UserId {UserId} by {ClaimUserId} at {Time}", request.UserId, userIdStr, DateTime.Now);
                return Unauthorized("You can only create a CV for your own account.");
            }

            var cvCount = await _context.CVs.CountAsync(c => c.UserId == userId && c.Type == CvType.Upload);
            if (cvCount >= 5)
            {
                _logger.LogWarning("UserId {UserId} exceeded CV upload limit (5) at {Time}", userId, DateTime.Now);
                return BadRequest("You can only upload a maximum of 5 CVs. Please delete old CVs to upload new ones.");
            }

            if (request.File == null || request.File.Length == 0)
            {
                _logger.LogWarning("No file selected for CV creation by UserId {UserId} at {Time}", userId, DateTime.Now);
                return BadRequest("No file selected.");
            }

            if (!request.File.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) &&
                !Path.GetExtension(request.File.FileName).ToLower().EndsWith(".pdf"))
            {
                _logger.LogWarning("Invalid file format for CV by UserId {UserId} at {Time}. Only PDF is allowed.", userId, DateTime.Now);
                return BadRequest("Only PDF files are allowed.");
            }

            string tempCvPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");
            string originalFileName = Path.GetFileNameWithoutExtension(request.File.FileName); // Lấy tên tệp gốc
            try
            {
                using (var stream = new FileStream(tempCvPath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }

               
                var fileUrl = await _cloudinaryService.UploadCvAsync(
                    new FormFile(
                        new FileStream(tempCvPath, FileMode.Open, FileAccess.Read, FileShare.Read),
                        0,
                        new FileInfo(tempCvPath).Length,
                        null,
                        Path.GetFileName(tempCvPath)),
                    SanitizeFileName(originalFileName) 
                );

                if (string.IsNullOrEmpty(fileUrl))
                {
                    _logger.LogError("Failed to upload CV to Cloudinary for UserId {UserId} at {Time}", userId, DateTime.Now);
                    return StatusCode(500, "Failed to upload CV to Cloudinary.");
                }

                var cv = new CV
                {
                    UserId = userId,
                    FileUrl = fileUrl,
                    FullCvJson = null, 
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Type = CvType.Upload
                };

                _context.CVs.Add(cv);
                await _context.SaveChangesAsync();
                _logger.LogInformation("CV saved with CVId {CVId} for UserId {UserId} at {Time}", cv.CVId, userId, DateTime.Now);

                // Đưa xử lý trích xuất vào tác vụ nền
                _taskQueue.QueueBackgroundWorkItem(async token =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var innerContext = scope.ServiceProvider.GetRequiredService<JobFinderDbContext>();
                    var innerSemanticService = scope.ServiceProvider.GetRequiredService<SemanticMatchingService>();

                    try
                    {
                        using var transaction = await innerContext.Database.BeginTransactionAsync();

                        var innerCv = await innerContext.CVs.FindAsync(cv.CVId);
                        if (innerCv == null)
                        {
                            _logger.LogError("CV {CVId} not found during background processing for UserId {UserId} at {Time}", cv.CVId, userId, DateTime.Now);
                            return;
                        }

                        string extractedText = string.Empty;
                        try
                        {
                            // Đảm bảo stream được đóng đúng cách
                            using (var stream = new FileStream(tempCvPath, FileMode.Open, FileAccess.Read, FileShare.Read))
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
                                _logger.LogWarning("CV content is empty or unreadable for CVId {CVId}, UserId {UserId} at {Time}", cv.CVId, userId, DateTime.Now);
                                innerCv.FullCvJson = JsonSerializer.Serialize(new
                                {
                                    Text = string.Empty,
                                    TranslatedText = string.Empty,
                                    CVData = new CVData()
                                }, new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
                                innerCv.UpdatedAt = DateTime.UtcNow;
                                await innerContext.SaveChangesAsync();
                                await transaction.CommitAsync();
                                return;
                            }

                            var (success, extractError, extractedCvData) = await innerSemanticService.ExtractCvDataAsync(null, extractedText);
                            if (!success)
                            {
                                _logger.LogError("Failed to extract CV data for CVId {CVId}, UserId {UserId} at {Time}: {Error}", cv.CVId, userId, DateTime.Now, extractError);
                                innerCv.FullCvJson = JsonSerializer.Serialize(new
                                {
                                    Text = extractedText,
                                    TranslatedText = string.Empty,
                                    CVData = new CVData()
                                }, new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
                                innerCv.UpdatedAt = DateTime.UtcNow;
                                await innerContext.SaveChangesAsync();
                                await transaction.CommitAsync();
                                return;
                            }

                            innerCv.FullCvJson = JsonSerializer.Serialize(new
                            {
                                Text = extractedText,
                                TranslatedText = string.Empty,
                                CVData = extractedCvData
                            }, new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
                            innerCv.UpdatedAt = DateTime.UtcNow;
                            await innerContext.SaveChangesAsync();
                            await transaction.CommitAsync();

                            _logger.LogInformation("CV processing completed for CVId {CVId}, UserId {UserId} at {Time}", cv.CVId, userId, DateTime.Now);
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            _logger.LogError(ex, "PDF extraction failed for CVId {CVId}, UserId {UserId} at {Time}. Details: {Message}", cv.CVId, userId, DateTime.Now, ex.Message);
                            innerCv.FullCvJson = JsonSerializer.Serialize(new
                            {
                                Text = string.Empty,
                                TranslatedText = string.Empty,
                                CVData = new CVData()
                            }, new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
                            innerCv.UpdatedAt = DateTime.UtcNow;
                            await innerContext.SaveChangesAsync();
                            return;
                        }
                    }
                    finally
                    {
                        if (System.IO.File.Exists(tempCvPath))
                        {
                            try
                            {
                                // Thêm cơ chế retry khi xóa tệp
                                int retries = 3;
                                while (retries > 0)
                                {
                                    try
                                    {
                                        System.IO.File.Delete(tempCvPath);
                                        _logger.LogInformation("Temporary CV file {TempCvPath} deleted successfully for CVId {CVId}, UserId {UserId} at {Time}", tempCvPath, cv.CVId, userId, DateTime.Now);
                                        break;
                                    }
                                    catch (IOException)
                                    {
                                        retries--;
                                        if (retries == 0) throw;
                                        await Task.Delay(1000); // Chờ 1 giây trước khi thử lại
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Failed to delete temporary CV file {TempCvPath} for CVId {CVId}, UserId {UserId} at {Time}", tempCvPath, cv.CVId, userId, DateTime.Now);
                            }
                        }
                    }
                });

                return Ok(new
                {
                    Success = true,
                    Message = "CV uploaded successfully. Processing content in background.",
                    CVId = cv.CVId,
                    FileUrl = fileUrl
                });
            }
            catch (Exception ex)
            {
                if (System.IO.File.Exists(tempCvPath))
                {
                    try
                    {
                        System.IO.File.Delete(tempCvPath);
                    }
                    catch (Exception fileEx)
                    {
                        _logger.LogWarning(fileEx, "Failed to delete temporary CV file {TempCvPath} for UserId {UserId} at {Time}", tempCvPath, userId, DateTime.Now);
                    }
                }
                _logger.LogError(ex, "Error initiating CV upload for UserId {UserId} at {Time}. Details: {Message}", userId, DateTime.Now, ex.Message);
                return StatusCode(500, new { Success = false, ErrorMessage = $"An error occurred while initiating CV upload: {ex.Message}" });
            }
        }

        private string CleanExtractedText(string text)
        {
            return text?.Trim() ?? string.Empty;
        }

        private string SanitizeFileName(string fileName)
        {
           
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var c in invalidChars)
            {
                fileName = fileName.Replace(c, '_');
            }
         
            fileName = fileName.Replace(" ", "_").Trim();
            if (fileName.Length > 100)
            {
                fileName = fileName.Substring(0, 100);
            }
            return fileName;
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

      
    }
}