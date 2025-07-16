using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Questions.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Persistence;

namespace QLDT_Becamex.Src.Application.Features.Questions.Handlers
{
    public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateQuestionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
        {
            // Kiểm tra sự tồn tại của bài kiểm tra (Test)
            var existTest = await _unitOfWork.TestRepository.GetByIdAsync(request.TestId);
            if (existTest == null)
            {
                throw new AppException("Bài kiểm tra không tồn tại", 404);  // Ném ngoại lệ nếu bài kiểm tra không tồn tại
            }
            // Tìm câu hỏi cần cập nhật trong cơ sở dữ liệu
            var existingQuestion = await _unitOfWork.QuestionRepository.GetFirstOrDefaultAsync(q => q.Id == request.QuestionId && q.TestId == existTest.Id);
            if (existingQuestion == null)
            {
                throw new AppException("Câu hỏi không tồn tại", 404);  // Ném ngoại lệ nếu không tìm thấy câu hỏi
            }

            // Cập nhật câu hỏi từ DTO
            //existingQuestion.Update(request.Request);  // Gọi phương thức Update trên entity
            existingQuestion.Update(request.Request);  // Cập nhật các trường từ DTO vào entity
            await _unitOfWork.CompleteAsync();

            // Trả về thông báo thành công
            return "Cập nhật câu hỏi thành công";
        }

    }
}
