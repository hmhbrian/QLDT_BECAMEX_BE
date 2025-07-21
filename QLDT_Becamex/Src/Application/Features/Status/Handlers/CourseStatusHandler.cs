using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Status.Commands;
using QLDT_Becamex.Src.Application.Features.Status.Dtos;
using QLDT_Becamex.Src.Application.Features.Status.Queries;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Application.Features.Status.Handlers
{
    public class CourseStatusHandler :
        IRequestHandler<GetAllCourseStatusesQuery, IEnumerable<StatusDto>>,
        IRequestHandler<CreateCourseStatusCommand, Unit>,
        IRequestHandler<UpdateCourseStatusCommand, Unit>,
        IRequestHandler<DeleteCourseStatusesCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CourseStatusHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StatusDto>> Handle(GetAllCourseStatusesQuery request, CancellationToken cancellationToken)
        {
            var list = await _unitOfWork.CourseStatusRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<StatusDto>>(list);
        }

        public async Task<Unit> Handle(CreateCourseStatusCommand request, CancellationToken cancellationToken)
        {
            var existing = await _unitOfWork.CourseStatusRepository.GetFirstOrDefaultAsync(
                us => us.StatusName.ToLower() == request.Request.Name.ToLower());
            if (existing != null)
                throw new AppException("Trạng thái đã tồn tại", 409);

            var entity = new CourseStatus { StatusName = request.Request.Name };
            await _unitOfWork.CourseStatusRepository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }

        public async Task<Unit> Handle(UpdateCourseStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.CourseStatusRepository.GetByIdAsync(request.Id);
            if (entity == null)
                throw new AppException("Không tìm thấy trạng thái", 404);

            var conflict = await _unitOfWork.CourseStatusRepository.GetFirstOrDefaultAsync(
                cs => cs.StatusName.ToLower() == request.Request.Name.ToLower() && cs.Id != request.Id);
            if (conflict != null)
                throw new AppException("Tên trạng thái đã tồn tại", 409);

            var updateStatus = new CourseStatus
            {
                Id = request.Id,
                StatusName = request.Request.Name?.Trim()!
            };
            _unitOfWork.CourseStatusRepository.Update(entity, updateStatus);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteCourseStatusesCommand request, CancellationToken cancellationToken)
        {
            var entities = await _unitOfWork.CourseStatusRepository.FindAsync(cs => request.Ids.Contains(cs.Id));
            if (entities == null || !entities.Any())
                throw new AppException("Không tìm thấy trạng thái cần xóa", 404);

            _unitOfWork.CourseStatusRepository.RemoveRange(entities);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }
    }
}