using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Lessons.Queries;
using QLDT_Becamex.Src.Application.Features.Tests.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;

namespace QLDT_Becamex.Src.Application.Features.Tests.Handlers
{
    public class GetCountCompletedTestOfCourseQueryHandler : IRequestHandler<GetCountCompletedTestOfCourseQuery, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public GetCountCompletedTestOfCourseQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<int> Handle(GetCountCompletedTestOfCourseQuery request, CancellationToken cancellationToken)
        {
            var (userId, role) = _userService.GetCurrentUserAuthenticationInfo();
            if (string.IsNullOrEmpty(userId))
            {
                // Sử dụng AppException của bạn với mã lỗi phù hợp
                throw new AppException("User ID not found. User must be authenticated.", 401);
            }

            // Validate CourseId
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
            {
                throw new AppException("Khóa học không tồn tại", 404);
            }

            // Lấy danh sách test của khóa học theo CourseId
            var tests = await _unitOfWork.TestRepository.GetFlexibleAsync(
                predicate: l => l.CourseId == request.CourseId,
                orderBy: q => q.OrderBy(l => l.Position)
            );

            var testResult = await _unitOfWork.TestResultRepository.GetFlexibleAsync(
                predicate: lp => lp.UserId == userId && tests.Select(l => l.Id).Contains(lp.TestId)
            );

            int count = 0;
            foreach (var test in testResult)
            {
                if (test.IsPassed)
                {
                    count++;
                }
            }

            return count;
        }
    }
}