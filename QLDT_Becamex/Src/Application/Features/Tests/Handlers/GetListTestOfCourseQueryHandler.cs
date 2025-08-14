using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Queries;
using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Shared.Helpers;

namespace QLDT_Becamex.Src.Application.Features.Tests.Handlers
{
    public class GetListTestOfCourseQueryHandler : IRequestHandler<GetListTestOfCourseQuery, List<AllTestDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public GetListTestOfCourseQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<List<AllTestDto>> Handle(GetListTestOfCourseQuery request, CancellationToken cancellationToken)
        {
            // Lấy User ID từ BaseService
            var (currentUserId, role) = _userService.GetCurrentUserAuthenticationInfo();

            // Validate CourseId
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
            {
                throw new AppException("Khóa học không tồn tại", 404);
            }

            // Kiểm tra quyền truy cập test của khóa học
            if (role == ConstantRole.HOCVIEN)
            {
                var courseUser = await _unitOfWork.UserCourseRepository.GetFirstOrDefaultAsync(
                    predicate: cu => cu.CourseId == request.CourseId && cu.UserId == currentUserId);
                if (courseUser == null)
                {
                    throw new AppException("Bạn không có quyền truy cập bài học của khóa học này", 403);
                }

                //Kiểm tra chỉ hiển thị test
                var currentDate = DateTimeHelper.GetVietnamTimeNow();// lấy thời gian hiện tại theo múi giờ VN
                if (course.StartDate > currentDate)
                    throw new AppException("Không thể hiển thị bài kiểm tra do chưa tới thời gian bắt đầu.", 404);
            }

            

            var tests = await _unitOfWork.TestRepository.GetFlexibleAsync(
                predicate: t => t.CourseId == request.CourseId,
                orderBy: q => q.OrderBy(t => t.Position),
                includes: q => q
                        .Include(d => d.Questions)
                        .Include(d => d.CreatedBy)
                        .Include(d => d.UpdatedBy)

            );

            var dto = _mapper.Map<List<AllTestDto>>(tests);

            foreach (var testDto in dto)
            {
                var isExist = await _unitOfWork.TestResultRepository
                    .GetFirstOrDefaultAsync(tr => tr.UserId == currentUserId && tr.TestId == testDto.Id);
                if(isExist != null)
                {
                    testDto.IsDone = true; // ✅ Gán thẳng vào DTO
                    testDto.IsPassed = isExist.IsPassed;
                    testDto.Score = isExist.Score > 0 ? (float)isExist.Score : 0;
                }    
                   
            }


            return dto;
        }
    }
}
