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
            var testsInCourse = await _unitOfWork.TestRepository.GetFlexibleAsync(
                predicate: t => t.CourseId == courseId,
                asNoTracking: true
            );
            // Kiểm tra trùng position trong cùng khóa học
            if (testsInCourse.Any(t => t.Position == test.Position && t.Id != test.Id))
            {
                throw new AppException($"Position {test.Position} đã được sử dụng bởi một bài kiểm tra khác trong khóa học này", 400);
            }

            // Xử lý title
            var ProcessTitle = request.Title.Trim();
            if (!string.IsNullOrEmpty(ProcessTitle) && ProcessTitle.StartsWith("Bài kiểm tra ") && ProcessTitle.Contains(": "))
            {
                int index = ProcessTitle.IndexOf(": ");
                ProcessTitle = ProcessTitle.Substring(index + 2).Trim(); // Lấy phần sau dấu ": "
            }
            request.Title = ProcessTitle; // Cập nhật tiêu đề đã xử lý

            // Map TestUpdateDto to existing Test
            var updateTest = _mapper.Map(request, test);

            // Set foreign key properties
            updateTest.UpdatedById = userId; // Use userId from authentication info
            updateTest.UpdatedAt = DateTime.UtcNow;
            // Update Test in repository
            _unitOfWork.TestRepository.Update(test, updateTest);

            // Save changes to database
            await _unitOfWork.CompleteAsync();

            // Return Test Id as string
            return test.Id.ToString();
        }
    }
}