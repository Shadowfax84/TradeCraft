using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using TC_Backend.Exceptions;
using TC_Backend.Models;
using TC_Backend.Services.Interfaces;

namespace TC_Backend.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryService> _logger;

        public CloudinaryService(Cloudinary cloudinary, ILogger<CloudinaryService> logger)
        {
            _cloudinary = cloudinary;
            _logger = logger;
        }

        public async Task<(bool IsSuccess, UserFile? File)> UploadFile(IFormFile file, string userId, string filename)
        {
            _logger.LogInformation("Starting upload of file {Filename} for user {UserId}", filename, userId);

            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.");

            try
            {
                using var stream = file.OpenReadStream();
                var folderPath = $"UserUploadFiles/{userId}";

                var uploadParams = new RawUploadParams()
                {
                    File = new FileDescription(filename, stream),
                    Folder = folderPath
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError("Cloudinary upload failed for file {Filename} for user {UserId} with error {Error}",
                        filename, userId, uploadResult.Error?.Message);
                    throw new CloudinaryUploadException(uploadResult.Error?.Message ?? "Unknown upload error.");
                }

                var userFile = new UserFile
                {
                    FileId = Guid.NewGuid(),
                    UserId = userId,
                    Url = uploadResult.SecureUrl.ToString(),
                    PublicId = uploadResult.PublicId,
                    UploadedOn = DateTime.UtcNow,
                    FileName = filename,
                    FileType = file.ContentType
                };

                _logger.LogInformation("Successfully uploaded file {Filename} for user {UserId}", filename, userId);

                return (true, userFile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during file upload for user {UserId} file {Filename}", userId, filename);
                throw;
            }
        }

        public async Task<(string Url, string PublicId)> UploadProfileImg(IFormFile image, string userId)
        {
            _logger.LogInformation("Starting profile image upload for user {UserId}", userId);

            if (image == null || image.Length == 0)
                throw new ArgumentException("Image is empty.");

            if (image.Length > 3 * 1024 * 1024)
                throw new ArgumentException("Image size exceeds 3MB.");

            try
            {
                using var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using var img = Image.Load(memoryStream);
                using var outputStream = new MemoryStream();

                img.Save(outputStream, new JpegEncoder());
                outputStream.Position = 0;

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription($"{userId}.jpg", outputStream),
                    Folder = "user_profilePics"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError("Cloudinary profile image upload failed for user {UserId} with error {Error}",
                        userId, uploadResult.Error?.Message);
                    throw new CloudinaryUploadException(uploadResult.Error?.Message ?? "Unknown upload error.");
                }
                _logger.LogInformation("Successfully uploaded profile image for user {UserId}", userId);

                return (uploadResult.SecureUrl.ToString(), uploadResult.PublicId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during profile image upload for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string publicId, string userId)
        {
            _logger.LogInformation("Attempting to delete file with public ID {PublicId} for user {UserId}", publicId, userId);

            if (string.IsNullOrWhiteSpace(publicId))
                throw new ArgumentException("PublicId must be provided.");

            try
            {
                var deletionParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Auto
                };

                var result = await _cloudinary.DestroyAsync(deletionParams);

                if (result.StatusCode == System.Net.HttpStatusCode.OK ||
                    result.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("Successfully deleted or file not found for public ID {PublicId} user {UserId}",
                        publicId, userId);
                    return true;
                }
                else
                {
                    _logger.LogError("Cloudinary deletion failed for public ID {PublicId} user {UserId} with error {Error}",
                        publicId, userId, result.Error?.Message);
                    throw new CloudinaryDeleteException(result.Error?.Message ?? "Unknown deletion error.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during file deletion for public ID {PublicId} user {UserId}", publicId, userId);
                throw;
            }
        }
    }
}