//snapshot 1 trang cv
using ImageMagick;

namespace JOB_FINDER_API.Models.Services
{
    public class CvSnapshotService : ICvSnapshotService
    {
        public async Task<string> CaptureCvAsImageAsync(CV cv)
        {
            if (cv == null || string.IsNullOrEmpty(cv.FileUrl))
                throw new ArgumentException("CV or FileUrl is invalid");

            var sourcePath = cv.FileUrl;
            var snapshotsDir = Path.Combine("wwwroot", "snapshots");
            Directory.CreateDirectory(snapshotsDir);

            string destFileName;
            string destPath;

            var ext = Path.GetExtension(sourcePath).ToLower();
            if (ext == ".pdf")
            {
                // Convert first page of PDF to PNG
                destFileName = $"cv_snapshot_{cv.Id}_{DateTime.UtcNow.Ticks}.png";
                destPath = Path.Combine(snapshotsDir, destFileName);

                using (var images = new MagickImageCollection())
                {
                    images.Read(sourcePath);
                    using (var image = (MagickImage)images[0].Clone())
                    {
                        image.Format = MagickFormat.Png;
                        await image.WriteAsync(destPath);
                    }
                }
            }
            else if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
            {
                // Just copy the image file
                destFileName = $"cv_snapshot_{cv.Id}_{DateTime.UtcNow.Ticks}{ext}";
                destPath = Path.Combine(snapshotsDir, destFileName);
                using (var sourceStream = File.OpenRead(sourcePath))
                using (var destStream = File.Create(destPath))
                {
                    await sourceStream.CopyToAsync(destStream);
                }
            }
            else
            {
                throw new NotSupportedException("Only PDF, PNG, or JPG files are supported for snapshot.");
            }

            return $"/snapshots/{destFileName}";
        }
    }
}


//snaphot cv nhiều trang
/*using ImageMagick;

namespace JOB_FINDER_API.Models.Services
{
    public class CvSnapshotService : ICvSnapshotService
    {
        public async Task<List<string>> CaptureCvAsImagesAsync(CV cv)
        {
            if (cv == null || string.IsNullOrEmpty(cv.FileUrl))
                throw new ArgumentException("CV or FileUrl is invalid");

            var sourcePath = cv.FileUrl;
            var snapshotsDir = Path.Combine("wwwroot", "snapshots");
            Directory.CreateDirectory(snapshotsDir);

            var ext = Path.GetExtension(sourcePath).ToLower();
            var imagePaths = new List<string>();

            if (ext == ".pdf")
            {
                using (var images = new MagickImageCollection())
                {
                    images.Read(sourcePath);
                    for (int i = 0; i < images.Count; i++)
                    {
                        var destFileName = $"cv_snapshot_{cv.Id}_{DateTime.UtcNow.Ticks}_p{i + 1}.png";
                        var destPath = Path.Combine(snapshotsDir, destFileName);
                        using (var image = (MagickImage)images[i].Clone())
                        {
                            image.Format = MagickFormat.Png;
                            await image.WriteAsync(destPath);
                        }
                        imagePaths.Add($"/snapshots/{destFileName}");
                    }
                }
            }
            else if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
            {
                var destFileName = $"cv_snapshot_{cv.Id}_{DateTime.UtcNow.Ticks}{ext}";
                var destPath = Path.Combine(snapshotsDir, destFileName);
                using (var sourceStream = File.OpenRead(sourcePath))
                using (var destStream = File.Create(destPath))
                {
                    await sourceStream.CopyToAsync(destStream);
                }
                imagePaths.Add($"/snapshots/{destFileName}");
            }
            else
            {
                throw new NotSupportedException("Only PDF, PNG, or JPG files are supported for snapshot.");
            }

            return imagePaths;
        }
    }
}*/