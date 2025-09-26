using MediatR;
using QLDT_Becamex.Src.Application.Features.Reports.Dtos;
using QLDT_Becamex.Src.Application.Features.Reports.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Application.Features.Reports.Handlers
{
    public class GetListCourseAndAvgFeedbackQueryHandler : IRequestHandler<GetListCourseAndAvgFeedbackQuery, List<CourseAndAvgFeedbackDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetListCourseAndAvgFeedbackQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CourseAndAvgFeedbackDto>> Handle(GetListCourseAndAvgFeedbackQuery request, CancellationToken cancellationToken)
        {
            var courses = await _unitOfWork.CourseRepository.GetFlexibleAsync(
                c => !c.IsDeleted,
                orderBy: null,
                asNoTracking: true
            );
            if (courses == null || !courses.Any())
            {
                return new List<CourseAndAvgFeedbackDto>();
            }
            var courseAndAvgFeedbackDtos = new List<CourseAndAvgFeedbackDto>();
            foreach (var course in courses)
            {
                var feedbacks = await _unitOfWork.FeedbackRepository.GetFlexibleAsync(f => f.CourseId == course.Id);
                if (feedbacks.Count() == 0)
                {
                    continue;
                }
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

                var courseAndAvgFeedbackDto = new CourseAndAvgFeedbackDto();
                courseAndAvgFeedbackDto.courseName = course.Name;
                courseAndAvgFeedbackDto.AvgFeedback = avgFeedback;
                courseAndAvgFeedbackDtos.Add(courseAndAvgFeedbackDto);
            }
            courseAndAvgFeedbackDtos.Sort((x, y) => y.avgFeedbackScore.CompareTo(x.avgFeedbackScore));
            return courseAndAvgFeedbackDtos;
        }
    }
}