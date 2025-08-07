using MediatR;
using Microsoft.EntityFrameworkCore;// Dành cho các thao tác LINQ như FirstOrDefaultAsync hoặc SingleOrDefaultAsync
using QLDT_Becamex.Src.Application.Common.Dtos; // Dành cho AppException
using QLDT_Becamex.Src.Application.Features.Lessons.Commands;
using QLDT_Becamex.Src.Domain.Interfaces; // Dành cho IBaseService, ICloudinaryService
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Infrastructure.Services.CloudinaryServices;

namespace QLDT_Becamex.Src.Application.Features.Lessons.Handlers
{
    public class DeleteLessonCommandHandler : IRequestHandler<DeleteLessonCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ICloudinaryService _cloudinaryService;

        public DeleteLessonCommandHandler(IUnitOfWork unitOfWork, IUserService userService, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _cloudinaryService = cloudinaryService;
        }

        public async Task Handle(DeleteLessonCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy User ID từ BaseService
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("Không tìm thấy thông tin người dùng được xác thực.", 401);
            }

            // 2. Lấy danh sách bài học theo CourseId + LessonIds
            var lessons = await _unitOfWork.LessonRepository
                .FindAsync(l => l.CourseId == request.CourseId && request.LessonIds.Contains(l.Id));

            if (lessons == null || !lessons.Any())
            {
                throw new AppException("Không tìm thấy bất kỳ bài học nào để xoá.", 404);
            }

            //Kiểm tra khóa học được phép xóa hay không
            var course = await _unitOfWork.CourseRepository.GetFirstOrDefaultAsync(
                predicate: a => a.Id == request.CourseId,
                includes: p => p.Include(a => a.Status));

            if (course == null)
            {
                throw new AppException("Khóa học không tồn tại", 404);
            }
            if (course.Status != null && course.Status.Key > 1)
            {
                throw new AppException("Không thể xóa bài học vì đã bắt đầu", 403);
            }

            // Kiểm tra bài học có dữ liệu trong LessonProgress
            var nonDeletableLessonIds = new List<int>();
            var lessonProgressExists = await _unitOfWork.LessonProgressRepository
                .FindAsync(tr => request.LessonIds.Contains(tr.LessonId));
            nonDeletableLessonIds = lessonProgressExists.Select(tr => tr.LessonId).Distinct().ToList();

            // Lọc các bài học có thể xóa
            var deletableLessons = lessons.Where(l => !nonDeletableLessonIds.Contains(l.Id)).ToList();

            // Nếu tất cả bài học đều không thể xóa, ném ngoại lệ
            if (!deletableLessons.Any() && nonDeletableLessonIds.Any())
            {
                throw new AppException($"Không thể xóa bài học với ID {string.Join(", ", nonDeletableLessonIds)} vì đã có người tham gia", 403);
            }

            using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                foreach (var lesson in deletableLessons)
                {
                    // 3. Xoá file PDF nếu có
                    if (!string.IsNullOrEmpty(lesson.PublicIdUrlPdf))
                    {
                        try
                        {
                            await _cloudinaryService.DeleteFileAsync(lesson.PublicIdUrlPdf);
                        }
                        catch (AppException ex)
                        {
                            Console.WriteLine($"[WARN] Không thể xóa file Cloudinary: {ex.Message}");
                            // Tiếp tục xoá bài học, không rollback toàn bộ vì 1 file fail
                        }
                    }

                    // 4. Xoá lesson khỏi repo
                    _unitOfWork.LessonRepository.Remove(lesson);
                }
                // 5. Lưu thay đổi
                await _unitOfWork.CompleteAsync();

                //6.Cập nhật lại position các lesson còn lại
                await _unitOfWork.LessonRepository.ReorderPositionsAsync(request.CourseId, cancellationToken);

                //7.Lưu thay đổi position
                await _unitOfWork.CompleteAsync();

                //8.Commit transaction
                await transaction.CommitAsync(cancellationToken);
            }
            catch (AppException)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw; // Ném lại AppException để client nhận thông báo
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new AppException($"Lỗi khi xóa bài học và cập nhật vị trí: {ex.Message}", 500);
            }

        }
    }
}