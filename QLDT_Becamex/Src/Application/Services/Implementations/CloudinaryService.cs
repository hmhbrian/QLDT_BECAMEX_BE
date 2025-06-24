using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using QLDT_Becamex.Src.Services.Interfaces;

namespace QLDT_Becamex.Src.Services.Implementations
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration config)
        {
            // Truy xuất trực tiếp từ cấu hình
            var cloudName = config["CloudinarySettings:CloudName"];
            var apiKey = config["CloudinarySettings:ApiKey"];
            var apiSecret = config["CloudinarySettings:ApiSecret"];

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string?> UploadImageAsync(IFormFile file)
        {
            if (file.Length == 0) return null;

            try
            {
                using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "avatars",
                    Transformation = new Transformation()
                        .Width(500)
                        .Height(500)
                        .Crop("fill")
                        .Gravity("face")
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                return result.SecureUrl?.ToString();
            }
            catch (Exception ex)
            {
                // Bạn có thể log ở đây, ví dụ:
                // _logger.LogError(ex, "Upload ảnh thất bại");

                // hoặc đơn giản hơn nếu không có logger
                Console.WriteLine($"[Cloudinary ERROR] {ex.Message}");

                return null;
            }
        }

    }
}
