using AutoMapper;
using QLDT_Becamex.Src.Application.Features.Departments.Dtos;
using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Infrastructure.Services
{
    public interface IUserService
    {
        public (string? UserId, string? Role) GetCurrentUserAuthenticationInfo();

    }
}
