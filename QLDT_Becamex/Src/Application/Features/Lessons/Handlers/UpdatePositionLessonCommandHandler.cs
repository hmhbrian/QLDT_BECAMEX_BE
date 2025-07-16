using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Lessons.Commands;
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Infrastructure.Services.UserServices;

namespace QLDT_Becamex.Src.Application.Features.Lessons.Handlers
{
    public class UpdatePositionLessonCommandHandler : IRequestHandler<UpdatePositionLessonCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public UpdatePositionLessonCommandHandler(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }
        public async Task Handle(UpdatePositionLessonCommand request, CancellationToken cancellationToken)
        {
            // Lấy User ID từ BaseService
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("User ID not found. User must be authenticated.", 401);
            }

            //Kiếm tra course có tồn tại không
            var existingCourse = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId);

            if (existingCourse == null)
            {
                throw new AppException($"Course with ID: {request.CourseId} not found.", 404);
            }

            //Kiểm tra lesson cần di chuyển có tồn tại không
            var lessonToMove = await _unitOfWork.LessonRepository.GetFirstOrDefaultAsync(
                predicate: l => l.Id == request.LessonId && l.CourseId == request.CourseId
            );
            if (lessonToMove == null)
            {
                throw new AppException($"Lesson with ID: {request.LessonId} not found.", 404);
            }

            //transasction giúp đảm bảo tính toàn vẹn dữ liệu khi cập nhật position của lesson, tất cả thành công hoặc không có thay đổi xảy ra
            using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                int newPosition;
                if (!request.PreviousLessonId.HasValue || request.PreviousLessonId == 0)
                {
                    //Nếu không có PreviousLessonId thì đặt vị trí là 1
                    newPosition = 1;
                }
                else
                {
                    //Kiểm tra lesson trước đó có tồn tại không
                    var previousLesson = await _unitOfWork.LessonRepository.GetFirstOrDefaultAsync(
                        predicate: l => l.Id == request!.PreviousLessonId && l.CourseId == request.CourseId
                    );
                    if (previousLesson == null)
                    {
                        throw new AppException($"Previous lesson with ID: {request!.PreviousLessonId} not found.", 404);
                    }
                    //Đặt vị trí mới là vị trí của lesson trước đó + 1
                    newPosition = previousLesson.Position + 1;
                }

                //Lấy Position hiện tại của Lesson để so sánh
                int oldPosition = lessonToMove.Position;

                //Cập nhật position của các lesson cùng khóa học
                if (newPosition != oldPosition)
                {
                    if (newPosition < oldPosition)
                    {
                        //Nếu vị trí mới nhỏ hơn vị trí cũ, tăng position của các lesson từ newPosition đến oldPosition - 1
                        await _unitOfWork.LessonRepository.UpdatePositionAsync(request?.CourseId!, newPosition, oldPosition - 1, 1);
                    }
                    else
                    {
                        //Nếu vị trí mới lớn hơn vị trí cũ, giảm position của các lesson từ oldPosition + 1 đến newPosition
                        newPosition = newPosition - 1; // Giảm vị trí mới để tránh xung đột với lessonToMove
                        await _unitOfWork.LessonRepository.UpdatePositionAsync(request?.CourseId!, oldPosition + 1, newPosition, -1);
                    }
                    lessonToMove.Position = newPosition; // Cập nhật vị trí mới cho lesson
                    lessonToMove.UpdatedAt = DateTime.UtcNow;
                    lessonToMove.UpdatedById = userId;
                    //_unitOfWork.LessonRepository.Update(lessonToMove); // Cập nhật lesson

                    await _unitOfWork.CompleteAsync(); // Lưu thay đổi
                }

                await transaction.CommitAsync(cancellationToken); // Commit transaction
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi xảy ra
                await transaction.RollbackAsync(cancellationToken);
                throw new AppException($"Lỗi khi cập nhật vị trí bài học: {ex.Message}", 500);
            }
        }
    }
}
