using JOB_FINDER_API.Data;
using JOB_FINDER_API.Models;
using Microsoft.EntityFrameworkCore;

namespace JOB_FINDER_API.Models.Services
{
    public class VideoService
    {
        private readonly JobFinderDbContext _context;

        public VideoService(JobFinderDbContext context)
        {
            _context = context;
        }

        public async Task SaveVideoUrlAsync(int candidateProfileId, string videoUrl)
        {
            var profile = await _context.CandidateProfiles.FindAsync(candidateProfileId);
            if (profile == null)
            {
                throw new ArgumentException("Candidate profile not found.");
            }

            profile.VideoUrl = videoUrl;
            profile.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<CandidateProfile?> GetCandidateProfileAsync(int candidateProfileId)
        {
            return await _context.CandidateProfiles
                .FirstOrDefaultAsync(p => p.CandidateProfileId == candidateProfileId);
        }
    }
}