using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.CourseCategory.Dtos;
using QLDT_Becamex.Src.Application.Features.CourseCategory.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Application.Features.CourseCategory.Commands;

namespace QLDT_Becamex.Src.Application.Features.CourseCategory.Handlers
{
    public class CourseCategoryHandler : IRequestHandler<CreateCourseCategoryCommand, Unit>,
      IRequestHandler<UpdateCourseCategoryCommand, Unit>,
      IRequestHandler<DeleteCourseCategoryCommand, Unit>,
       IRequestHandler<GetAlCourseCategoryQuery, IEnumerable<CourseCategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CourseCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(CreateCourseCategoryCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Request.Name))
            {
                throw new AppException("Tên danh mục không được để trống", 400);
            }
            var normalizedName = request.Request.Name.Trim().ToLower();

            var existingCategory = await _unitOfWork.CourseCategoryRepository.GetFirstOrDefaultAsync(c => c.CategoryName!.ToLower() == normalizedName);
            if (existingCategory != null)
            {
                throw new AppException("Danh mục khóa học đã tồn tại", 409);
            }

            var courseCategory = _mapper.Map<Domain.Entities.CourseCategory>(request.Request);

            courseCategory.CategoryName = request.Request.Name?.Trim();
            courseCategory.Description = request.Request.Description?.Trim();

            await _unitOfWork.CourseCategoryRepository.AddAsync(courseCategory);
            await _unitOfWork.CompleteAsync();
            _mapper.Map<CourseCategoryDto>(courseCategory);
            return Unit.Value;
        }

        public async Task<Unit> Handle(UpdateCourseCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.CourseCategoryRepository.GetByIdAsync(request.id);
            if (entity == null)
                throw new AppException("Không tìm thấy loại khóa học", 404);

            if (string.IsNullOrWhiteSpace(request.Request.Name))
            {
                throw new AppException("Tên danh mục không được để trống", 400);
            }

            var normalizedName = request.Request.Name.Trim().ToLower();

            var existingCategory = await _unitOfWork.CourseCategoryRepository.GetFirstOrDefaultAsync(c => c.CategoryName!.ToLower() == normalizedName && c.Id != request.id);
            if (existingCategory != null)
            {
                throw new AppException("Danh mục khóa học đã tồn tại", 409);
            }

            var updateCategory = new Domain.Entities.CourseCategory
            {
                Id = request.id,
                CategoryName = request.Request.Name?.Trim(),
                Description = request.Request.Description?.Trim()
            };

            _unitOfWork.CourseCategoryRepository.Update(entity, updateCategory);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteCourseCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.CourseCategoryRepository.FindAsync(l => request.Ids.Contains(l.Id));

            if (category == null || !category.Any())
                throw new AppException("Không tìm thấy loại khóa học nào với ID đã cho", 404);

            _unitOfWork.CourseCategoryRepository.RemoveRange(category);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }

        public async Task<IEnumerable<CourseCategoryDto>> Handle(GetAlCourseCategoryQuery request, CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.CourseCategoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CourseCategoryDto>>(categories);
        }
    }
}
