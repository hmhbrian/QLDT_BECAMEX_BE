using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.EmployeeLevels.Commands;
using QLDT_Becamex.Src.Application.Features.EmployeeLevels.Dtos;
using QLDT_Becamex.Src.Application.Features.EmployeeLevels.Queries;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;


namespace QLDT_Becamex.Src.Application.Features.EmployeeLevels.Handlers
{
    public class ELevelHandler :
        IRequestHandler<CreateELevelCommand, string>,
        IRequestHandler<UpdateELevelCommand, string>,
        IRequestHandler<DeleteELevelCommand, string>,
        IRequestHandler<GetAllELevelsQuery, IEnumerable<ELevelDto>>

    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ELevelHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<string> Handle(CreateELevelCommand request, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.EmployeeLevelRepository.GetFirstOrDefaultAsync(x => x.ELevelName!.ToLower() == request.Request.ELevelName.ToLower());
            if (exists != null)
                throw new AppException("Cấp bậc đã tồn tại", 409);

            var entity = _mapper.Map<EmployeeLevel>(request.Request);
            await _unitOfWork.EmployeeLevelRepository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            _mapper.Map<ELevelDto>(entity);
            return entity.ELevelId.ToString();
        }

        public async Task<string> Handle(UpdateELevelCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.EmployeeLevelRepository.GetByIdAsync(request.Id);
            if (entity == null)
                throw new AppException("Không tìm thấy cấp bậc", 404);

            var exists = await _unitOfWork.EmployeeLevelRepository.GetFirstOrDefaultAsync(x => x.ELevelName!.ToLower() == request.Request.ELevelName.ToLower() && x.ELevelId != request.Id);
            if (exists != null)
                throw new AppException("Tên cấp bậc đã tồn tại", 409);
            var updateELevel = new EmployeeLevel
            {
                ELevelId = request.Id,
                ELevelName = request.Request.ELevelName?.Trim()
            };
            //entity.PositionName = request.Request.PositionName;
            _unitOfWork.EmployeeLevelRepository.Update(entity, updateELevel);
            await _unitOfWork.CompleteAsync();

            _mapper.Map<ELevelDto>(entity);
            return entity.ELevelId.ToString();
        }

        public async Task<string> Handle(DeleteELevelCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.EmployeeLevelRepository.GetByIdAsync(request.Id);
            if (entity == null)
                throw new AppException("Không tìm thấy cấp bậc để xoá", 404);

            _unitOfWork.EmployeeLevelRepository.Remove(entity);
            await _unitOfWork.CompleteAsync();
            return entity.ELevelId.ToString()!;
        }

        public async Task<IEnumerable<ELevelDto>> Handle(GetAllELevelsQuery request, CancellationToken cancellationToken)
        {
            var list = await _unitOfWork.EmployeeLevelRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ELevelDto>>(list);
        }

    }
}