using AutoMapper;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Dtos;
using QLDT_Becamex.Src.Dtos.Courses;
using QLDT_Becamex.Src.Dtos.Departments;
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
            CreateMap<DepartmentRq, Department>().ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId == 0 ? null : src.ParentId));
            CreateMap<Department, DepartmentDto>();

            //Position
            CreateMap<Position, PositionDto>();
            CreateMap<PositionDto, Position>();
            CreateMap<PositionRq, Position>();

            //Role
            CreateMap<RoleDto, IdentityRole>();
            CreateMap<IdentityRole, RoleDto>();
            CreateMap<RoleRq, IdentityRole>();

            // Course
            CreateMap<Course, QLDT_Becamex.Src.Dtos.Courses.CourseDto>().ReverseMap();
            CreateMap<Material, QLDT_Becamex.Src.Dtos.Courses.MaterialDto>().ReverseMap();
            CreateMap<Lesson, QLDT_Becamex.Src.Dtos.Courses.LessonDto>().ReverseMap();
            CreateMap<Test, QLDT_Becamex.Src.Dtos.Courses.TestDto>().ReverseMap();
            CreateMap<Question, QLDT_Becamex.Src.Dtos.Courses.QuestionDto>().ReverseMap();
            CreateMap<SyllabusItem, QLDT_Becamex.Src.Dtos.Courses.SyllabusItemDto>().ReverseMap();
            CreateMap<Slide, QLDT_Becamex.Src.Dtos.Courses.SlideDto>().ReverseMap();
        }
    }
}
