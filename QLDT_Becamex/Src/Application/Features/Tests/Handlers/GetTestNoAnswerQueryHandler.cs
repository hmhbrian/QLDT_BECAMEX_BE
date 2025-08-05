using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using AutoMapper;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Entities;
using System.Linq.Expressions;
using QLDT_Becamex.Src.Application.Features.Tests.Queries;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Questions.Handlers
{
    public class GetTestNoAnswerQueryHandler : IRequestHandler<GetTestNoAnswerQuery, List<QuestionNoAnswerDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTestNoAnswerQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<QuestionNoAnswerDto>> Handle(GetTestNoAnswerQuery request, CancellationToken cancellationToken)
        {
            var existTest = await _unitOfWork.TestRepository.GetFirstOrDefaultAsync(t => t.Id == request.TestId && t.CourseId == request.courseId);
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
            var items = questions.Select(q => _mapper.Map<QuestionNoAnswerDto>(q)).ToList();

            return items;
        }

    }
}
