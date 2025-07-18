using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Features.Tests.Commands;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;
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

        // Lấy thông tin định danh của người dùng đang thực hiện request.
        // Điều này đảm bảo kết quả được lưu chính xác cho người dùng đã đăng nhập.
        var (currentUserId, _) = _userService.GetCurrentUserAuthenticationInfo();

        // Lấy thông tin của bài test (ví dụ: ngưỡng điểm qua môn) từ CSDL.
        var test = await _unitOfWork.TestRepository.GetByIdAsync(request.TestId);

        // Validate: Nếu không tìm thấy bài test, ném ra một Exception để dừng xử lý.
        if (test == null)
        {
            throw new Exception($"Không tìm thấy bài test với ID: {request.TestId}");
        }

        // Trích xuất tất cả các ID của câu hỏi từ payload người dùng gửi lên.
        // Việc này để chuẩn bị cho một truy vấn duy nhất lấy tất cả đáp án đúng.
        var questionIds = request.SubmittedAnswers.Select(a => a.QuestionId).ToList();

        // Tối ưu hóa hiệu năng: Lấy tất cả các câu hỏi và đáp án đúng trong một lần gọi CSDL duy nhất.
        // Tránh việc phải gọi CSDL N lần bên trong vòng lặp (N+1 query problem).
        var questionsEnumerable = await _unitOfWork.QuestionRepository.GetFlexibleAsync(
            predicate: q => questionIds.Contains(q.Id),
            asNoTracking: true // Sử dụng AsNoTracking vì chỉ đọc dữ liệu, không cần theo dõi thay đổi.
        );
        // Chuyển danh sách câu hỏi sang một Dictionary với Key là QuestionId.
        // Giúp việc tra cứu câu hỏi trong vòng lặp cực nhanh (độ phức tạp O(1)).
        var questions = questionsEnumerable.ToDictionary(q => q.Id, q => q);


        // --- B2: TẠO KẾT QUẢ VÀ CHẤM ĐIỂM ---

        // Tạo đối tượng "Phiếu kết quả" chính. Đây là bản ghi cha sẽ được lưu vào CSDL.
        var newTestResult = new TestResult
        {
            Id = Guid.NewGuid().ToString(),
            TestId = request.TestId,
            UserId = currentUserId,
            StartedAt = request.StartedAt // Có thể thay bằng thời gian bắt đầu thực tế nếu được gửi từ client.
        };

        int correctAnswerCount = 0; // Biến đếm số câu trả lời đúng.

        // Duyệt qua từng câu trả lời mà người dùng đã gửi.
        foreach (var userAnswerDto in request.SubmittedAnswers)
        {
            // Safety check: Bỏ qua nếu vì lý do nào đó câu trả lời không có câu hỏi tương ứng trong CSDL.
            if (!questions.TryGetValue(userAnswerDto.QuestionId, out var question))
            {
                continue;
            }

            // Chuẩn hóa dữ liệu đầu vào: Chuyển danh sách lựa chọn của người dùng thành một chuỗi được sắp xếp.
            // Ví dụ: ["C", "A"] -> "A,C". Điều này đảm bảo việc so sánh luôn nhất quán.
            var userSelectionsString = string.Join(",", userAnswerDto.SelectedOptions.OrderBy(s => s));

            // Gọi hàm helper để so sánh câu trả lời của người dùng với đáp án đúng.
            bool isCorrect = AreSelectionsCorrect(userSelectionsString, question.CorrectOption);

            if (isCorrect)
            {
                correctAnswerCount++;
            }

            // Tạo bản ghi chi tiết cho từng câu trả lời.
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

        // Ghi nhận thời điểm nộp bài.
        newTestResult.SubmittedAt = DateTime.UtcNow;
        // Tính điểm theo thang 100. Kiểm tra `questions.Count > 0` để tránh lỗi chia cho 0.
        newTestResult.Score = questions.Count > 0 ? (float)correctAnswerCount / questions.Count * 100 : 0;
        // Xác định trạng thái "Đạt" hay "Không đạt" dựa trên ngưỡng điểm của bài test.
        newTestResult.IsPassed = newTestResult.Score >= test.PassThreshold;


        TestResultDto testResultDto = _mapper.Map<TestResultDto>(newTestResult);
        // --- B4: LƯU VÀO CƠ SỞ DỮ LIỆU ---

        // Đánh dấu đối tượng `newTestResult` (và tất cả các `UserAnswer` con của nó) để thêm vào CSDL.
        await _unitOfWork.TestResultRepository.AddAsync(newTestResult);

        // Thực thi tất cả các thay đổi trên vào CSDL trong một giao dịch (transaction) duy nhất.
        // Đảm bảo tính toàn vẹn: hoặc tất cả đều thành công, hoặc không có gì được lưu.
        await _unitOfWork.CompleteAsync();


        // --- B5: TRẢ KẾT QUẢ VỀ ---

        // Trả về ID của bản ghi `TestResult` vừa được tạo.
        // Client có thể dùng ID này để chuyển hướng người dùng đến trang xem kết quả.
        return testResultDto;
    }

    /// <summary>
    /// Hàm helper so sánh hai bộ đáp án một cách chính xác, không phân biệt thứ tự.
    /// </summary>
    /// <param name="userSelections">Lựa chọn của người dùng (chuỗi đã chuẩn hóa, ví dụ: "A,C")</param>
    /// <param name="correctAnswers">Đáp án đúng từ CSDL (ví dụ: "A,C")</param>
    /// <returns>True nếu hai bộ đáp án khớp hoàn toàn, ngược lại là false.</returns>
    private bool AreSelectionsCorrect(string userSelections, string? correctAnswers)
    {
        // Xử lý trường hợp một hoặc cả hai chuỗi rỗng.
        if (string.IsNullOrEmpty(userSelections) || string.IsNullOrEmpty(correctAnswers))
        {
            return string.IsNullOrEmpty(userSelections) && string.IsNullOrEmpty(correctAnswers);
        }
        // Chuyển chuỗi thành HashSet để so sánh không phân biệt thứ tự và loại bỏ trùng lặp.
        var userSet = new HashSet<string>(userSelections.Split(','));
        var correctSet = new HashSet<string>(correctAnswers.Split(','));
        // SetEquals kiểm tra xem hai bộ có chứa chính xác các phần tử giống nhau không.
        return userSet.SetEquals(correctSet);
    }
}