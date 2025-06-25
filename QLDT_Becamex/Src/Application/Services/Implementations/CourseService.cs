using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Dtos;
using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Models;
using QLDT_Becamex.Src.Services.Interfaces;
using QLDT_Becamex.Src.Shared.Helpers;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace QLDT_Becamex.Src.Services.Implementations
{
    /// <summary>
    /// Triển khai dịch vụ quản lý khóa học.
    /// </summary>
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private const int CancelledStatusId = 5;

        /// <summary>
        /// Khởi tạo một phiên bản mới của lớp <see cref="CourseService"/>.
        /// </summary>
        /// <param name="mapper">Đối tượng AutoMapper để ánh xạ giữa các đối tượng.</param>
        /// <param name="unitOfWork">Đối tượng Unit of Work để quản lý các repositories và giao dịch cơ sở dữ liệu.</param>
        /// <param name="cloudinaryService">Dịch vụ Cloudinary để tải ảnh lên.</param>
        public CourseService(IMapper mapper, IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        /// <summary>
        /// Phương thức trợ giúp để lấy tất cả các ID phòng ban con một cách đệ quy.
        /// </summary>
        /// <param name="parentDepartmentId">ID của phòng ban cha.</param>
        /// <returns>Một danh sách các ID phòng ban con.</returns>
        private async Task<List<int>> GetAllChildDepartmentIds(int parentDepartmentId)
        {
            try
            {
                var childDepartmentIds = new List<int>();
                // Sử dụng Repository để truy vấn trực tiếp các phòng ban con
                var directChildren = await _unitOfWork.DepartmentRepository
                                                            .FindAsync(d => d.ParentId == parentDepartmentId);

                foreach (var child in directChildren)
                {
                    childDepartmentIds.Add(child.DepartmentId);
                    // Đệ quy để lấy các phòng ban con của phòng ban con này
                    childDepartmentIds.AddRange(await GetAllChildDepartmentIds(child.DepartmentId));
                }
                return childDepartmentIds;
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần thiết
                Console.WriteLine($"Lỗi khi lấy ID phòng ban con: {ex.Message}");
                return new List<int>(); // Trả về danh sách rỗng nếu có lỗi
            }
        }

        /// <summary>
        /// Tạo một khóa học mới.
        /// </summary>
        /// <param name="request">Đối tượng chứa thông tin yêu cầu tạo khóa học.</param>
        /// <returns>Đối tượng Result cho biết kết quả của thao tác.</returns>
        public async Task<Result> CreateAsync(CourseDtoRq request)
        {
            try
            {
                // Kiểm tra mã khóa học đã tồn tại chưa
                var codeExists = await _unitOfWork.CourseRepository.AnyAsync(c => c.Code == request.Code.Trim().ToLower());
                if (codeExists)
                {
                    return Result.Failure(
                        message: "Tạo khóa học thất bại",
                        error: "Mã khóa học đã tồn tại",
                        code: "EXISTS", // Thay đổi mã lỗi theo bảng: CONFLICT -> EXISTS
                        statusCode: 409
                    );
                }

                // Kiểm tra tên khóa học đã tồn tại chưa
                var nameExists = await _unitOfWork.CourseRepository.AnyAsync(c => c.Name == request.Name);
                if (nameExists)
                {
                    return Result.Failure(
                        message: "Tạo khóa học thất bại",
                        error: "Tên khóa học đã tồn tại",
                        code: "EXISTS", // Thay đổi mã lỗi theo bảng: CONFLICT -> EXISTS
                        statusCode: 409
                    );
                }

                // Kiểm tra StatusId có hợp lệ không
                if (request.StatusId.HasValue)
                {
                    var statusExists = await _unitOfWork.CourseStatusRepository.AnyAsync(s => s.Id == request.StatusId.Value);
                    if (!statusExists)
                    {
                        return Result.Failure(
                            message: "Tạo khóa học thất bại",
                            error: "Trạng thái khóa học không hợp lệ",
                            code: "INVALID", // Thay đổi mã lỗi theo bảng: INVALID_COURSE_STATUS -> INVALID
                            statusCode: 400
                        );
                    }
                }

                // Kiểm tra logic ngày tháng
                var dateValidationResult = ValidateDateLogic(request);
                if (!dateValidationResult.IsSuccess)
                {
                    return Result.Failure(
                        message: dateValidationResult.Message,
                        error: dateValidationResult.Errors.First(),
                        code: dateValidationResult.Code, // Mã lỗi đã được thay đổi trong ValidateDateLogic
                        statusCode: dateValidationResult.StatusCode
                    );
                }

                // Xử lý DepartmentIds bao gồm các phòng ban con
                // Kiểm tra DepartmentIds có hợp lệ không và thu thập tất cả các ID liên quan (cha và con)
                if (request.DepartmentIds != null && request.DepartmentIds.Any())
                {
                    var allDepartmentIdsToAssign = new HashSet<int>();
                    var invalidDepartments = new List<int>();

                    foreach (var deptId in request.DepartmentIds)
                    {
                        var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(deptId);
                        if (department == null)
                        {
                            invalidDepartments.Add(deptId);
                        }
                        else
                        {
                            allDepartmentIdsToAssign.Add(deptId); // Thêm phòng ban cha
                            // Lấy tất cả các phòng ban con và thêm vào danh sách
                            var children = await GetAllChildDepartmentIds(deptId);
                            foreach (var childId in children)
                            {
                                allDepartmentIdsToAssign.Add(childId); // Thêm tất cả các phòng ban con
                            }
                        }
                    }

                    if (invalidDepartments.Any())
                    {
                        return Result.Failure(
                            message: "Tạo khóa học thất bại",
                            error: $"Phòng ban không hợp lệ: {string.Join(", ", invalidDepartments)}",
                            code: "INVALID", // Thay đổi mã lỗi theo bảng: INVALID_DEPARTMENTS -> INVALID
                            statusCode: 400
                        );
                    }

                    // Cập nhật request.DepartmentIds với danh sách hoàn chỉnh bao gồm cả phòng ban cha và con
                    request.DepartmentIds = allDepartmentIdsToAssign.ToList();
                }

                // Kiểm tra PositionIds có hợp lệ không
                if (request.PositionIds != null && request.PositionIds.Any())
                {
                    var invalidPositions = new List<int>();
                    foreach (var posId in request.PositionIds)
                    {
                        var exists = await _unitOfWork.PositionRepostiory.AnyAsync(p => p.PositionId == posId);
                        if (!exists)
                        {
                            invalidPositions.Add(posId);
                        }
                    }

                    if (invalidPositions.Any())
                    {
                        return Result.Failure(
                            message: "Tạo khóa học thất bại",
                            error: $"Vị trí không hợp lệ: {string.Join(", ", invalidPositions)}",
                            code: "INVALID", // Thay đổi mã lỗi theo bảng: INVALID_POSITIONS -> INVALID
                            statusCode: 400
                        );
                    }
                }

                //Upload image
                string? imageUrl = null;
                if (request.ThumbUrl != null)
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(request.ThumbUrl);
                }

                // Tạo Course entity
                var course = _mapper.Map<Course>(request);
                course.Id = Guid.NewGuid().ToString();
                course.CreatedAt = DateTime.Now;
                course.ThumbUrl = imageUrl;

                // Thêm course vào database
                await _unitOfWork.CourseRepository.AddAsync(course);

                // Xử lý quan hệ many-to-many với Department
                if (request.DepartmentIds != null && request.DepartmentIds.Any())
                {
                    var courseDepartments = request.DepartmentIds.Select(deptId => new CourseDepartment
                    {
                        CourseId = course.Id,
                        DepartmentId = deptId
                    }).ToList();

                    await _unitOfWork.CourseDepartmentRepository.AddRangeAsync(courseDepartments);
                }

                // Xử lý quan hệ many-to-many với Position
                if (request.PositionIds != null && request.PositionIds.Any())
                {
                    var coursePositions = request.PositionIds.Select(posId => new CoursePosition
                    {
                        CourseId = course.Id,
                        PositionId = posId
                    }).ToList();

                    await _unitOfWork.CoursePositionRepository.AddRangeAsync(coursePositions);
                }


                // --- LOGIC MỚI ĐÃ SỬA ĐỔI: Xử lý UserCourse nếu Optional là "bắt buộc" ---
                if (request.Optional == ConstantCourse.OPTIONAL_BATBUOC)
                {
                    var usersToEnroll = new HashSet<string>();

                    // Chỉ xử lý nếu cả DepartmentIds và PositionIds đều được cung cấp
                    if (request.DepartmentIds != null && request.DepartmentIds.Any() &&
                        request.PositionIds != null && request.PositionIds.Any())
                    {
                        // Lấy tất cả người dùng thuộc các phòng ban đã gán và các vị trí đã gán
                        var matchingUsers = await _unitOfWork.UserRepository
                            .FindAsync(u => u.DepartmentId.HasValue && request.DepartmentIds.Contains(u.DepartmentId.Value) &&
                                             u.PositionId.HasValue && request.PositionIds.Contains(u.PositionId.Value));

                        foreach (var user in matchingUsers)
                        {
                            usersToEnroll.Add(user.Id);
                        }
                    }
                    // Nếu một trong hai danh sách (DepartmentIds hoặc PositionIds) trống,
                    // thì không có người dùng nào thỏa mãn điều kiện "thuộc cả hai",
                    // do đó usersToEnroll sẽ vẫn rỗng, điều này là hợp lý.

                    // Tạo các bản ghi UserCourse cho tất cả người dùng đã thu thập
                    if (usersToEnroll.Any())
                    {
                        var userCourses = usersToEnroll.Select(userId => new UserCourse
                        {
                            UserId = userId,
                            CourseId = course.Id,
                            AssignedAt = DateTime.Now,
                            IsMandatory = true, // Đánh dấu là bắt buộc
                            Status = ConstantStatus.ASSIGINED // Trạng thái ban đầu
                        }).ToList();

                        await _unitOfWork.UserCourseRepository.AddRangeAsync(userCourses);
                    }
                }
                // --- KẾT THÚC LOGIC MỚI ĐÃ SỬA ĐỔI ---


                await _unitOfWork.CompleteAsync();

                return Result.Success(
                    message: "Tạo khóa học thành công",
                    code: "SUCCESS", // Giữ nguyên SUCCESS cho trường hợp thành công
                    statusCode: 201
                );
            }
            catch (Exception ex)
            {
                return Result.Failure(
                    error: "Lỗi hệ thống: " + ex.Message,
                    code: "SYSTEM_ERROR", // Thay đổi mã lỗi theo bảng: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Cập nhật thông tin một khóa học hiện có.
        /// </summary>
        /// <param name="id">ID của khóa học cần cập nhật.</param>
        /// <param name="request">Đối tượng chứa thông tin yêu cầu cập nhật khóa học.</param>
        /// <returns>Đối tượng Result cho biết kết quả của thao tác.</returns>
        public async Task<Result> UpdateAsync(string id, CourseDtoRq request)
        {
            try
            {
                // Kiểm tra khóa học có tồn tại không
                var existingCourse = await _unitOfWork.CourseRepository.GetByIdAsync(id);
                if (existingCourse == null)
                {
                    return Result.Failure(
                        message: "Cập nhật khóa học thất bại",
                        error: "Khóa học không tồn tại",
                        code: "NOT_FOUND", // Thay đổi mã lỗi theo bảng: COURSE_NOT_FOUND -> NOT_FOUND
                        statusCode: 404
                    );
                }

                // Kiểm tra mã khóa học đã tồn tại chưa (ngoại trừ khóa học hiện tại)
                var codeExists = await _unitOfWork.CourseRepository.AnyAsync(c => c.Code == request.Code && c.Id != id);
                if (codeExists)
                {
                    return Result.Failure(
                        message: "Cập nhật khóa học thất bại",
                        error: "Mã khóa học đã tồn tại",
                        code: "EXISTS", // Thay đổi mã lỗi theo bảng: COURSE_CODE_EXISTS -> EXISTS
                        statusCode: 409
                    );
                }

                // Kiểm tra tên khóa học đã tồn tại chưa (ngoại trừ khóa học hiện tại)
                var nameExists = await _unitOfWork.CourseRepository.AnyAsync(c => c.Name == request.Name && c.Id != id);
                if (nameExists)
                {
                    return Result.Failure(
                        message: "Cập nhật khóa học thất bại",
                        error: "Tên khóa học đã tồn tại",
                        code: "EXISTS", // Thay đổi mã lỗi theo bảng: COURSE_NAME_EXISTS -> EXISTS
                        statusCode: 409
                    );
                }

                // Kiểm tra StatusId có hợp lệ không
                if (request.StatusId.HasValue)
                {
                    var statusExists = await _unitOfWork.CourseStatusRepository.AnyAsync(s => s.Id == request.StatusId.Value);
                    if (!statusExists)
                    {
                        return Result.Failure(
                            message: "Cập nhật khóa học thất bại",
                            error: "Trạng thái khóa học không hợp lệ",
                            code: "INVALID", // Thay đổi mã lỗi theo bảng: INVALID_COURSE_STATUS -> INVALID
                            statusCode: 400
                        );
                    }
                }

                // Kiểm tra logic ngày tháng
                var dateValidationResult = ValidateDateLogic(request);
                if (!dateValidationResult.IsSuccess)
                {
                    return Result.Failure(
                        message: dateValidationResult.Message,
                        error: dateValidationResult.Errors.First(),
                        code: dateValidationResult.Code, // Mã lỗi đã được thay đổi trong ValidateDateLogic
                        statusCode: dateValidationResult.StatusCode
                    );
                }

                // Xử lý DepartmentIds bao gồm các phòng ban con
                if (request.DepartmentIds != null && request.DepartmentIds.Any())
                {
                    var allDepartmentIdsToAssign = new HashSet<int>();
                    var invalidDepartments = new List<int>();

                    foreach (var deptId in request.DepartmentIds)
                    {
                        var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(deptId);
                        if (department == null)
                        {
                            invalidDepartments.Add(deptId);
                        }
                        else
                        {
                            allDepartmentIdsToAssign.Add(deptId);
                            var children = await GetAllChildDepartmentIds(deptId);
                            foreach (var childId in children)
                            {
                                allDepartmentIdsToAssign.Add(childId);
                            }
                        }
                    }

                    if (invalidDepartments.Any())
                    {
                        return Result.Failure(
                            message: "Cập nhật khóa học thất bại",
                            error: $"Phòng ban không hợp lệ: {string.Join(", ", invalidDepartments)}",
                            code: "INVALID", // Thay đổi mã lỗi theo bảng: INVALID_DEPARTMENTS -> INVALID
                            statusCode: 400
                        );
                    }
                    request.DepartmentIds = allDepartmentIdsToAssign.ToList();
                }

                // Kiểm tra PositionIds có hợp lệ không
                if (request.PositionIds != null && request.PositionIds.Any())
                {
                    var invalidPositions = new List<int>();
                    foreach (var posId in request.PositionIds)
                    {
                        var exists = await _unitOfWork.PositionRepostiory.AnyAsync(p => p.PositionId == posId);
                        if (!exists)
                        {
                            invalidPositions.Add(posId);
                        }
                    }

                    if (invalidPositions.Any())
                    {
                        return Result.Failure(
                            message: "Cập nhật khóa học thất bại",
                            error: $"Vị trí không hợp lệ: {string.Join(", ", invalidPositions)}",
                            code: "INVALID", // Thay đổi mã lỗi theo bảng: INVALID_POSITIONS -> INVALID
                            statusCode: 400
                        );
                    }
                }

                // Cập nhật thông tin course
                string? imageUrl = null;
                if (request.ThumbUrl != null)
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(request.ThumbUrl);
                }
                _mapper.Map(request, existingCourse);
                existingCourse.ModifiedAt = DateTime.Now;
                existingCourse.ThumbUrl = imageUrl;

                // Xử lý quan hệ many-to-many với Department
                if (request.DepartmentIds != null && request.DepartmentIds.Any())
                {
                    var existingCourseDepartments = await _unitOfWork.CourseDepartmentRepository
                        .FindAsync(cd => cd.CourseId == id);
                    _unitOfWork.CourseDepartmentRepository.RemoveRange(existingCourseDepartments);

                    var courseDepartments = request.DepartmentIds.Select(deptId => new CourseDepartment
                    {
                        CourseId = id,
                        DepartmentId = deptId
                    }).ToList();

                    await _unitOfWork.CourseDepartmentRepository.AddRangeAsync(courseDepartments);
                }
                else
                {
                    var existingCourseDepartments = await _unitOfWork.CourseDepartmentRepository
                        .FindAsync(cd => cd.CourseId == id);
                    _unitOfWork.CourseDepartmentRepository.RemoveRange(existingCourseDepartments);
                }

                // Xử lý quan hệ many-to-many với Position
                if (request.PositionIds != null && request.PositionIds.Any())
                {
                    var existingCoursePositions = await _unitOfWork.CoursePositionRepository
                        .FindAsync(cp => cp.CourseId == id);
                    _unitOfWork.CoursePositionRepository.RemoveRange(existingCoursePositions);

                    var coursePositions = request.PositionIds.Select(posId => new CoursePosition
                    {
                        CourseId = id,
                        PositionId = posId
                    }).ToList();

                    await _unitOfWork.CoursePositionRepository.AddRangeAsync(coursePositions);
                }
                else
                {
                    var existingCoursePositions = await _unitOfWork.CoursePositionRepository
                        .FindAsync(cp => cp.CourseId == id);
                    _unitOfWork.CoursePositionRepository.RemoveRange(existingCoursePositions);
                }

                // --- LOGIC MỚI BỊ THIẾU TRƯỚC ĐÂY: Xử lý UserCourse nếu Optional là "bắt buộc" ---
                // Điều quan trọng là phải kiểm tra trạng thái 'Optional' mới của yêu cầu cập nhật.
                if (request.Optional == ConstantCourse.OPTIONAL_BATBUOC)
                {
                    // Lấy danh sách UserCourse hiện tại của khóa học.
                    // Cần cẩn trọng khi xóa tất cả:
                    //   - Nếu bạn muốn bảo toàn trạng thái hiện tại của người dùng (InProgress, Completed),
                    //     thì thay vì xóa tất cả, bạn nên chỉ xóa những người dùng không còn thuộc
                    //     tiêu chí mới hoặc những người dùng đã được gán tự động.
                    //   - Tuy nhiên, yêu cầu của bạn là "thêm tất cả user nằm trong phòng ban đã gán khóa học
                    //     và position đã gán khóa học", ngụ ý cập nhật lại danh sách bắt buộc.
                    //     Vì vậy, việc xóa và thêm lại là cách đơn giản nhất để đảm bảo đồng bộ hoàn toàn.
                    var existingUserCourses = await _unitOfWork.UserCourseRepository
                        .FindAsync(uc => uc.CourseId == id && uc.IsMandatory); // Chỉ xóa các khóa học BẮT BUỘC cũ

                    _unitOfWork.UserCourseRepository.RemoveRange(existingUserCourses);

                    var usersToEnroll = new HashSet<string>();

                    if (request.DepartmentIds != null && request.DepartmentIds.Any() &&
                        request.PositionIds != null && request.PositionIds.Any())
                    {
                        var matchingUsers = await _unitOfWork.UserRepository
                            .FindAsync(u => u.DepartmentId.HasValue && request.DepartmentIds.Contains(u.DepartmentId.Value) &&
                                             u.PositionId.HasValue && request.PositionIds.Contains(u.PositionId.Value));

                        foreach (var user in matchingUsers)
                        {
                            usersToEnroll.Add(user.Id);
                        }
                    }

                    if (usersToEnroll.Any())
                    {
                        var userCourses = usersToEnroll.Select(userId => new UserCourse
                        {
                            UserId = userId,
                            CourseId = id, // Sử dụng id của khóa học đang được cập nhật
                            AssignedAt = DateTime.Now,
                            IsMandatory = true,
                            Status = "Assigned", // Trạng thái ban đầu khi gán lại
                            // Nếu muốn giữ trạng thái cũ cho người dùng đã học, cần phức tạp hơn:
                            // Ví dụ: Lấy UserCourse cũ nếu có và sao chép trạng thái/tiến độ.
                            // Nhưng với yêu cầu "thêm tất cả user", việc gán lại là phù hợp.
                        }).ToList();

                        await _unitOfWork.UserCourseRepository.AddRangeAsync(userCourses);
                    }
                }
                else // Nếu Optional KHÔNG phải là "bắt buộc" nữa (ví dụ: chuyển thành "tùy chọn" hoặc bỏ trống)
                {
                    // Xóa tất cả các bản ghi UserCourse được đánh dấu là bắt buộc cho khóa học này
                    var mandatoryUserCoursesToRemove = await _unitOfWork.UserCourseRepository
                        .FindAsync(uc => uc.CourseId == id && uc.IsMandatory);
                    _unitOfWork.UserCourseRepository.RemoveRange(mandatoryUserCoursesToRemove);
                }
                // --- KẾT THÚC LOGIC CẬP NHẬT ---

                _unitOfWork.CourseRepository.Update(existingCourse);
                await _unitOfWork.CompleteAsync();

                return Result.Success(
                    message: "Cập nhật khóa học thành công",
                    code: "SUCCESS", // Giữ nguyên SUCCESS cho trường hợp thành công
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                return Result.Failure(
                    error: "Lỗi hệ thống: " + ex.Message,
                    code: "SYSTEM_ERROR", // Thay đổi mã lỗi theo bảng: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }

        private Result ValidateDateLogic(CourseDtoRq request)
        {
            // Kiểm tra ngày đăng ký phải trước ngày bắt đầu khóa học
            if (request.RegistrationStartDate.HasValue && request.StartDate.HasValue)
            {
                if (request.RegistrationStartDate.Value >= request.StartDate.Value)
                {
                    return Result.Failure(
                        message: "Ngày bắt đầu đăng ký phải trước ngày bắt đầu khóa học",
                        error: "Ngày đăng ký không hợp lệ",
                        code: "INVALID", // Thay đổi mã lỗi theo bảng: INVALID_REGISTRATION_START_DATE -> INVALID
                        statusCode: 400
                    );
                }
            }

            // Kiểm tra ngày kết thúc đăng ký phải trước hoặc bằng ngày bắt đầu khóa học
            if (request.RegistrationClosingDate.HasValue && request.StartDate.HasValue)
            {
                if (request.RegistrationClosingDate.Value > request.StartDate.Value)
                {
                    return Result.Failure(
                        message: "Ngày kết thúc đăng ký phải trước hoặc bằng ngày bắt đầu khóa học",
                        error: "Ngày kết thúc đăng ký không hợp lệ",
                        code: "INVALID", // Thay đổi mã lỗi theo bảng: INVALID_REGISTRATION_END_DATE -> INVALID
                        statusCode: 400
                    );
                }
            }

            // Kiểm tra ngày bắt đầu đăng ký phải trước ngày kết thúc đăng ký
            if (request.RegistrationStartDate.HasValue && request.RegistrationClosingDate.HasValue)
            {
                if (request.RegistrationStartDate.Value >= request.RegistrationClosingDate.Value)
                {
                    return Result.Failure(
                        message: "Ngày bắt đầu đăng ký phải trước ngày kết thúc đăng ký",
                        error: "Khoảng thời gian đăng ký không hợp lệ",
                        code: "INVALID", // Thay đổi mã lỗi theo bảng: INVALID_REGISTRATION_PERIOD -> INVALID
                        statusCode: 400
                    );
                }
            }

            // Kiểm tra ngày bắt đầu khóa học phải trước ngày kết thúc
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                if (request.StartDate.Value >= request.EndDate.Value)
                {
                    return Result.Failure(
                        message: "Ngày bắt đầu khóa học phải trước ngày kết thúc",
                        error: "Thời gian khóa học không hợp lệ",
                        code: "INVALID", // Thay đổi mã lỗi theo bảng: INVALID_COURSE_PERIOD -> INVALID
                        statusCode: 400
                    );
                }
            }

            return Result.Success(
                message: "Validation thành công",
                code: "SUCCESS", // Giữ nguyên SUCCESS cho trường hợp thành công
                statusCode: 200
            );
        }

        public async Task<Result<CourseDto>> GetCourseAsync(string id)
        {
            try
            {
                var course = await _unitOfWork.CourseRepository.GetFirstOrDefaultAsync(
                    predicate: u => u.Id == id,
                    includes: q => q
                        .Include(c => c.CourseDepartments)
                        .Include(c => c.CoursePositions)
                        .Include(c => c.UserCourses)
                        .Include(c => c.Status)
                );
                if (course == null)
                {
                    return Result<CourseDto>.Failure(
                    error: "Get course fail!",
                    code: "NOT_FOUND",
                    statusCode: 404
                );
                }
                var courseDto = _mapper.Map<CourseDto>(course);

                return Result<CourseDto>.Success(

                   message: "Get course success!",
                   code: "SUCCESS",
                   statusCode: 200,
                   data: courseDto
               );

            }
            catch (Exception ex)
            {
                return Result<CourseDto>.Failure(
                   error: ex.Message,
                   message: "An error occurred while retrieving the user list.",
                   code: "SYSTEM_ERROR",
                   statusCode: 500
               );
            }
        }

        public async Task<Result<PagedResult<CourseDto>>> GetAllCoursesAsync(bool isDeleted, BaseQueryParam queryParam)
        {
            try
            {
                int totalItemCourse = await _unitOfWork.CourseRepository.CountAsync(c => isDeleted ? (c.StatusId != CancelledStatusId) : (c.StatusId == CancelledStatusId));

                Func<IQueryable<Course>, IOrderedQueryable<Course>>? orderByFunc = query =>
                {
                    bool isDesc = queryParam.SortType?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true;

                    return queryParam.SortField?.ToLower() switch
                    {
                        "name" => isDesc ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                        "created.at" => isDesc ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
                        _ => query.OrderBy(c => c.Name)
                    };
                };

                var courses = await _unitOfWork.CourseRepository.GetFlexibleAsync(
                    predicate: c => isDeleted ? (c.StatusId != CancelledStatusId) : (c.StatusId == CancelledStatusId),
                    orderBy: orderByFunc,
                    page: queryParam.Page,
                    pageSize: queryParam.Limit,
                    asNoTracking: true,
                    includes: q => q
                        .Include(c => c.CourseDepartments)!
                            .ThenInclude(cd => cd.Department)
                        .Include(c => c.CoursePositions)!
                            .ThenInclude(cp => cp.Position)
                        .Include(c => c.Status)
                );

                int effectiveLimit = queryParam.Limit;
                int totalPages = (int)Math.Ceiling((double)totalItemCourse / effectiveLimit);
                var pagedResultInfo = new Pagination
                {
                    TotalItems = totalItemCourse,
                    ItemsPerPage = effectiveLimit,
                    CurrentPage = queryParam.Page,
                    TotalPages = totalPages
                };
                var courseDtos = _mapper.Map<List<CourseDto>>(courses);

                var pagedResultData = new PagedResult<CourseDto>
                {
                    Items = courseDtos,
                    Pagination = pagedResultInfo
                };

                return Result<PagedResult<CourseDto>>.Success(
                    pagedResultData,
                    message: "Tải danh sách khóa học thành công.",
                    code: "SUCCESS",
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                return Result<PagedResult<CourseDto>>.Failure(
                    message: ex.Message,
                    error: "Lỗi! Vui lòng thử lại sau.",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        public async Task<Result<PagedResult<CourseDto>>> SearchCoursesAsync(BaseQueryParamFilter queryParam)
        {
            try
            {
                //Xây dựng predicate
                Expression<Func<Course, bool>>? predicate = null;

                //lọc theo StatusIds
                if (!string.IsNullOrEmpty(queryParam.StatusIds))
                {
                    var statusIds = queryParam.StatusIds.Split(',')
                        .Select(s => int.TryParse(s.Trim(), out var id) ? id : -1)
                        .Where(id => id != -1)
                        .ToList();
                    if (statusIds.Any())
                    {
                        Expression<Func<Course, bool>> statusPredicate = c => statusIds.Contains(c.Status.Id);
                        predicate = predicate == null ? statusPredicate : predicate.And(statusPredicate);
                    }
                }

                // Lọc theo DepartmentIds
                if (!string.IsNullOrEmpty(queryParam.DepartmentIds))
                {
                    var departmentIds = queryParam.DepartmentIds.Split(',')
                        .Select(s => int.TryParse(s.Trim(), out var id) ? id : -1)
                        .Where(id => id != -1)
                        .ToList();
                    if (departmentIds.Any())
                    {
                        Expression<Func<Course, bool>> deptPredicate = c =>
                            c.CourseDepartments != null && c.CourseDepartments.Any(cd => departmentIds.Contains(cd.DepartmentId));
                        predicate = predicate == null ? deptPredicate : predicate.And(deptPredicate);
                    }
                }

                // Lọc theo PositionIds
                if (!string.IsNullOrEmpty(queryParam.PositionIds))
                {
                    var positionIds = queryParam.PositionIds.Split(',')
                        .Select(s => int.TryParse(s.Trim(), out var id) ? id : -1)
                        .Where(id => id != -1)
                        .ToList();
                    if (positionIds.Any())
                    {
                        Expression<Func<Course, bool>> posPredicate = c =>
                            c.CoursePositions != null && c.CoursePositions.Any(cp => positionIds.Contains(cp.PositionId));
                        predicate = predicate == null ? posPredicate : predicate.And(posPredicate);
                    }
                }

                // Lọc theo CreatedAt
                if (!string.IsNullOrEmpty(queryParam.FromDate) || !string.IsNullOrEmpty(queryParam.ToDate))
                {
                    if (!DateTime.TryParseExact(queryParam.FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fromDate))
                        fromDate = DateTime.MinValue;
                    if (!DateTime.TryParseExact(queryParam.ToDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var toDate))
                        toDate = DateTime.MaxValue;
                    else
                        toDate = toDate.AddDays(1).AddTicks(-1); // Bao gồm cả ngày toDate

                    if (fromDate != DateTime.MinValue || toDate != DateTime.MaxValue)
                    {
                        Expression<Func<Course, bool>> datePredicate = c => c.CreatedAt >= fromDate && c.CreatedAt <= toDate;
                        predicate = predicate == null ? datePredicate : predicate.And(datePredicate);
                    }
                }

                // Xác định cách sắp xếp
                Func<IQueryable<Course>, IOrderedQueryable<Course>>? orderByFunc = query =>
                {
                    bool isDesc = queryParam.SortType?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true;
                    return queryParam.SortField?.ToLower() switch
                    {
                        "name" => isDesc ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                        "createdAt" => isDesc ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
                        _ => query.OrderBy(c => c.CreatedAt)
                    };
                };

                // Lấy queryable với include
                var query = _unitOfWork.CourseRepository.GetQueryable()
                    .Include(c => c.CourseDepartments)!
                        .ThenInclude(cd => cd.Department)
                    .Include(c => c.CoursePositions)!
                        .ThenInclude(cp => cp.Position)
                    .Include(c => c.Status)
                    .AsNoTracking();

                // Áp dụng predicate nếu có
                if (predicate != null)
                    query = query.Where(predicate);

                // Lọc theo keyword trên client-side
                List<Course> courses;
                if (!string.IsNullOrEmpty(queryParam.Keyword))
                {
                    string normalizedKeyword = StringHelper.RemoveDiacritics(queryParam.Keyword).ToLowerInvariant().Trim();
                    courses = await query.ToListAsync(); // Chuyển sang client-side
                    courses = courses
                        .Where(c =>
                            StringHelper.RemoveDiacritics(c.Name ?? "").ToLowerInvariant().Contains(normalizedKeyword) ||
                            StringHelper.RemoveDiacritics(c.Code ?? "").ToLowerInvariant().Contains(normalizedKeyword))
                        .ToList();
                }
                else
                {
                    courses = await query.ToListAsync();
                }

                // Lấy tổng số khóa học tìm được
                int totalItemCourse = courses.Count;

                // Áp dụng phân trang
                var pagedCourses = courses
                    .Skip((queryParam.Page - 1) * queryParam.Limit)
                    .Take(queryParam.Limit)
                    .ToList();

                // Tính toán phân trang
                int effectiveLimit = queryParam.Limit > 0 ? queryParam.Limit : 10; // Tránh chia cho 0
                int totalPages = (int)Math.Ceiling((double)totalItemCourse / effectiveLimit);
                var pagedResultInfo = new Pagination
                {
                    TotalItems = totalItemCourse,
                    ItemsPerPage = effectiveLimit,
                    CurrentPage = queryParam.Page,
                    TotalPages = totalPages
                };

                // Ánh xạ sang CourseDto
                var courseDtos = _mapper.Map<List<CourseDto>>(pagedCourses);

                // Tạo kết quả phân trang
                var PagedResultData = new PagedResult<CourseDto>
                {
                    Items = courseDtos,
                    Pagination = pagedResultInfo
                };

                if (PagedResultData == null || !PagedResultData.Items.Any())
                {
                    return Result<PagedResult<CourseDto>>.Failure(
                        error: "Không tìm thấy khóa học nào phù hợp với tiêu chí tìm kiếm.",
                        message: "Không tìm thấy khóa học nào phù hợp với tiêu chí tìm kiếm.",
                        code: "NOT_FOUND",
                        statusCode: 404
                    );
                }

                return Result<PagedResult<CourseDto>>.Success(
                    PagedResultData,
                    message: "Tìm kiếm khóa học thành công.",
                    code: "SUCCESS",
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] SearchCourseAsync: {ex}");
                return Result<PagedResult<CourseDto>>.Failure(
                    error: ex.Message,
                    message: "Đã xảy ra lỗi khi tìm kiếm khóa học.",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        public async Task<Result<bool>> DeleteCourseAsync(string id)
        {
            try
            {
                // Kiểm tra khóa học có tồn tại không
                var course = await _unitOfWork.CourseRepository.GetByIdAsync(id);
                if (course == null)
                {
                    return Result<bool>.Failure(
                        message: "Xóa khóa học thất bại",
                        error: "Khóa học không tồn tại",
                        code: "NOT_FOUND",
                        statusCode: 404
                    );
                }

                //Kiểm tra ngày xóa phải trước ngày bắt đầu đăng ký
                if(course.RegistrationStartDate.HasValue && DateTime.Now > course.RegistrationStartDate.Value)
                {
                    return Result<bool>.Failure(
                        message: "Xóa khóa học thất bại",
                        error: "Ngày xóa phải trước ngày bắt đầu đăng ký",
                        code: "INVALID",
                        statusCode: 400
                    );
                }

                course.StatusId = CancelledStatusId; //CancelledStatusId là trạng thái "Hủy"
                course.ModifiedAt = DateTime.Now;

                _unitOfWork.CourseRepository.Update(course);
                await _unitOfWork.CompleteAsync();

                return Result<bool>.Success(
                    message: "Xóa khóa học thành công",
                    code: "SUCCESS",
                    statusCode: 200,
                    data: true
                );
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(
                    error: "Lỗi hệ thống: " + ex.Message,
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }
    }
}