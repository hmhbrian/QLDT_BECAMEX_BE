using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
namespace QLDT_Becamex.Src.Application.Features.Tests.Handlers
{
    public class CreateTestCommandHandler : IRequestHandler<CreateTestCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBaseService _baseService;

        public CreateTestCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IBaseService baseService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _baseService = baseService;
        }

        public async Task<string> Handle(CreateTestCommand command, CancellationToken cancellationToken)
        {
            var (userId, _) = _baseService.GetCurrentUserAuthenticationInfo();
            var request = command.Request;
            var courseId = command.CourseId;
            // Check course existence
            var courseExists = await _unitOfWork.CourseRepository.AnyAsync(c => c.Id == courseId);
            if (!courseExists)
            {
                throw new AppException("Khóa học không tồn tại", 404);
            }
            var userCreatedExists = await _unitOfWork.UserRepository.AnyAsync(c => c.Id == userId);
            if (!userCreatedExists)
            {
                throw new AppException("User tạo bài không tồn tại", 404);
            }
            // Map TestCreateDto to Test
            var test = _mapper.Map<Test>(request);

            // Set navigation properties
            test.CourseId = courseId;
            test.UserIdCreated = userId; // Use userId from authentication info
            test.UserIdEdited = userId;
            test.CreatedAt = DateTime.UtcNow;
            test.UpdatedAt = DateTime.UtcNow;
            // Set test_id for each Question in Tests
            if (test.Questions != null)
            {
                foreach (var question in test.Questions)
                {
                    question.CreatedAt = DateTime.UtcNow;
                    question.UpdatedAt = DateTime.UtcNow;
                }
            }

            // Add Test to repository
            await _unitOfWork.TestRepository.AddAsync(test);

            // Save changes to database
            await _unitOfWork.CompleteAsync();

            // Return Test Id as string
            return test.Id.ToString();
        }
    }
}