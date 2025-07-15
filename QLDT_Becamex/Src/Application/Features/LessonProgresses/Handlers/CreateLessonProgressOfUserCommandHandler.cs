using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.LessonProgresses.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;

namespace QLDT_Becamex.Src.Application.Features.LessonProgresses.Handlers
{
    public class CreateLessonProgressOfUserCommandHandler : IRequestHandler<CreateLessonProgressOfUserCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public CreateLessonProgressOfUserCommandHandler(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }
        public async Task Handle(CreateLessonProgressOfUserCommand request, CancellationToken cancellationToken)
        {
            // Lấy User ID từ BaseService
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("User ID not found. User must be authenticated.", 404);
            }
            var existingLesson = await _unitOfWork.LessonRepository.GetByIdAsync(request.Request.LessonId);
            var isCompleted = false; // Mặc định là chưa hoàn thành
            if (existingLesson != null)
            {
                var currentPage = request.Request.CurrentPage > 0 ? request.Request.CurrentPage : 0; // Nếu CurrentPage không được cung cấp, mặc định là 0
                var currentTime = request.Request.CurrentTimeSecond > 0 ? request.Request.CurrentTimeSecond : 0; 

                if (currentPage > 0 && currentPage == existingLesson.TotalPages)
                    isCompleted = true; // Nếu CurrentPage bằng tổng số trang thì đánh dấu là hoàn thành
                else if (currentTime > 0 && currentTime >= existingLesson.TotalDurationSeconds)
                    isCompleted = true; // Nếu CurrentTimeSecond lớn hơn hoặc bằng tổng thời gian thì đánh dấu là hoàn thành
            }
            else
            {
                throw new AppException("Lesson ID not found.", 404);
            }

            var lessonprogress = await _unitOfWork.LessonProgressRepository.GetFirstOrDefaultAsync(
                predicate: c => c.UserId == userId && c.LessonId == request.Request.LessonId);
            
            if(lessonprogress != null) {
                if (lessonprogress.CurrentPage < request.Request.CurrentPage || lessonprogress.CurrentTimeSeconds < request.Request.CurrentTimeSecond)
                {
                    // Nếu đã có LessonProgress, cập nhật nó
                    var updatedLessonProgress = new LessonProgress();
                    updatedLessonProgress.UserId = userId;
                    updatedLessonProgress.Update(request.Request, isCompleted);
                    // Cập nhật vào repository
                    _unitOfWork.LessonProgressRepository.Update(lessonprogress, updatedLessonProgress);
                }
            }
            else
            {
                // Tạo mới LessonProgress
                var NewLessonProgress = new LessonProgress();

                NewLessonProgress.Create(userId, request.Request, isCompleted);
                // Thêm vào repository
                await _unitOfWork.LessonProgressRepository.AddAsync(NewLessonProgress);
            }
            
            
            // Lưu thay đổi
            await _unitOfWork.CompleteAsync();
        }
    }
}
