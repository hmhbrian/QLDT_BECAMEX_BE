

using AutoMapper;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Dtos;
using QLDT_Becamex.Src.Dtos.Positions;
using QLDT_Becamex.Src.Dtos.Roles;
using QLDT_Becamex.Src.Dtos.Users;
using QLDT_Becamex.Src.Models;

namespace QLDT_Becamex.Src.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //User
            CreateMap<ApplicationUser, UserDto>();
            //CreateMap<UserDTO, User>().ForMember(dest => dest.Id, opt => opt.Ignore());

            //Department
            CreateMap<CreateDepartmentDto, Department>().ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId == "NULL" ? null : src.ParentId));
            CreateMap<Department, DepartmentDto>();

            //Position
            CreateMap<Position, PositionDto>();
            CreateMap<PositionDto, Position>();
            CreateMap<PositionRq, Position>();

            //Role
            CreateMap<RoleDto, IdentityRole>();
            CreateMap<IdentityRole, RoleDto>();
            CreateMap<RoleRq, IdentityRole>();
        }
    }
}
