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
    public class EnrollCourseCommandHandler : IRequestHandler<EnrollCourseCommand, string>
    {
        // Remove ApplicationDbContext direct dependency if IUnitOfWork fully abstracts it
        // private readonly ApplicationDbContext _context; 
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public EnrollCourseCommandHandler(IUserService userService, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(EnrollCourseCommand request, CancellationToken cancellationToken)
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
            // Use _unitOfWork.UserCourseRepository for consistency if it exists
            var existingEnrollment = await _unitOfWork.UserCourseRepository
                                                       .AnyAsync(uc => uc.UserId == currentUserId && uc.CourseId == request.CourseId);

            if (existingEnrollment)
            {
                throw new AppException("Bạn đã ghi danh khóa học này rồi.", 409); // Conflict
            }

            // 4. Check enrollment conditions (e.g., registration date, max participants)
            if (course.RegistrationClosingDate.HasValue && course.RegistrationClosingDate.Value < DateTime.Now)
            {
                throw new AppException("Đã hết thời gian đăng ký cho khóa học này.", 400);
            }

            // Check maximum participants if applicable
            if (course.MaxParticipant.HasValue && course.MaxParticipant > 0)
            {
                var currentParticipants = await _unitOfWork.UserCourseRepository
                                                            .CountAsync(uc => uc.CourseId == request.CourseId);
                if (currentParticipants >= course.MaxParticipant.Value)
                {
                    throw new AppException("Khóa học đã đủ số lượng người tham gia tối đa.", 400);
                }
            }


            // 5. Create a new UserCourse record
            var userCourse = new UserCourse
            {
                UserId = currentUserId,
                CourseId = request.CourseId,
                AssignedAt = DateTime.Now,
                Optional = 0,
                IsMandatory = false,
                Status = ConstantStatus.ASSIGNED,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
            };

            await _unitOfWork.UserCourseRepository.AddAsync(userCourse);
            await _unitOfWork.CompleteAsync(); // Commit the transaction

            return "Tham gia khóa học thành công";
        }
    }
}