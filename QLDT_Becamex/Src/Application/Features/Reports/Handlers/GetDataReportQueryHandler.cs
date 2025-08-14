using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Features.Reports.Dtos;
using QLDT_Becamex.Src.Application.Features.Reports.Queries;
using QLDT_Becamex.Src.Constant;
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
            var (startDate, endDate) = GetDateRange(request.month, request.quarter, request.year);

            var numberOfCourses = await GetNumberOfCoursesAsync(startDate, endDate);
            var numberOfStudents = await GetNumberOfStudentsAsync(startDate, endDate);
            var averageCompletedPercentage = await GetAverageCompletedPercentageAsync(startDate, endDate);
            var averageTime = await GetAverageTimeAsync(startDate, endDate);
            var averagePositiveFeedback = await GetAveragePositiveFeedbackAsync(startDate, endDate);

            return new DataReportDto
            {
                NumberOfCourses = numberOfCourses,
                NumberOfStudents = numberOfStudents,
                AverangeCompletedPercentage = averageCompletedPercentage,
                AverangeTime = averageTime,
                AveragePositiveFeedback = averagePositiveFeedback
            };
        }

        private (DateTime? start, DateTime? end) GetDateRange(int? month, int? quarter, int? year)
        {
            int currentYear = DateTime.Now.Year;

            if (quarter != null)
            {
                int useYear = year ?? currentYear;
                int startMonth = (quarter.Value - 1) * 3 + 1;
                DateTime start = new DateTime(useYear, startMonth, 1);
                DateTime end = start.AddMonths(3).AddDays(-1);
                return (start, end);
            }

            if (month != null)
            {
                int useYear = year ?? currentYear;
                DateTime start = new DateTime(useYear, month.Value, 1);
                DateTime end = start.AddMonths(1).AddDays(-1);
                return (start, end);
            }

            if (year != null)
            {
                DateTime start = new DateTime(year.Value, 1, 1);
                DateTime end = new DateTime(year.Value, 12, 31);
                return (start, end);
            }

            // Không có month, quarter, hay year nào → không lọc
            return (null, null);
        }

        private async Task<int> GetNumberOfCoursesAsync(DateTime? start, DateTime? end)
        {
            var courses = await _unitOfWork.CourseRepository.GetFlexibleAsync(
                c => c.StatusId != 1 && !c.IsDeleted,
                orderBy: null
            );

            if (start != null && end != null)
            {
                courses = courses.Where(c => c.CreatedAt.HasValue && c.CreatedAt.Value >= start && c.CreatedAt.Value <= end).ToList();
            }

            return courses.Count();
        }

        private async Task<int> GetNumberOfStudentsAsync(DateTime? start, DateTime? end)
        {
            const string roleName = "HOCVIEN";
            var query = from user in _context.Users
                        join userRole in _context.UserRoles on user.Id equals userRole.UserId
                        join role in _context.Roles on userRole.RoleId equals role.Id
                        where role.Name == roleName && user.CreatedAt.HasValue && !user.IsDeleted
                        select user;

            if (start != null && end != null)
            {
                query = query.Where(u => u.CreatedAt >= start && u.CreatedAt <= end);
            }

            return await query.CountAsync();
        }

        private async Task<float> GetAverageCompletedPercentageAsync(DateTime? start, DateTime? end)
        {
            var userCourses = await _unitOfWork.UserCourseRepository.GetFlexibleAsync(
                uc => uc.Course != null && !uc.Course.IsDeleted,
                orderBy: null
            );
            var completedCourses = await _unitOfWork.UserCourseRepository.GetFlexibleAsync(
                uc => uc.Course != null && !uc.Course.IsDeleted && uc.Status == ConstantStatus.COMPLETED,
                orderBy: null
            );

            if (start != null && end != null)
            {
                userCourses = userCourses.Where(uc => uc.ModifiedAt >= start && uc.ModifiedAt <= end).ToList();
                completedCourses = completedCourses.Where(uc => uc.ModifiedAt >= start && uc.ModifiedAt <= end).ToList();
            }

            if (!userCourses.Any() || !completedCourses.Any())
                return 0;

            float percent = (float)completedCourses.Count() / userCourses.Count() * 100;
            return percent;
        }

        private async Task<float> GetAverageTimeAsync(DateTime? start, DateTime? end)
        {
            var userCourses = await _unitOfWork.UserCourseRepository.GetFlexibleAsync(
                uc => uc.Course != null && !uc.Course.IsDeleted,
                orderBy: null
            );

            if (start != null && end != null)
            {
                userCourses = userCourses.Where(uc => uc.ModifiedAt >= start && uc.ModifiedAt <= end).ToList();
            }

            if (!userCourses.Any())
                return 0;

            float totalTime = 0;

            foreach (var uc in userCourses)
            {
                var course = await _unitOfWork.CourseRepository.GetFirstOrDefaultAsync(c => c.Id == uc.CourseId);
                float time = ((float?)course?.Sessions ?? 0) * ((float?)course?.HoursPerSessions ?? 0);
                totalTime += time;
            }

            return totalTime / userCourses.Count();
        }

        private async Task<float> GetAveragePositiveFeedbackAsync(DateTime? start, DateTime? end)
        {
            var feedbacks = await _unitOfWork.FeedbackRepository.GetFlexibleAsync(
                f => f.Course != null && !f.Course.IsDeleted,
                orderBy: null
            );
            if (start != null && end != null)
                {
                    feedbacks = feedbacks.Where(f => f.SubmissionDate.HasValue && f.SubmissionDate.Value >= start && f.SubmissionDate.Value <= end).ToList();
                }

            if (!feedbacks.Any())
            {
                return 0;
            }

            int positiveCount = 0;

            foreach (var f in feedbacks)
            {
                float avg = (f.Q1_relevance + f.Q2_clarity + f.Q3_structure + f.Q4_duration + f.Q5_material) / 5;
                if (avg >= 4) positiveCount++;
            }

            return (float)positiveCount / feedbacks.Count() * 100;
        }
    }
}
