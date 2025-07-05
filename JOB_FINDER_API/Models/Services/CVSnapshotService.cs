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

            var urls = new List<string>();
            var ext = Path.GetExtension(cv.FileUrl).ToLower();

            // Tải file từ URL về dưới dạng stream
            using (var httpClient = new HttpClient())
            using (var stream = await httpClient.GetStreamAsync(cv.FileUrl))
            {
                if (ext == ".pdf")
                {
                    using (var images = new MagickImageCollection())
                    {
                        var settings = new MagickReadSettings
                        {
                            Density = new Density(300, 300)
                        };
                        images.Read(stream, settings); // Đọc từ stream thay vì URL
                        uint a4Width = 2480;
                        uint a4Height = 3508;

                        for (int i = 0; i < images.Count; i++)
                        {
                            var destFileName = $"cv_snapshot_{cv.CVId}_{DateTime.UtcNow.Ticks}_p{i + 1}.png";

                            using (var image = (MagickImage)images[i].Clone())
                            {
                                image.Format = MagickFormat.Png;
                                image.Extent(a4Width, a4Height, Gravity.Center, MagickColors.White);

                                using (var memoryStream = new MemoryStream())
                                {
                                    await image.WriteAsync(memoryStream);
                                    memoryStream.Position = 0;

                                    var formFile = new FormFile(memoryStream, 0, memoryStream.Length, null, destFileName)
                                    {
                                        Headers = new HeaderDictionary(),
                                        ContentType = "image/png"
                                    };

                                    var url = await _cloudinaryService.UploadImageAsync(formFile);
                                    urls.Add(url);
                                }
                            }
                        }
                    }
                }
                else if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                {
                    using (var images = new MagickImageCollection())
                    {
                        var settings = new MagickReadSettings
                        {
                            Density = new Density(200, 200)
                        };
                        images.Read(stream, settings);

                        var destFileName = $"cv_snapshot_{cv.CVId}_{DateTime.UtcNow.Ticks}{ext}";

                        using (var image = (MagickImage)images[0].Clone())
                        {
                            image.Format = MagickFormat.Png;

                            using (var memoryStream = new MemoryStream())
                            {
                                await image.WriteAsync(memoryStream);
                                memoryStream.Position = 0;

                                var formFile = new FormFile(memoryStream, 0, memoryStream.Length, null, destFileName)
                                {
                                    Headers = new HeaderDictionary(),
                                    ContentType = "image/png"
                                };

                                var url = await _cloudinaryService.UploadImageAsync(formFile);
                                urls.Add(url);
                            }
                        }
                    }
                }
                else
                {
                    throw new NotSupportedException("Only PDF, PNG, or JPG files are supported for snapshot.");
                }
            }

            return urls;
        }
    }
}