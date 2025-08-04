using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Shared.Helpers;

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers
{
    public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCourseCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(DeleteCourseCommand command, CancellationToken cancellationToken)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(command.Id);
            if (course == null)
                throw new AppException("Khóa học không tồn tại", 404);

            var currentDate = DateTimeHelper.GetVietnamTimeNow();
            if (course.RegistrationStartDate.HasValue && currentDate > course.RegistrationStartDate.Value)
                throw new AppException("Ngày xóa phải trước ngày bắt đầu đăng ký", 400);

            course.IsDeleted = true;
            course.UpdatedAt = currentDate;

            await _unitOfWork.CompleteAsync();

            return course.Id;
        }
    }
}
