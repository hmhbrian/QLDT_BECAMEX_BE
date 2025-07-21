using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Features.Reports.Dtos;
using QLDT_Becamex.Src.Application.Features.Reports.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Persistence;

namespace QLDT_Becamex.Src.Application.Features.Reports.Handlers
{
    public class GetDataReportQueryHandler : IRequestHandler<GetDataReportQuery, DataReportDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public GetDataReportQueryHandler(
            IUnitOfWork unitOfWork,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<DataReportDto> Handle(GetDataReportQuery request, CancellationToken cancellationToken)
        {
            int month = request.month;

            var numberOfCourses = await GetNumberOfCoursesAsync(month);
            var numberOfStudents = await GetNumberOfStudentsAsync(month);
            var averageCompletedPercentage = await GetAverageCompletedPercentageAsync(month);
            var averageTime = await GetAverageTimeAsync(month);
            var averagePositiveFeedback = await GetAveragePositiveFeedbackAsync(month);
            return new DataReportDto
            {
                NumberOfCourses = numberOfCourses,
                NumberOfStudents = numberOfStudents,
                AverangeCompletedPercentage = averageCompletedPercentage,
                AverangeTime = averageTime,
                AveragePositiveFeedback = averagePositiveFeedback
            };
        }

        private async Task<int> GetNumberOfCoursesAsync(int month)
        {
            var courses = await _unitOfWork.CourseRepository.GetFlexibleAsync(
                c => c.StartDate.HasValue && c.StartDate.Value.Month == month && c.StatusId != 1,
                orderBy: null
            );
            return courses?.Count() ?? 0;
        }

        private async Task<int> GetNumberOfStudentsAsync(int month)
        {
            const string roleName = "HOCVIEN";

            var query = from user in _context.Users
                        join userRole in _context.UserRoles on user.Id equals userRole.UserId
                        join role in _context.Roles on userRole.RoleId equals role.Id
                        where role.Name == roleName
                              && user.CreatedAt.HasValue
                              && user.CreatedAt.Value.Month == month
                              && !user.IsDeleted
                        select user;

            return await query.CountAsync();
        }
        private async Task<float> GetAverageCompletedPercentageAsync(int month)
        {
            var userCourses = await _unitOfWork.UserCourseRepository.GetFlexibleAsync(
                uc => uc.ModifiedAt.Month == month,
                orderBy: null
            );
            var completeUserCourses = await _unitOfWork.UserCourseRepository.GetFlexibleAsync(
                uc => uc.ModifiedAt.Month == month && uc.Status == "Completed",
                orderBy: null
            );
            if (userCourses == null || !userCourses.Any())
                return 0;
            if (completeUserCourses == null || !completeUserCourses.Any())
                return 0;
            float averangeCompletedPercentage = (float)completeUserCourses.Count() / userCourses.Count() * 100;
            return averangeCompletedPercentage;
        }

        private async Task<float> GetAverageTimeAsync(int month)
        {
            var userCourses = await _unitOfWork.UserCourseRepository.GetFlexibleAsync(
                uc => uc.ModifiedAt.Month == month,
                orderBy: null
            );
            if (userCourses == null || !userCourses.Any())
                return 0;
            float totalTime = 0;
            foreach (var userCourse in userCourses)
            {
                var course = await _unitOfWork.CourseRepository.GetFirstOrDefaultAsync(c => c.Id == userCourse.CourseId);
                float time = ((float?)(course?.Sessions) ?? 0) * ((float?)(course?.HoursPerSessions) ?? 0);
                totalTime += time;
            }
            return totalTime / userCourses.Count();
        }
        private async Task<float> GetAveragePositiveFeedbackAsync(int month)
        {
            var feedBacks = await _unitOfWork.FeedbackRepository.GetFlexibleAsync(
                f => f.CreatedAt.HasValue && f.CreatedAt.Value.Month == month,
                orderBy: null
            );
            if (feedBacks == null || !feedBacks.Any())
                return 0;
            int positiveFeedbackCount = 0;
            foreach (var feedback in feedBacks)
            {
                float totalFeedback = feedback.Q1_relevance + feedback.Q2_clarity + feedback.Q3_structure + feedback.Q4_duration + feedback.Q5_material;
                float averageFeedback = totalFeedback / 5;
                if (averageFeedback >= 4)
                {
                    positiveFeedbackCount++;
                }
            }
            return (float)positiveFeedbackCount / feedBacks.Count() * 100;
        }
    }
}
