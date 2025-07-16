using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Questions.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;


namespace QLDT_Becamex.Src.Application.Features.Questions.Handlers
{
    public class DeleteQuestionsCommandHandler : IRequestHandler<DeleteQuestionsCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteQuestionsCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(DeleteQuestionsCommand request, CancellationToken cancellationToken)
        {
            var questionIds = request.QuestionIds;

            var existTest = await _unitOfWork.TestRepository.GetByIdAsync(request.TestId);
            if (existTest == null)
            {
                throw new AppException("Bài kiểm tra không tồn tại", 404);  // Ném ngoại lệ nếu bài kiểm tra không tồn tại
            }
            // Kiểm tra xem danh sách ID câu hỏi có hợp lệ không
            if (questionIds == null || !questionIds.Any())
            {
                throw new AppException("Danh sách câu hỏi cần xóa không hợp lệ", 400);
            }
            // Lấy danh sách entity tương ứng với các ID
            var questions = await _unitOfWork.QuestionRepository.FindAsync(q => questionIds.Contains(q.Id) && q.TestId == request.TestId);

            if (!questions.Any())
            {
                throw new AppException("Không tìm thấy câu hỏi nào phù hợp để xóa", 404);
            }
            // Xóa câu hỏi khỏi cơ sở dữ liệu
            _unitOfWork.QuestionRepository.RemoveRange(questions);
            await _unitOfWork.CompleteAsync();

            var remainingQuestions = await _unitOfWork.QuestionRepository.GetFlexibleAsync(
                predicate: t => t.TestId == request.TestId,
                orderBy: q => q.OrderBy(t => t.Position)
            );

            int position = 1;
            foreach (var q in remainingQuestions.OrderBy(q => q.Position))
            {
                q.UpdatePosition(position); // Cập nhật lại vị trí của các câu hỏi còn lại
                position++;
            }

            await _unitOfWork.CompleteAsync();

            // Trả về thông báo thành công
            return $"Đã xóa {questionIds.Count} câu hỏi thành công.";
        }
    }
}
