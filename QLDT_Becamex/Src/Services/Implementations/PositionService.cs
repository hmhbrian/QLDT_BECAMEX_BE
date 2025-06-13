using QLDT_Becamex.Src.Dtos.Positions;
using QLDT_Becamex.Src.Models;
using QLDT_Becamex.Src.Services.Interfaces;
using AutoMapper;
using QLDT_Becamex.Src.UnitOfWork;
using QLDT_Becamex.Src.Dtos.Results;

namespace QLDT_Becamex.Src.Services.Implementations
{
    public class PositionService : IPositionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PositionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> CreatePositionAsync(PositionRq rq)
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
                        code: "POSITION_NAME_EXISTS",
                        statusCode: 400
                    );
                }

                Position position = new Position()
                {
                    PositionName = rq.PositionName,
                };

                await _unitOfWork.PositionRepostiory.AddAsync(position);
                await _unitOfWork.CompleteAsync();

                return Result.Success(
                    message: "Tạo vị trí thành công.",
                    code: "CREATE_POSITION_SUCCESS",
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết tại đây (ví dụ: logger.LogError(ex, "Error creating position.");)
                // Console.WriteLine($"Error creating position: {ex.Message}"); // Dùng để debug nhanh
                return Result.Failure(
                    message: ex.Message,
                    error: "Đã xảy ra lỗi hệ thống khi tạo vị trí. Vui lòng thử lại sau.",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        // --- READ ---
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
                        code: "POSITION_NOT_FOUND",
                        statusCode: 404
                    );
                }
                PositionDto positionDto = _mapper.Map<PositionDto>(position);
                return Result<PositionDto>.Success(
                    message: "Lấy thông tin vị trí thành công.",
                    code: "GET_POSITION_SUCCESS",
                    statusCode: 200,
                    data: positionDto
                );
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết tại đây
                // Console.WriteLine($"Error getting position by ID: {ex.Message}");
                return Result<PositionDto>.Failure(
                    message: "Lấy thông tin vị trí thất bại",
                    error: "Đã xảy ra lỗi hệ thống khi lấy thông tin vị trí. Vui lòng thử lại sau.",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        public async Task<Result<IEnumerable<PositionDto>>> GetAllPositionsAsync()
        {
            try
            {
                IEnumerable<Position> positions = await _unitOfWork.PositionRepostiory.GetAllAsync();

                IEnumerable<PositionDto> positionDtos = _mapper.Map<IEnumerable<PositionDto>>(positions);
                return Result<IEnumerable<PositionDto>>.Success(
                    message: "Lấy danh sách vị trí thành công.",
                    code: "GET_ALL_POSITIONS_SUCCESS",
                    statusCode: 200,
                    data: positionDtos
                );
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết tại đây
                // Console.WriteLine($"Error getting all positions: {ex.Message}");
                return Result<IEnumerable<PositionDto>>.Failure(
                    message: ex.Message,
                    error: "Đã xảy ra lỗi hệ thống khi lấy danh sách vị trí. Vui lòng thử lại sau.",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        // --- UPDATE ---
        public async Task<Result> UpdatePositionAsync(int id, PositionRq rq)
        {
            try
            {
                var position = await _unitOfWork.PositionRepostiory.GetByIdAsync(id);

                if (position == null)
                {
                    return Result.Failure(
                        message: "Cập nhật vị trí thất bại",
                        error: "Vị trí không tồn tại.",
                        code: "POSITION_NOT_FOUND",
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
                    return Result.Failure(
                        message: "Cập nhật vị trí thất bại",
                        error: "Tên vị trí đã được sử dụng bởi vị trí khác.",
                        code: "POSITION_NAME_ALREADY_EXISTS",
                        statusCode: 400
                    );
                }

                _mapper.Map(rq, position);

                // Nếu bạn cần gán UpdatedAt tự động, hãy thêm nó vào đây.
                // Ví dụ:
                // position.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.PositionRepostiory.Update(position);
                await _unitOfWork.CompleteAsync();

                var positionDto = _mapper.Map<PositionDto>(position);
                return Result.Success(
                    message: "Cập nhật vị trí thành công.",
                    code: "UPDATE_POSITION_SUCCESS",
                    statusCode: 200


                );
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết tại đây
                // Console.WriteLine($"Error updating position: {ex.Message}");
                return Result<PositionDto>.Failure(
                    message: ex.Message,
                    error: "Đã xảy ra lỗi hệ thống khi cập nhật vị trí. Vui lòng thử lại sau.",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        public async Task<Result> DeletePositionAsync(int id)
        {
            try
            {
                var position = await _unitOfWork.PositionRepostiory.GetByIdAsync(id);

                if (position == null)
                {
                    return Result.Failure(
                        message: "Xóa vị trí thất bại",
                        error: "Vị trí không tồn tại.",
                        code: "POSITION_NOT_FOUND",
                        statusCode: 404
                    );
                }

                // Thực hiện xóa cứng
                _unitOfWork.PositionRepostiory.Remove(position);
                await _unitOfWork.CompleteAsync();

                return Result.Success(
                    message: "Xóa vị trí thành công (hard delete).",
                    code: "HARD_DELETE_POSITION_SUCCESS",
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết tại đây
                // Console.WriteLine($"Error deleting position: {ex.Message}");
                return Result.Failure(
                    message: ex.Message,
                    error: "Đã xảy ra lỗi hệ thống khi xóa vị trí. Vui lòng thử lại sau.",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }
    }
}