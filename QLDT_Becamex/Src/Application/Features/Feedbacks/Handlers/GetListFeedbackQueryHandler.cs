using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Application.Features.Feedbacks.Queries;
using QLDT_Becamex.Src.Application.Features.Feedbacks.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Tests.Handlers
{
    public class GetListFeedbackQueryHandler : IRequestHandler<GetListFeedbackQuery, List<FeedbacksDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetListFeedbackQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<FeedbacksDto>> Handle(GetListFeedbackQuery request, CancellationToken cancellationToken)
        {
            // Validate CourseId
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
            {
                throw new AppException("Khóa học không tồn tại", 404);
            }
            
            var feedbacks = await _unitOfWork.FeedbackRepository.GetFlexibleAsync(
                predicate: t => t.CourseId == request.CourseId,
                orderBy: null
            );

            var dto = _mapper.Map<List<FeedbacksDto>>(feedbacks);

            return dto;
        }
    }
}
