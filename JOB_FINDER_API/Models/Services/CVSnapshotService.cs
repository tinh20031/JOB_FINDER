namespace JOB_FINDER_API.Models.Services
{
    public class CvSnapshotService : ICvSnapshotService
    {
        public async Task<string> CaptureCvAsImageAsync(CV cv)
        {
            if (cv == null || string.IsNullOrEmpty(cv.FileUrl))
                throw new ArgumentException("CV or FileUrl is invalid");

            var sourcePath = cv.FileUrl;
            var fileName = $"cv_snapshot_{cv.Id}_{DateTime.UtcNow.Ticks}{Path.GetExtension(sourcePath)}";
            var snapshotsDir = Path.Combine("wwwroot", "snapshots");
            Directory.CreateDirectory(snapshotsDir); // Đảm bảo thư mục tồn tại
            var destPath = Path.Combine(snapshotsDir, fileName);

            using (var sourceStream = File.OpenRead(sourcePath))
            using (var destStream = File.Create(destPath))
            {
                await sourceStream.CopyToAsync(destStream);
            }

            // Trả về đường dẫn truy cập file snapshot
            return $"/snapshots/{fileName}";
        }
    }
}
