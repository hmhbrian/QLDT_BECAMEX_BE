using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Questions.Commands;
using QLDT_Becamex.Src.Application.Features.Questions.Dtos;
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
            // Kiểm tra bài kiểm tra tồn tại
            var existTest = await _unitOfWork.TestRepository.GetByIdAsync(request.TestId);
            if (existTest == null)
            {
                throw new AppException("Bài kiểm tra không tồn tại", 404);
            }
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                int maxPosition = await _unitOfWork.QuestionRepository.GetMaxPositionAsync(request.TestId);

                foreach (var questionDto in request.Request)
                {
                    maxPosition++; // Tăng maxPosition cho mỗi câu hỏi mới
                    // Tạo câu hỏi và gán Position
                    var newQuestion = new Question();
                    newQuestion.Create(request.TestId, questionDto, maxPosition);

                    // Lưu vào DB
                    await _unitOfWork.QuestionRepository.AddAsync(newQuestion);
                }
                
                await _unitOfWork.CompleteAsync();

                await transaction.CommitAsync();

                return $"Tạo câu hỏi thành công";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new AppException($"Lỗi khi tạo câu hỏi: {ex.Message}", 500);
            }
        }
    }
}
