using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.TypeDocument.Commands;
using QLDT_Becamex.Src.Application.Features.TypeDocument.Dtos;
using QLDT_Becamex.Src.Application.Features.TypeDocument.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Application.Features.TypeDocument.Handlers
{
    public class TypeDocumentHandler : IRequestHandler<CreateTypeDocumentCommand, Unit>,
      IRequestHandler<UpdateTypeDocumentCommand, Unit>,
      IRequestHandler<DeleteTypeDocumentCommand, Unit>,
       IRequestHandler<GetAlTypeDocumentQuery, IEnumerable<TypeDocumentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TypeDocumentHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(CreateTypeDocumentCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Request.NameType))
            {
                throw new AppException("Tên danh mục không được để trống", 400);
            }
            var normalizedName = request.Request.NameType.Trim().ToLower();

            var existingCategory = await _unitOfWork.TypeDocumentRepository.GetFirstOrDefaultAsync(c => c.NameType!.ToLower() == normalizedName);
            if (existingCategory != null)
            {
                throw new AppException("Danh mục khóa học đã tồn tại", 409);
            }

            var TypeDocument = _mapper.Map<Domain.Entities.TypeDocument>(request.Request);

            TypeDocument.NameType = request.Request.NameType?.Trim()!;
            TypeDocument.Key = request.Request.Key;

            await _unitOfWork.TypeDocumentRepository.AddAsync(TypeDocument);
            await _unitOfWork.CompleteAsync();
            _mapper.Map<TypeDocumentDto>(TypeDocument);
            return Unit.Value;
        }

        public async Task<Unit> Handle(UpdateTypeDocumentCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.TypeDocumentRepository.GetByIdAsync(request.id);
            if (entity == null)
                throw new AppException("Không tìm thấy loại khóa học", 404);

            if (string.IsNullOrWhiteSpace(request.Request.NameType))
            {
                throw new AppException("Tên danh mục không được để trống", 400);
            }

            var normalizedName = request.Request.NameType.Trim().ToLower();

            var existingCategory = await _unitOfWork.TypeDocumentRepository.GetFirstOrDefaultAsync(c => c.NameType!.ToLower() == normalizedName && c.Id != request.id);
            if (existingCategory != null)
            {
                throw new AppException("Danh mục khóa học đã tồn tại", 409);
            }
            var updateTypeDocument = new Domain.Entities.TypeDocument
            {
                Id = request.id,
                NameType = request.Request.NameType?.Trim()!,
                Key = request.Request.Key
            };
            //entity.NameType = request.Request.NameType?.Trim()!;
            _unitOfWork.TypeDocumentRepository.Update(entity, updateTypeDocument);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteTypeDocumentCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.TypeDocumentRepository.FindAsync(l => request.Ids.Contains(l.Id));

            if (category == null || !category.Any())
                throw new AppException("Không tìm thấy loại khóa học nào với ID đã cho", 404);

            _unitOfWork.TypeDocumentRepository.RemoveRange(category);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }

        public async Task<IEnumerable<TypeDocumentDto>> Handle(GetAlTypeDocumentQuery request, CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.TypeDocumentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TypeDocumentDto>>(categories);
        }
    }
}
