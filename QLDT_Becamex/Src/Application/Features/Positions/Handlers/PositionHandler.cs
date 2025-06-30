using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Positions.Commands;
using QLDT_Becamex.Src.Application.Features.Positions.Dtos;
using QLDT_Becamex.Src.Application.Features.Positions.Queries;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;


namespace QLDT_Becamex.Src.Application.Features.Positions.Handlers
{
    public class PositionHandler :
        IRequestHandler<CreatePositionCommand, string>,
        IRequestHandler<UpdatePositionCommand, string>,
        IRequestHandler<DeletePositionCommand, string>,
        IRequestHandler<GetAllPositionsQuery, IEnumerable<PositionDto>>

    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PositionHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<string> Handle(CreatePositionCommand request, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.PositionRepository.GetFirstOrDefaultAsync(x => x.PositionName!.ToLower() == request.Request.PositionName.ToLower());
            if (exists != null)
                throw new AppException("Vị trí đã tồn tại", 409);

            var entity = _mapper.Map<Position>(request.Request);
            await _unitOfWork.PositionRepository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            _mapper.Map<PositionDto>(entity);
            return exists.PositionId.ToString();
        }

        public async Task<string> Handle(UpdatePositionCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.PositionRepository.GetByIdAsync(request.Id);
            if (entity == null)
                throw new AppException("Không tìm thấy vị trí", 404);

            var exists = await _unitOfWork.PositionRepository.GetFirstOrDefaultAsync(x => x.PositionName!.ToLower() == request.Request.PositionName.ToLower() && x.PositionId != request.Id);
            if (exists != null)
                throw new AppException("Tên vị trí đã tồn tại", 409);

            entity.PositionName = request.Request.PositionName;
            _unitOfWork.PositionRepository.Update(entity);
            await _unitOfWork.CompleteAsync();

            _mapper.Map<PositionDto>(entity);
            return exists.PositionId.ToString();
        }

        public async Task<string> Handle(DeletePositionCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.PositionRepository.GetByIdAsync(request.Id);
            if (entity == null)
                throw new AppException("Không tìm thấy vị trí để xoá", 404);

            _unitOfWork.PositionRepository.Remove(entity);
            await _unitOfWork.CompleteAsync();
            return entity.PositionId.ToString()!;
        }

        public async Task<IEnumerable<PositionDto>> Handle(GetAllPositionsQuery request, CancellationToken cancellationToken)
        {
            var list = await _unitOfWork.PositionRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PositionDto>>(list);
        }

    }
}