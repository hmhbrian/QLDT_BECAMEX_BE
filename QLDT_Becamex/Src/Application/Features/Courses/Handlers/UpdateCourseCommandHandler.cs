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
using QLDT_Becamex.Src.Shared.Helpers;
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


        /// <summary>
        /// Xử lý yêu cầu cập nhật thông tin một khóa học.
        /// </summary>
        /// <param name="command">Đối tượng command chứa ID khóa học và dữ liệu mới.</param>
        /// <param name="cancellationToken">Token để hủy bỏ tác vụ.</param>
        /// <returns>ID của khóa học đã được cập nhật.</returns>
        public async Task<string> Handle(UpdateCourseCommand command, CancellationToken cancellationToken)
        {
            // === BƯỚC 1: KHỞI TẠO VÀ LẤY THÔNG TIN BAN ĐẦU ===
            var id = command.Id; // Lấy ID của khóa học từ command
            var request = command.Request; // Lấy đối tượng request chứa dữ liệu cập nhật
            var (currentUserId, _) = _userService.GetCurrentUserAuthenticationInfo(); // Lấy ID của người dùng đang thực hiện hành động

            // Lấy thông tin khóa học hiện tại từ cơ sở dữ liệu
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(id);

            // === BƯỚC 2: XÁC THỰC DỮ LIỆU ĐẦU VÀO VÀ QUYỀN HẠN ===

            // Kiểm tra xem khóa học có tồn tại hay không
            if (course == null)
                throw new AppException("Khóa học không tồn tại", 404); // Lỗi 404 Not Found

            // Kiểm tra xem người dùng đã được xác thực (đăng nhập) chưa
            if (string.IsNullOrEmpty(currentUserId))
                throw new AppException("Bạn không có quyền cập nhật khóa học này", 403); // Lỗi 403 Forbidden

            //if (course.StartDate.HasValue && course.StartDate <= DateTimeHelper.GetVietnamTimeNow())
            //{
            //    throw new AppException("Khóa học đã bắt đầu, không thể chỉnh sửa", 403);
            //}

            // Kiểm tra mã khóa học có bị trùng với một khóa học khác không
            if (await _unitOfWork.CourseRepository.AnyAsync(c => c.Code == request.Code && c.Id != id))
                throw new AppException("Mã khóa học đã tồn tại", 409); // Lỗi 409 Conflict

            // Kiểm tra tên khóa học có bị trùng với một khóa học khác không
            if (await _unitOfWork.CourseRepository.AnyAsync(c => c.Name == request.Name && c.Id != id))
                throw new AppException("Tên khóa học đã tồn tại", 409); // Lỗi 409 Conflict
            

            // Kiểm tra xem ID trạng thái khóa học có hợp lệ không (nếu được cung cấp)
            if (request.StatusId.HasValue && !await _unitOfWork.CourseStatusRepository.AnyAsync(s => s.Id == request.StatusId.Value))
                throw new AppException("Trạng thái khóa học không hợp lệ", 400); // Lỗi 400 Bad Request

            // Kiểm tra xem ID loại khóa học có hợp lệ không (nếu được cung cấp)
            if (request.CategoryId.HasValue && !await _unitOfWork.CourseCategoryRepository.AnyAsync(s => s.Id == request.CategoryId.Value))
                throw new AppException("Loại khóa học không hợp lệ", 400); // Lỗi 400 Bad Request

            // === BƯỚC 3: XỬ LÝ VÀ XÁC THỰC CÁC DANH SÁCH ID LIÊN QUAN ===

            // Xử lý danh sách phòng ban
            if (request.DepartmentIds != null && request.DepartmentIds.Any())
            {
                // Lấy các ID phòng ban hợp lệ từ DB
                var validDeptIds = await _unitOfWork.DepartmentRepository.GetQueryable()
                    .Where(d => request.DepartmentIds.Contains(d.DepartmentId))
                    .Select(d => d.DepartmentId)
                    .ToListAsync();

                // Tìm các ID không hợp lệ (có trong request nhưng không có trong DB)
                var invalidDepts = request.DepartmentIds.Except(validDeptIds).ToList();
                if (invalidDepts.Any())
                    throw new AppException($"Phòng ban không hợp lệ: {string.Join(", ", invalidDepts)}", 400);

                // Mở rộng danh sách phòng ban: bao gồm cả các phòng ban con của các phòng ban đã chọn
                var allDepartmentIdsIncludingChildren = new HashSet<int>(validDeptIds);
                foreach (var deptId in validDeptIds)
                {
                    var children = await _departmentService.GetAllChildDepartmentIds(deptId);
                    foreach (var child in children)
                        allDepartmentIdsIncludingChildren.Add(child);
                }
                // Cập nhật lại danh sách ID phòng ban trong request với danh sách đã mở rộng
                request.DepartmentIds = allDepartmentIdsIncludingChildren.ToList();
            }

            // Xử lý danh sách cấp bậc
            if (request.ELevelIds != null && request.ELevelIds.Any())
            {
                var validPosIds = await _unitOfWork.EmployeeLevelRepository.GetQueryable()
                    .Where(p => request.ELevelIds.Contains(p.ELevelId))
                    .Select(p => p.ELevelId)
                    .ToListAsync();

                var invalidELevels = request.ELevelIds.Except(validPosIds).ToList();
                if (invalidELevels.Any())
                    throw new AppException($"Cấp bậc không hợp lệ: {string.Join(", ", invalidELevels)}", 400);
            }

            // Xử lý danh sách học viên (chỉ kiểm tra nếu đây không phải khóa học bắt buộc gán theo tiêu chí)
            if (request.Optional != ConstantCourse.OPTIONAL_BATBUOC && request.UserIds != null && request.UserIds.Any())
            {
                var validUserIds = await _unitOfWork.UserRepository.GetQueryable()
                    .Where(u => request.UserIds.Contains(u.Id))
                    .Select(u => u.Id)
                    .ToListAsync();

                var invalidUserIds = request.UserIds.Except(validUserIds).ToList();
                if (invalidUserIds.Any())
                    throw new AppException($"Người dùng không hợp lệ: {string.Join(", ", invalidUserIds)}", 400);
            }

            // === BƯỚC 4: CẬP NHẬT THÔNG TIN CHÍNH CỦA KHÓA HỌC ===

            // Sử dụng AutoMapper để ánh xạ dữ liệu từ request DTO sang đối tượng Course entity


            // Xử lý việc upload ảnh thumbnail (nếu có)
            string? imageUrl = null;

            if (request.ThumbUrl != null)
            {
                // Lấy public id từ ảnh cũ 
                if (course.ThumbUrl != null)
                {
                    var oldPublicId = ExtractPublicId(course.ThumbUrl);
                    if (!string.IsNullOrEmpty(oldPublicId))
                    {
                        await _cloudinaryService.DeleteImageAsync(oldPublicId);
                    }
                }

                // Upload ảnh mới
                imageUrl = await _cloudinaryService.UploadImageAsync(request.ThumbUrl);
            }
            var updateCourse = _mapper.Map(request, course);
            if (!string.IsNullOrEmpty(imageUrl))
            {
                updateCourse.ThumbUrl = imageUrl;
            }

            // Ghi nhận lại thông tin về lần cập nhật này
            updateCourse.UpdatedAt = DateTimeHelper.GetVietnamTimeNow();
            updateCourse.UpdatedById = currentUserId;

            if (request.Name != null && course.Name != request.Name)
                updateCourse.NormalizeCourseName = StringHelper.RemoveDiacritics(request.Name).ToUpperInvariant().Replace(" ", "");

            // Đánh dấu đối tượng course cần được cập nhật trong Unit of Work
            _unitOfWork.CourseRepository.Update(course, updateCourse);

            // === BƯỚC 5: CẬP NHẬT CÁC BẢNG QUAN HỆ MANY-TO-MANY ===

            // Cập nhật liên kết Khóa học - Phòng ban
            if (request.DepartmentIds != null)
            {
                var currentCourseDepartments = await _unitOfWork.CourseDepartmentRepository.FindAsync(cd => cd.CourseId == id);
                var currentDeptIds = currentCourseDepartments.Select(cd => cd.DepartmentId).ToHashSet();
                var incomingDeptIds = request.DepartmentIds.ToHashSet();

                if (!currentDeptIds.SetEquals(incomingDeptIds))
                {
                    _unitOfWork.CourseDepartmentRepository.RemoveRange(currentCourseDepartments);

                    if (incomingDeptIds.Any())
                    {
                        var newCourseDepartments = incomingDeptIds
                            .Select(d => new CourseDepartment { CourseId = id, DepartmentId = d })
                            .ToList();
                        await _unitOfWork.CourseDepartmentRepository.AddRangeAsync(newCourseDepartments);
                    }
                }
            }

            // Cập nhật liên kết Khóa học - Chức vụ
            if (request.ELevelIds != null)
            {
                var currentCourseELevels = await _unitOfWork.CourseELevelRepository.FindAsync(cp => cp.CourseId == id);
                var currentELevelIds = currentCourseELevels.Select(cp => cp.ELevelId).ToHashSet();
                var incomingELevelIds = request.ELevelIds.ToHashSet();

                if (!currentELevelIds.SetEquals(incomingELevelIds))
                {
                    _unitOfWork.CourseELevelRepository.RemoveRange(currentCourseELevels);

                    if (incomingELevelIds.Any())
                    {
                        var newCourseELevels = incomingELevelIds
                            .Select(p => new CourseELevel { CourseId = id, ELevelId = p })
                            .ToList();
                        await _unitOfWork.CourseELevelRepository.AddRangeAsync(newCourseELevels);
                    }
                }
            }

            // === BƯỚC 6: GÁN LẠI KHÓA HỌC CHO HỌC VIÊN ===

            var newUserCoursesToAssign = new List<UserCourse>();
            var assignedUserIds = new HashSet<string>();

            if (request.Optional == ConstantCourse.OPTIONAL_BATBUOC)
            {
                if (request.DepartmentIds?.Any() == true && request.DepartmentIds != null)
                {
                    if (request.ELevelIds?.Any() == true && request.ELevelIds != null)
                    {
                        var matchedUsers = await _unitOfWork.UserRepository.GetQueryable()
                            .Where(u => u.DepartmentId.HasValue && request.DepartmentIds.Contains(u.DepartmentId.Value)
                                     && u.ELevelId.HasValue && request.ELevelIds.Contains(u.ELevelId.Value))
                            .Select(u => u.Id)
                            .ToListAsync();

                        foreach (var userId in matchedUsers)
                        {
                            if (assignedUserIds.Add(userId))
                            {
                                newUserCoursesToAssign.Add(new UserCourse
                                {
                                    CourseId = id,
                                    UserId = userId,
                                    Optional = 1,
                                    IsMandatory = true
                                });
                            }
                        }
                    }
                    else
                    {
                        var matchedUsers = await _unitOfWork.UserRepository.GetQueryable()
                            .Where(u => u.DepartmentId.HasValue && request.DepartmentIds.Contains(u.DepartmentId.Value))
                            .Select(u => u.Id)
                            .ToListAsync();

                        foreach (var userId in matchedUsers)
                        {
                            if (assignedUserIds.Add(userId))
                            {
                                newUserCoursesToAssign.Add(new UserCourse
                                {
                                    CourseId = id,
                                    UserId = userId,
                                    Optional = 1,
                                    IsMandatory = true
                                });
                            }
                        }
                    }
                }

                if (request.UserIds?.Any() == true && request.UserIds != null)
                {
                    foreach (var userId in request.UserIds)
                    {
                        if (assignedUserIds.Add(userId))
                        {
                            newUserCoursesToAssign.Add(new UserCourse
                            {
                                CourseId = id,
                                UserId = userId,
                                IsMandatory = true,
                                Optional = 1
                            });
                        }
                    }
                }

                // So sánh và cập nhật nếu có thay đổi học viên
                var currentUserCourses = await _unitOfWork.UserCourseRepository.FindAsync(uc => uc.CourseId == id && uc.Optional == 1);
                var currentUserIds = currentUserCourses.Select(uc => uc.UserId).ToHashSet();
                var newUserIds = newUserCoursesToAssign.Select(uc => uc.UserId).ToHashSet();

                if (newUserIds.Count > 0 && !currentUserIds.SetEquals(newUserIds))
                {
                    _unitOfWork.UserCourseRepository.RemoveRange(currentUserCourses);
                    if (newUserCoursesToAssign.Any())
                    {
                        await _unitOfWork.UserCourseRepository.AddRangeAsync(newUserCoursesToAssign);
                    }
                }
            }
            else
            {
                if (request.UserIds?.Any() == true && request.UserIds != null)
                {
                    foreach (var userId in request.UserIds)
                    {
                        if (assignedUserIds.Add(userId))
                        {
                            newUserCoursesToAssign.Add(new UserCourse
                            {
                                CourseId = id,
                                UserId = userId,
                                IsMandatory = false,
                                Optional = 1
                            });
                        }
                    }
                }

                // So sánh và cập nhật nếu có thay đổi học viên
                var currentUserCourses = await _unitOfWork.UserCourseRepository.FindAsync(uc => uc.CourseId == id && uc.Optional == 1);
                var currentUserIds = currentUserCourses.Select(uc => uc.UserId).ToHashSet();
                var newUserIds = newUserCoursesToAssign.Select(uc => uc.UserId).ToHashSet();

                if (!currentUserIds.SetEquals(newUserIds))
                {
                    _unitOfWork.UserCourseRepository.RemoveRange(currentUserCourses);
                    if (newUserCoursesToAssign.Any())
                    {
                        await _unitOfWork.UserCourseRepository.AddRangeAsync(newUserCoursesToAssign);
                    }
                }
            }



            // === BƯỚC 7: LƯU THAY ĐỔI ===
            await _unitOfWork.CompleteAsync();

            // === BƯỚC 8: TRẢ KẾT QUẢ ===
            return course.Id;

        }


        private string? ExtractPublicId(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return null;

            // Example: https://res.cloudinary.com/ttdn/image/upload/v1752635597/avatars/lcin0x1qjmmkybabgzc8.jpg
            try
            {
                var uri = new Uri(imageUrl);
                var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                // Lấy tất cả phần sau "upload/" và bỏ phần đuôi .jpg
                var uploadIndex = Array.IndexOf(segments, "upload");
                if (uploadIndex >= 0 && uploadIndex + 1 < segments.Length)
                {
                    var publicIdParts = segments.Skip(uploadIndex + 1).ToArray(); // ["v1752635597", "avatars", "lcin0x1qjmmkybabgzc8.jpg"]

                    // Bỏ version nếu có (v...)
                    if (publicIdParts[0].StartsWith("v"))
                    {
                        publicIdParts = publicIdParts.Skip(1).ToArray();
                    }

                    // Ghép lại path, rồi bỏ đuôi
                    var fullPath = string.Join('/', publicIdParts);
                    var lastDot = fullPath.LastIndexOf('.');
                    if (lastDot > 0)
                    {
                        fullPath = fullPath.Substring(0, lastDot); // Bỏ .jpg
                    }

                    return fullPath; // avatars/lcin0x1qjmmkybabgzc8
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

    }
}