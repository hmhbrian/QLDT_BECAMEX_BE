namespace QLDT_Becamex.Src.Infrastructure.Services.CloudinaryServices
{
    public interface ICloudinaryService
    {
        public Task<string?> UploadImageAsync(IFormFile file, string? folderName = "avatars");
        public Task<(string url, string publicId)?> UploadPdfAsync(IFormFile file, string folderName);
        public Task<bool> DeleteFileAsync(string publicId);
        public Task<bool> DeleteImageAsync(string publicId);
    }
}
