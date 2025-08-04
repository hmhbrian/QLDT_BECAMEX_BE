using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Domain.Entities;
using AutoMapper;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Queries;

namespace QLDT_Becamex.Src.Application.Features.Users.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery command, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userManager.Users
                .Include(u => u.ELevel)
                .Include(u => u.Department)
                .Include(u => u.UserStatus)
                .Include(u => u.ManagerU)
                .Include(u => u.CreateBy)
                .Include(u => u.CreateBy)
                .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

            if (user == null)
                throw new AppException("Không tìm thấy người dùng", 404);

            var dto = _mapper.Map<UserDto>(user);

            var roles = await _userManager.GetRolesAsync(user);
            dto.Role = roles.FirstOrDefault();

            return dto;
        }
    }
}
