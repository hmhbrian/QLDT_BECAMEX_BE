using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Questions.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Persistence;
using QLDT_Becamex.Src.Infrastructure.Services;

namespace QLDT_Becamex.Src.Application.Features.Questions.Handlers
{
    public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateQuestionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
        {
            var createQuestionDto = request.Request;

            // Kiểm tra bài kiểm tra tồn tại
            var existTest = await _unitOfWork.TestRepository.GetByIdAsync(request.TestId);
            if (existTest == null)
            {
                throw new AppException("Bài kiểm tra không tồn tại", 404);
            }

            var allQuestions = await _unitOfWork.QuestionRepository.GetAllAsync();
            var existingQuestions = allQuestions.Where(q => q.TestId == request.TestId).ToList();

            int maxPosition = existingQuestions.Any()
                ? existingQuestions.Max(q => q.Position)
                : 0;

            // Tạo câu hỏi và gán Position
            var newQuestion = new Question();
            newQuestion.Create(request.TestId, createQuestionDto);
            newQuestion.Position = maxPosition + 1;
            newQuestion.CreatedAt = DateTime.UtcNow;
            newQuestion.UpdatedAt = DateTime.UtcNow;

            // Lưu vào DB
            await _unitOfWork.QuestionRepository.AddAsync(newQuestion);
            await _unitOfWork.CompleteAsync();

            return $"Tạo câu hỏi thành công. ID của câu hỏi mới là: {newQuestion.Id}";
        }
    }
}
