using TC_Backend.Models;

namespace TC_Backend.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<(bool IsSuccess, UserFile? File)> UploadFile(IFormFile file, string userId, string filename);
        Task<(string Url, string PublicId)> UploadProfileImg(IFormFile image, string userId);
        Task<bool> DeleteFileAsync(string publicId, string userId);
    }
}