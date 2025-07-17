using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Application.Features.Tests.Queries;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Domain.Entities;
using Microsoft.Extensions.Configuration.UserSecrets;
using Xunit.Sdk;

namespace QLDT_Becamex.Src.Application.Features.Tests.Handlers
{
    public class GetTestResultQueryHandler : IRequestHandler<GetTestResultQuery, TestResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public GetTestResultQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<TestResultDto> Handle(GetTestResultQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var (userId, __) = _userService.GetCurrentUserAuthenticationInfo();
                var courseExists = await _unitOfWork.CourseRepository.AnyAsync(c => c.Id == request.CourseId);
                if (!courseExists)
                {
                    throw new AppException("Khóa học không tồn tại", 404);
                }
                var test = await _unitOfWork.TestRepository.GetFlexibleAsync(
                    predicate: t => t.Id == request.Id && t.CourseId == request.CourseId,
                    orderBy: null,
                    page: null,
                    pageSize: 1, // Giới hạn 1 bản ghi
                    asNoTracking: true,
                    includes: t => t.Include(t => t.Questions).Include(d => d.CreatedBy).Include(d => d.UpdatedBy)
                );

                var testEntity = test.FirstOrDefault();
                if (testEntity == null)
                {
                    throw new AppException("Bài kiểm tra không tồn tại", 404);
                }
                var testResult = await _unitOfWork.TestResultRepository.GetFlexibleAsync(
                    predicate: tr => tr.TestId == testEntity.Id && tr.UserId == userId,
                    orderBy: null,
                    page: null,
                    pageSize: 1,
                    asNoTracking: true,
                    includes: tr => tr.Include(tr => tr.Test)
                );
                var testResultEntity = testResult.FirstOrDefault();
                if (testResultEntity == null)
                {
                    throw new AppException("Bạn chưa làm bài kiểm tra này", 404);
                }
                var testResultDto = new TestResultDto
                {
                    Score = testResultEntity.Score,
                    IsPassed = testResultEntity.IsPassed,
                    StartedAt = testResultEntity.StartedAt,
                    SubmittedAt = testResultEntity.SubmittedAt,
                };
                return testResultDto;
            }
            catch (Exception ex)
            {
                throw new AppException($"Lỗi khi lấy bài kiểm tra: {ex.Message}", 500);
            }
        }
    }
}