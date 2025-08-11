// Handlers/GetCourseQueryHandler.cs
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers
{
    public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, CourseDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCourseByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CourseDto> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            var course = await _unitOfWork.CourseRepository.GetFirstOrDefaultAsync(
                predicate: c => c.Id == request.Id,
                includes: q => q
                    .Include(c => c.CourseDepartments)!.ThenInclude(cd => cd.Department)
                    .Include(c => c.CourseELevels)!.ThenInclude(cp => cp.ELevel)
                    .Include(c => c.UserCourses)!.ThenInclude(uc => uc.User)
                    .Include(c => c.Status)
                    .Include(c => c.Category)
                    .Include(c => c.CreateBy)
                    .Include(c => c.UpdateBy)
            );

            if (course == null)
                throw new AppException("Khóa học không tồn tại", 404);

            var dto = _mapper.Map<CourseDto>(course);

            return dto;
        }
    }
}
