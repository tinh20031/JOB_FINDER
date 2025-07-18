using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JOB_FINDER_API.Services;
using JOB_FINDER_API.Models.Services;

namespace JOB_FINDER_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly CloudinaryService _cloudinaryService;
        private readonly VideoService _videoService;

        public VideoController(CloudinaryService cloudinaryService, VideoService videoService)
        {
            _cloudinaryService = cloudinaryService;
            _videoService = videoService;
        }

        [HttpPost("upload-video")]
        [Authorize]
        public async Task<IActionResult> UploadVideo(IFormFile file, [FromQuery] int candidateProfileId)
        {
            try
            {
                // Get the authenticated user's ID
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated.");
                }

                // Validate candidateProfileId belongs to the authenticated user
                var profile = await _videoService.GetCandidateProfileAsync(candidateProfileId);
                if (profile == null || profile.UserId.ToString() != userId)
                {
                    return Forbidden("You are not authorized to upload a video for this profile.");
                }

                // Upload video to Cloudinary
                var url = await _cloudinaryService.UploadVideoAsync(file);
                if (url == null)
                {
                    return BadRequest("No file uploaded or file is empty.");
                }

                // Save video URL to CandidateProfile
                await _videoService.SaveVideoUrlAsync(candidateProfileId, url);

                return Ok(new { url });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Upload failed: {ex.Message}" });
            }
        }

        private IActionResult Forbidden(string message)
        {
            return StatusCode(403, new { error = message });
        }
    }
}