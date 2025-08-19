using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Application.Features.AuditLogs.Dtos;
using QLDT_Becamex.Src.Application.Features.Certificates.Dtos;
using QLDT_Becamex.Src.Application.Features.CourseAttachedFile.Dtos;
using QLDT_Becamex.Src.Application.Features.CourseCategory.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;
using QLDT_Becamex.Src.Application.Features.Departments.Dtos;
using QLDT_Becamex.Src.Application.Features.EmployeeLevels.Dtos;
using QLDT_Becamex.Src.Application.Features.Feedbacks.Dtos;
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;
using QLDT_Becamex.Src.Application.Features.Questions.Dtos;
using QLDT_Becamex.Src.Application.Features.Roles.Dtos;
using QLDT_Becamex.Src.Application.Features.Status.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;
using QLDT_Becamex.Src.Application.Features.TypeDocument.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Shared.Helpers;
using System.Globalization;
using System.Text.Json;

namespace QLDT_Becamex.Src.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //User
            Action<object, object, ResolutionContext> ignoreNavigation = (src, dest, context) => { };

            CreateMap<UserStatus, StatusDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.CreatedBy,
                opt => opt.MapFrom(src => src.CreateBy != null
                    ? new UserSumaryDto { Id = src.CreateBy.Id, Name = src.CreateBy.FullName }
                    : null))
            .ForMember(dest => dest.UpdatedBy,
                opt => opt.MapFrom(src => src.UpdateBy != null
                    ? new UserSumaryDto { Id = src.UpdateBy.Id, Name = src.UpdateBy.FullName }
                    : null))
            .ForMember(dest => dest.ManagerBy,
                opt => opt.MapFrom(src => src.ManagerU != null
                    ? new UserSumaryDto { Id = src.ManagerU.Id, Name = src.ManagerU.FullName }
                    : null))
            .ForMember(dest => dest.Department,
                    opt => opt.MapFrom(src => src.Department != null
                    ? new DepartmentShortenDto { DepartmentId = src.Department.DepartmentId, DepartmentName = src.Department.DepartmentName }
                    : null));


            CreateMap<ApplicationUser, UserSumaryDto>();
            CreateMap<ApplicationUser, ManagerDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            //UserStatus
            CreateMap<UserStatus, UserStatusDto>().ReverseMap();
            CreateMap<UserStatusDtoRq, UserStatus>();

            //CourseCategory
            CreateMap<CourseCategory, CourseCategoryDto>().ReverseMap();
            CreateMap<CourseCategoryRqDto, CourseCategory>();

            //Department
            CreateMap<DepartmentRequestDto, Department>()
                .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId == 0 ? null : src.ParentId))
                .ForMember(dest => dest.StatusId, opt => opt.Condition(src => src.StatusId != null));
            CreateMap<Department, DepartmentDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.DepartmentName : null))
                .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.Children));

            //DepartmentStatus
            CreateMap<DepartmentStatus, StatusDto>().ReverseMap();
            CreateMap<CreateStatusDto, DepartmentStatus>().ReverseMap();

            //EmployeeLevel
            CreateMap<EmployeeLevel, ELevelDto>().ReverseMap();
            CreateMap<CreateELevelDto, EmployeeLevel>();

            //Role
            CreateMap<IdentityRole, RoleDto>().ReverseMap();
            CreateMap<CreateRoleDto, IdentityRole>();

            //TypeDocument
            CreateMap<TypeDocument, TypeDocumentDto>().ReverseMap();
            CreateMap<TypeDocumentRqDto, TypeDocument>();

            //Course
            CreateMap<CreateCourseDto, Course>()
                .ForMember(dest => dest.ThumbUrl, opt => opt.Condition(src => src.ThumbUrl != null))
                .ForMember(dest => dest.StatusId, opt => opt.Condition(src => src.StatusId != null))
                .ForMember(dest => dest.RegistrationStartDate, opt => opt.MapFrom(src => src.RegistrationStartDate.HasValue ? DateTimeHelper.ToVietnamTime(src.RegistrationStartDate!.Value) : (DateTime?)null))
                .ForMember(dest => dest.RegistrationClosingDate, opt => opt.MapFrom(src => src.RegistrationClosingDate.HasValue ? DateTimeHelper.ToVietnamTime(src.RegistrationClosingDate!.Value) : (DateTime?)null))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.HasValue ? DateTimeHelper.ToVietnamTime(src.StartDate!.Value) : (DateTime?)null))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.HasValue ? DateTimeHelper.ToVietnamTime(src.EndDate!.Value) : (DateTime?)null));

            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.Departments, opt => opt.MapFrom(src => (src.CourseDepartments ?? Enumerable.Empty<CourseDepartment>()).Select(cd => new DepartmentShortenDto
                {
                    DepartmentId = cd.DepartmentId,
                    DepartmentName = cd.Department.DepartmentName,
                }).ToList()))
                .ForMember(dest => dest.ELevels, opt => opt.MapFrom(src => (src.CourseELevels ?? Enumerable.Empty<CourseELevel>()).Select(cp => new ELevelDto
                {
                    ELevelId = cp.ELevelId,
                    ELevelName = cp.ELevel.ELevelName,
                }).ToList()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Students, opt => opt.MapFrom(src =>
                    (src.UserCourses ?? Enumerable.Empty<UserCourse>())
                    .Where(cp => cp.User != null)
                    .Select(cp => new UserSumaryDto
                    {
                        Id = cp.UserId,
                        Name = cp.User!.FullName ?? null
                    }).ToList()
                ))

                .ForMember(dest => dest.CreatedBy,
                opt => opt.MapFrom(src => src.CreateBy != null
                    ? new UserSumaryDto { Id = src.CreateBy.Id, Name = src.CreateBy.FullName }
                    : null))
                .ForMember(dest => dest.UpdatedBy,
                opt => opt.MapFrom(src => src.UpdateBy != null
                    ? new UserSumaryDto { Id = src.UpdateBy.Id, Name = src.UpdateBy.FullName }
                    : null));

            CreateMap<CourseDto, Course>()
                .ForMember(dest => dest.RegistrationStartDate, opt => opt.MapFrom((src, dest) =>
                  src.RegistrationStartDate.HasValue 
                      ? DateTimeHelper.ToVietnamTime(DateTime.SpecifyKind(src.RegistrationStartDate.Value, DateTimeKind.Utc))
                      : (DateTime?)null))
                .ForMember(dest => dest.RegistrationClosingDate, opt => opt.MapFrom((src, dest) =>
                  src.RegistrationClosingDate.HasValue
                      ? DateTimeHelper.ToVietnamTime(DateTime.SpecifyKind(src.RegistrationClosingDate.Value, DateTimeKind.Utc))
                      : (DateTime?)null))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom((src, dest) =>
                  src.StartDate.HasValue
                      ? DateTimeHelper.ToVietnamTime(DateTime.SpecifyKind(src.StartDate.Value, DateTimeKind.Utc))
                      : (DateTime?)null))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom((src, dest) =>
                  src.EndDate.HasValue
                      ? DateTimeHelper.ToVietnamTime(DateTime.SpecifyKind(src.EndDate.Value, DateTimeKind.Utc))
                      : (DateTime?)null));

            CreateMap<UpdateCourseDto, Course>()
              .ForMember(dest => dest.RegistrationStartDate, opt => opt.MapFrom((src, dest) =>
                  src.RegistrationStartDate.HasValue && !Equals(src.RegistrationStartDate.Value, dest.RegistrationStartDate)
                      ? DateTimeHelper.ToVietnamTime(DateTime.SpecifyKind(src.RegistrationStartDate.Value, DateTimeKind.Utc))
                      : dest.RegistrationStartDate))

              .ForMember(dest => dest.RegistrationClosingDate, opt => opt.MapFrom((src, dest) =>
                  src.RegistrationClosingDate.HasValue && !Equals(src.RegistrationClosingDate.Value, dest.RegistrationClosingDate)
                      ? DateTimeHelper.ToVietnamTime(DateTime.SpecifyKind(src.RegistrationClosingDate.Value, DateTimeKind.Utc))
                      : dest.RegistrationClosingDate))

              .ForMember(dest => dest.StartDate, opt => opt.MapFrom((src, dest) =>
                  src.StartDate.HasValue && !Equals(src.StartDate.Value, dest.StartDate)
                      ? DateTimeHelper.ToVietnamTime(DateTime.SpecifyKind(src.StartDate.Value, DateTimeKind.Utc))
                      : dest.StartDate))

              .ForMember(dest => dest.EndDate, opt => opt.MapFrom((src, dest) =>
                  src.EndDate.HasValue && !Equals(src.EndDate.Value, dest.EndDate)
                      ? DateTimeHelper.ToVietnamTime(DateTime.SpecifyKind(src.EndDate.Value, DateTimeKind.Utc))
                      : dest.EndDate)) // Áp dụng điều kiện chung: chỉ map nếu khác null và khác giá trị cũ
              .ForAllMembers(opt => opt.Condition((src, dest, srcMember, destMember) =>
                  srcMember != null && !Equals(srcMember, destMember)));



            //EnrollCourse
            CreateMap<UserEnrollCourseDto, Course>();
            CreateMap<Course, UserEnrollCourseDto>();

            //UpcomingCourse
            CreateMap<Course, UserUpcomingCourseDto>();

            //EnrollCompletedCourse
            CreateMap<UserEnrollCompletedCourseDto, Course>();
            CreateMap<Course, UserEnrollCompletedCourseDto>();

            //CourseStatus
            CreateMap<Course, CourseSumary>().ReverseMap();
            CreateMap<CourseStatus, StatusDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.StatusName));
            CreateMap<CreateStatusDto, CourseStatus>().ReverseMap();

            // Question
            CreateMap<QuestionDto, Question>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.TestId, opt => opt.Ignore())
                    .ForMember(dest => dest.Test, opt => opt.Ignore());
            CreateMap<Question, QuestionDto>();
            CreateMap<QuestionNoAnswerDto, Question>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.TestId, opt => opt.Ignore())
                    .ForMember(dest => dest.Test, opt => opt.Ignore());
            CreateMap<Question, QuestionNoAnswerDto>();

            // Test
            CreateMap<TestCreateDto, Test>()
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions ?? new List<CreateQuestionDto>()))
                .AfterMap(ignoreNavigation);

            CreateMap<Test, DetailTestDto>()
                    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => $"Bài kiểm tra {src.Position}: {(string.IsNullOrEmpty(src.Title) ? ""
                        : char.ToUpper(src.Title[0]) + src.Title.Substring(1))}"))
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                    .ForMember(dest => dest.PassThreshold, opt => opt.MapFrom(src => src.PassThreshold * 100.0))
                    .ForMember(dest => dest.Questions, opt => opt.MapFrom((src, dest, destMember, context) => src.Questions != null ? src.Questions.Select(q => context.Mapper.Map<QuestionDto>(q)).ToList() : new List<QuestionDto>()))
                      .ForMember(dest => dest.CreatedBy,
                     opt => opt.MapFrom(src => src.CreatedBy != null
                    ? new UserSumaryDto { Id = src.CreatedBy.Id, Name = src.CreatedBy.FullName }
                    : null))
                    .ForMember(dest => dest.UpdatedBy,
                    opt => opt.MapFrom(src => src.UpdatedBy != null
                    ? new UserSumaryDto { Id = src.UpdatedBy.Id, Name = src.UpdatedBy.FullName }
                    : null));

            CreateMap<TestUpdateDto, Test>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.PassThreshold, opt => opt.MapFrom(src => Math.Round((src.PassThreshold / 100.0), 1)))
                .AfterMap(ignoreNavigation);

            CreateMap<Test, AllTestDto>()
                    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => $"Bài kiểm tra {src.Position}: {(string.IsNullOrEmpty(src.Title) ? ""
                        : char.ToUpper(src.Title[0]) + src.Title.Substring(1))}"))
                    .ForMember(dest => dest.CountQuestion, opt => opt.MapFrom(src => src.Questions != null ? src.Questions.Count : 0))
                    .ForMember(dest => dest.PassThreshold, opt => opt.MapFrom(src => src.PassThreshold * 100.0))
                    .ForMember(dest => dest.CreatedBy,
                     opt => opt.MapFrom(src => src.CreatedBy != null
                    ? new UserSumaryDto { Id = src.CreatedBy.Id, Name = src.CreatedBy.FullName }
                    : null))
                    .ForMember(dest => dest.UpdatedBy,
                    opt => opt.MapFrom(src => src.UpdatedBy != null
                    ? new UserSumaryDto { Id = src.UpdatedBy.Id, Name = src.UpdatedBy.FullName }
                    : null));

            CreateMap<TestResult, TestResultDto>();
            CreateMap<TestResult, DetailTestResultDto>();
            CreateMap<Test, TestSummaryDto>(); // Map từ entity Test → DTO TestSummaryDto
            
            //Question
            CreateMap<CreateQuestionDto, Question>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TestId, opt => opt.Ignore())
                .ForMember(dest => dest.Test, opt => opt.Ignore());


            //CourseAttachedFile
            CreateMap<CourseAttachedFile, CourseAttachedFileDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TypeDoc != null ? src.TypeDoc.NameType : "Unknown"));

            //Lesson
            CreateMap<Lesson, AllLessonDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => $"Bài {src.Position}: {(string.IsNullOrEmpty(src.Title) ? ""
                        : char.ToUpper(src.Title[0]) + src.Title.Substring(1))}"))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TypeDoc.NameType));

            CreateMap<Lesson, DetailLessonDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => $"Bài {src.Position}: {(string.IsNullOrEmpty(src.Title) ? ""
                        : char.ToUpper(src.Title[0]) + src.Title.Substring(1))}"))

                .ForMember(dest => dest.UserIdCreated, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.UserIdEdited, opt => opt.MapFrom(src => src.UpdatedBy))
                .ForMember(dest => dest.UserNameCreated, opt => opt.MapFrom(src => src.CreatedBy != null ? src.CreatedBy.FullName : null))
                .ForMember(dest => dest.UserNameEdited, opt => opt.MapFrom(src => src.UpdatedBy != null ? src.UpdatedBy.FullName : null));

            // Feedback
            CreateMap<CreateFeedbackDto, Feedback>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.SubmissionDate, opt => opt.Ignore())
                .AfterMap(ignoreNavigation);
            CreateMap<Feedback, CreateFeedbackDto>();

            CreateMap<Feedback, FeedbacksDto>()
                     .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.FullName != null ? src.User.FullName : "Unknown"));
            CreateMap<FeedbacksDto, Feedback>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.SubmissionDate, opt => opt.Ignore())
                .AfterMap(ignoreNavigation);

            //AuditLog
            //CreateMap<AuditLog, AuditLogDto>()
            //    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName ?? "Unknown"))
            //    .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp.ToString("dddd, dd MMMM, yyyy, HH:mm", new CultureInfo("vi-VN"))));
            CreateMap<AuditLog, AuditLogDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User!.UserName ?? "Unknown"))
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp.ToString("dddd, dd MMMM, yyyy, HH:mm", new CultureInfo("vi-VN"))))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Action, opt => opt.MapFrom(src => src.Action));



            //Certifiates
            CreateMap<Certificates, CertDetailDto>().ForMember(dest => dest.User,
                     opt => opt.MapFrom(src => src.User != null
                    ? new UserSumaryDto { Id = src.User.Id, Name = src.User.FullName }
                    : null));

            CreateMap<Certificates, CertListDto>().ForMember(dest => dest.User,
                     opt => opt.MapFrom(src => src.User != null
                    ? new UserSumaryDto { Id = src.User.Id, Name = src.User.FullName }
                    : null));

        }
    }
}
