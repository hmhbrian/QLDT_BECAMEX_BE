using MediatR;
using QLDT_Becamex.Src.Application.Features.Reports.Dtos;
using QLDT_Becamex.Src.Application.Features.Reports.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Application.Features.Reports.Handlers
{
    public class GetAvgFeedbackQueryHandler : IRequestHandler<GetAvgFeedbackQuery, AvgFeedbackDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAvgFeedbackQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<AvgFeedbackDto> Handle(GetAvgFeedbackQuery request, CancellationToken cancellationToken)
        {
            var feedbacks = await _unitOfWork.FeedbackRepository.GetAllAsync();
            var avgFeedback = new AvgFeedbackDto();

            foreach (var feedback in feedbacks)
            {
                avgFeedback.Q1_relevanceAvg += feedback.Q1_relevance;
                avgFeedback.Q2_clarityAvg += feedback.Q2_clarity;
                avgFeedback.Q3_structureAvg += feedback.Q3_structure;
                avgFeedback.Q4_durationAvg += feedback.Q4_duration;
                avgFeedback.Q5_materialAvg += feedback.Q5_material;
            }
            avgFeedback.Q1_relevanceAvg /= feedbacks.Count();
            avgFeedback.Q2_clarityAvg /= feedbacks.Count();
            avgFeedback.Q3_structureAvg /= feedbacks.Count();
            avgFeedback.Q4_durationAvg /= feedbacks.Count();
            avgFeedback.Q5_materialAvg /= feedbacks.Count();

            return avgFeedback;
        }
    }
}