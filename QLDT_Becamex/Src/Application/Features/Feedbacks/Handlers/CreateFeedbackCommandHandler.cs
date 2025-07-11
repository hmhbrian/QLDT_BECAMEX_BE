using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Feedbacks.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
namespace QLDT_Becamex.Src.Application.Features.Feedbacks.Handlers
{
    public class CreateFeedbackCommandHandler : IRequestHandler<CreateFeedbackCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public CreateFeedbackCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<string> Handle(CreateFeedbackCommand command, CancellationToken cancellationToken)
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
            var userCreatedExists = await _unitOfWork.UserRepository.AnyAsync(c => c.Id == userId);
            if (!userCreatedExists)
            {
                throw new AppException("User đánh giá không tồn tại", 404);
            }
            var userCourseExists = await _unitOfWork.UserCourseRepository
                .AnyAsync(uc => uc.CourseId == courseId && uc.UserId == userId);
            if (!userCourseExists)
            {
                throw new AppException("User chưa đăng ký khóa học này", 400);
            }
            // Check if feedback already exists for this user and course
            var feedbackExists = await _unitOfWork.FeedbackRepository
            .AnyAsync(f => f.CourseId == courseId && f.UserId == userId);

            if (feedbackExists)
            {
                throw new AppException("Bạn đã đánh giá khóa học này rồi", 400);
            }
            var feedback = _mapper.Map<Feedback>(request);

            // Set navigation properties
            feedback.CourseId = courseId;
            feedback.UserId = userId;
            feedback.SubmissionDate = DateTime.UtcNow;

            // Add Feedback to repository
            await _unitOfWork.FeedbackRepository.AddAsync(feedback);

            // Save changes to database
            await _unitOfWork.CompleteAsync();

            // Return Test Id as string
            return feedback.Id.ToString();
        }
    }
}