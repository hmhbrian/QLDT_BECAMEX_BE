using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Lessons.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Lessons.Handlers
{
    public class GetListLessonOfCourseQueryHandler : IRequestHandler<GetListLessonOfCourseQuery, List<AllLessonDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetListLessonOfCourseQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<AllLessonDto>> Handle(GetListLessonOfCourseQuery request, CancellationToken cancellationToken)
        {
            // Validate CourseId
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
            {
                throw new AppException("Khóa học không tồn tại", 404);
            }
            var lessons = await _unitOfWork.LessonRepository.GetFlexibleAsync(
                predicate: l => l.CourseId == request.CourseId,
                orderBy: q => q.OrderBy(l => l.Position)
            );
            if (lessons == null || !lessons.Any())
                throw new AppException("Không tìm thấy bài học nào cho khóa học này", 404);

            var dto = _mapper.Map<List<AllLessonDto>>(lessons);
            return dto;
        }
    }
}
