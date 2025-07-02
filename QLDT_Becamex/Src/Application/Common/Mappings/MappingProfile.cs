using AutoMapper;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.CourseCategory.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;
using QLDT_Becamex.Src.Application.Features.Departments.Dtos;
using QLDT_Becamex.Src.Application.Features.Lecturer.Dtos;
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;
using QLDT_Becamex.Src.Application.Features.Positions.Dtos;
using QLDT_Becamex.Src.Application.Features.Roles.Dtos;
using QLDT_Becamex.Src.Application.Features.Status.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Application.Common.Mappings
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

            //Lecturer
            CreateMap<Lecturer, LecturerDto>().ReverseMap();
            CreateMap<LecturerDtoRq, Lecturer>();

            //CourseCategory
            CreateMap<CourseCategory, CourseCategoryDto>().ReverseMap();
            CreateMap<CourseCategoryRqDto, CourseCategory>();

            //Department
            CreateMap<DepartmentRequestDto, Department>().ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId == 0 ? null : src.ParentId));
            CreateMap<Department, DepartmentDto>();

            //Position
            CreateMap<Position, PositionDto>().ReverseMap();
            CreateMap<CreatePositionDto, Position>();

            //Role
            CreateMap<IdentityRole, RoleDto>().ReverseMap();
            CreateMap<CreateRoleDto, IdentityRole>();

            //Course
            CreateMap<CreateCourseDto, Course>()
                .ForMember(dest => dest.ThumbUrl, opt => opt.Condition(src => src.ThumbUrl != null))
                .ForMember(dest => dest.StatusId, opt => opt.Condition(src => src.StatusId != null));

            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.Departments, opt => opt.MapFrom(src => (src.CourseDepartments ?? Enumerable.Empty<CourseDepartment>()).Select(cd => new DepartmentDto
                {
                    DepartmentId = cd.DepartmentId,
                    DepartmentName = cd.Department.DepartmentName,
                }).ToList()))
                .ForMember(dest => dest.Positions, opt => opt.MapFrom(src => (src.CoursePositions ?? Enumerable.Empty<CoursePosition>()).Select(cp => new PositionDto
                {
                    PositionId = cp.PositionId,
                    PositionName = cp.Position.PositionName,
                }).ToList()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Lecturer, opt => opt.MapFrom(src => src.Lecturer))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));

            CreateMap<CourseDto, Course>();


            //CourseStatus
            CreateMap<CourseStatus, CourseStatusDto>().ReverseMap();
            CreateMap<CreateCourseStatusDto, CourseStatus>().ReverseMap();

            //Lesson
            CreateMap<Lesson, AllLessonDto>().ReverseMap();
            CreateMap<Lesson, DetailLessonDto>()
                .ForMember(dest => dest.UserIdCreated, opt => opt.MapFrom(src => src.UserId_created))
                .ForMember(dest => dest.UserIdEdited, opt => opt.MapFrom(src => src.UserId_edited));
        }
    }
}
