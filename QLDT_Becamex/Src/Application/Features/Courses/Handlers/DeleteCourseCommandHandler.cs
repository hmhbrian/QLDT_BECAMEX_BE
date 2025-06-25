using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Commands;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers
{
    public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        private const int CancelledStatusId = 99; // ✅ Đổi lại theo ID thực tế của trạng thái "Huỷ"

        public DeleteCourseCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(DeleteCourseCommand command, CancellationToken cancellationToken)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(command.Id);
            if (course == null)
                throw new AppException("Khóa học không tồn tại", 404);

            if (course.RegistrationStartDate.HasValue && DateTime.Now > course.RegistrationStartDate.Value)
                throw new AppException("Ngày xóa phải trước ngày bắt đầu đăng ký", 400);

            course.StatusId = CancelledStatusId;
            course.ModifiedAt = DateTime.Now;

            _unitOfWork.CourseRepository.Update(course);
            await _unitOfWork.CompleteAsync();

            return course.Id;
        }
    }
}
