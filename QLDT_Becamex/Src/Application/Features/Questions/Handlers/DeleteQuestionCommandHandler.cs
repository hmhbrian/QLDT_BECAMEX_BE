using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Questions.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;


namespace QLDT_Becamex.Src.Application.Features.Questions.Handlers
{
    public class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteQuestionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
        {
            var questionId = request.QuestionId;

            var existTest = await _unitOfWork.TestRepository.GetByIdAsync(request.TestId);
            if (existTest == null)
            {
                throw new AppException("Bài kiểm tra không tồn tại", 404);
            }

            var question = await _unitOfWork.QuestionRepository.FindAsync(q => q.Id == questionId && q.TestId == request.TestId);
            if (!question.Any())
            {
                throw new AppException("Câu hỏi cần xóa không tồn tại trong bài kiểm tra này", 404);
            }

            _unitOfWork.QuestionRepository.Remove(question.First());
            await _unitOfWork.CompleteAsync();

            var allQuestions = await _unitOfWork.QuestionRepository.GetAllAsync();
            var remainingQuestions = allQuestions.Where(q => q.TestId == request.TestId).ToList();

            int position = 1;
            foreach (var q in remainingQuestions.OrderBy(q => q.Position))
            {
                q.UpdatePosition(position); // Cập nhật lại vị trí của các câu hỏi còn lại
                position++;
            }

            await _unitOfWork.CompleteAsync();

            return $"Đã xóa câu hỏi thành công.";
        }

    }
}
