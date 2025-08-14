using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Commands;
using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Infrastructure.Services.CloudinaryServices;
using QLDT_Becamex.Src.Infrastructure.Services.DepartmentServices;
using QLDT_Becamex.Src.Shared.Helpers;
using Xceed.Pdf.Layout.Shape;


namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers
{
    public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IDepartmentService _departmentService;
        private readonly IUserService _userService;



        public CreateCourseCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryService cloudinaryService, IDepartmentService departmentService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
            _departmentService = departmentService;
            _userService = userService;
        }

        public async Task<string> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var (currentUserId, _) = _userService.GetCurrentUserAuthenticationInfo();
            var dto = request.Request;

            if (string.IsNullOrEmpty(currentUserId))
                throw new AppException("Bạn không có quyền tạo khóa học này", 403);

            if (await _unitOfWork.CourseRepository.AnyAsync(c => c.Code == dto.Code.Trim().ToLower()))
                throw new AppException("Mã khóa học đã tồn tại", 409);

            if (await _unitOfWork.CourseRepository.AnyAsync(c => c.Name == dto.Name))
                throw new AppException("Tên khóa học đã tồn tại", 409);

            if (dto.StatusId.HasValue)
            {
                var statusExists = await _unitOfWork.CourseStatusRepository.AnyAsync(s => s.Id == dto.StatusId.Value);
                if (!statusExists)
                    throw new AppException("Trạng thái khóa học không hợp lệ", 400);
            }

            if (dto.CategoryId.HasValue)
            {
                var categoryExists = await _unitOfWork.CourseCategoryRepository.AnyAsync(s => s.Id == dto.CategoryId.Value);
                if (!categoryExists)
                    throw new AppException("Loại khóa học không hợp lệ", 400);
            }

            if (dto.DepartmentIds != null && dto.DepartmentIds.Any())
            {
                var allDepartmentIds = new HashSet<int>();
                var invalidDepts = new List<int>();

                foreach (var deptId in dto.DepartmentIds)
                {
                    var dept = await _unitOfWork.DepartmentRepository.GetByIdAsync(deptId);
                    if (dept == null) invalidDepts.Add(deptId);
                    else
                    {
                        allDepartmentIds.Add(deptId);
                        var children = await _departmentService.GetAllChildDepartmentIds(deptId);
                        foreach (var child in children)
                            allDepartmentIds.Add(child);
                    }
                }

                if (invalidDepts.Any())
                    throw new AppException($"Phòng ban không hợp lệ: {string.Join(", ", invalidDepts)}", 400);

                dto.DepartmentIds = allDepartmentIds.ToList();
            }

            if (dto.ELevelIds != null && dto.ELevelIds.Any())
            {
                var invalidELevels = new List<int>();
                foreach (var ElevelId in dto.ELevelIds)
                {
                    var exists = await _unitOfWork.EmployeeLevelRepository.AnyAsync(p => p.ELevelId == ElevelId);
                    if (!exists) invalidELevels.Add(ElevelId);
                }

                if (invalidELevels.Any())
                    throw new AppException($"Vị trí không hợp lệ: {string.Join(", ", invalidELevels)}", 400);
            }

            string? imageUrl = null;
            if (dto.ThumbUrl != null)
                imageUrl = await _cloudinaryService.UploadImageAsync(dto.ThumbUrl);

            var course = _mapper.Map<Course>(dto);
            course.Id = Guid.NewGuid().ToString();
            course.CreatedAt = DateTime.Now;
            course.UpdatedAt = DateTime.Now;
            course.ThumbUrl = imageUrl;
            course.CreatedById = currentUserId;
            course.NormalizeCourseName = StringHelper.RemoveDiacritics(dto.Name).ToUpperInvariant().Replace(" ", "");
            await _unitOfWork.CourseRepository.AddAsync(course);

            if (dto.DepartmentIds != null && dto.DepartmentIds.Any())
            {
                var courseDepartments = dto.DepartmentIds.Select(deptId => new CourseDepartment
                {
                    CourseId = course.Id,
                    DepartmentId = deptId
                }).ToList();

                await _unitOfWork.CourseDepartmentRepository.AddRangeAsync(courseDepartments);
            }

            if (dto.ELevelIds != null && dto.ELevelIds.Any())
            {
                var courseELevels = dto.ELevelIds.Select(posId => new CourseELevel
                {
                    CourseId = course.Id,
                    ELevelId = posId
                }).ToList();

                await _unitOfWork.CourseELevelRepository.AddRangeAsync(courseELevels);
            }
            if (dto.UserIds == null)
                Console.WriteLine("StudentIds is null, no students to enroll.");
            else
                Console.WriteLine(dto.UserIds);
            var userCoursesToCreate = new List<UserCourse>();
            // --- Ghi danh người dùng vào khóa học ---
            if (dto.Optional == ConstantCourse.OPTIONAL_BATBUOC && dto.UserIds != null && dto.UserIds.Any())
            {
                var now = DateTime.Now;

                userCoursesToCreate = dto.UserIds.Select(userId => new UserCourse
                {
                    UserId = userId,
                    CourseId = course.Id,
                    AssignedAt = now,
                    Optional = 1,
                    IsMandatory = true,
                    Status = 1,
                    CreatedAt = now,
                    ModifiedAt = now
                }).ToList();
            }

            if (userCoursesToCreate.Any())
            {
                await _unitOfWork.UserCourseRepository.AddRangeAsync(userCoursesToCreate);
            }

            await _unitOfWork.CompleteAsync();

            return course.Id;
        }
    }
}
