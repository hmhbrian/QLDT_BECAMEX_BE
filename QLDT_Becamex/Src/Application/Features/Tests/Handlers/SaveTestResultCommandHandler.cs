using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Commands;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Infrastructure.Services.CourseServices;

public class SaveTestResultCommandHandler : IRequestHandler<SaveTestResultCommand, TestResultDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;
    private readonly ICourseService _courseService;
    private readonly IMapper _mapper;

    public SaveTestResultCommandHandler(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper, IMediator mediator, ICourseService courseService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
        _courseService = courseService;
        _mapper = mapper;
    }

    public async Task<TestResultDto> Handle(SaveTestResultCommand request, CancellationToken cancellationToken)
    {
        // --- B1: LẤY DỮ LIỆU VÀ VALIDATE ---
        var (currentUserId, _) = _userService.GetCurrentUserAuthenticationInfo();
        if (currentUserId == null)
        {
            throw new AppException("Không xác định được người dùng hiện tại.", 401);
        }

        var test = await _unitOfWork.TestRepository.GetByIdAsync(request.TestId);
        if (test == null)
        {
            throw new AppException($"Không tìm thấy bài test với ID: {request.TestId}", 404);
        }

        bool isDone = await _unitOfWork.TestResultRepository
              .AnyAsync(tr => tr.UserId == currentUserId && tr.TestId == test.Id);

        if (isDone)
        {
            throw new AppException($"Không được phép. Bạn chỉ đươc phép làm bài kiểm tra 1 lần duy nhất.", 403);
        }

        var questionIds = request.SubmittedAnswers.Select(a => a.QuestionId).ToList();
        var questionsEnumerable = await _unitOfWork.QuestionRepository.GetFlexibleAsync(
            predicate: q => questionIds.Contains(q.Id) || q.TestId == test.Id, // đảm bảo lấy đủ các câu trong bài test
            asNoTracking: true
        );
        var questions = questionsEnumerable.ToDictionary(q => q.Id, q => q);

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

        // Chuyển submittedAnswers thành Dictionary để tra cứu nhanh
        var submittedDict = request.SubmittedAnswers.ToDictionary(a => a.QuestionId, a => a);

        foreach (var question in questions.Values)
        {
            UserAnswer userAnswer;

            if (submittedDict.TryGetValue(question.Id, out var userAnswerDto))
            {
                var userSelectionsString = string.Join(",", userAnswerDto.SelectedOptions.OrderBy(s => s));
                bool isCorrect = AreSelectionsCorrect(userSelectionsString, question.CorrectOption);

                if (isCorrect)
                    correctAnswerCount++;

                userAnswer = new UserAnswer
                {
                    TestResultId = newTestResult.Id,
                    QuestionId = question.Id,
                    SelectedOptions = userSelectionsString,
                    IsCorrect = isCorrect
                };
            }
            else
            {
                // Người dùng bỏ qua câu hỏi này
                userAnswer = new UserAnswer
                {
                    TestResultId = newTestResult.Id,
                    QuestionId = question.Id,
                    SelectedOptions = "",
                    IsCorrect = false
                };
            }

            await _unitOfWork.UserAnswerRepository.AddAsync(userAnswer);
        }

        // --- B3: HOÀN TẤT VÀ TÍNH ĐIỂM TỔNG KẾT ---
        newTestResult.SubmittedAt = DateTime.UtcNow;
        newTestResult.Score = totalQuestions > 0 ? (float)correctAnswerCount / totalQuestions * 100 : 0;
        newTestResult.IsPassed = newTestResult.Score >= test.PassThreshold * 100;

        // --- BỔ SUNG: TÍNH TOÁN VÀ GÁN SỐ CÂU ĐÚNG/SAI ---
        newTestResult.CorrectAnswerCount = correctAnswerCount;
        newTestResult.IncorrectAnswerCount = totalQuestions - correctAnswerCount;

        // --- B4: LƯU VÀO CƠ SỞ DỮ LIỆU ---
        await _unitOfWork.TestResultRepository.AddAsync(newTestResult);
        await _unitOfWork.CompleteAsync();

        // --- B5: TRẢ KẾT QUẢ VỀ ---
        TestResultDto testResultDto = _mapper.Map<TestResultDto>(newTestResult);
        var user = await _unitOfWork.UserRepository.GetByIdAsync(currentUserId);
        testResultDto.User = user != null
            ? new UserSumaryDto() { Id = user.Id, Name = user.FullName }
            : null;
        if (test.CourseId == null)
        {
            throw new AppException("Bài kiểm tra không thuộc khóa học nào", 404);
        }

        await _courseService.CalculateAndPersistAsync(test.CourseId, currentUserId, cancellationToken);
        return testResultDto;
    }
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
