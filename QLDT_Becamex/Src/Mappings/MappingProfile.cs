

using AutoMapper;
using QLDT_Becamex.Src.Dtos;
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
        }
    }
}
