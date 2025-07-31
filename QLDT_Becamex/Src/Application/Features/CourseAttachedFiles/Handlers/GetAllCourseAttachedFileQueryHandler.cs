using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Features.CourseAttachedFile.Dtos;
using QLDT_Becamex.Src.Application.Features.CourseAttachedFiles.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace QLDT_Becamex.Src.Application.Features.CourseAttachedFiles.Handlers
{
    public class GetAllCourseAttachedFileQueryHandler :
        IRequestHandler<GetAllCourseAttachedFileQuery, List<CourseAttachedFileDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public GetAllCourseAttachedFileQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<CourseAttachedFileDto>> Handle(
            GetAllCourseAttachedFileQuery request,
            CancellationToken cancellationToken)
        {
            // 1. Lấy tất cả các file đính kèm từ repository dựa trên CourseId
            var attachedFiles = await _unitOfWork.CourseAttachedFileRepository
                .GetFlexibleAsync(
                predicate: f => f.CourseId == request.CourseId,
                includes: f => f.Include(l => l.TypeDoc)
            );

            if (attachedFiles == null || !attachedFiles.Any())
            {
                // Tùy chọn: ném lỗi nếu không tìm thấy, hoặc trả về danh sách rỗng
                // Ví dụ: throw new AppException($"Không tìm thấy file đính kèm nào cho khóa học với ID: {request.CourseId}", 404);
                return new List<CourseAttachedFileDto>(); // Trả về danh sách rỗng nếu không có
            }
            foreach (var file in attachedFiles)
            {
                Console.WriteLine($"File ID: {file.Id}, TypeDoc: {file.TypeDoc?.NameType ?? "null"}, Link: {file.Link}");
            }
            // 2. Ánh xạ các đối tượng Domain Entities sang DTO
            var attachedFileDtos = _mapper.Map<List<CourseAttachedFileDto>>(attachedFiles);

            return attachedFileDtos;
        }
    }
}