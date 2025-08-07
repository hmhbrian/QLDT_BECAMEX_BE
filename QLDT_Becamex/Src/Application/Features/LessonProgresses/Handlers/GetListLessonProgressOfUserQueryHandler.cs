using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.LessonProgresses.Dtos;
using QLDT_Becamex.Src.Application.Features.LessonProgresses.Queries;
using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Shared.Helpers;
using Xceed.Pdf.Layout.Shape;

namespace QLDT_Becamex.Src.Application.Features.LessonProgresses.Handlers
{
    public class GetListLessonProgressOfUserQueryHandler : IRequestHandler<GetListLessonProgressOfUserQuery, List<AllLessonProgressDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public GetListLessonProgressOfUserQueryHandler(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }
        public async Task<List<AllLessonProgressDto>> Handle(GetListLessonProgressOfUserQuery request, CancellationToken cancellationToken)
        {
            // Lấy User ID từ BaseService
            var (userId, role) = _userService.GetCurrentUserAuthenticationInfo();
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException("User ID not found. User must be authenticated.", 404);
            }

            // Validate CourseId
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
            {
                throw new AppException("Khóa học không tồn tại", 404);
            }

            // Kiểm tra quyền truy cập khóa họcz
            if (role == ConstantRole.HOCVIEN)
            {
                var courseUser = await _unitOfWork.UserCourseRepository.GetFirstOrDefaultAsync(
                    predicate: cu => cu.CourseId == request.CourseId && cu.UserId == userId);
                if (courseUser == null)
                {
                    throw new AppException("Bạn không có quyền truy cập bài học của khóa học này", 404);
                }

                //Kiểm tra không hiển thị bài học khi chưa tới ngày bắt đầu
                var currentDate = DateTimeHelper.GetVietnamTimeNow();// lấy thời gian hiện tại theo múi giờ VN
                if (course.StartDate > currentDate)
                    throw new AppException("Không thể hiển thị bài học do chưa tới thời gian bắt đầu.", 404);
            }


            // Lấy danh sách bài học theo CourseId
            var lesson = await _unitOfWork.LessonRepository.GetFlexibleAsync(
                predicate: lp => lp.CourseId == request.CourseId,
                orderBy: q => q.OrderBy(lp => lp.Position),
                includes: q => q
                    .Include(l => l.TypeDoc)
                    .Include(l => l.LessonProgress!.Where(lp => lp.UserId == userId))
            );
            // Chuyển đổi sang DTO và tính toán ProgressPercenttage
            var dtoList = lesson.Select(lesson =>
            {
                var progress = lesson.LessonProgress?.FirstOrDefault();
                int currentPage = 0;
                int currentTimeSeconds = 0;
                double progressPercentage = 0;
                if (progress != null)
                {
                    if (progress.IsCompleted)
                    {
                        progressPercentage = 1.0; // Hoàn thành
                    }
                    else
                    {
                        if (lesson.TypeDocId == 1) //PDF
                        {
                            if (lesson.TotalPages.HasValue && lesson.TotalPages > 0 && progress.CurrentPage.HasValue)
                            {
                                progressPercentage = (double)progress.CurrentPage!.Value / lesson.TotalPages.Value;
                                currentPage = progress.CurrentPage.Value;
                            }
                        }
                        else if (lesson.TypeDocId == 2) //Video
                        {
                            if (lesson.TotalDurationSeconds.HasValue && lesson.TotalDurationSeconds > 0 && progress.CurrentTimeSeconds.HasValue)
                            {
                                progressPercentage = (double)progress.CurrentTimeSeconds!.Value / lesson.TotalDurationSeconds.Value;
                                currentTimeSeconds = progress.CurrentTimeSeconds.Value;
                            }
                        }
                    }
                }

                // Làm tròn tới 2 chữ số thập phân
                progressPercentage = Math.Round(progressPercentage, 2);

                // Đảm bảo ProgressPercentage nằm trong khoảng 0-1
                progressPercentage = Math.Max(0, Math.Min(1, progressPercentage));

                return new AllLessonProgressDto
                {
                    Id = lesson.Id,
                    Title = string.IsNullOrEmpty(lesson.Title) ? ""
                        : char.ToUpper(lesson.Title[0]) + lesson.Title.Substring(1),
                    UrlPdf = lesson.FileUrl,
                    ProgressPercentage = progressPercentage,
                    Type = lesson.TypeDoc?.NameType ?? "Unknown",
                    CurrentPage = lesson.TypeDocId == 1 ? currentPage : null, // Chỉ trả CurrentPage nếu là PDF
                    CurrentTimeSecond = lesson.TypeDocId == 2 ? currentTimeSeconds : null // Chỉ trả CurrentTimeSecond nếu là Video
                };
            }).ToList();

            return dtoList;
        }
    }
}
