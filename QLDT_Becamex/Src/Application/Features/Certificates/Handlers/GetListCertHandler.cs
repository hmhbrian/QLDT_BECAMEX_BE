using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Features.Certificates.Dtos;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Application.Features.Certificates.Queries;
using QLDT_Becamex.Src.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace QLDT_Becamex.Src.Application.Features.Certificates.Handlers
{
    public class GetListCertHandler : IRequestHandler<GetListCertQuery, List<CertListDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;


        public GetListCertHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<List<CertListDto>> Handle(GetListCertQuery request, CancellationToken cancellationToken)
        {
            var (currentUserId, _) = _userService.GetCurrentUserAuthenticationInfo();

            var certs = await _unitOfWork.CertificatesRepository.GetFlexibleAsync(predicate: c => c.UserId == currentUserId,
                includes: q => q
                .Include(d => d.User)
                .Include(d => d.Course));

            return _mapper.Map<List<CertListDto>>(certs);
        }
    }
}
