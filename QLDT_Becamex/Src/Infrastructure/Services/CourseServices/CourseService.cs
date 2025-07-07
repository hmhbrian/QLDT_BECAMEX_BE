using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Infrastructure.Services.CourseServices
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void UpdateCourseStatus(Course course)
        {
            var currentDate = DateTime.UtcNow;

            if (currentDate < course.RegistrationStartDate)
                course.StatusId = 1; //Lưu nháp
            else if(currentDate >= course.RegistrationStartDate && currentDate <= course.RegistrationClosingDate)
                course.StatusId = 2; //Sắp khai giảng
            else if (currentDate > course.RegistrationClosingDate && currentDate <= course.EndDate)
                course.StatusId = 3; //Đang mở
            else if (currentDate > course.EndDate)
                course.StatusId = 4; //Đã kết thúc
        }
    }
}
