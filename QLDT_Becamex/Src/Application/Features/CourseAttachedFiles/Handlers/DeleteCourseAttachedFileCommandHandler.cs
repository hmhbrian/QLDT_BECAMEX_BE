using MediatR;
using Microsoft.IdentityModel.Tokens;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services.CloudinaryServices;
using Microsoft.EntityFrameworkCore;
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
                                                .GetFirstOrDefaultAsync(
                                                    predicate: a => a.Id == request.CourseAttachedFileId,
                                                    includes: a => a.Include(i => i.TypeDoc));

            if (attachedFile == null || attachedFile.CourseId != request.CourseId)
            {
                throw new AppException("Không tìm thấy file đính kèm hoặc ID khóa học không khớp.", 404);
            }

            var existingCourse = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId);

            if (existingCourse == null)
            {
                throw new AppException($"Course with ID: {request.CourseId} not found.", 404);
            }

            // Nếu đây là một file được tải lên (không phải link) và có URL
            if (attachedFile.TypeDoc?.Key == 0 && !string.IsNullOrEmpty(attachedFile.Link))
            {
                
                if (!string.IsNullOrEmpty(attachedFile.PublicIdUrlPdf))
                {
                    var deleteSuccess = await _cloudinaryService.DeleteFileAsync(attachedFile.PublicIdUrlPdf);
                    if (!deleteSuccess)
                    {
                        // Xử lý lỗi nếu không thể xóa file trên Cloudinary
                        Console.WriteLine($"[HANDLER ERROR] Không thể xóa file Cloudinary với Public ID: {attachedFile.PublicIdUrlPdf}");
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