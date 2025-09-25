using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace JOB_FINDER_API.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string?> UploadImageAsync(IFormFile file)
        {
            if (file.Length <= 0) return null;

            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                //Transformation = new Transformation().Crop("fill").Gravity("face").Width(400).Height(400),
                Folder = "image_user"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }

        public async Task<string?> UploadCvAsync(IFormFile file)
        {
            if (file.Length <= 0) return null;
            string safeFileName = Path.GetFileNameWithoutExtension(file.FileName)
                                   .Replace(" ", "_")
                                   .Replace(".", "_");

            await using var stream = file.OpenReadStream(); 
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "cv_user",
                AccessMode = "public",
                PublicId = safeFileName
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }
        public async Task<string?> UploadCvAsync(IFormFile file, string publicId)
        {
            if (file == null || file.Length <= 0)
            {
                return null;
            }

            string safePublicId = publicId
                .Replace(" ", "_")
                .Replace(".", "_")
                .Replace("/", "_")
                .Replace("\\", "_");

            await using var stream = file.OpenReadStream();
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "cv_user",
                AccessMode = "public",
                PublicId = safePublicId
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }

        public async Task<(string? MediaUrl, string? MediaType, string? FileName)> UploadMediaAsync(IFormFile file, bool isSticker = false)
        {
            if (file == null || file.Length <= 0) return (null, null, null);

            var allowedTypes = new[]
            {
                "image/jpeg", "image/png", "image/gif",
                "application/pdf",
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            };

            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                throw new ArgumentException("Unsupported file type. Allowed: JPEG, PNG, GIF, PDF, DOC, DOCX.");
            }

            if (file.Length > 10 * 1024 * 1024)
            {
                throw new ArgumentException("File size exceeds 10MB limit.");
            }

            string mediaType = file.ContentType.ToLower().StartsWith("image/")
                ? (isSticker ? "sticker" : "image")
                : "file";
            string fileName = file.FileName;

            if (mediaType == "image" || mediaType == "sticker")
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, file.OpenReadStream()),
                    Folder = mediaType == "sticker" ? "job_finder/stickers" : "job_finder/images",
                    Transformation = new Transformation()
                        .Quality("auto")
                        .FetchFormat("auto")
                        .Width(mediaType == "sticker" ? 100 : 500)
                        .Crop("fit"),
                    PublicId = Guid.NewGuid().ToString(),
                 
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return (uploadResult.SecureUrl.ToString(), mediaType, fileName);
            }
            else
            {
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(fileName, file.OpenReadStream()),
                    Folder = "job_finder/files",
                    AccessMode = "public",
                    PublicId = Guid.NewGuid().ToString(),
        
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return (uploadResult.SecureUrl.ToString(), mediaType, fileName);
            }
        }

        public async Task<string?> UploadVideoAsync(IFormFile file)
        {
            if (file == null || file.Length <= 0) return null;

        
            var allowedVideoTypes = new[] { "video/mp4", "video/webm", "video/ogg" };
            if (!allowedVideoTypes.Contains(file.ContentType.ToLower()))
            {
                throw new ArgumentException("Unsupported video format. Allowed: MP4, WebM, OGG.");
            }


            if (file.Length > 100 * 1024 * 1024)
            {
                throw new ArgumentException("Video file size exceeds 100MB limit.");
            }

            await using var stream = file.OpenReadStream();
            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "videos", 
                PublicId = Guid.NewGuid().ToString(),
                Transformation = new Transformation()
                    .Quality("auto")
                    .FetchFormat("auto")
            };

          
            var uploadResult = file.Length > 10 * 1024 * 1024
                ? await _cloudinary.UploadLargeAsync(uploadParams)
                : await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new Exception(uploadResult.Error.Message);
            }

            return uploadResult.SecureUrl.ToString();
        }



    }
}