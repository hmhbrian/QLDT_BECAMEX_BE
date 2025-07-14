// QLDT_Becamex.Src.Application.Features.Lessons.Handlers/UpdateLessonCommandHandler.cs
using MediatR;
using PdfSharpCore.Pdf.IO;
using QLDT_Becamex.Src.Application.Common.Dtos; // AppException
using QLDT_Becamex.Src.Application.Features.Lessons.Commands;
using QLDT_Becamex.Src.Domain.Entities;
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
            var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(request.LessonId);

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
            string? newUrl = lesson.FileUrl; // Giữ lại URL cũ làm mặc định
            string? oldUrl = lesson.FileUrl; // Lưu URL PDF cũ để xóa sau
            string? newPdfPublicId = null; // Lưu URL PDF cũ để xóa sau
            int TypeId = 0;
            int totalDurationSeconds = 0; // biến tổng thời gian video
            int totalPages = 0;// biến tổng số trang của PDF

            if (request.Request.Link != null && request.Request.TotalDurationSeconds > 0) // Kiểm tra nếu là link
            {
                TypeId = 2; // video
                newUrl = request.Request.Link!; // Lấy URL từ request
                totalDurationSeconds = request.Request.TotalDurationSeconds; // Lấy tổng thời gian của video
            }
            else if (request.Request.FilePdf != null && request.Request.FilePdf.Length > 0)
            {
                using (var stream = request.Request.FilePdf.OpenReadStream())
                {
                    using (var pdfDocument = PdfReader.Open(stream, PdfDocumentOpenMode.ReadOnly))
                    {
                        totalPages = pdfDocument.PageCount; // Lấy tổng số trang của PDF

                        var folderName = "lesson_pdfs"; // Thư mục trên Cloudinary

                        // Tải file mới lên Cloudinary
                        var uploadResult = await _cloudinaryService.UploadPdfAsync(request.Request.FilePdf, folderName);

                        newUrl = uploadResult.Value.url;       // Lấy URL mới
                        newPdfPublicId = uploadResult.Value.publicId; // Lấy PublicId mới
                        TypeId = 1; // PDF

                        if (string.IsNullOrEmpty(newUrl))
                        {
                            throw new AppException("Failed to upload new PDF file to Cloudinary.", 500);
                        }

                    }
                }
            }
            else
            {
                throw new AppException("Updating a lesson must have Pdf file or link video.", 400); // Mã 400 cho Bad Request
            }


            // Nếu có URL cũ là file pdf và nó khác với URL mới (đảm bảo không xóa nhầm file vừa upload nếu có lỗi)
            if (lesson.TypeDocId == 1 && !string.IsNullOrEmpty(oldUrl) && oldUrl != newUrl)
            {

                if (!string.IsNullOrEmpty(newPdfPublicId))
                {
                    // Xóa file PDF cũ trên Cloudinary
                    var deleteSuccess = await _cloudinaryService.DeleteFileAsync(lesson.PublicIdUrlPdf!);
                    if (!deleteSuccess)
                    {
                        Console.WriteLine($"Warning: Failed to delete old PDF file {newPdfPublicId} from Cloudinary.");
                    }
                }
            }
            lesson.Update(request.CourseId, userId, request.Request, newUrl, newPdfPublicId!,TypeId, totalDurationSeconds, totalPages);

            //_unitOfWork.LessonRepository.Update(lesson, updatelesson);

            await _unitOfWork.CompleteAsync();
        }
    }
}