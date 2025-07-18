using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Features.Tests.Commands;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;

public class SaveTestResultCommandHandler : IRequestHandler<SaveTestResultCommand, TestResultDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public SaveTestResultCommandHandler(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<TestResultDto> Handle(SaveTestResultCommand request, CancellationToken cancellationToken)
    {
        // --- B1: LẤY DỮ LIỆU VÀ VALIDATE ---
        var (currentUserId, _) = _userService.GetCurrentUserAuthenticationInfo();

        var test = await _unitOfWork.TestRepository.GetByIdAsync(request.TestId);
        if (test == null)
        {
            throw new Exception($"Không tìm thấy bài test với ID: {request.TestId}");
        }

        var questionIds = request.SubmittedAnswers.Select(a => a.QuestionId).ToList();
        var questionsEnumerable = await _unitOfWork.QuestionRepository.GetFlexibleAsync(
            predicate: q => questionIds.Contains(q.Id),
            asNoTracking: true
        );
        var questions = questionsEnumerable.ToDictionary(q => q.Id, q => q);

        // Tổng số câu hỏi trong bài làm
        int totalQuestions = questions.Count;

        // --- B2: TẠO KẾT QUẢ VÀ CHẤM ĐIỂM ---
        var newTestResult = new TestResult
        {
            Id = Guid.NewGuid().ToString(),
            TestId = request.TestId,
            UserId = currentUserId,
            StartedAt = request.StartedAt
        };

        int correctAnswerCount = 0;

        foreach (var userAnswerDto in request.SubmittedAnswers)
        {
            if (!questions.TryGetValue(userAnswerDto.QuestionId, out var question))
            {
                continue;
            }

            var userSelectionsString = string.Join(",", userAnswerDto.SelectedOptions.OrderBy(s => s));
            bool isCorrect = AreSelectionsCorrect(userSelectionsString, question.CorrectOption);

            if (isCorrect)
            {
                correctAnswerCount++;
            }

            var userAnswer = new UserAnswer
            {
                TestResultId = newTestResult.Id,
                QuestionId = userAnswerDto.QuestionId,
                SelectedOptions = userSelectionsString,
                IsCorrect = isCorrect
            };
            await _unitOfWork.UserAnswerRepository.AddAsync(userAnswer);
        }

        // --- B3: HOÀN TẤT VÀ TÍNH ĐIỂM TỔNG KẾT ---
        newTestResult.SubmittedAt = DateTime.UtcNow;
        newTestResult.Score = totalQuestions > 0 ? (float)correctAnswerCount / totalQuestions * 100 : 0;
        newTestResult.IsPassed = newTestResult.Score >= test.PassThreshold * 100;

        // --- BỔ SUNG: TÍNH TOÁN VÀ GÁN SỐ CÂU ĐÚNG/SAI ---
        newTestResult.CorrectAnswerCount = correctAnswerCount;
        newTestResult.IncorrectAnswerCount = totalQuestions - correctAnswerCount;
        // --- KẾT THÚC BỔ SUNG ---

        // --- B4: LƯU VÀO CƠ SỞ DỮ LIỆU ---
        await _unitOfWork.TestResultRepository.AddAsync(newTestResult);
        await _unitOfWork.CompleteAsync();

        // --- B5: TRẢ KẾT QUẢ VỀ ---
        // Map entity sang DTO. AutoMapper sẽ tự động map các trường mới nếu tên trùng khớp.
        TestResultDto testResultDto = _mapper.Map<TestResultDto>(newTestResult);
        var user = await _unitOfWork.UserRepository.GetByIdAsync(currentUserId);
        testResultDto.User = new UserSumaryDto() { Id = user.Id, Name = user.FullName };


        return testResultDto;
    }

    // (Hàm AreSelectionsCorrect không thay đổi)
    private bool AreSelectionsCorrect(string userSelections, string? correctAnswers)
    {
        if (string.IsNullOrEmpty(userSelections) || string.IsNullOrEmpty(correctAnswers))
        {
            return string.IsNullOrEmpty(userSelections) && string.IsNullOrEmpty(correctAnswers);
        }
        var userSet = new HashSet<string>(userSelections.Split(','));
        var correctSet = new HashSet<string>(correctAnswers.Split(','));
        return userSet.SetEquals(correctSet);
    }
}