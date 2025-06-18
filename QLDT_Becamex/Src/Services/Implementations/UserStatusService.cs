using AutoMapper;
using QLDT_Becamex.Src.Dtos.Results;
using QLDT_Becamex.Src.Dtos.UserStatus;
using QLDT_Becamex.Src.Services.Interfaces;
using QLDT_Becamex.Src.Models; // Model UserStatus của bạn
using QLDT_Becamex.Src.UnitOfWork;

namespace QLDT_Becamex.Src.Services.Implementations
{
    public class UserStatusService : IUserStatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserStatusService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo một trạng thái người dùng mới.
        /// </summary>
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
                        error: "User status with this name already exists.",
                        code: "USER_STATUS_ALREADY_EXISTS",
                        statusCode: 409 // Conflict
                    );
                }

                var userStatus = _mapper.Map<UserStatus>(rq);


                await _unitOfWork.UserStatusRepostiory.AddAsync(userStatus);
                await _unitOfWork.CompleteAsync();

                var userStatusDto = _mapper.Map<UserStatusDto>(userStatus);
                return Result<UserStatusDto>.Success(
                    data: userStatusDto,
                    message: "User status created successfully.",
                    code: "USER_STATUS_CREATED",
                    statusCode: 201
                );
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết
                Console.WriteLine($"[ERROR] UserStatusService.CreateAsync: {ex.Message}");
                return Result<UserStatusDto>.Failure(
                    error: ex.Message,
                    message: "An error occurred while creating the user status.",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Xóa một trạng thái người dùng theo ID.
        /// </summary>
        public async Task<Result> DeleteAsync(int id)
        {
            try
            {
                var userStatus = await _unitOfWork.UserStatusRepostiory.GetByIdAsync(id);
                if (userStatus == null)
                {
                    return Result.Failure(
                        error: "User status not found.",
                        code: "USER_STATUS_NOT_FOUND",
                        statusCode: 404
                    );
                }

                _unitOfWork.UserStatusRepostiory.Remove(userStatus);
                await _unitOfWork.CompleteAsync();

                return Result.Success(
                    message: "User status deleted successfully.",
                    code: "USER_STATUS_DELETED",
                    statusCode: 204 // No Content for successful deletion
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] UserStatusService.DeleteAsync: {ex.Message}");
                return Result.Failure(
                    error: ex.Message,
                    message: "An error occurred while deleting the user status.",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Lấy tất cả các trạng thái người dùng.
        /// </summary>
        public async Task<Result<IEnumerable<UserStatusDto>>> GetAllAsync()
        {
            try
            {
                var userStatuses = await _unitOfWork.UserStatusRepostiory.GetAllAsync();
                var userStatusDtos = _mapper.Map<IEnumerable<UserStatusDto>>(userStatuses);

                return Result<IEnumerable<UserStatusDto>>.Success(
                    data: userStatusDtos,
                    message: "User statuses retrieved successfully.",
                    code: "GET_ALL_USER_STATUSES_SUCCESS",
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] UserStatusService.GetAllAsync: {ex.Message}");
                return Result<IEnumerable<UserStatusDto>>.Failure(
                    error: ex.Message,
                    message: "An error occurred while retrieving user statuses.",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Cập nhật một trạng thái người dùng hiện có.
        /// </summary>
        public async Task<Result> UpdateAsync(int id, UserStatusDtoRq rq)
        {
            try
            {
                var userStatus = await _unitOfWork.UserStatusRepostiory.GetByIdAsync(id);
                if (userStatus == null)
                {
                    return Result.Failure(
                        error: "User status not found.",
                        code: "USER_STATUS_NOT_FOUND",
                        statusCode: 404
                    );
                }

                // Kiểm tra xem tên mới có trùng với trạng thái khác không (trừ chính nó)
                var existingStatusWithName = await _unitOfWork.UserStatusRepostiory.GetFirstOrDefaultAsync(
                    predicate: us => us.Name.ToLower() == rq.Name.ToLower() && us.Id != id
                );

                if (existingStatusWithName != null)
                {
                    return Result.Failure(
                        error: "Another user status with this name already exists.",
                        code: "USER_STATUS_NAME_DUPLICATE",
                        statusCode: 409 // Conflict
                    );
                }

                // Ánh xạ các thuộc tính từ DTO request vào entity hiện có
                userStatus.Name = rq.Name;
                await _unitOfWork.CompleteAsync();

                return Result.Success(
                    message: "User status updated successfully.",
                    code: "USER_STATUS_UPDATED",
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] UserStatusService.UpdateAsync: {ex.Message}");
                return Result.Failure(
                    error: ex.Message,
                    message: "An error occurred while updating the user status.",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }
    }
}