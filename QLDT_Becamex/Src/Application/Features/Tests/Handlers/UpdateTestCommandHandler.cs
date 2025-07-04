using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;

namespace QLDT_Becamex.Src.Application.Features.Tests.Handlers
{
    public class UpdateTestCommandHandler : IRequestHandler<UpdateTestCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public UpdateTestCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<string> Handle(UpdateTestCommand command, CancellationToken cancellationToken)
        {
            var id = command.Id;
            var request = command.Request;
            var courseId = command.CourseId;
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
            // Kiểm tra Course existence
            var courseExists = await _unitOfWork.CourseRepository.AnyAsync(c => c.Id == courseId);
            if (!courseExists) 
            {
                throw new AppException("Khóa học không tồn tại", 404);
            }
            // Kiểm tra Test existence
            var test = await _unitOfWork.TestRepository.GetByIdAsync(id);
            if (test == null || test.CourseId != courseId)
            {
                throw new AppException("Bài kiểm tra không tồn tại", 404);
            }

            // Kiểm tra UserEdited existence
            var userEditedExists = await _unitOfWork.UserRepository.AnyAsync(c => c.Id == userId);
            if (!userEditedExists)
            {
                throw new AppException("User chỉnh sửa bài không tồn tại", 404);
            }

            // Map TestUpdateDto to existing Test
            _mapper.Map(request, test);

            // Set foreign key properties
            test.UserIdEdited = userId; // Use userId from authentication info
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