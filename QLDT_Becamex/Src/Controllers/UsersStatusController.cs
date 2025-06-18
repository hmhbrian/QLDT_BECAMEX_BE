using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Dtos.Results; // Đảm bảo đúng namespace cho Result
using QLDT_Becamex.Src.Dtos.UserStatus; // DTO cho UserStatus
using QLDT_Becamex.Src.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLDT_Becamex.Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Route sẽ là /api/UsersStatus
    // [Authorize] // Bỏ ghi chú nếu bạn muốn áp dụng xác thực cho toàn bộ controller
    public class UsersStatusController : ControllerBase
    {
        private readonly IUserStatusService _userStatusService;

        public UsersStatusController(IUserStatusService userStatusService)
        {
            _userStatusService = userStatusService;
        }

        // --- Tạo mới một trạng thái người dùng ---
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        // [Authorize(Roles = "Admin")] // Ví dụ: chỉ Admin mới được tạo
        public async Task<IActionResult> CreateUserStatus([FromBody] UserStatusDtoRq request)
        {
            // Kiểm tra ModelState để bắt lỗi validation từ DTO Request
            if (!ModelState.IsValid)
            {
                // Trả về BadRequest với các lỗi validation chi tiết
                return BadRequest(
                    Result.Failure(
                        message: "Validation failed.",
                        errors: ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                        code: "VALIDATION_ERROR",
                        statusCode: StatusCodes.Status400BadRequest
                    )
                );
            }

            var result = await _userStatusService.CreateAsync(request);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status201Created, new
                {
                    message = result.Message,
                    code = result.Code,
                    data = result.Data // Trả về dữ liệu đã tạo
                });
            }
            else
            {
                // Trường hợp lỗi (ví dụ: tên trạng thái đã tồn tại)
                return StatusCode(result.StatusCode ?? StatusCodes.Status500InternalServerError, new
                {
                    message = result.Message,
                    errors = result.Errors?.Any() == true ? result.Errors : new List<string> { result.Message },
                    code = result.Code
                });
            }
        }

        // --- Lấy tất cả các trạng thái người dùng ---
        [HttpGet]
        // [Authorize] // Ví dụ: yêu cầu xác thực để xem danh sách
        public async Task<IActionResult> GetAllUserStatuses()
        {
            var result = await _userStatusService.GetAllAsync();

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    message = result.Message,
                    code = result.Code,
                    data = result.Data
                });
            }
            else
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status500InternalServerError, new
                {
                    message = result.Message,
                    errors = result.Errors?.Any() == true ? result.Errors : new List<string> { result.Message },
                    code = result.Code
                });
            }
        }


        // --- Cập nhật một trạng thái người dùng ---
        [HttpPut("{id}")] // Route sẽ là /api/UsersStatus/{id}
        [Authorize(Roles = "ADMIN")]
        // [Authorize(Roles = "Admin")] // Ví dụ: chỉ Admin mới được cập nhật
        public async Task<IActionResult> UpdateUserStatus(int id, [FromBody] UserStatusDtoRq request)
        {
            // Kiểm tra ModelState cho validation
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    Result.Failure(
                        message: "Validation failed.",
                        errors: ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                        code: "VALIDATION_ERROR",
                        statusCode: StatusCodes.Status400BadRequest
                    )
                );
            }

            var result = await _userStatusService.UpdateAsync(id, request);

            if (result.IsSuccess)
            {
                // Cập nhật thành công, thường trả về 200 OK hoặc 204 No Content
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new
                {
                    message = result.Message,
                    code = result.Code,
                    // Không cần trả về data ở đây nếu service không trả về
                });
            }
            else
            {
                // Xử lý các trường hợp lỗi (ví dụ: không tìm thấy, trùng tên)
                return StatusCode(result.StatusCode ?? StatusCodes.Status500InternalServerError, new
                {
                    message = result.Message,
                    errors = result.Errors?.Any() == true ? result.Errors : new List<string> { result.Message },
                    code = result.Code
                });
            }
        }

        // --- Xóa một trạng thái người dùng ---
        [HttpDelete("{id}")] // Route sẽ là /api/UsersStatus/{id}
        [Authorize(Roles = "ADMIN")]
        // [Authorize(Roles = "Admin")] // Ví dụ: chỉ Admin mới được xóa
        public async Task<IActionResult> DeleteUserStatus(int id)
        {
            var result = await _userStatusService.DeleteAsync(id);

            if (result.IsSuccess)
            {
                // Xóa thành công, trả về 204 No Content
                return Ok(new
                {
                    message = result.Message,
                    code = result.Code,
                    statusCode = result.StatusCode
                });
            }
            else
            {
                // Xử lý lỗi (ví dụ: không tìm thấy)
                return StatusCode(result.StatusCode ?? StatusCodes.Status500InternalServerError, new
                {
                    message = result.Message,
                    errors = result.Errors?.Any() == true ? result.Errors : new List<string> { result.Message },
                    code = result.Code
                });
            }
        }
    }
}