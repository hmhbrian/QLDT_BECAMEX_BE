using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
using System.Threading;
using System.Threading.Tasks;
// Thêm các namespace cần thiết khác như repository, service, etc.

namespace QLDT_Becamex.Src.Application.Features.CourseAttachedFiles.Commands
{
    public class DeleteCourseAttachedFileCommandHandler : IRequestHandler<DeleteCourseAttachedFileCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork; // Ví dụ: inject unit of work hoặc repository
        private readonly ICloudinaryService _cloudinaryService; // Nếu cần xóa file trên Cloudinary

        public DeleteCourseAttachedFileCommandHandler(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<string> Handle(DeleteCourseAttachedFileCommand request, CancellationToken cancellationToken)
        {
            var attachedFile = await _unitOfWork.CourseAttachedFileRepository
                                                .GetByIdAsync(request.CourseAttachedFileId);

            if (attachedFile == null || attachedFile.CourseId != request.CourseId)
            {
                throw new AppException("Không tìm thấy file đính kèm hoặc ID khóa học không khớp.", 404);
            }

            // Nếu đây là một file được tải lên (không phải link) và có URL
            if (attachedFile.Type != "Link" && !string.IsNullOrEmpty(attachedFile.Link))
            {
                // Bạn cần trích xuất Public ID từ URL.
                // Đây là một ví dụ đơn giản, bạn có thể cần một hàm helper phức tạp hơn
                // tùy thuộc vào cấu trúc URL Cloudinary của bạn.
                // Public ID thường nằm sau phần /v.../ và trước phần mở rộng file.
                // Ví dụ URL: https://res.cloudinary.com/your_cloud_name/raw/upload/v12345/documents/my_document.pdf
                // Public ID sẽ là: documents/my_document
                var publicId = _cloudinaryService.GetPublicIdFromCloudinaryUrl(attachedFile.Link);

                if (!string.IsNullOrEmpty(publicId))
                {
                    var deleteSuccess = await _cloudinaryService.DeleteFileAsync(publicId);
                    if (!deleteSuccess)
                    {
                        // Xử lý lỗi nếu không thể xóa file trên Cloudinary
                        Console.WriteLine($"[HANDLER ERROR] Không thể xóa file Cloudinary với Public ID: {publicId}");
                        // Tùy chọn: bạn có thể ném lỗi hoặc chỉ ghi log và tiếp tục xóa trong DB
                    }
                }
            }

            // Xóa file đính kèm khỏi database
            _unitOfWork.CourseAttachedFileRepository.Remove(attachedFile);
            await _unitOfWork.CompleteAsync(); // Lưu thay đổi vào DB

            return $"File đính kèm với ID '{request.CourseAttachedFileId}' đã được xóa thành công.";
        }
    }
}