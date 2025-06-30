using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Commands;
using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Shared.Helpers;

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers
{
    public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IBaseService _baseService;
        public UpdateCourseCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryService cloudinaryService, IBaseService baseService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
            _baseService = baseService;
        }

        public async Task<string> Handle(UpdateCourseCommand command, CancellationToken cancellationToken)
        {
            var id = command.Id;
            var request = command.Request;
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(id);
            if (course == null)
                throw new AppException("Khóa học không tồn tại", 404);

            if (await _unitOfWork.CourseRepository.AnyAsync(c => c.Code == request.Code && c.Id != id))
                throw new AppException("Mã khóa học đã tồn tại", 409);

            if (await _unitOfWork.CourseRepository.AnyAsync(c => c.Name == request.Name && c.Id != id))
                throw new AppException("Tên khóa học đã tồn tại", 409);

            if (request.StatusId.HasValue)
            {
                var statusExists = await _unitOfWork.CourseStatusRepository.AnyAsync(s => s.Id == request.StatusId);
                if (!statusExists)
                    throw new AppException("Trạng thái khóa học không hợp lệ", 400);
            }
            if (request.CategoryId.HasValue)
            {
                var CategoryExists = await _unitOfWork.CourseCategoryRepository.AnyAsync(s => s.Id == request.CategoryId.Value);
                if (!CategoryExists)
                    throw new AppException("Loại khóa học không hợp lệ", 400);
            }

            if (request.LecturerId.HasValue)
            {
                var LecturerExists = await _unitOfWork.LecturerRepository.AnyAsync(s => s.Id == request.LecturerId.Value);
                if (!LecturerExists)
                    throw new AppException("Giảng viên khóa học không hợp lệ", 400);
            }

            if (request.DepartmentIds != null && request.DepartmentIds.Any())
            {
                var allIds = new HashSet<int>();
                var invalid = new List<int>();
                foreach (var deptId in request.DepartmentIds)
                {
                    var dept = await _unitOfWork.DepartmentRepository.GetByIdAsync(deptId);
                    if (dept == null) invalid.Add(deptId);
                    else
                    {
                        allIds.Add(deptId);
                        var children = await _baseService.GetAllChildDepartmentIds(deptId);
                        foreach (var child in children) allIds.Add(child);
                    }
                
                }
                if (invalid.Any())
                    throw new AppException($"Phòng ban không hợp lệ: {string.Join(", ", invalid)}", 400);
                request.DepartmentIds = allIds.ToList();
            }

            if (request.PositionIds != null && request.PositionIds.Any())
            {
                var invalid = new List<int>();
                foreach (var posId in request.PositionIds)
                {
                    var exists = await _unitOfWork.PositionRepository.AnyAsync(p => p.PositionId == posId);
                    if (!exists) invalid.Add(posId);
                }
                if (invalid.Any())
                    throw new AppException($"Vị trí không hợp lệ: {string.Join(", ", invalid)}", 400);
            }
            _mapper.Map(request, course);
            string? imageUrl = null;
            if (request.ThumbUrl != null)
            {
                imageUrl = await _cloudinaryService.UploadImageAsync(request.ThumbUrl);
                
            }
            course.ThumbUrl = imageUrl;
            
            course.ModifiedAt = DateTime.Now;
            _unitOfWork.CourseRepository.Update(course);

            // Replace course-department
            var oldDeps = await _unitOfWork.CourseDepartmentRepository.FindAsync(cd => cd.CourseId == id);
            _unitOfWork.CourseDepartmentRepository.RemoveRange(oldDeps);
            if (request.DepartmentIds != null && request.DepartmentIds.Any())
            {
                var deps = request.DepartmentIds.Select(d => new CourseDepartment { CourseId = id, DepartmentId = d });
                await _unitOfWork.CourseDepartmentRepository.AddRangeAsync(deps);
            }

            // Replace course-position
            var oldPos = await _unitOfWork.CoursePositionRepository.FindAsync(cp => cp.CourseId == id);
            _unitOfWork.CoursePositionRepository.RemoveRange(oldPos);
            if (request.PositionIds != null && request.PositionIds.Any())
            {
                var pos = request.PositionIds.Select(p => new CoursePosition { CourseId = id, PositionId = p });
                await _unitOfWork.CoursePositionRepository.AddRangeAsync(pos);
            }

            // Replace user-course nếu là bắt buộc
            var oldUsers = await _unitOfWork.UserCourseRepository.FindAsync(uc => uc.CourseId == id && uc.IsMandatory);
            _unitOfWork.UserCourseRepository.RemoveRange(oldUsers);
            if (request.Optional == ConstantCourse.OPTIONAL_BATBUOC)
            {
                var userSet = new HashSet<string>();
                if (request.DepartmentIds != null && request.PositionIds != null &&
                    request.DepartmentIds.Any() && request.PositionIds.Any())
                {
                    var users = await _unitOfWork.UserRepository.FindAsync(u =>
                        u.DepartmentId.HasValue && request.DepartmentIds.Contains(u.DepartmentId.Value) &&
                        u.PositionId.HasValue && request.PositionIds.Contains(u.PositionId.Value));
                    foreach (var user in users) userSet.Add(user.Id);
                }
                var userCourses = userSet.Select(uid => new UserCourse
                {
                    UserId = uid,
                    CourseId = id,
                    AssignedAt = DateTime.Now,
                    IsMandatory = true,
                    Status = ConstantStatus.ASSIGINED
                });
                await _unitOfWork.UserCourseRepository.AddRangeAsync(userCourses);
            }

            await _unitOfWork.CompleteAsync();

            return course.Id;
        }
    }
}
