using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Features.Courses.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Constant; // Constants like ConstantStatus.ASSIGINED
using QLDT_Becamex.Src.Infrastructure.Services; // For IUserService
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Application.Common.Dtos; // For IUnitOfWork

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers
{
    public class CancelEnrollCourseCommandHandler : IRequestHandler<CancelEnrollCourseCommand, string>
    {
        // Remove ApplicationDbContext direct dependency if IUnitOfWork fully abstracts it
        // private readonly ApplicationDbContext _context; 
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public CancelEnrollCourseCommandHandler(IUserService userService, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(CancelEnrollCourseCommand request, CancellationToken cancellationToken)
        {
            // 1. Get current user's ID
            // The IUserService abstraction is good here, ensures userId is correctly retrieved.
            var (currentUserId, _) = _userService.GetCurrentUserAuthenticationInfo();

            if (string.IsNullOrEmpty(currentUserId))
            {
                throw new AppException("Người dùng chưa được xác thực hoặc không tìm thấy ID người dùng.", 401);
            }

            // 2. Check if the course exists
            var course = await _unitOfWork.CourseRepository.GetFirstOrDefaultAsync(c => c.Id == request.CourseId && c.IsPrivate == false && c.IsDeleted == false);

            if (course == null)
            {
                throw new AppException("Khóa học không tồn tại hoặc không được phép.", 404);
            }

            // 3. Check if the user is already enrolled in this course
            var enrollCourse = await _unitOfWork.UserCourseRepository
                .GetFirstOrDefaultAsync(uc => uc.UserId == currentUserId && uc.CourseId == request.CourseId);

            if (enrollCourse == null)
            {
                throw new AppException("Bạn chưa ghi danh khóa học này.", 400); // Conflict
            }
            else
            {
                if (enrollCourse.Optional == 1)
                {
                    throw new AppException("Bạn là học viên bắt buộc của khóa học này nên không thể hủy đăng ký.", 400);
                }
            }

            // 4. Check enrollment conditions (e.g., registration date, max participants)
            if (course.RegistrationClosingDate.HasValue && course.RegistrationClosingDate.Value < DateTime.Now)
            {
                throw new AppException("Đã hết thời gian hủy đăng ký cho khóa học này.", 400);
            }

            _unitOfWork.UserCourseRepository.Remove(enrollCourse);
            await _unitOfWork.CompleteAsync(); // Commit the transaction

            return "Hủy đăng kí khóa học thành công";
        }
    }
}