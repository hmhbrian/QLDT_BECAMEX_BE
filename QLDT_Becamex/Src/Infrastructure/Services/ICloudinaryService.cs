namespace QLDT_Becamex.Src.Infrastructure.Services
{
    public interface ICloudinaryService
    {
        public Task<string?> UploadImageAsync(IFormFile file);
        public Task<string?> UploadPdfAsync(IFormFile file);
        public string? GetPublicIdFromCloudinaryUrl(string url);
        public Task<bool> DeleteFileAsync(string publicId);
    }
}
