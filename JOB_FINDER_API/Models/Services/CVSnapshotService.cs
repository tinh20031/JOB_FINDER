// JOB_FINDER_API\Models\Services\CVSnapshotService.cs
using ImageMagick;
using JOB_FINDER_API.Models;
using JOB_FINDER_API.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JOB_FINDER_API.Models.Services
{
    public class CvSnapshotService : ICvSnapshotService
    {
        private readonly CloudinaryService _cloudinaryService;

        public CvSnapshotService(CloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }
        public async Task<List<string>> CaptureCvAsImagesAsync(CV cv)
        {
            if (cv == null || string.IsNullOrEmpty(cv.FileUrl))
                throw new ArgumentException("CV or FileUrl is invalid");

            var sourcePath = cv.FileUrl;
            var snapshotsDir = Path.Combine("wwwroot", "snapshots");
            Directory.CreateDirectory(snapshotsDir);

            var ext = Path.GetExtension(sourcePath).ToLower();
            var urls = new List<string>();   

            if (ext == ".pdf")
            {
                using (var images = new MagickImageCollection())
                {
                    var settings = new MagickReadSettings
                    {
                        Density = new Density(300, 300)
                    };
                    images.Read(sourcePath, settings);
                    uint a4Width = 2480;
                    uint a4Height = 3508;
                    for (int i = 0; i < images.Count; i++)
                    {
                        var destFileName = $"cv_snapshot_{cv.Id}_{DateTime.UtcNow.Ticks}_p{i + 1}.png";
                        var destPath = Path.Combine(snapshotsDir, destFileName);

                        using (var image = (MagickImage)images[i].Clone())
                        {
                            image.Format = MagickFormat.Png;
                            image.Extent(a4Width, a4Height, Gravity.Center, MagickColors.White);
                            await image.WriteAsync(destPath);
                            // Debug: kiểm tra kích thước ảnh
                            Console.WriteLine($"Snapshot size: {image.Width}x{image.Height}");
                        }

                        // Upload to Cloudinary
                        await using (var stream = File.OpenRead(destPath))
                        {
                            var formFile = new FormFile(stream, 0, stream.Length, null, destFileName)
                            {
                                Headers = new HeaderDictionary(),
                                ContentType = "image/png"
                            };
                            var url = await _cloudinaryService.UploadImageAsync(formFile);
                            urls.Add(url);
                        }

                        if (File.Exists(destPath))
                        {
                            File.Delete(destPath);
                        }
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
                // Resize/canvas về đúng khổ A4
                using (var images = new MagickImageCollection())
                {
                    var settings = new MagickReadSettings
                    {
                        Density = new Density(200, 200) // hoặc 300, 300 nếu muốn nét hơn
                    };
                    images.Read(sourcePath, settings);
                    using (var image = (MagickImage)images[0].Clone())
                    {
                        image.Format = MagickFormat.Png;
                        // Đảm bảo không crop, resize hoặc trim gì ở đây
                        await image.WriteAsync(destPath);
                    }
                }
                await using (var stream = File.OpenRead(destPath))
                {
                    var formFile = new FormFile(stream, 0, stream.Length, null, destFileName)
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "image/png"
                    };
                    var url = await _cloudinaryService.UploadImageAsync(formFile);
                    urls.Add(url);
                }
                if (File.Exists(destPath))
                {
                    File.Delete(destPath);
                }
            }
            else
            {
                throw new NotSupportedException("Only PDF, PNG, or JPG files are supported for snapshot.");
            }

            return urls;
        }
    }
}