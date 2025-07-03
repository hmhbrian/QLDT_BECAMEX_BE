using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Lessons.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Lessons.Handlers
{
    public class GetLessonByIdQueryHandler : IRequestHandler<GetLessonByIdQuery, DetailLessonDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetLessonByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<DetailLessonDto> Handle(GetLessonByIdQuery request, CancellationToken cancellationToken)
        {
            // Validate Id
            if (request.Id <= 0)
            {
                throw new AppException("Bài học không tồn tại", 404);
            }
            var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(request.Id);
            if (lesson == null)
                throw new AppException("Không tìm thấy bài học này", 404);
            var dto = _mapper.Map<DetailLessonDto>(lesson);
            return dto;
        }
    }
}
