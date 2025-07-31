using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Application.Features.Tests.Queries;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Tests.Handlers
{
    public class GetDetailTestResultQueryHandler : IRequestHandler<GetDetailTestResultQuery, DetailTestResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public GetDetailTestResultQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<DetailTestResultDto> Handle(GetDetailTestResultQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var (userId, __) = _userService.GetCurrentUserAuthenticationInfo();
                if (string.IsNullOrEmpty(userId))
                {
                    throw new AppException("Người dùng không hợp lệ", 400);
                }
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
                    orderBy: q => q.OrderByDescending(tr => tr.Score),
                    page: null,
                    pageSize: 1,
                    asNoTracking: true,
                    includes: tr => tr.Include(tr => tr.Test)
                );
                var testResultEntity = testResult.FirstOrDefault();
                if (testResultEntity == null)
                {
                    return null;
                }
                var userAnswers = await _unitOfWork.UserAnswerRepository.GetFlexibleAsync(
                    predicate: ua => ua.TestResultId == testResultEntity.Id,
                    orderBy: null,
                    page: null,
                    pageSize: null,
                    asNoTracking: true,
                    includes: ua => ua.Include(ua => ua.Question)
                );
                var userAnswerAndCorrectAnswerDtos = new List<UserAnswerAndCorrectAnswerDto >();
                foreach (var answer in userAnswers)
                {
                    var question = testEntity.Questions != null
                        ? testEntity.Questions.FirstOrDefault(q => q.Id == answer.QuestionId)
                        : null;
                    if (question == null)
                    {
                        continue;
                    }
                    userAnswerAndCorrectAnswerDtos.Add(new UserAnswerAndCorrectAnswerDto
                    {
                        Question = answer.Question,
                        SelectedOptions = answer.SelectedOptions,
                        CorrectAnswer = question.CorrectOption,
                        IsCorrect = answer.IsCorrect
                    });
                }
                // var detailTestResultDto = new DetailTestResultDto
                // {
                //     Score = testResultEntity.Score,
                //     IsPassed = testResultEntity.IsPassed,
                //     StartedAt = testResultEntity.StartedAt,
                //     CorrectAnswerCount = testResultEntity.CorrectAnswerCount,
                //     IncorrectAnswerCount = testResultEntity.IncorrectAnswerCount,
                //     SubmittedAt = testResultEntity.SubmittedAt,
                //     UserAnswers = userAnswerAndCorrectAnswerDtos
                // };
                var detailTestResultDto = _mapper.Map<DetailTestResultDto>(testResultEntity);
                detailTestResultDto.UserAnswers = userAnswerAndCorrectAnswerDtos;
                var userSumaryDto = new UserSumaryDto
                {
                    Id = userId,
                    Name = _unitOfWork.UserRepository.GetByIdAsync(userId).Result?.UserName,
                };
                detailTestResultDto.User = userSumaryDto;
                return detailTestResultDto;
            }
            catch (Exception ex)
            {
                throw new AppException($"Lỗi khi lấy bài kiểm tra: {ex.Message}", 500);
            }
        }
    }
}