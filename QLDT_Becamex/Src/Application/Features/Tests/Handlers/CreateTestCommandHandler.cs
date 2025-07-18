using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Commands;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
namespace QLDT_Becamex.Src.Application.Features.Tests.Handlers
{
    public class CreateTestCommandHandler : IRequestHandler<CreateTestCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public CreateTestCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<string> Handle(CreateTestCommand command, CancellationToken cancellationToken)
        {
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
            var request = command.Request;
            var courseId = command.CourseId;
            // Check course existence
            var courseExists = await _unitOfWork.CourseRepository.AnyAsync(c => c.Id == courseId);
            if (!courseExists)
            {
                throw new AppException("Khóa học không tồn tại", 404);
            }
            // get max test position in course
            var testsInCourse = await _unitOfWork.TestRepository.GetFlexibleAsync(
                predicate: t => t.CourseId == courseId,
                asNoTracking: true
            );
            int maxPosition = testsInCourse.Any()
                ? testsInCourse.Max(t => t.Position)
                : 0;
            // Check if user exists
            var userCreatedExists = await _unitOfWork.UserRepository.AnyAsync(c => c.Id == userId);
            if (!userCreatedExists)
            {
                throw new AppException("User tạo bài không tồn tại", 404);
            }
            // Map TestCreateDto to Test
            var test = _mapper.Map<Test>(request);
            test.Create(courseId, userId!, request, maxPosition + 1);

            // Set test_id for each Question in Tests
            if (test.Questions != null)
            {
                int index = 1;
                foreach (var question in test.Questions)
                {
                    question.CreatedAt = DateTime.UtcNow;
                    question.UpdatedAt = DateTime.UtcNow;
                    question.Position = index++;
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