using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;
using QLDT_Becamex.Src.Application.Features.Lessons.Queries;
using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;

namespace QLDT_Becamex.Src.Application.Features.Lessons.Handlers
{
    public class GetCountCompletedLessonOfCourseQueryHandler : IRequestHandler<GetCountCompletedLessonOfCourseQuery, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public GetCountCompletedLessonOfCourseQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<int> Handle(GetCountCompletedLessonOfCourseQuery request, CancellationToken cancellationToken)
        {
            var (userId, role) = _userService.GetCurrentUserAuthenticationInfo();

            if (string.IsNullOrEmpty(userId))
            {
                // Sử dụng AppException của bạn với mã lỗi phù hợp
                throw new AppException("User ID not found. User must be authenticated.", 401);
            }
            // Validate CourseId
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
            {
                throw new AppException("Khóa học không tồn tại", 404);
            }

            // Lấy danh sách bài học của khóa học theo CourseId
            var lessons = await _unitOfWork.LessonRepository.GetFlexibleAsync(
                predicate: l => l.CourseId == request.CourseId,
                orderBy: q => q.OrderBy(l => l.Position),
                includes: q => q.Include(l => l.TypeDoc)
            );

            // Kiểm tra nếu không có bài học nào
            // if (lessons == null || !lessons.Any())
            //     throw new AppException("Không tìm thấy bài học nào cho khóa học này", 200);

            var lessonProgress = await _unitOfWork.LessonProgressRepository.GetFlexibleAsync(
                predicate: lp => lp.UserId == userId && lessons.Select(l => l.Id).Contains(lp.LessonId),
                includes: q => q.Include(lp => lp.Lesson)
            );

            int count = 0;
            foreach (var lesson in lessonProgress)
            {
                if (lesson.IsCompleted)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
