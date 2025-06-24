namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface ICloudinaryService
    {
        public Task<string?> UploadImageAsync(IFormFile file);
    }
}
