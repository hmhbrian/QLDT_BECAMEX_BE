using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Application.Features.Feedbacks.Queries;
using QLDT_Becamex.Src.Application.Features.Feedbacks.Dtos;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Domain.Entities;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace QLDT_Becamex.Src.Application.Features.Tests.Handlers
{
    public class GetListFeedbackQueryHandler : IRequestHandler<GetListFeedbackQuery, List<FeedbacksDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public GetListFeedbackQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<List<FeedbacksDto>> Handle(GetListFeedbackQuery request, CancellationToken cancellationToken)
        {
            // Validate CourseId
            var (userId, role) = _userService.GetCurrentUserAuthenticationInfo();
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
            {
                throw new AppException("Khóa học không tồn tại", 404);
            }
            Expression<Func<Feedback, bool>>? predicate = f => f.Course != null;
            if (role == ConstantRole.HOCVIEN)
            {
                predicate = predicate.And(f => f.UserId == userId && f.CourseId == request.CourseId);
            }
            else
            {
                predicate = predicate.And(f => f.CourseId == request.CourseId);
            }
            var feedbacks = await _unitOfWork.FeedbackRepository.GetFlexibleAsync(
                predicate: predicate,
                orderBy: null,
                includes: f => f.Include(p => p.User)
            );
            var dto = _mapper.Map<List<FeedbacksDto>>(feedbacks);

            foreach (var feedback in dto)
            {
                // Calculate average score
                feedback.averageScore = (feedback.Q1_relevance + feedback.Q2_clarity + feedback.Q3_structure +
                                          feedback.Q4_duration + feedback.Q5_material) / 5.0f;
            }

            return dto;
        }
    }
}