using AutoMapper;
using QLDT_Becamex.Src.Services.Interfaces;
using QLDT_Becamex.Src.Models; // Model UserStatus của bạn
using QLDT_Becamex.Src.UnitOfWork;
using System; // Thêm để sử dụng Exception
using System.Collections.Generic; // Thêm để sử dụng IEnumerable và List
using System.Linq;
using QLDT_Becamex.Src.Dtos; // Thêm để sử dụng LINQ

namespace QLDT_Becamex.Src.Services.Implementations
{
    /// <summary>
    /// Triển khai dịch vụ quản lý trạng thái người dùng.
    /// </summary>
    public class UserStatusService : IUserStatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Khởi tạo một phiên bản mới của lớp <see cref="UserStatusService"/>.
        /// </summary>
        /// <param name="unitOfWork">Đối tượng Unit of Work để quản lý các repositories và giao dịch cơ sở dữ liệu.</param>
        /// <param name="mapper">Đối tượng AutoMapper để ánh xạ giữa các đối tượng.</param>
        public UserStatusService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo một trạng thái người dùng mới.
        /// </summary>
        /// <param name="rq">Đối tượng chứa thông tin yêu cầu tạo trạng thái người dùng.</param>
        /// <returns>Đối tượng Result chứa trạng thái người dùng đã tạo hoặc lỗi nếu thất bại.</returns>
        public async Task<Result<UserStatusDto>> CreateAsync(UserStatusDtoRq rq)
        {
            try
            {
                // Kiểm tra xem trạng thái đã tồn tại chưa
                var existingStatus = await _unitOfWork.UserStatusRepostiory.GetFirstOrDefaultAsync(
                    predicate: us => us.Name.ToLower() == rq.Name.ToLower()
                );

                if (existingStatus != null)
                {
                    return Result<UserStatusDto>.Failure(
                        error: "Trạng thái người dùng với tên này đã tồn tại.",
                        message: "Tạo trạng thái người dùng thất bại.",
                        code: "EXISTS", // Thay đổi mã lỗi theo bảng: USER_STATUS_ALREADY_EXISTS -> EXISTS
                        statusCode: 409 // Conflict
                    );
                }

                var userStatus = _mapper.Map<UserStatus>(rq);

                await _unitOfWork.UserStatusRepostiory.AddAsync(userStatus);
                await _unitOfWork.CompleteAsync();

                var userStatusDto = _mapper.Map<UserStatusDto>(userStatus);
                return Result<UserStatusDto>.Success(
                    data: userStatusDto,
                    message: "Trạng thái người dùng đã được tạo thành công.",
                    code: "SUCCESS", // Thay đổi mã lỗi theo bảng: USER_STATUS_CREATED -> SUCCESS
                    statusCode: 201
                );
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết
                Console.WriteLine($"[ERROR] UserStatusService.CreateAsync: {ex.Message}");
                return Result<UserStatusDto>.Failure(
                    error: ex.Message,
                    message: "Đã xảy ra lỗi khi tạo trạng thái người dùng.",
                    code: "SYSTEM_ERROR", // Mã lỗi chung: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Xóa một hoặc nhiều trạng thái người dùng theo ID.
        /// </summary>
        /// <param name="ids">Danh sách các ID của trạng thái người dùng cần xóa.</param>
        /// <returns>Đối tượng Result cho biết kết quả của thao tác.</returns>
        public async Task<ApiResponse> DeleteAsync(List<int> ids)
        {
            try
            {
                // Lấy tất cả các entity cần xóa
                var entities = await _unitOfWork.UserStatusRepostiory.FindAsync(cs => ids.Contains(cs.Id));

                if (entities == null || !entities.Any())
                {
                    return ApiResponse.Failure(
                        error: "Không tìm thấy trạng thái người dùng nào với các ID được cung cấp.",
                        message: "Xóa trạng thái người dùng thất bại.",
                        code: "NOT_FOUND", // Mã lỗi chung: NOT_FOUND
                        statusCode: 404
                    );
                }

                _unitOfWork.UserStatusRepostiory.RemoveRange(entities);
                await _unitOfWork.CompleteAsync();

                return ApiResponse.Success(message: "Xóa trạng thái người dùng thành công", code: "SUCCESS", statusCode: 200);
            }
            catch (Exception ex)
            {
                return ApiResponse.Failure(
                    error: ex.Message,
                    message: "Đã xảy ra lỗi khi xóa trạng thái người dùng.",
                    code: "SYSTEM_ERROR", // Mã lỗi chung: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }


        /// <summary>
        /// Lấy tất cả các trạng thái người dùng.
        /// </summary>
        /// <returns>Đối tượng Result chứa danh sách các trạng thái người dùng hoặc lỗi nếu thất bại.</returns>
        public async Task<Result<IEnumerable<UserStatusDto>>> GetAllAsync()
        {
            try
            {
                var userStatuses = await _unitOfWork.UserStatusRepostiory.GetAllAsync();
                var userStatusDtos = _mapper.Map<IEnumerable<UserStatusDto>>(userStatuses);

                return Result<IEnumerable<UserStatusDto>>.Success(
                    data: userStatusDtos,
                    message: "Các trạng thái người dùng đã được truy xuất thành công.",
                    code: "SUCCESS", // Thay đổi mã lỗi theo bảng: GET_ALL_USER_STATUSES_SUCCESS -> SUCCESS
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] UserStatusService.GetAllAsync: {ex.Message}");
                return Result<IEnumerable<UserStatusDto>>.Failure(
                    error: ex.Message,
                    message: "Đã xảy ra lỗi khi truy xuất các trạng thái người dùng.",
                    code: "SYSTEM_ERROR", // Mã lỗi chung: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Cập nhật một trạng thái người dùng hiện có.
        /// </summary>
        /// <param name="id">ID của trạng thái người dùng cần cập nhật.</param>
        /// <param name="rq">Đối tượng chứa thông tin yêu cầu cập nhật trạng thái người dùng.</param>
        /// <returns>Đối tượng Result cho biết kết quả của thao tác.</returns>
        public async Task<ApiResponse> UpdateAsync(int id, UserStatusDtoRq rq)
        {
            try
            {
                var userStatus = await _unitOfWork.UserStatusRepostiory.GetByIdAsync(id);
                if (userStatus == null)
                {
                    return ApiResponse.Failure(
                        error: "Không tìm thấy trạng thái người dùng.",
                        message: "Cập nhật trạng thái người dùng thất bại.",
                        code: "NOT_FOUND", // Thay đổi mã lỗi theo bảng: USER_STATUS_NOT_FOUND -> NOT_FOUND
                        statusCode: 404
                    );
                }

                // Kiểm tra xem tên mới có trùng với trạng thái khác không (trừ chính nó)
                var existingStatusWithName = await _unitOfWork.UserStatusRepostiory.GetFirstOrDefaultAsync(
                    predicate: us => us.Name.ToLower() == rq.Name.ToLower() && us.Id != id
                );

                if (existingStatusWithName != null)
                {
                    return ApiResponse.Failure(
                        error: "Một trạng thái người dùng khác với tên này đã tồn tại.",
                        message: "Cập nhật trạng thái người dùng thất bại.",
                        code: "EXISTS", // Thay đổi mã lỗi theo bảng: USER_STATUS_NAME_DUPLICATE -> EXISTS
                        statusCode: 409 // Conflict
                    );
                }

                // Ánh xạ các thuộc tính từ DTO request vào entity hiện có
                userStatus.Name = rq.Name;
                _unitOfWork.UserStatusRepostiory.Update(userStatus); // Cần gọi update trên repository
                await _unitOfWork.CompleteAsync();

                return ApiResponse.Success(
                    message: "Trạng thái người dùng đã được cập nhật thành công.",
                    code: "SUCCESS", // Thay đổi mã lỗi theo bảng: USER_STATUS_UPDATED -> SUCCESS
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] UserStatusService.UpdateAsync: {ex.Message}");
                return ApiResponse.Failure(
                    error: ex.Message,
                    message: "Đã xảy ra lỗi khi cập nhật trạng thái người dùng.",
                    code: "SYSTEM_ERROR", // Mã lỗi chung: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }
    }
}