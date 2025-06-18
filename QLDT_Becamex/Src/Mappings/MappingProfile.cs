using AutoMapper;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Dtos.Departments;
using QLDT_Becamex.Src.Dtos.Positions;
using QLDT_Becamex.Src.Dtos.Roles;
using QLDT_Becamex.Src.Dtos.Users;
using QLDT_Becamex.Src.Dtos.UserStatus;
using QLDT_Becamex.Src.Models;

namespace QLDT_Becamex.Src.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //User
            CreateMap<ApplicationUser, UserDto>();

            //UserStatus
            CreateMap<UserStatus, UserStatusDto>().ReverseMap();
            CreateMap<UserStatusDtoRq, UserStatus>();


            //Department
            CreateMap<DepartmentRq, Department>().ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId == 0 ? null : src.ParentId));
            CreateMap<Department, DepartmentDto>();

            //Position
            CreateMap<Position, PositionDto>().ReverseMap();

            CreateMap<PositionRq, Position>();

            //Role
            CreateMap<IdentityRole, RoleDto>().ReverseMap();
            CreateMap<RoleRq, IdentityRole>();
        }
    }
}
