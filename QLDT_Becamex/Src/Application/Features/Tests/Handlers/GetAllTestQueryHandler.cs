using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Application.Features.Tests.Handlers
{
    public class GetAllTestQueryHandler : IRequestHandler<GetAllTestQuery, List<TestDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllTestQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<TestDto>> Handle(GetAllTestQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var tests = await _unitOfWork.TestRepository.GetFlexibleAsync(
                    predicate: null,
                    orderBy: null,
                    page: null,
                    pageSize: null,
                    asNoTracking: true,
                    includes: t => t.Include(t => t.Questions)
                );

                var testDtos = _mapper.Map<List<TestDto>>(tests);
                return testDtos;
            }
            catch (Exception)
            {
                throw new AppException("Vui lòng thử lại sau", 500);
            }
        }
    }
}