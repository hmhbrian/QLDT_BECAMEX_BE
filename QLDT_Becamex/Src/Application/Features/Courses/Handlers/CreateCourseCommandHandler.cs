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
            {
                // Sử dụng AppException của bạn với mã lỗi phù hợp
                throw new AppException("Bạn không có quyền tạo khóa học này", 403);
            }

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
                var CategoryExists = await _unitOfWork.CourseCategoryRepository.AnyAsync(s => s.Id == dto.CategoryId.Value);
                if (!CategoryExists)
                    throw new AppException("Loại khóa học không hợp lệ", 400);
            }

            if (dto.LecturerId.HasValue)
            {
                var LecturerExists = await _unitOfWork.LecturerRepository.AnyAsync(s => s.Id == dto.LecturerId.Value);
                if (!LecturerExists)
                    throw new AppException("Giảng viên khóa học không hợp lệ", 400);
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

            if (dto.PositionIds != null && dto.PositionIds.Any())
            {
                var invalidPositions = new List<int>();
                foreach (var posId in dto.PositionIds)
                {
                    var exists = await _unitOfWork.PositionRepository.AnyAsync(p => p.PositionId == posId);
                    if (!exists) invalidPositions.Add(posId);
                }

                if (invalidPositions.Any())
                    throw new AppException($"Vị trí không hợp lệ: {string.Join(", ", invalidPositions)}", 400);
            }

            string? imageUrl = null;
            if (dto.ThumbUrl != null)
                imageUrl = await _cloudinaryService.UploadImageAsync(dto.ThumbUrl);

            var course = _mapper.Map<Course>(dto);
            course.Id = Guid.NewGuid().ToString();
            course.CreatedAt = DateTime.Now;
            course.ModifiedAt = DateTime.Now;
            course.ThumbUrl = imageUrl;
            course.CreateById = currentUserId;
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

            if (dto.PositionIds != null && dto.PositionIds.Any())
            {
                var coursePositions = dto.PositionIds.Select(posId => new CoursePosition
                {
                    CourseId = course.Id,
                    PositionId = posId
                }).ToList();

                await _unitOfWork.CoursePositionRepository.AddRangeAsync(coursePositions);
            }

            if (dto.Optional == ConstantCourse.OPTIONAL_BATBUOC)
            {
                var usersToEnroll = new HashSet<string>();

                if (dto.DepartmentIds != null && dto.DepartmentIds.Any() &&
                    dto.PositionIds != null && dto.PositionIds.Any())
                {
                    var matchedUsers = await _unitOfWork.UserRepository
                        .FindAsync(u => u.DepartmentId.HasValue && dto.DepartmentIds.Contains(u.DepartmentId.Value) &&
                                        u.PositionId.HasValue && dto.PositionIds.Contains(u.PositionId.Value));

                    foreach (var user in matchedUsers)
                        usersToEnroll.Add(user.Id);
                }

                if (usersToEnroll.Any())
                {
                    var userCourses = usersToEnroll.Select(userId => new UserCourse
                    {
                        UserId = userId,
                        CourseId = course.Id,
                        AssignedAt = DateTime.Now,
                        IsMandatory = true,
                        Status = ConstantStatus.ASSIGINED,
                        CreatedAt = DateTime.Now,
                        ModifiedAt = DateTime.Now,
                    }).ToList();

                    await _unitOfWork.UserCourseRepository.AddRangeAsync(userCourses);
                }
            }

            await _unitOfWork.CompleteAsync();

            return course.Id;
        }
    }
}
