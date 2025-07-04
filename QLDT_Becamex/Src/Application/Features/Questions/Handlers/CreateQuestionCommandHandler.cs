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

            // Kiểm tra sự tồn tại của bài kiểm tra (Test)
            var existTest = await _unitOfWork.TestRepository.GetByIdAsync(request.TestId);
            if (existTest == null)
            {
                throw new AppException("Bài kiểm tra không tồn tại", 404);  // Ném ngoại lệ nếu bài kiểm tra không tồn tại
            }

            // Tạo đối tượng Question từ DTO
            var newQuestion = new Question();
            newQuestion.Create(createQuestionDto);

            // Lưu câu hỏi vào cơ sở dữ liệu
            await _unitOfWork.QuestionRepository.AddAsync(newQuestion);
            await _unitOfWork.CompleteAsync();

            // Trả về ID của câu hỏi mới tạo hoặc một thông báo thành công
            return $"Tạo câu hỏi thành công. ID của câu hỏi mới là: {newQuestion.Id}";
        }

    }
}
