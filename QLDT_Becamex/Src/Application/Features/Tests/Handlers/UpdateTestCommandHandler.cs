using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Application.Features.Tests.Handlers
{
    public class UpdateTestCommandHandler : IRequestHandler<UpdateTestCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateTestCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<string> Handle(UpdateTestCommand command, CancellationToken cancellationToken)
        {
            var id = command.Id;
            var request = command.Request;

            // Kiểm tra Test existence
            var test = await _unitOfWork.TestRepository.GetByIdAsync(id);
            if (test == null)
            {
                throw new AppException("Bài kiểm tra không tồn tại", 404);
            }

            // Kiểm tra UserEdited existence
            var userEditedExists = await _unitOfWork.UserRepository.AnyAsync(c => c.Id == request.userId_edited);
            if (!userEditedExists)
            {
                throw new AppException("User chỉnh sửa bài không tồn tại", 404);
            }

            // Map TestUpdateDto to existing Test
            _mapper.Map(request, test);

            // Set foreign key properties
            test.UserIdEdited = request.userId_edited;
            test.UpdatedAt = DateTime.UtcNow;

            // Handle Questions
            if (request.Tests != null)
            {
                // Load existing questions
                var existingQuestions = test.Questions?.ToList() ?? new List<Question>();

                // Map new questions from request
                var newQuestions = _mapper.Map<List<Question>>(request.Tests);

                // Update test_id and UpdatedAt for new questions
                foreach (var question in newQuestions)
                {
                    question.TestId = test.Id;
                    question.CreatedAt = question.CreatedAt == default ? DateTime.UtcNow : question.CreatedAt;
                    question.UpdatedAt = DateTime.UtcNow;
                }

                // Replace existing questions with new ones
                test.Questions = newQuestions;
            }
            else
            {
                // If Tests is null, keep existing questions
                test.Questions = test.Questions ?? new List<Question>();
            }

            // Update Test in repository
            _unitOfWork.TestRepository.Update(test);

            // Save changes to database
            await _unitOfWork.CompleteAsync();

            // Return Test Id as string
            return test.Id.ToString();
        }
    }
}