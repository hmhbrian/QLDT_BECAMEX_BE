using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Dtos;
using QLDT_Becamex.Src.Services.Interfaces;


namespace QLDT_Becamex.Src.Controllers
{
    /// <summary>
    /// API Controller để quản lý các trạng thái người dùng (User Status).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")] // Route sẽ là /api/UsersStatus
    // [Authorize] // Bỏ ghi chú nếu bạn muốn áp dụng xác thực cho toàn bộ controller
    public class UsersStatusController : ControllerBase
    {
        private readonly IUserStatusService _userStatusService;

        /// <summary>
        /// Khởi tạo một phiên bản mới của lớp <see cref="UsersStatusController"/>.
        /// </summary>
        /// <param name="userStatusService">Dịch vụ quản lý trạng thái người dùng.</param>
        public UsersStatusController(IUserStatusService userStatusService)
        {
            _userStatusService = userStatusService;
        }

        /// <summary>
        /// Tạo một trạng thái người dùng mới. Chỉ ADMIN mới có quyền.
        /// </summary>
        /// <param name="request">Đối tượng chứa thông tin yêu cầu tạo trạng thái người dùng mới.</param>
        /// <returns>ActionResult chứa kết quả của thao tác tạo trạng thái người dùng.</returns>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateUserStatus([FromBody] UserStatusDtoRq request)
        {
            // Kiểm tra ModelState để bắt lỗi validation từ DTO Request
            if (!ModelState.IsValid)
            {
                // Trả về BadRequest với các lỗi validation chi tiết
                return BadRequest(new // Sử dụng new {} thay vì Result.Failure để tuân thủ định dạng response Controller
                {
                    message = "Dữ liệu không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                    code = "INVALID", // Mã lỗi chung: INVALID

                });
            }

            var result = await _userStatusService.CreateAsync(request);

            if (result.IsSuccess)
            {
                // Mặc định Status201Created cho tạo thành công
                return StatusCode(result.StatusCode ?? StatusCodes.Status201Created, new
                {
                    message = result.Message,
                    code = result.Code,
                    data = result.Data, // Trả về dữ liệu đã tạo
                    statusCode = result.StatusCode ?? StatusCodes.Status201Created
                });
            }
            else
            {
                // Trường hợp lỗi (ví dụ: tên trạng thái đã tồn tại)
                var statusCode = result.StatusCode ?? StatusCodes.Status400BadRequest; // Mặc định 400 cho lỗi không thành công
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors?.Any() == true ? result.Errors : new List<string> { result.Message },
                    code = result.Code,

                });
            }
        }

        /// <summary>
        /// Lấy tất cả các trạng thái người dùng.
        /// </summary>
        /// <returns>ActionResult chứa danh sách các trạng thái người dùng.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUserStatuses()
        {
            var result = await _userStatusService.GetAllAsync();

            if (result.IsSuccess)
            {
                return Ok(new // Ok trả về 200 OK mặc định
                {
                    message = result.Message,
                    code = result.Code,
                    data = result.Data,
                    statusCode = result.StatusCode ?? StatusCodes.Status200OK
                });
            }
            else
            {
                var statusCode = result.StatusCode ?? StatusCodes.Status500InternalServerError; // Mặc định 500 cho lỗi GetAll
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors?.Any() == true ? result.Errors : new List<string> { result.Message },
                    code = result.Code,

                });
            }
        }

        /// <summary>
        /// Cập nhật thông tin của một trạng thái người dùng hiện có. Chỉ ADMIN mới có quyền.
        /// </summary>
        /// <param name="id">ID của trạng thái người dùng cần cập nhật.</param>
        /// <param name="request">Đối tượng yêu cầu chứa thông tin cập nhật.</param>
        /// <returns>ActionResult chứa kết quả của thao tác cập nhật trạng thái người dùng.</returns>
        [HttpPut("{id}")] // Route sẽ là /api/UsersStatus/{id}
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateUserStatus(int id, [FromBody] UserStatusDtoRq request)
        {
            // Kiểm tra ModelState cho validation
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                    code = "INVALID", // Mã lỗi chung: INVALID

                });
            }

            var result = await _userStatusService.UpdateAsync(id, request);

            if (result.IsSuccess)
            {
                // Cập nhật thành công, thường trả về 200 OK
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new
                {
                    message = result.Message,
                    code = result.Code,
                    statusCode = result.StatusCode ?? StatusCodes.Status200OK
                    // Không cần trả về data ở đây nếu service không trả về
                });
            }
            else
            {
                // Xử lý các trường hợp lỗi (ví dụ: không tìm thấy, trùng tên)
                var statusCode = result.StatusCode ?? StatusCodes.Status400BadRequest; // Mặc định 400 cho lỗi không thành công
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors?.Any() == true ? result.Errors : new List<string> { result.Message },
                    code = result.Code,

                });
            }
        }

        /// <summary>
        /// Xóa một hoặc nhiều trạng thái người dùng. Chỉ ADMIN mới có quyền.
        /// </summary>
        /// <param name="ids">Danh sách các ID của trạng thái người dùng cần xóa.</param>
        /// <returns>ActionResult chứa kết quả của thao tác xóa trạng thái người dùng.</returns>
        [HttpDelete] // Route sẽ là /api/UsersStatus (nếu ids được truyền qua body/query)
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteUserStatus([FromForm] List<int> ids) // Thay đổi sang [FromBody] nếu muốn truyền list trong body, [FromQuery] nếu qua query string. [FromForm] là phổ biến cho form-data
        {
            // Kiểm tra danh sách ID hợp lệ (nếu dùng FromForm/FromQuery có thể ids là null nếu không gửi gì)
            if (ids == null || !ids.Any())
            {
                return BadRequest(new
                {
                    message = "Dữ liệu không hợp lệ.",
                    error = "Danh sách ID không được để trống.",
                    code = "INVALID",

                });
            }

            var result = await _userStatusService.DeleteAsync(ids);

            if (result.IsSuccess)
            {
                // Xóa thành công, thường trả về 200 OK (hoặc 204 No Content nếu không muốn trả về body)
                return Ok(new
                {
                    message = result.Message,
                    code = result.Code,
                    statusCode = result.StatusCode ?? StatusCodes.Status200OK
                });
            }
            else
            {
                // Xử lý lỗi (ví dụ: không tìm thấy)
                var statusCode = result.StatusCode ?? StatusCodes.Status400BadRequest; // Mặc định 400 cho lỗi không thành công
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors?.Any() == true ? result.Errors : new List<string> { result.Message },
                    code = result.Code,

                });
            }
        }
    }
}