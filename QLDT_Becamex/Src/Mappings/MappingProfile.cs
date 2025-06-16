using AutoMapper;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Dtos;
using QLDT_Becamex.Src.Dtos.Courses;
using QLDT_Becamex.Src.Dtos.Departments;
using QLDT_Becamex.Src.Dtos.Positions;
using QLDT_Becamex.Src.Dtos.Roles;
using QLDT_Becamex.Src.Dtos.Users;
using QLDT_Becamex.Src.Models;
using System.Linq;

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
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => 
                    src.CourseDepartments.Select(cd => cd.DepartmentId.ToString()).ToList()))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src =>
                    src.CourseLevels.Select(cl => cl.Level).ToList()))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => new DurationDto 
                { 
                    Sessions = src.Sessions,
                    HoursPerSession = src.HoursPerSession
                }))
                .ForMember(dest => dest.Materials, opt => opt.MapFrom(src => src.Materials))
                .ForMember(dest => dest.Lessons, opt => opt.MapFrom(src => src.Lessons))
                .ForMember(dest => dest.Tests, opt => opt.MapFrom(src => src.Tests))
                .ForMember(dest => dest.Syllabus, opt => opt.MapFrom(src => src.SyllabusItems))
                .ForMember(dest => dest.Slides, opt => opt.MapFrom(src => src.Slides));

            CreateMap<Material, MaterialDto>();
            CreateMap<MaterialDto, Material>();
            CreateMap<Lesson, LessonDto>();
            CreateMap<LessonDto, Lesson>();
            CreateMap<Test, TestDto>();
            CreateMap<TestDto, Test>();
            CreateMap<Question, QuestionDto>();
            CreateMap<QuestionDto, Question>();
            CreateMap<SyllabusItem, SyllabusItemDto>();
            CreateMap<SyllabusItemDto, SyllabusItem>();
            CreateMap<Slide, SlideDto>();
            CreateMap<SlideDto, Slide>();

            CreateMap<CourseDto, Course>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title ?? string.Empty))
                .ForMember(dest => dest.CourseCode, opt => opt.MapFrom(src => src.CourseCode ?? string.Empty))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty))
                .ForMember(dest => dest.Objectives, opt => opt.MapFrom(src => src.Objectives ?? string.Empty))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category ?? string.Empty))
                .ForMember(dest => dest.LearningType, opt => opt.MapFrom(src => src.LearningType ?? string.Empty))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image ?? string.Empty))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? string.Empty))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location ?? string.Empty))
                .ForMember(dest => dest.EnrollmentType, opt => opt.MapFrom(src => src.EnrollmentType ?? string.Empty))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy ?? string.Empty))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy ?? string.Empty))
                .ForMember(dest => dest.Instructor, opt => opt.MapFrom(src => src.Instructor ?? string.Empty))
                .ForMember(dest => dest.Materials, opt => opt.MapFrom(src => src.Materials))
                .ForMember(dest => dest.Lessons, opt => opt.MapFrom(src => src.Lessons))
                .ForMember(dest => dest.Tests, opt => opt.MapFrom(src => src.Tests))
                .ForMember(dest => dest.SyllabusItems, opt => opt.MapFrom(src => src.Syllabus))
                .ForMember(dest => dest.Slides, opt => opt.MapFrom(src => src.Slides))
                .ForMember(dest => dest.CourseDepartments, opt => opt.Ignore())
                .ForMember(dest => dest.CourseLevels, opt => opt.Ignore())
                .ForMember(dest => dest.Prerequisites, opt => opt.Ignore())
                .ForMember(dest => dest.CourseTrainees, opt => opt.Ignore())
                .ForMember(dest => dest.Sessions, opt => opt.MapFrom(src => src.Duration != null ? src.Duration.Sessions : 0))
                .ForMember(dest => dest.HoursPerSession, opt => opt.MapFrom(src => src.Duration != null ? src.Duration.HoursPerSession : 0));

            CreateMap<Material, MaterialDto>().ReverseMap();
            CreateMap<Lesson, QLDT_Becamex.Src.Dtos.Courses.LessonDto>().ReverseMap();
            CreateMap<Test, QLDT_Becamex.Src.Dtos.Courses.TestDto>().ReverseMap();
            CreateMap<Question, QLDT_Becamex.Src.Dtos.Courses.QuestionDto>().ReverseMap();
            CreateMap<SyllabusItem, QLDT_Becamex.Src.Dtos.Courses.SyllabusItemDto>().ReverseMap();
            CreateMap<Slide, QLDT_Becamex.Src.Dtos.Courses.SlideDto>().ReverseMap();
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => 
                    src.CourseDepartments.Select(cd => cd.DepartmentId.ToString()).ToList()))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src =>
                    src.CourseLevels.Select(cl => cl.Level).ToList()))
                .ForMember(dest => dest.Prerequisites, opt => opt.MapFrom(src =>
                    src.Prerequisites.Select(cp => cp.PrerequisiteId.ToString()).ToList()))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => new DurationDto 
                { 
                    Sessions = src.Sessions,
                    HoursPerSession = src.HoursPerSession
                }))
                .ForMember(dest => dest.EnrolledTrainees, opt => opt.MapFrom(src =>
                    src.CourseTrainees.Select(ct => ct.TraineeId).ToList()));

            CreateMap<CourseDto, Course>()
                .ForMember(dest => dest.CourseDepartments, opt => opt.Ignore())
                .ForMember(dest => dest.CourseLevels, opt => opt.Ignore())
                .ForMember(dest => dest.Prerequisites, opt => opt.Ignore())
                .ForMember(dest => dest.CourseTrainees, opt => opt.Ignore())
                .ForMember(dest => dest.Sessions, opt => opt.MapFrom(src => src.Duration != null ? src.Duration.Sessions : 0))
                .ForMember(dest => dest.HoursPerSession, opt => opt.MapFrom(src => src.Duration != null ? src.Duration.HoursPerSession : 0));
        }
    }
}
