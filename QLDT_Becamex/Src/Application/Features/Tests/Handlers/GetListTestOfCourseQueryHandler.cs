using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;
using static QLDT_Becamex.Src.Application.Features.Tests.Dtos.TestReponseDTO;

namespace QLDT_Becamex.Src.Application.Features.Tests.Handlers
{
    public class GetListTestOfCourseQueryHandler : IRequestHandler<GetListTestOfCourseQuery, List<AllTestDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetListTestOfCourseQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<AllTestDto>> Handle(GetListTestOfCourseQuery request, CancellationToken cancellationToken)
        {
            // Validate CourseId
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
            {
                throw new AppException("Khóa học không tồn tại", 404);
            }

            var tests = await _unitOfWork.TestRepository.GetFlexibleAsync(
                predicate: t => t.CourseId == request.CourseId,
                includes: q => q
                        .Include(d => d.Questions)
            );

            if (tests == null || !tests.Any())
                throw new AppException("Không tìm thấy bài kiểm tra nào cho khóa học này", 404);
            var dto = _mapper.Map<List<AllTestDto>>(tests);

            return dto;
        }
    }
}
