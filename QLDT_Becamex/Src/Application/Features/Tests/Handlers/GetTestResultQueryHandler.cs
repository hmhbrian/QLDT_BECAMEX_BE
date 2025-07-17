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
    public class GetTestResultQueryHandler : IRequestHandler<GetTestResultQuery, List<UserAnswer>>
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

        public async Task<List<UserAnswer>> Handle(GetTestResultQuery request, CancellationToken cancellationToken)
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
                var userAnswers = new List<UserAnswer>();
                foreach (var question in testEntity.Questions)
                {
                    var userAnswer = await _unitOfWork.UserAnswerRepository.GetByIdAsync(question.Id);
                    if (userAnswer == null)
                    {
                        throw new AppException("Lỗi khi lấy bài kiểm tra");
                    }
                    var testResult = await _unitOfWork.TestResultRepository.AnyAsync(tr => tr.UserId == userId);
                    if (!testResult)
                    {
                        continue;
                    }
                    userAnswers.Add(userAnswer);
                }
                return userAnswers;
            }
            catch (Exception ex)
            {
                throw new AppException($"Lỗi khi lấy bài kiểm tra: {ex.Message}", 500);
            }
        }
    }
}