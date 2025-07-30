using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Commands;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Events;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;

public class SaveTestResultCommandHandler : IRequestHandler<SaveTestResultCommand, TestResultDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public SaveTestResultCommandHandler(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
        _mapper = mapper;
        _mediator = mediator;
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

        bool isFinish = await MeetTheStandards(currentUserId, test.CourseId);
        // Kiểm tra nếu user đã hoàn thành khóa học (lessons + passed all tests)
        if (isFinish)
        {
            await _mediator.Publish(new TestSubmittedEvent(currentUserId, test.CourseId));
        }
        // --- B5: TRẢ KẾT QUẢ VỀ ---
        TestResultDto testResultDto = _mapper.Map<TestResultDto>(newTestResult);
        testResultDto.IsFinish = isFinish;
        var user = await _unitOfWork.UserRepository.GetByIdAsync(currentUserId);
        testResultDto.User = user != null
            ? new UserSumaryDto() { Id = user.Id, Name = user.FullName }
            : null;
        if (test.CourseId == null)
        {
            throw new AppException("Bài kiểm tra không thuộc khóa học nào", 404);
        }
        await IsCompletedCourse(test.CourseId, currentUserId);
        return testResultDto;
    }

    private async Task<bool> MeetTheStandards(string userId, string courseId)
    {
        // 1. Lấy tất cả bài học thuộc khóa học
        var lessons = await _unitOfWork.LessonRepository.GetQueryable()
            .Where(l => l.CourseId == courseId)
            .ToListAsync();

        // 2. Lấy LessonProgress của user trong khóa học
        var lessonProgresses = await _unitOfWork.LessonProgressRepository.GetQueryable()
            .Where(lp => lp.UserId == userId && lessons.Select(l => l.Id).Contains(lp.LessonId))
            .ToListAsync();

        // 3. Kiểm tra xem user đã hoàn thành hết chưa
        bool allLessonsCompleted = lessons.All(lesson =>
            lessonProgresses.Any(lp => lp.LessonId == lesson.Id && lp.IsCompleted));

        // 4. Lấy tất cả bài test thuộc khóa học
        var tests = await _unitOfWork.TestRepository.GetQueryable()
            .Where(t => t.CourseId == courseId)
            .ToListAsync();

        // 5. Lấy kết quả test của user
        var testResults = await _unitOfWork.TestResultRepository.GetQueryable()
            .Where(tr => tr.UserId == userId && tests.Select(t => t.Id).Contains(tr.TestId))
            .ToListAsync();

        // 6. Kiểm tra xem user đã làm và qua hết chưa
        bool allTestsPassed = tests.All(test =>
            testResults.Any(tr => tr.TestId == test.Id && tr.IsPassed));

        // 7. Nếu hoàn thành hết và vượt qua hết thì đạt chuẩn
        return allLessonsCompleted && allTestsPassed;
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
    private async Task IsCompletedCourse(string courseId, string userId)
    {
        var userCourse = await _unitOfWork.UserCourseRepository.GetFirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseId);
        if (userCourse == null)
        {
            throw new AppException("Khóa học không tồn tại cho người dùng", 404);
        }
        var lessons = await _unitOfWork.LessonRepository
            .GetFlexibleAsync(c => c.CourseId == courseId);
        var lessonsProgress = await _unitOfWork.LessonProgressRepository
            .GetFlexibleAsync(lp => lp.UserId == userId && lp.Lesson.CourseId == courseId);
        if (lessonsProgress.Count() != lessons.Count())
        {
            Console.WriteLine($"Not all lessons completed for user {userId} in course {courseId}");
            userCourse.Status = "In Progress";
            await _unitOfWork.CompleteAsync();
            return;
        }
        foreach (var lesson in lessonsProgress)
        {
            if (!lesson.IsCompleted)
            {
                Console.WriteLine($"Lesson {lesson.LessonId} is not completed for user {userId}");
                userCourse.Status = "In Progress";
                await _unitOfWork.CompleteAsync();
                return;
            }
            Console.WriteLine($"Lesson {lesson.LessonId} is completed for user {userId}");
        }
        var tests = await _unitOfWork.TestRepository
            .GetFlexibleAsync(t => t.CourseId == courseId);
        var testResults = await _unitOfWork.TestResultRepository
            .GetFlexibleAsync(tr => tr.UserId == userId && tr.Test != null && tr.Test.CourseId == courseId);
        var highestScorePerTest = testResults
            .GroupBy(tr => tr.TestId)
            .Select(g => g.OrderByDescending(tr => tr.Score).First())
            .ToList();
        if (highestScorePerTest.Count() != tests.Count())
        {
            Console.WriteLine($"Not all tests completed for user {userId} in course {courseId}");
            userCourse.Status = "In Progress";
            await _unitOfWork.CompleteAsync();
            return;
        }
        foreach (var test in highestScorePerTest)
        {
            if (!test.IsPassed)
            {
                Console.WriteLine($"Test {test.TestId} is not passed for user {userId}");
                userCourse.Status = "In Progress";
                await _unitOfWork.CompleteAsync();
                return;
            }
            Console.WriteLine($"Test {test.TestId} is passed for user {userId}");
        }
        userCourse.Status = "Completed";
        Console.WriteLine($"Course {courseId} is completed for user {userId}");
        await _unitOfWork.CompleteAsync();
    }
}
