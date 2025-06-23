using QLDT_Becamex.Src.Models;
using QLDT_Becamex.Src.Services.Interfaces;
using AutoMapper;
using QLDT_Becamex.Src.UnitOfWork;
using System; // Thêm để sử dụng Exception
using System.Collections.Generic; // Thêm để sử dụng IEnumerable
using System.Linq;
using QLDT_Becamex.Src.Dtos; // Thêm để sử dụng LINQ

namespace QLDT_Becamex.Src.Services.Implementations
{
    /// <summary>
    /// Triển khai dịch vụ quản lý vị trí (chức danh).
    /// </summary>
    public class PositionService : IPositionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Khởi tạo một phiên bản mới của lớp <see cref="PositionService"/>.
        /// </summary>
        /// <param name="unitOfWork">Đối tượng Unit of Work để quản lý các repositories và giao dịch cơ sở dữ liệu.</param>
        /// <param name="mapper">Đối tượng AutoMapper để ánh xạ giữa các đối tượng.</param>
        public PositionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo một vị trí mới.
        /// </summary>
        /// <param name="rq">Đối tượng chứa thông tin yêu cầu tạo vị trí.</param>
        /// <returns>Đối tượng Result cho biết kết quả của thao tác.</returns>
        public async Task<ApiResponse> CreatePositionAsync(PositionRq rq)
        {
            try
            {
                // Kiểm tra xem tên vị trí đã tồn tại chưa
                var existingPosition = await _unitOfWork.PositionRepostiory.AnyAsync(
                    p => p.PositionName!.ToLower() == rq.PositionName.ToLower());


                if (existingPosition) // <-- Logic của bạn được giữ nguyên
                {
                    return Result<PositionDto>.Failure(
                        message: "Tạo vị trí thất bại",
                        error: "Tên vị trí đã tồn tại.",
                        code: "EXISTS", // Thay đổi mã lỗi theo bảng: POSITION_NAME_EXISTS -> EXISTS
                        statusCode: 409 // 409: Xung đột dữ liệu
                    );
                }

                Position position = new Position()
                {
                    PositionName = rq.PositionName,
                };

                await _unitOfWork.PositionRepostiory.AddAsync(position);
                await _unitOfWork.CompleteAsync();

                return ApiResponse.Success(
                    message: "Tạo vị trí thành công.",
                    code: "SUCCESS", // Thay đổi mã lỗi theo bảng: CREATE_POSITION_SUCCESS -> SUCCESS
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết tại đây (ví dụ: logger.LogError(ex, "Error creating position.");)
                // Console.WriteLine($"Error creating position: {ex.Message}"); // Dùng để debug nhanh
                return ApiResponse.Failure(
                    message: "Đã xảy ra lỗi hệ thống khi tạo vị trí. Vui lòng thử lại sau.",
                    error: ex.Message,
                    code: "SYSTEM_ERROR", // Mã lỗi chung: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Lấy thông tin vị trí theo ID.
        /// </summary>
        /// <param name="id">ID của vị trí cần lấy.</param>
        /// <returns>Đối tượng Result chứa thông tin vị trí hoặc lỗi nếu không tìm thấy.</returns>
        public async Task<Result<PositionDto>> GetPositionByIdAsync(int id)
        {
            try
            {
                Position? position = await _unitOfWork.PositionRepostiory.GetByIdAsync(id);

                if (position == null)
                {
                    return Result<PositionDto>.Failure(
                        message: "Lấy thông tin vị trí thất bại",
                        error: "Vị trí không tồn tại.",
                        code: "NOT_FOUND", // Thay đổi mã lỗi theo bảng: POSITION_NOT_FOUND -> NOT_FOUND
                        statusCode: 404
                    );
                }
                PositionDto positionDto = _mapper.Map<PositionDto>(position);
                return Result<PositionDto>.Success(
                    message: "Lấy thông tin vị trí thành công.",
                    code: "SUCCESS", // Thay đổi mã lỗi theo bảng: GET_POSITION_SUCCESS -> SUCCESS
                    statusCode: 200,
                    data: positionDto
                );
            }
            catch (Exception ex) // Bắt Exception để ghi log và trả về lỗi hệ thống
            {
                // Ghi log lỗi chi tiết tại đây
                // Console.WriteLine($"Error getting position by ID: {ex.Message}");
                return Result<PositionDto>.Failure(
                    message: "Đã xảy ra lỗi hệ thống khi lấy thông tin vị trí. Vui lòng thử lại sau.",
                    error: ex.Message,
                    code: "SYSTEM_ERROR", // Mã lỗi chung: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Lấy tất cả các vị trí.
        /// </summary>
        /// <returns>Đối tượng Result chứa danh sách các vị trí hoặc lỗi.</returns>
        public async Task<Result<IEnumerable<PositionDto>>> GetAllPositionsAsync()
        {
            try
            {
                IEnumerable<Position> positions = await _unitOfWork.PositionRepostiory.GetAllAsync();

                IEnumerable<PositionDto> positionDtos = _mapper.Map<IEnumerable<PositionDto>>(positions);
                return Result<IEnumerable<PositionDto>>.Success(
                    message: "Lấy danh sách vị trí thành công.",
                    code: "SUCCESS", // Thay đổi mã lỗi theo bảng: GET_ALL_POSITIONS_SUCCESS -> SUCCESS
                    statusCode: 200,
                    data: positionDtos
                );
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết tại đây
                // Console.WriteLine($"Error getting all positions: {ex.Message}");
                return Result<IEnumerable<PositionDto>>.Failure(
                    message: "Đã xảy ra lỗi hệ thống khi lấy danh sách vị trí. Vui lòng thử lại sau.",
                    error: ex.Message,
                    code: "SYSTEM_ERROR", // Mã lỗi chung: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Cập nhật thông tin một vị trí hiện có.
        /// </summary>
        /// <param name="id">ID của vị trí cần cập nhật.</param>
        /// <param name="rq">Đối tượng chứa thông tin yêu cầu cập nhật vị trí.</param>
        /// <returns>Đối tượng Result cho biết kết quả của thao tác.</returns>
        public async Task<ApiResponse> UpdatePositionAsync(int id, PositionRq rq)
        {
            try
            {
                var position = await _unitOfWork.PositionRepostiory.GetByIdAsync(id);

                if (position == null)
                {
                    return ApiResponse.Failure(
                        message: "Cập nhật vị trí thất bại",
                        error: "Vị trí không tồn tại.",
                        code: "NOT_FOUND", // Thay đổi mã lỗi theo bảng: POSITION_NOT_FOUND -> NOT_FOUND
                        statusCode: 404
                    );
                }

                // Kiểm tra tên vị trí mới có trùng với vị trí khác (không phải vị trí đang cập nhật)
                var existingPositionWithSameName = await _unitOfWork.PositionRepostiory.AnyAsync(
                    p => p.PositionName!.ToLower() == rq.PositionName.ToLower() && p.PositionId != id);

                // Logic của bạn: kiểm tra existingPositionWithSameName != null
                // (Tương tự như CreatePositionAsync, FindAsync trả về IEnumerable<T>).
                if (existingPositionWithSameName) // <-- Logic của bạn được giữ nguyên
                {
                    return ApiResponse.Failure(
                        message: "Cập nhật vị trí thất bại",
                        error: "Tên vị trí đã được sử dụng bởi vị trí khác.",
                        code: "EXISTS", // Thay đổi mã lỗi theo bảng: POSITION_NAME_ALREADY_EXISTS -> EXISTS
                        statusCode: 409 // 409: Xung đột dữ liệu
                    );
                }

                _mapper.Map(rq, position);

                // Nếu bạn cần gán UpdatedAt tự động, hãy thêm nó vào đây.
                // Ví dụ:
                // position.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.PositionRepostiory.Update(position);
                await _unitOfWork.CompleteAsync();

                // Trả về Result.Success mà không cần dữ liệu nếu không muốn trả về lại PositionDto
                return ApiResponse.Success(
                    message: "Cập nhật vị trí thành công.",
                    code: "SUCCESS", // Thay đổi mã lỗi theo bảng: UPDATE_POSITION_SUCCESS -> SUCCESS
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết tại đây
                // Console.WriteLine($"Error updating position: {ex.Message}");
                // Khi có lỗi, Result.Failure không nên trả về PositionDto (Result<PositionDto>) nếu mục đích là chỉ báo lỗi.
                // Nếu muốn trả về PositionDto khi thành công, thì cần một Result<PositionDto> riêng cho trường hợp thành công.
                // Dựa trên chữ ký UpdateAsync(int id, PositionRq rq) trả về Task<Result>, không có generic T.
                // Vì vậy, trả về Result.Failure là đúng.
                return ApiResponse.Failure( // Sửa lại thành Result.Failure (không có generic)
                    message: "Đã xảy ra lỗi hệ thống khi cập nhật vị trí. Vui lòng thử lại sau.",
                    error: ex.Message,
                    code: "SYSTEM_ERROR", // Mã lỗi chung: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Xóa một vị trí.
        /// </summary>
        /// <param name="id">ID của vị trí cần xóa.</param>
        /// <returns>Đối tượng Result cho biết kết quả của thao tác.</returns>
        public async Task<ApiResponse> DeletePositionAsync(int id)
        {
            try
            {
                var position = await _unitOfWork.PositionRepostiory.GetByIdAsync(id);

                if (position == null)
                {
                    return ApiResponse.Failure(
                        message: "Xóa vị trí thất bại",
                        error: "Vị trí không tồn tại.",
                        code: "NOT_FOUND", // Thay đổi mã lỗi theo bảng: POSITION_NOT_FOUND -> NOT_FOUND
                        statusCode: 404
                    );
                }

                // Thực hiện xóa cứng
                _unitOfWork.PositionRepostiory.Remove(position);
                await _unitOfWork.CompleteAsync();

                return ApiResponse.Success(
                    message: "Xóa vị trí thành công (hard delete).",
                    code: "SUCCESS", // Thay đổi mã lỗi theo bảng: HARD_DELETE_POSITION_SUCCESS -> SUCCESS
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết tại đây
                // Console.WriteLine($"Error deleting position: {ex.Message}");
                return ApiResponse.Failure(
                    message: "Đã xảy ra lỗi hệ thống khi xóa vị trí. Vui lòng thử lại sau.",
                    error: ex.Message,
                    code: "SYSTEM_ERROR", // Mã lỗi chung: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }
    }
}