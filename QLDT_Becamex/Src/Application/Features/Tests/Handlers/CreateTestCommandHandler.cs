using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
namespace QLDT_Becamex.Src.Application.Features.Tests.Handlers
{
    public class CreateTestCommandHandler : IRequestHandler<CreateTestCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateTestCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<string> Handle(CreateTestCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            // Check course existence
            var courseExists = await _unitOfWork.CourseRepository.AnyAsync(c => c.Id == request.course_id);
            if (!courseExists)
            {
                throw new AppException("Khóa học không tồn tại", 404);
            }
            var userCreatedExists = await _unitOfWork.UserRepository.AnyAsync(c => c.Id == request.userId_created);
            if (!userCreatedExists)
            {
                throw new AppException("User tạo bài không tồn tại", 404);
            }
            // Map TestCreateDto to Test
            var test = _mapper.Map<Test>(request);

            // Set navigation properties
            test.course_id = request.course_id;
            test.userId_created = request.userId_created;
            test.userId_edited = request.userId_created;
            // Set test_id for each Question in Tests
            if (test.Tests != null)
            {
                foreach (var question in test.Tests)
                {
                    question.test_id = 0; // Will be updated after saving
                    question.CreatedAt = DateTime.UtcNow;
                    question.UpdatedAt = DateTime.UtcNow;
                }
            }

            // Add Test to repository
            await _unitOfWork.TestRepository.AddAsync(test);

            // Save changes to database
            await _unitOfWork.CompleteAsync();

            // Update test_id for Questions after Test.Id is generated
            if (test.Tests != null)
            {
                foreach (var question in test.Tests)
                {
                    question.test_id = test.Id;
                }
                await _unitOfWork.CompleteAsync();
            }

            // Return Test Id as string
            return test.Id.ToString();
        }
    }
}