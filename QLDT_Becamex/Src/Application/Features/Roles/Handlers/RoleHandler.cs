
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Roles.Commands;
using QLDT_Becamex.Src.Application.Features.Roles.Dtos;
using QLDT_Becamex.Src.Application.Features.Roles.Queries;


namespace QLDT_Becamex.Src.Application.Features.Roles.Handlers
{
    public class RoleHandler :
        IRequestHandler<CreateRoleCommand, string>,
        IRequestHandler<UpdateRoleCommand, string>,
        IRequestHandler<DeleteRoleCommand, string>,
        IRequestHandler<GetAllRolesQuery, IEnumerable<RoleDto>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public RoleHandler(RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<string> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            if (await _roleManager.RoleExistsAsync(request.Request.RoleName))
                throw new AppException($"Vai trò '{request.Request.RoleName}' đã tồn tại.", 409);

            var role = _mapper.Map<IdentityRole>(request.Request);
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
                throw new AppException(string.Join("; ", result.Errors.Select(e => e.Description)), 400);

            _mapper.Map<RoleDto>(role);
            return role.Id;
        }

        public async Task<string> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.Id);
            if (role == null)
                throw new AppException("Vai trò không tồn tại.", 404);

            if (!string.Equals(role.Name, request.Request.RoleName, StringComparison.OrdinalIgnoreCase))
            {
                var exists = await _roleManager.FindByNameAsync(request.Request.RoleName);
                if (exists != null && exists.Id != request.Id)
                    throw new AppException($"Tên vai trò '{request.Request.RoleName}' đã được sử dụng.", 409);
            }

            role.Name = request.Request.RoleName;
            role.NormalizedName = request.Request.RoleName.ToUpper();
            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
                throw new AppException(string.Join("; ", result.Errors.Select(e => e.Description)), 400);

            _mapper.Map<RoleDto>(role);
            return role.Id;
        }

        public async Task<string> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.Id);
            if (role == null)
                throw new AppException("Vai trò không tồn tại.", 404);

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
                throw new AppException(string.Join("; ", result.Errors.Select(e => e.Description)), 400);

            return role.Id;
        }

        public Task<IEnumerable<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = _roleManager.Roles.ToList();
            var mapped = _mapper.Map<IEnumerable<RoleDto>>(roles);
            return Task.FromResult(mapped);
        }
    }
}