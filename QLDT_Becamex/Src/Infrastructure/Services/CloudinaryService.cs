using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace QLDT_Becamex.Src.Infrastructure.Services
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
        public async Task<string?> UploadPdfAsync(IFormFile file)
        {
            // Bước 1: Kiểm tra xem tệp có phải là PDF hay không
            if (file == null || file.Length == 0)
            {
                Console.WriteLine("[Cloudinary WARNING] Tệp không có hoặc rỗng.");
                return null;
            }

            // Kiểm tra kiểu MIME của tệp. Chỉ chấp nhận PDF.
            if (file.ContentType != "application/pdf")
            {
                Console.WriteLine($"[Cloudinary WARNING] Tệp không phải PDF. Kiểu tệp: {file.ContentType}");
                return null;
            }

            try
            {
                // Bước 2: Mở luồng tệp
                using var stream = file.OpenReadStream();

                // Bước 3: Thiết lập tham số tải lên cho tệp RAW (PDF không phải là hình ảnh)
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "documents", // Bạn có thể thay đổi thư mục lưu trữ PDF tùy ý
                    PublicId = Path.GetFileNameWithoutExtension(file.FileName) // Đặt PublicId theo tên file (không có phần mở rộng)
                };

                // Bước 4: Tải lên Cloudinary
                var result = await _cloudinary.UploadAsync(uploadParams);

                // Bước 5: Trả về URL an toàn
                if (result.Error != null)
                {
                    Console.WriteLine($"[Cloudinary ERROR] Lỗi tải lên PDF: {result.Error.Message}");
                    return null;
                }

                return result.SecureUrl?.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Cloudinary ERROR] Lỗi trong quá trình tải lên PDF: {ex.Message}");
                return null;
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

        /// <summary>
        /// Trích xuất Public ID từ một URL Cloudinary.
        /// Public ID bao gồm folder và tên tệp (không có phần mở rộng và không có phần version).
        /// Ví dụ: "https://res.cloudinary.com/cloudname/raw/upload/v12345/documents/my_doc.pdf" -> "documents/my_doc"
        /// </summary>
        /// <param name="url">URL của tệp trên Cloudinary.</param>
        /// <returns>Public ID của tệp hoặc null nếu không thể trích xuất.</returns>
        public string? GetPublicIdFromCloudinaryUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                var uploadIndex = url.IndexOf("/upload/");
                if (uploadIndex == -1)
                    return null;

                var pathAfterUpload = url.Substring(uploadIndex + "/upload/".Length); // ví dụ: v123456/documents/sasas.pdf

                // Cắt bỏ phần version (v123456/)
                var parts = pathAfterUpload.Split('/', 2);
                if (parts.Length < 2)
                    return null;

                var fullPath = parts[1]; // documents/sasas.pdf

                return fullPath; // KHÔNG bỏ đuôi
            }
            catch
            {
                return null;
            }
        }

    }
}
