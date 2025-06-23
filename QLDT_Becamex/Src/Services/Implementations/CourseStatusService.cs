using AutoMapper;
using QLDT_Becamex.Src.Models;
using QLDT_Becamex.Src.UnitOfWork;
using QLDT_Becamex.Src.Services.Interfaces;
using System;
using System.Collections.Generic; // Thêm dòng này để sử dụng IEnumerable và List
using System.Linq;
using QLDT_Becamex.Src.Dtos; // Thêm dòng này để sử dụng LINQ

namespace QLDT_Becamex.Src.Services.Implementations
{
    /// <summary>
    /// Triển khai dịch vụ quản lý trạng thái khóa học.
    /// </summary>
    public class CourseStatusService : ICourseStatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Khởi tạo một phiên bản mới của lớp <see cref="CourseStatusService"/>.
        /// </summary>
        /// <param name="unitOfWork">Đối tượng Unit of Work để quản lý các repositories và giao dịch cơ sở dữ liệu.</param>
        /// <param name="mapper">Đối tượng AutoMapper để ánh xạ giữa các đối tượng.</param>
        public CourseStatusService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy tất cả các trạng thái khóa học.
        /// </summary>
        /// <returns>Một đối tượng Result chứa danh sách các trạng thái khóa học hoặc thông báo lỗi.</returns>
        public async Task<Result<IEnumerable<CourseStatusDto>>> GetAllAsync()
        {
            try
            {
                var list = await _unitOfWork.CourseStatusRepository.GetAllAsync();
                var mapped = _mapper.Map<IEnumerable<CourseStatusDto>>(list);
                return Result<IEnumerable<CourseStatusDto>>.Success(data: mapped, message: "Lấy danh sách trạng thái khóa học thành công", code: "SUCCESS", statusCode: 200);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<CourseStatusDto>>.Failure(
                    error: "Lỗi hệ thống: " + ex.Message,
                    message: "Đã xảy ra lỗi khi truy xuất trạng thái khóa học.",
                    code: "SYSTEM_ERROR", // Mã lỗi chung: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Tạo một trạng thái khóa học mới.
        /// </summary>
        /// <param name="rq">Đối tượng yêu cầu chứa thông tin để tạo trạng thái khóa học.</param>
        /// <returns>Một đối tượng Result cho biết kết quả của thao tác.</returns>
        public async Task<ApiResponse> CreateAsync(CourseStatusDtoRq rq)
        {
            try
            {
                var existing = await _unitOfWork.CourseStatusRepository.GetFirstOrDefaultAsync(
                    predicate: us => us.Name.ToLower() == rq.Name.ToLower()
                );

                if (existing != null)
                {
                    return ApiResponse.Failure(
                        error: "Trạng thái khóa học với tên này đã tồn tại.",
                        message: "Tạo trạng thái khóa học thất bại.",
                        code: "EXISTS", // Mã lỗi chung: CONFLICT -> EXISTS (theo bảng của bạn 409 là EXISTS)
                        statusCode: 409
                    );
                }

                var courseStatus = new CourseStatus { Name = rq.Name };

                await _unitOfWork.CourseStatusRepository.AddAsync(courseStatus);
                await _unitOfWork.CompleteAsync();

                return ApiResponse.Success(message: "Tạo trạng thái khóa học thành công", code: "SUCCESS", statusCode: 201);
            }
            catch (Exception ex)
            {
                return ApiResponse.Failure(
                    error: "Lỗi hệ thống: " + ex.Message,
                    message: "Đã xảy ra lỗi khi tạo trạng thái khóa học.",
                    code: "SYSTEM_ERROR", // Mã lỗi chung: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Cập nhật một trạng thái khóa học hiện có.
        /// </summary>
        /// <param name="id">ID của trạng thái khóa học cần cập nhật.</param>
        /// <param name="rq">Đối tượng yêu cầu chứa thông tin cập nhật cho trạng thái khóa học.</param>
        /// <returns>Một đối tượng Result cho biết kết quả của thao tác.</returns>
        public async Task<ApiResponse> UpdateAsync(int id, CourseStatusDtoRq rq)
        {
            try
            {
                var entity = await _unitOfWork.CourseStatusRepository.GetByIdAsync(id);
                if (entity == null)
                {
                    return ApiResponse.Failure(
                        error: "Không tìm thấy trạng thái khóa học.",
                        message: "Cập nhật trạng thái khóa học thất bại.",
                        code: "NOT_FOUND", // Mã lỗi chung: NOT_FOUND
                        statusCode: 404
                    );
                }

                // Kiểm tra xem tên mới có trùng với tên của một trạng thái khác không (ngoại trừ chính nó)
                var nameConflict = await _unitOfWork.CourseStatusRepository.GetFirstOrDefaultAsync(
                    predicate: cs => cs.Name.ToLower() == rq.Name.ToLower() && cs.Id != id
                );
                if (nameConflict != null)
                {
                    return ApiResponse.Failure(
                        error: "Tên trạng thái khóa học đã tồn tại.",
                        message: "Cập nhật trạng thái khóa học thất bại.",
                        code: "EXISTS", // Mã lỗi chung: CONFLICT -> EXISTS
                        statusCode: 409
                    );
                }

                entity.Name = rq.Name;
                _unitOfWork.CourseStatusRepository.Update(entity);
                await _unitOfWork.CompleteAsync();

                return ApiResponse.Success(message: "Cập nhật trạng thái khóa học thành công", code: "SUCCESS", statusCode: 200);
            }
            catch (Exception ex)
            {
                return ApiResponse.Failure(
                    error: "Lỗi hệ thống: " + ex.Message,
                    message: "Đã xảy ra lỗi khi cập nhật trạng thái khóa học.",
                    code: "SYSTEM_ERROR", // Mã lỗi chung: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Xóa một hoặc nhiều trạng thái khóa học.
        /// </summary>
        /// <param name="ids">Danh sách các ID của trạng thái khóa học cần xóa.</param>
        /// <returns>Một đối tượng Result cho biết kết quả của thao tác.</returns>
        public async Task<ApiResponse> DeleteAsync(List<int> ids)
        {
            try
            {
                // Lấy tất cả các entity cần xóa
                var entities = await _unitOfWork.CourseStatusRepository.FindAsync(cs => ids.Contains(cs.Id));

                if (entities == null || !entities.Any())
                {
                    return ApiResponse.Failure(
                        error: "Không tìm thấy trạng thái khóa học nào với các ID được cung cấp.",
                        message: "Xóa trạng thái khóa học thất bại.",
                        code: "NOT_FOUND", // Mã lỗi chung: NOT_FOUND
                        statusCode: 404
                    );
                }

                _unitOfWork.CourseStatusRepository.RemoveRange(entities);
                await _unitOfWork.CompleteAsync();

                return ApiResponse.Success(message: "Xóa trạng thái khóa học thành công", code: "SUCCESS", statusCode: 200);
            }
            catch (Exception ex)
            {
                return ApiResponse.Failure(
                    error: "Lỗi hệ thống: " + ex.Message,
                    message: "Đã xảy ra lỗi khi xóa trạng thái khóa học.",
                    code: "SYSTEM_ERROR", // Mã lỗi chung: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }
    }
}