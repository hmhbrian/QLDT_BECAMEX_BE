// QLDT_Becamex.Src.Application.Features.Lessons.Handlers/UpdateLessonCommandHandler.cs
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos; // AppException
using QLDT_Becamex.Src.Application.Features.Lessons.Commands;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Infrastructure.Services.CloudinaryServices;

namespace QLDT_Becamex.Src.Application.Features.Lessons.Handlers
{
    public class UpdateLessonCommandHandler : IRequestHandler<UpdateLessonCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ICloudinaryService _cloudinaryService;

        public UpdateLessonCommandHandler(IUnitOfWork unitOfWork, IUserService userService, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork; 
            _userService = userService;
            _cloudinaryService = cloudinaryService;
        }

        public async Task Handle(UpdateLessonCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy User ID từ BaseService
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("User ID not found. User must be authenticated.", 401);
            }

            // 2. Tìm bài học cần cập nhật
            var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(request.LessonId); // Giả định có GetByIdAsync

            if (lesson == null)
            {
                throw new AppException($"Lesson with ID: {request.LessonId} not found.", 404);
            }

            var existingCourse = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId);

            if (existingCourse == null)
            {
                throw new AppException($"Course with ID: {request.CourseId} not found.", 404);
            }

            var ProcessTitle = request.Request.Title?.Trim();
            if (!string.IsNullOrEmpty(ProcessTitle) && ProcessTitle.StartsWith("Bài ") && ProcessTitle.Contains(": "))
            {
                int index = ProcessTitle.IndexOf(": ");
                ProcessTitle = ProcessTitle.Substring(index + 2).Trim(); // Lấy phần sau dấu ": "
            }
            request.Request.Title = ProcessTitle; // Cập nhật tiêu đề đã xử lý

            // 3. Xử lý file PDF mới nếu có và xóa file cũ
            string? newPdfUrl = lesson.FileUrl; // Giữ lại URL cũ làm mặc định
            string? oldPdfUrl = lesson.FileUrl; // Lưu URL PDF cũ để xóa sau
            string? newPdfPublicId = null; // Lưu URL PDF cũ để xóa sau

            if (request.Request.FilePdf != null && request.Request.FilePdf.Length > 0)
            {
                var folderName = "lesson_pdfs"; // Thư mục trên Cloudinary

                // Tải file mới lên Cloudinary
                var uploadResult = await _cloudinaryService.UploadPdfAsync(request.Request.FilePdf, folderName);

                newPdfUrl = uploadResult.Value.url;       // Lấy URL mới
                newPdfPublicId = uploadResult.Value.publicId; // Lấy PublicId mới

                if (string.IsNullOrEmpty(newPdfUrl))
                {
                    throw new AppException("Failed to upload new PDF file to Cloudinary.", 500);
                }

                // Nếu có URL cũ và nó khác với URL mới (đảm bảo không xóa nhầm file vừa upload nếu có lỗi)
                if (!string.IsNullOrEmpty(oldPdfUrl) && oldPdfUrl != newPdfUrl)
                {

                    if (!string.IsNullOrEmpty(newPdfPublicId))
                    {
                        // Xóa file PDF cũ trên Cloudinary
                        var deleteSuccess = await _cloudinaryService.DeleteFileAsync(lesson.PublicIdUrlPdf);
                        if (!deleteSuccess)
                        {
                            Console.WriteLine($"Warning: Failed to delete old PDF file {newPdfPublicId} from Cloudinary.");
                        }
                    }
                }
            }

            lesson.Update(request.CourseId, userId, request.Request, newPdfUrl, newPdfPublicId);

            _unitOfWork.LessonRepository.Update(lesson);

            await _unitOfWork.CompleteAsync();
        }
    }
}