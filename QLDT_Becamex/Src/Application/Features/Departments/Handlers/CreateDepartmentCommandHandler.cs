using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Departments.Commands;
using QLDT_Becamex.Src.Application.Features.Departments.Dtos;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services.DepartmentServices;



namespace QLDT_Becamex.Src.Application.Features.Departments.Handlers
{
    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDepartmentService _departmentService;

        public CreateDepartmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDepartmentService departmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _departmentService = departmentService;

        }
        public async Task<string> Handle(CreateDepartmentCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            // Kiểm tra tên phòng ban đã tồn tại chưa
            var nameExists = await _unitOfWork.DepartmentRepository.AnyAsync(d => d.DepartmentName == request.DepartmentName || d.DepartmentCode == request.DepartmentCode);
            if (nameExists)
                throw new AppException("Tên phòng ban hoặc mã phòng ban đã tồn tại", 409);

            if (request.StatusId.HasValue)
            {
                var statusExists = await _unitOfWork.DepartmentStatusRepository.AnyAsync(s => s.Id == request.StatusId.Value);
                if (!statusExists)
                    throw new AppException("Trạng thái phòng ban không hợp lệ", 400);
            }

            // Kiểm tra và xử lý ParentId
            int? parentId = request.ParentId == 0 ? null : request.ParentId;
            int calculatedLevel = 1;
            if (parentId != null)
            {
                var parent = await _unitOfWork.DepartmentRepository.GetByIdAsync(parentId.Value);
                if (parent != null)
                    calculatedLevel = parent.Level + 1;
            }

            // Kiểm tra ManagerId
            await _departmentService.ValidateManagerIdDeparmentAsync(request.ManagerId, true, null, null);


            var department = _mapper.Map<Department>(request);
            department.ParentId = parentId;
            department.ManagerId = request.ManagerId?.Trim();
            department.Level = calculatedLevel;
            department.CreatedAt = DateTime.Now;
            department.UpdatedAt = DateTime.Now;

            await _unitOfWork.DepartmentRepository.AddAsync(department);
            await _unitOfWork.CompleteAsync();

            var resultDto = _mapper.Map<DepartmentDto>(department);
            resultDto.DepartmentId = department.DepartmentId;

            if (department.ParentId != null)
            {
                var parent = await _unitOfWork.DepartmentRepository.GetByIdAsync(department.ParentId.Value);
                resultDto.ParentName = parent?.DepartmentName;
            }

            if (!string.IsNullOrEmpty(department.ManagerId))
            {
                var manager = await _unitOfWork.UserRepository.GetByIdAsync(department.ManagerId);
                resultDto.ManagerName = manager?.FullName;
            }

            return department.DepartmentId.ToString();
        }
    }
}
