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
    public class DepartmentStatusHandler :
        IRequestHandler<GetAllDepartmentStatusesQuery, IEnumerable<StatusDto>>,
        IRequestHandler<CreateDepartmentStatusCommand, Unit>,
        IRequestHandler<UpdateDepartmentStatusCommand, Unit>,
        IRequestHandler<DeleteDepartmentStatusCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentStatusHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StatusDto>> Handle(GetAllDepartmentStatusesQuery request, CancellationToken cancellationToken)
        {
            var list = await _unitOfWork.DepartmentStatusRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<StatusDto>>(list);
        }

        public async Task<Unit> Handle(CreateDepartmentStatusCommand request, CancellationToken cancellationToken)
        {
            var existing = await _unitOfWork.DepartmentStatusRepository.GetFirstOrDefaultAsync(
                us => us.Name.ToLower() == request.Request.Name.ToLower());
            if (existing != null)
                throw new AppException("Trạng thái đã tồn tại", 409);

            var entity = new DepartmentStatus { Name = request.Request.Name };
            await _unitOfWork.DepartmentStatusRepository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }

        public async Task<Unit> Handle(UpdateDepartmentStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.DepartmentStatusRepository.GetByIdAsync(request.Id);
            if (entity == null)
                throw new AppException("Không tìm thấy trạng thái", 404);

            var conflict = await _unitOfWork.DepartmentStatusRepository.GetFirstOrDefaultAsync(
                cs => cs.Name.ToLower() == request.Request.Name.ToLower() && cs.Id != request.Id);
            if (conflict != null)
                throw new AppException("Tên trạng thái đã tồn tại", 409);

            var updateStatus = new DepartmentStatus
            {
                Id = request.Id,
                Name = request.Request.Name?.Trim()!
            };
            //entity.Name = request.Request.Name;
            _unitOfWork.DepartmentStatusRepository.Update(entity, updateStatus);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteDepartmentStatusCommand request, CancellationToken cancellationToken)
        {
            var entities = await _unitOfWork.DepartmentStatusRepository.FindAsync(cs => request.Ids.Contains(cs.Id));
            if (entities == null || !entities.Any())
                throw new AppException("Không tìm thấy trạng thái cần xóa", 404);

            _unitOfWork.DepartmentStatusRepository.RemoveRange(entities);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }
    }
}
