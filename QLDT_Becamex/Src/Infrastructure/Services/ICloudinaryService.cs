namespace QLDT_Becamex.Src.Infrastructure.Services
{
    public interface ICloudinaryService
    {
        public Task<string?> UploadImageAsync(IFormFile file);
    }
}
