using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Queries;
using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;

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
            var (userId, role) = _userService.GetCurrentUserAuthenticationInfo();

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
                    predicate: cu => cu.CourseId == request.CourseId && cu.UserId == userId);
                if (courseUser == null)
                {
                    throw new AppException("Bạn không có quyền truy cập bài học của khóa học này", 403);
                }
            }

            var tests = await _unitOfWork.TestRepository.GetFlexibleAsync(
                predicate: t => t.CourseId == request.CourseId,
                orderBy: q => q.OrderBy(t => t.Position),
                includes: q => q
                        .Include(d => d.Questions)
            );

            // if (tests == null || !tests.Any())
            //     throw new AppException("Không tìm thấy bài kiểm tra nào cho khóa học này", 200);
            var dto = _mapper.Map<List<AllTestDto>>(tests);

            return dto;
        }
    }
}
