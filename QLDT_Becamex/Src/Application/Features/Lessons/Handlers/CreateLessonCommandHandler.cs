using MediatR;
using QLDT_Becamex.Src.Application.Features.Lessons.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Infrastructure.Persistence; // Namespace cho IUnitOfWork
using QLDT_Becamex.Src.Infrastructure.Services; // Namespace cho CloudinaryService (nếu dùng trực tiếp hoặc service cụ thể)
using QLDT_Becamex.Src.Domain.Interfaces; // Namespace cho IBaseService và ICloudinaryService
using QLDT_Becamex.Src.Application.Common.Dtos; // Namespace cho AppException (nếu có)
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;
using QLDT_Becamex.Src.Infrastructure.Services.CloudinaryServices; // Để đảm bảo dùng đúng CreateLessonDto
// using Microsoft.AspNetCore.Http; // Không cần thiết nếu BaseService xử lý HttpContextAccessor

namespace QLDT_Becamex.Src.Application.Features.Lessons.Handlers
{
    public class CreateLessonCommandHandler : IRequestHandler<CreateLessonCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ICloudinaryService _cloudinaryService;

        public CreateLessonCommandHandler(IUnitOfWork unitOfWork, IUserService userService, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _cloudinaryService = cloudinaryService;
        }

        public async Task Handle(CreateLessonCommand request, CancellationToken cancellationToken)
        {
            // Lấy User ID từ BaseService
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();

            if (string.IsNullOrEmpty(userId))
            {
                // Sử dụng AppException của bạn với mã lỗi phù hợp
                throw new AppException("User ID not found. User must be authenticated.", 401);
            }

            var existingCourse = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId);

            if (existingCourse == null)
            {
                throw new AppException($"Course with ID: {request.CourseId} not found.", 404);
            }
            //Lấy position tối đa hiện tại của các bài học trong khóa học
            int maxPosition = await _unitOfWork.LessonRepository.GetMaxPositionAsync(request.CourseId);

            // --- 1. Tải file PDF lên Cloudinary ---
            string pdfUrl = null!;
            string filePublicId = null!;

            if (request.Request.FilePdf != null && request.Request.FilePdf.Length > 0)
            {

                var uploadResult = await _cloudinaryService.UploadPdfAsync(request.Request.FilePdf, "Lesson_pdfs");
                pdfUrl = uploadResult?.url!; // Lấy URL từ kết quả upload
                filePublicId = uploadResult?.publicId!; // Lấy PublicId từ kết quả upload

                if (string.IsNullOrEmpty(pdfUrl))
                {
                    throw new AppException("Failed to upload PDF file to Cloudinary.", 500);
                }
            }
            else
            {
                // Nếu file PDF là bắt buộc, hãy ném ngoại lệ
                throw new AppException("PDF file is required for creating a lesson.", 400); // Mã 400 cho Bad Request
            }


            // Nếu bạn vẫn đang sử dụng phương thức `Create` trên instance `Lesson`:
            var lesson = new Lesson();

            lesson.Create(request.CourseId, userId, request.Request, pdfUrl, filePublicId, maxPosition+1);

            // --- 3. Thêm entity vào Repository và lưu vào DB ---
            await _unitOfWork.LessonRepository.AddAsync(lesson);

            // Hoàn tất Unit of Work
            await _unitOfWork.CompleteAsync();

            // Không cần return vì lệnh này không trả về gì (IRequest)
        }
    }
}