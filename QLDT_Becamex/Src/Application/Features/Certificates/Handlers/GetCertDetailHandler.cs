using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Certificates.Dtos;
using QLDT_Becamex.Src.Application.Features.Certificates.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace QLDT_Becamex.Src.Application.Features.Certificates.Handlers
{
    public class GetCertDetailHandler : IRequestHandler<GetDetailCertQuery, CertDetailDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public GetCertDetailHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<CertDetailDto> Handle(GetDetailCertQuery request, CancellationToken cancellationToken)
        {
            var (currentUserId, role) = _userService.GetCurrentUserAuthenticationInfo();
           
            // Validate Course
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
                throw new AppException("Khóa học không tồn tại", 404);

            // Lấy list chứng chỉ thuộc course
            var certs = await _unitOfWork.CertificatesRepository.GetFirstOrDefaultAsync(predicate: c => c.CourseId == course.Id && c.UserId == currentUserId,
                  includes: q => q.Include(d => d.User).Include(d => d.Course));

            // Map sang DTO
            var dto = _mapper.Map<CertDetailDto>(certs);

            return dto;
        }
    }
}
