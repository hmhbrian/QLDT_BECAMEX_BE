using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Departments.Dtos;
using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using System.Security.Claims;

namespace QLDT_Becamex.Src.Infrastructure.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {

            _httpContextAccessor = httpContextAccessor;
        }
        public (string? UserId, string? Role) GetCurrentUserAuthenticationInfo()
        {
            var currentUser = _httpContextAccessor.HttpContext?.User;

            var userId = currentUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = currentUser?.FindFirst(ClaimTypes.Role)?.Value;


            return (userId, role);
        }


    }
}
