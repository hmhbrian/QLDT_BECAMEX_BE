using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Queries;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;


namespace QLDT_Becamex.Src.Application.Features.Users.Handlers
{
    public class UserStatusHandler :
        IRequestHandler<GetAllUserStatusesQuery, IEnumerable<UserStatusDto>>,
        IRequestHandler<CreateUserStatusCommand, Unit>,
        IRequestHandler<UpdateUserStatusCommand, Unit>,
        IRequestHandler<DeleteUserStatusesCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserStatusHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserStatusDto>> Handle(GetAllUserStatusesQuery request, CancellationToken cancellationToken)
        {
            var userStatuses = await _unitOfWork.UserStatusRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserStatusDto>>(userStatuses);
        }

        public async Task<Unit> Handle(CreateUserStatusCommand request, CancellationToken cancellationToken)
        {
            var existing = await _unitOfWork.UserStatusRepository.GetFirstOrDefaultAsync(
                us => us.Name.ToLower() == request.Request.Name.ToLower());
            if (existing != null)
                throw new AppException("Trạng thái đã tồn tại", 409);

            var entity = _mapper.Map<UserStatus>(request.Request);
            await _unitOfWork.UserStatusRepository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            _mapper.Map<UserStatusDto>(entity);
            return Unit.Value;
        }

        public async Task<Unit> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.UserStatusRepository.GetByIdAsync(request.Id);
            if (entity == null)
                throw new AppException("Không tìm thấy trạng thái", 404);

            var conflict = await _unitOfWork.UserStatusRepository.GetFirstOrDefaultAsync(
                us => us.Name.ToLower() == request.Request.Name.ToLower() && us.Id != request.Id);
            if (conflict != null)
                throw new AppException("Tên trạng thái đã tồn tại", 409);

            var updateUserStatus = new UserStatus
            {
                Id = request.Id,
                Name = request.Request.Name?.Trim()!
            };
            //entity.Name = request.Request.Name;
            _unitOfWork.UserStatusRepository.Update(entity, updateUserStatus);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteUserStatusesCommand request, CancellationToken cancellationToken)
        {
            var entities = await _unitOfWork.UserStatusRepository.FindAsync(us => request.Ids.Contains(us.Id));
            if (entities == null || !entities.Any())
                throw new AppException("Không tìm thấy trạng thái cần xóa", 404);

            _unitOfWork.UserStatusRepository.RemoveRange(entities);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }
    }
}