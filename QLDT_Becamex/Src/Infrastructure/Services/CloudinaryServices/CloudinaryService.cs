using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using QLDT_Becamex.Src.Application.Common.Dtos;

namespace QLDT_Becamex.Src.Infrastructure.Services.CloudinaryServices
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

        // Trong CloudinaryService.cs

        /// <summary>
        /// Tải lên một tệp PDF lên Cloudinary. Chỉ chấp nhận tệp có kiểu MIME là application/pdf.
        /// </summary>
        /// <param name="file">Tệp PDF cần tải lên (IFormFile).</param>
        /// <returns>URL an toàn của tệp PDF đã tải lên hoặc null nếu tải lên thất bại hoặc tệp không phải PDF.</returns>
        public async Task<(string url, string publicId)?> UploadPdfAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
            {
                // Ném AppException nếu file trống
                throw new AppException("No file provided or file is empty.", 400); // 400 Bad Request
            }

            if (file.ContentType != "application/pdf")
            {
                // Ném AppException nếu không phải file PDF
                throw new AppException($"Invalid file type. Expected 'application/pdf', but received '{file.ContentType}'.", 400); // 400 Bad Request
            }

            try
            {
                using var stream = file.OpenReadStream();
                var publicId = $"{folderName}/{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid().ToString("N")}";

                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folderName,
                    PublicId = publicId
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                if (result.Error != null)
                {
                    // Ghi log lỗi Cloudinary chi tiết hơn nếu cần
                    Console.WriteLine($"Cloudinary upload error for {file.FileName}: {result.Error.Message}");
                    // Ném AppException nếu có lỗi từ Cloudinary
                    throw new AppException($"Failed to upload PDF file to Cloudinary: {result.Error.Message}", 500); // 500 Internal Server Error
                }

                return (result.SecureUrl?.ToString() ?? string.Empty, result.PublicId);
            }
            catch (Exception ex)
            {
                // Ghi log ngoại lệ không mong muốn
                Console.WriteLine($"Unexpected error during Cloudinary PDF upload: {ex.Message}");
                // Ném AppException cho các lỗi không xác định khác
                throw new AppException($"An unexpected error occurred during PDF upload: {ex.Message}", 500); // 500 Internal Server Error
            }
        }

        /// <summary>
        /// Xóa một tệp khỏi Cloudinary bằng Public ID của nó.
        /// </summary>
        /// <param name="publicId">Public ID của tệp cần xóa trên Cloudinary (bao gồm cả folder nếu có).</param>
        /// <returns>True nếu xóa thành công, ngược lại là False.</returns>
        public async Task<bool> DeleteFileAsync(string publicId)
        {
            try
            {
                Console.WriteLine($"[Cloudinary] Deleting: {publicId}");

                var deletionParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Raw
                };

                var result = await _cloudinary.DestroyAsync(deletionParams);

                Console.WriteLine($"[Cloudinary] Delete result: {result.Result}");

                return result.Result == "ok" || result.Result == "deleted";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Cloudinary DELETE ERROR] {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(publicId))
                {
                    Console.WriteLine("[Cloudinary] Public ID is null or empty.");
                    return false;
                }

                Console.WriteLine($"[Cloudinary] Deleting image: {publicId}");

                var deletionParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Image // Đảm bảo đúng loại
                };

                var result = await _cloudinary.DestroyAsync(deletionParams);

                Console.WriteLine($"[Cloudinary] Delete result: {result.Result}");

                return result.Result == "ok" || result.Result == "deleted";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Cloudinary DELETE ERROR] {ex.Message}");
                return false;
            }
        }




        public async Task<string?> UploadImageAsync(IFormFile file, string? nameFolder = "avatars")
        {
            if (file.Length == 0) return null;

            try
            {
                using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = nameFolder ?? "avatars",
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
