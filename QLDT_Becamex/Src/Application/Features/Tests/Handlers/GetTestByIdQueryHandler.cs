using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Application.Features.Tests.Queries;

namespace QLDT_Becamex.Src.Application.Features.Tests.Handlers
{
    public class GetTestByIdQueryHandler : IRequestHandler<GetTestByIdQuery, DetailTestDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTestByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DetailTestDto> Handle(GetTestByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var courseExists = await _unitOfWork.CourseRepository.AnyAsync(c => c.Id == request.CourseId);
                if (!courseExists)
                {
                    throw new AppException("Khóa học không tồn tại", 404);
                }
                var test = await _unitOfWork.TestRepository.GetFlexibleAsync(
                    predicate: t => t.Id == request.Id && t.CourseId == request.CourseId,
                    orderBy: null,
                    page: null,
                    pageSize: 1, // Giới hạn 1 bản ghi
                    asNoTracking: true,
                    includes: t => t.Include(t => t.Questions)
                );

                var testEntity = test.FirstOrDefault();
                if (testEntity == null)
                {
                    throw new AppException("Bài kiểm tra không tồn tại", 404);
                }

                var testDto = _mapper.Map<DetailTestDto>(testEntity);
                return testDto;
            }
            catch (Exception ex)
            {
                throw new AppException($"Lỗi khi lấy bài kiểm tra: {ex.Message}", 500);
            }
        }
    }
}