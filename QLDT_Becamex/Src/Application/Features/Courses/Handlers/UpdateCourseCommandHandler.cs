using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Commands;
using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Infrastructure.Services.CloudinaryServices;
using QLDT_Becamex.Src.Infrastructure.Services.DepartmentServices;
using System.Linq;

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers
{
    public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IDepartmentService _departmentService;
        private readonly IUserService _userService;

        public UpdateCourseCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryService cloudinaryService, IDepartmentService departmentService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
            _departmentService = departmentService;
            _userService = userService;
        }

        public async Task<string> Handle(UpdateCourseCommand command, CancellationToken cancellationToken)
        {
            var id = command.Id;
            var request = command.Request;
            var (currentUserId, _) = _userService.GetCurrentUserAuthenticationInfo();

            var course = await _unitOfWork.CourseRepository.GetByIdAsync(id);

            if (course == null)
                throw new AppException("Khóa học không tồn tại", 404);

            if (string.IsNullOrEmpty(currentUserId))
                throw new AppException("Bạn không có quyền cập nhật khóa học này", 403);

            if (await _unitOfWork.CourseRepository.AnyAsync(c => c.Code == request.Code && c.Id != id))
                throw new AppException("Mã khóa học đã tồn tại", 409);

            if (await _unitOfWork.CourseRepository.AnyAsync(c => c.Name == request.Name && c.Id != id))
                throw new AppException("Tên khóa học đã tồn tại", 409);

            if (request.StatusId.HasValue && !await _unitOfWork.CourseStatusRepository.AnyAsync(s => s.Id == request.StatusId.Value))
                throw new AppException("Trạng thái khóa học không hợp lệ", 400);

            if (request.CategoryId.HasValue && !await _unitOfWork.CourseCategoryRepository.AnyAsync(s => s.Id == request.CategoryId.Value))
                throw new AppException("Loại khóa học không hợp lệ", 400);

            if (request.LecturerId.HasValue && !await _unitOfWork.LecturerRepository.AnyAsync(s => s.Id == request.LecturerId.Value))
                throw new AppException("Giảng viên khóa học không hợp lệ", 400);

            if (request.DepartmentIds != null && request.DepartmentIds.Any())
            {
                var validDeptIds = await _unitOfWork.DepartmentRepository.GetQueryable()
                    .Where(d => request.DepartmentIds.Contains(d.DepartmentId))
                    .Select(d => d.DepartmentId)
                    .ToListAsync();

                var invalidDepts = request.DepartmentIds.Except(validDeptIds).ToList();
                if (invalidDepts.Any())
                    throw new AppException($"Phòng ban không hợp lệ: {string.Join(", ", invalidDepts)}", 400);

                var allDepartmentIdsIncludingChildren = new HashSet<int>(validDeptIds);
                foreach (var deptId in validDeptIds)
                {
                    var children = await _departmentService.GetAllChildDepartmentIds(deptId);
                    foreach (var child in children)
                        allDepartmentIdsIncludingChildren.Add(child);
                }
                request.DepartmentIds = allDepartmentIdsIncludingChildren.ToList();
            }

            if (request.PositionIds != null && request.PositionIds.Any())
            {
                var validPosIds = await _unitOfWork.PositionRepository.GetQueryable()
                    .Where(p => request.PositionIds.Contains(p.PositionId))
                    .Select(p => p.PositionId)
                    .ToListAsync();

                var invalidPositions = request.PositionIds.Except(validPosIds).ToList();
                if (invalidPositions.Any())
                    throw new AppException($"Vị trí không hợp lệ: {string.Join(", ", invalidPositions)}", 400);
            }

            if (request.Optional != ConstantCourse.OPTIONAL_BATBUOC && request.StudentIds != null && request.StudentIds.Any())
            {
                var validUserIds = await _unitOfWork.UserRepository.GetQueryable()
                    .Where(u => request.StudentIds.Contains(u.Id))
                    .Select(u => u.Id)
                    .ToListAsync();

                var invalidUserIds = request.StudentIds.Except(validUserIds).ToList();
                if (invalidUserIds.Any())
                    throw new AppException($"Người dùng không hợp lệ: {string.Join(", ", invalidUserIds)}", 400);
            }

            var updateCourse = _mapper.Map(request, course);

            string? imageUrl = null;
            if (request.ThumbUrl != null)
            {
                imageUrl = await _cloudinaryService.UploadImageAsync(request.ThumbUrl);
                updateCourse.ThumbUrl = imageUrl;
            }

            updateCourse.UpdatedAt = DateTime.Now;
            updateCourse.UpdatedById = currentUserId;

            _unitOfWork.CourseRepository.Update(course, updateCourse);

            if (request.DepartmentIds != null)
            {
                var currentCourseDepartments = await _unitOfWork.CourseDepartmentRepository.FindAsync(cd => cd.CourseId == id);
                _unitOfWork.CourseDepartmentRepository.RemoveRange(currentCourseDepartments);

                if (request.DepartmentIds.Any())
                {
                    var newCourseDepartments = request.DepartmentIds.Select(d => new CourseDepartment { CourseId = id, DepartmentId = d }).ToList();
                    await _unitOfWork.CourseDepartmentRepository.AddRangeAsync(newCourseDepartments);
                }
            }

            if (request.PositionIds != null)
            {
                var currentCoursePositions = await _unitOfWork.CoursePositionRepository.FindAsync(cp => cp.CourseId == id);
                _unitOfWork.CoursePositionRepository.RemoveRange(currentCoursePositions);

                if (request.PositionIds.Any())
                {
                    var newCoursePositions = request.PositionIds.Select(p => new CoursePosition { CourseId = id, PositionId = p }).ToList();
                    await _unitOfWork.CoursePositionRepository.AddRangeAsync(newCoursePositions);
                }
            }

            var allCurrentUserCourses = await _unitOfWork.UserCourseRepository.FindAsync(uc => uc.CourseId == id);
            _unitOfWork.UserCourseRepository.RemoveRange(allCurrentUserCourses);

            var newUserCoursesToAssign = new List< UserCourse>();
            var assignedUserIds = new HashSet<string>();

            if (request.Optional == ConstantCourse.OPTIONAL_BATBUOC)
            {
                if (request.DepartmentIds != null && request.DepartmentIds.Any())
                {
                    if (request.PositionIds != null && request.PositionIds.Any())
                    {
                        var matchedUsersFromDeptPos = await _unitOfWork.UserRepository.GetQueryable()
                            .Where(u => u.DepartmentId.HasValue && request.DepartmentIds.Contains(u.DepartmentId.Value) &&
                                        u.PositionId.HasValue && request.PositionIds.Contains(u.PositionId.Value))
                            .Select(u => u.Id)
                            .ToListAsync();

                        foreach (var userId in matchedUsersFromDeptPos)
                        {
                            if (assignedUserIds.Add(userId))
                            {
                                newUserCoursesToAssign.Add(new UserCourse
                                {
                                    UserId = userId,
                                    CourseId = id,
                                    AssignedAt = DateTime.Now,
                                    IsMandatory = true,
                                    Status = ConstantStatus.ASSIGINED,
                                    CreatedAt = DateTime.Now,
                                    ModifiedAt = DateTime.Now,
                                });
                            }
                        }
                    }
                    else
                    {
                        var matchedUsersFromDeptOnly = await _unitOfWork.UserRepository.GetQueryable()
                            .Where(u => u.DepartmentId.HasValue && request.DepartmentIds.Contains(u.DepartmentId.Value))
                            .Select(u => u.Id)
                            .ToListAsync();

                        foreach (var userId in matchedUsersFromDeptOnly)
                        {
                            if (assignedUserIds.Add(userId))
                            {
                                newUserCoursesToAssign.Add(new UserCourse
                                {
                                    UserId = userId,
                                    CourseId = id,
                                    AssignedAt = DateTime.Now,
                                    IsMandatory = true,
                                    Status = ConstantStatus.ASSIGINED,
                                    CreatedAt = DateTime.Now,
                                    ModifiedAt = DateTime.Now,
                                });
                            }
                        }
                    }
                }

                if (request.StudentIds != null && request.StudentIds.Any())
                {
                    foreach (var userId in request.StudentIds)
                    {
                        if (assignedUserIds.Add(userId))
                        {
                            newUserCoursesToAssign.Add(new UserCourse
                            {
                                UserId = userId,
                                CourseId = id,
                                AssignedAt = DateTime.Now,
                                IsMandatory = true,
                                Status = ConstantStatus.ASSIGINED,
                                CreatedAt = DateTime.Now,
                                ModifiedAt = DateTime.Now,
                            });
                        }
                    }
                }
            }
            else
            {
                if (request.StudentIds != null && request.StudentIds.Any())
                {
                    foreach (var userId in request.StudentIds)
                    {
                        if (assignedUserIds.Add(userId))
                        {
                            newUserCoursesToAssign.Add(new UserCourse
                            {
                                UserId = userId,
                                CourseId = id,
                                AssignedAt = DateTime.Now,
                                IsMandatory = false,
                                Status = ConstantStatus.ASSIGINED,
                                CreatedAt = DateTime.Now,
                                ModifiedAt = DateTime.Now,
                            });
                        }
                    }
                }
            }

            if (newUserCoursesToAssign.Any())
            {
                await _unitOfWork.UserCourseRepository.AddRangeAsync(newUserCoursesToAssign);
            }

            await _unitOfWork.CompleteAsync();

            return course.Id;
        }

    }
}