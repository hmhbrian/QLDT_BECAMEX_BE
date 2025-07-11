using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Questions.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Features.Questions.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Entities;
using System.Linq.Expressions;

namespace QLDT_Becamex.Src.Application.Features.Questions.Handlers
{
    public class GetListQuestionQueryHandler : IRequestHandler<GetListQuestionQuery, PagedResult<QuestionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetListQuestionQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<QuestionDto>> Handle(GetListQuestionQuery request, CancellationToken cancellationToken)
        {
            var existTest = await _unitOfWork.TestRepository.GetByIdAsync(request.TestId);
            if (existTest == null)
            {
                throw new AppException("Bài kiểm tra không tồn tại", 404);  // Ném ngoại lệ nếu bài kiểm tra không tồn tại
            }

            var predicate = (Expression<Func<Question, bool>>)(q => q.TestId == request.TestId);
            var orderBy = (Func<IQueryable<Question>, IOrderedQueryable<Question>>)(q => q.OrderByDescending(x => x.CreatedAt));

            // Lấy danh sách câu hỏi đã phân trang, sắp xếp
            // var questions = await _unitOfWork.QuestionRepository.GetFlexibleAsync(
            //     predicate: predicate,
            //     orderBy: orderBy,
            //     page: request.BaseQueryParam.Page,
            //     pageSize: request.BaseQueryParam.Limit,
            //     asNoTracking: true
            // );
            
            // Lấy danh sách câu hỏi không phân trang
            var questions = await _unitOfWork.QuestionRepository.GetFlexibleAsync(
                predicate: predicate,
                orderBy: null,
                page: null,
                pageSize: null,
                asNoTracking: true
            );
            // Đếm tổng câu hỏi (không phân trang)
            var totalCount = await _unitOfWork.QuestionRepository.CountAsync(predicate);

            // Mapping thủ công vì không dùng được ProjectTo khi đã ToList xong
            var items = questions.Select(q => _mapper.Map<QuestionDto>(q)).ToList();

            var pagination = new Pagination(request.BaseQueryParam.Page, request.BaseQueryParam.Limit, totalCount);

            var pageResult = new PagedResult<QuestionDto>(items, pagination);
            return pageResult;
        }

    }
}
