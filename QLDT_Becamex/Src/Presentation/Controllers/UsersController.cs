using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Services.Interfaces;
using QLDT_Becamex.Src.Application.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using MediatR; // Cần thiết cho StatusCodes
using FluentResults;
using QLDT_Becamex.Src.Application.Common.Dtos;

namespace QLDT_Becamex.Src.Controllers
{
    /// <summary>
    /// API Controller để quản lý người dùng.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMediator _mediator;

        /// <summary>
        /// Khởi tạo một phiên bản mới của lớp <see cref="UsersController"/>.
        /// </summary>
        /// <param name="userService">Dịch vụ người dùng.</param>
        /// <param name="jwtService">Dịch vụ JWT để tạo token (nếu được sử dụng trực tiếp trong controller).</param>
        public UsersController(IUserService userService, IMediator mediator)
        {
            _userService = userService;
            _mediator = mediator;

        }

        /// <summary>
        /// Tạo một người dùng mới. Chỉ ADMIN và HR mới có quyền.
        /// </summary>
        /// <param name="dto">Đối tượng chứa thông tin người dùng cần tạo.</param>
        /// <returns>ActionResult chứa kết quả của thao tác tạo người dùng.</returns>
        //[HttpPost("create")]
        //[Authorize(Roles = "ADMIN, HR")]
        //public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(new
        //        {
        //            message = "Dữ liệu không hợp lệ.",
        //            errors = ModelState.Values.SelectMany(v => v.Errors)
        //                                      .Select(e => e.ErrorMessage),
        //            code = "INVALID", // Mã lỗi chung cho dữ liệu không hợp lệ
        //            statusCode = StatusCodes.Status400BadRequest
        //        });
        //    }

        //    Result result = await _userService.CreateUserAsync(dto);

        //    if (result.IsSuccess)
        //    {
        //        return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new // Mặc định 200OK cho thành công tạo nếu service không trả về 201
        //        {
        //            message = result.Message,
        //            statusCode = result.StatusCode ?? StatusCodes.Status200OK,
        //            code = result.Code
        //        });
        //    }
        //    else
        //    {
        //        // Khi là lỗi, có thể sử dụng statusCode từ Result hoặc mặc định là BadRequest (400)
        //        var statusCode = result.StatusCode ?? StatusCodes.Status400BadRequest;
        //        return StatusCode(statusCode, new // Sử dụng StatusCode thay vì BadRequest để linh hoạt hơn
        //        {
        //            message = result.Message,
        //            errors = result.Errors,

        //            code = result.Code
        //        });
        //    }
        //}

        [HttpPost("create")]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto request)
        {
            var userId = await _mediator.Send(new CreateUserCommand(request));
            return Ok(ApiResponse<string>.Ok(userId)); // Bao kết quả tại đây
        }

        /// <summary>
        /// Đăng nhập người dùng.
        /// </summary>
        /// <param name="dto">Đối tượng chứa thông tin đăng nhập (email và mật khẩu).</param>
        /// <returns>ActionResult chứa thông tin người dùng và token nếu đăng nhập thành công, hoặc lỗi nếu thất bại.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage),
                    code = "INVALID", // Mã lỗi chung cho dữ liệu không hợp lệ
                    statusCode = StatusCodes.Status400BadRequest
                });
            }

            Application.Dtos.Result<UserDto> result = await _userService.LoginAsync(dto);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new // Mặc định 200OK cho thành công
                {
                    message = result.Message,
                    statusCode = result.StatusCode ?? StatusCodes.Status200OK,
                    code = result.Code,
                    data = result.Data
                });
            }
            else
            {
                // Khi là lỗi, có thể sử dụng statusCode từ Result hoặc mặc định là Unauthorized (401)
                var statusCode = result.StatusCode ?? StatusCodes.Status401Unauthorized;
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors,

                    code = result.Code
                });
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết người dùng bằng ID. Chỉ ADMIN và HR mới có quyền.
        /// </summary>
        /// <param name="userId">ID của người dùng cần lấy thông tin.</param>
        /// <returns>ActionResult chứa thông tin người dùng hoặc lỗi nếu không tìm thấy.</returns>
        [HttpGet("{userId}")]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "ID người dùng không được để trống.", code = "INVALID", statusCode = StatusCodes.Status400BadRequest });
            }

            Application.Dtos.Result<UserDto> result = await _userService.GetUserAsync(userId);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new // Mặc định 200OK cho thành công
                {
                    message = result.Message,
                    data = result.Data,
                    statusCode = result.StatusCode ?? StatusCodes.Status200OK,
                    code = result.Code
                });
            }
            else
            {
                // Khi là lỗi, có thể sử dụng statusCode từ Result hoặc mặc định là NotFound (404)
                var statusCode = result.StatusCode ?? StatusCodes.Status404NotFound;
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors,

                    code = result.Code
                });
            }
        }

        /// <summary>
        /// Cập nhật thông tin hồ sơ của người dùng đang đăng nhập.
        /// Người dùng chỉ có thể cập nhật thông tin của chính mình.
        /// </summary>
        /// <param name="rq">Đối tượng chứa thông tin cập nhật hồ sơ.</param>
        /// <returns>ActionResult chứa kết quả của thao tác cập nhật.</returns>
        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateMyProfile([FromForm] UserUserUpdateDto rq)
        {
            // Lấy ID người dùng hiện tại từ token
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();

            if (string.IsNullOrEmpty(userId))
            {
                // Đây là trường hợp hiếm khi xảy ra nếu [Authorize] đã hoạt động đúng
                return Unauthorized(new { message = "Không tìm thấy thông tin người dùng xác thực.", code = "UNAUTHORIZED", statusCode = StatusCodes.Status401Unauthorized });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage),
                    code = "INVALID",
                    statusCode = StatusCodes.Status400BadRequest
                });
            }

            Application.Dtos.Result result = await _userService.UpdateMyProfileAsync(userId, rq);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new // Mặc định 200OK cho thành công
                {
                    message = result.Message,
                    statusCode = result.StatusCode ?? StatusCodes.Status200OK,
                    code = result.Code
                });
            }
            else
            {
                var statusCode = result.StatusCode ?? StatusCodes.Status400BadRequest;
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors,

                    code = result.Code
                });
            }
        }

        /// <summary>
        /// Cập nhật thông tin người dùng bởi ADMIN.
        /// </summary>
        /// <param name="userId">ID của người dùng cần cập nhật.</param>
        /// <param name="rq">Đối tượng chứa thông tin cập nhật.</param>
        /// <returns>ActionResult chứa kết quả của thao tác cập nhật.</returns>
        [HttpPut("admin/{userId}/update")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateUserByAdmin(string userId, [FromBody] UserAdminUpdateDto rq)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "ID người dùng không được để trống.", code = "INVALID", statusCode = StatusCodes.Status400BadRequest });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage),
                    code = "INVALID",
                    statusCode = StatusCodes.Status400BadRequest
                });
            }

            Application.Dtos.Result result = await _userService.UpdateUserByAdmin(userId, rq);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new // Mặc định 200OK cho thành công
                {
                    message = result.Message,
                    statusCode = result.StatusCode ?? StatusCodes.Status200OK,
                    code = result.Code
                });
            }
            else
            {
                var statusCode = result.StatusCode ?? StatusCodes.Status400BadRequest;
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors,

                    code = result.Code
                });
            }
        }

        /// <summary>
        /// Lấy tất cả người dùng (có phân trang và lọc). Chỉ ADMIN và HR mới có quyền.
        /// </summary>
        /// <param name="queryParams">Tham số truy vấn cho phân trang và sắp xếp.</param>
        /// <returns>ActionResult chứa danh sách người dùng đã phân trang.</returns>
        [HttpGet]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> GetAllUsers([FromQuery] BaseQueryParam queryParams)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu truy vấn không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage),
                    code = "INVALID",
                    statusCode = StatusCodes.Status400BadRequest
                });
            }

            var result = await _userService.GetUsersAsync(queryParams);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new // Mặc định 200OK cho thành công
                {
                    message = result.Message,
                    code = result.Code,
                    data = result.Data
                });
            }
            else
            {
                var statusCode = result.StatusCode ?? StatusCodes.Status500InternalServerError; // Mặc định 500 cho lỗi không xác định
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors != null && result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code,
                    statusCode = statusCode
                });
            }
        }

        /// <summary>
        /// Đổi mật khẩu của người dùng đang đăng nhập.
        /// </summary>
        /// <param name="rq">Đối tượng chứa mật khẩu cũ và mật khẩu mới.</param>
        /// <returns>ActionResult chứa kết quả của thao tác đổi mật khẩu.</returns>
        [HttpPatch("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordDto rq)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage),
                    code = "INVALID",
                    statusCode = StatusCodes.Status400BadRequest
                });
            }

            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Không tìm thấy thông tin người dùng xác thực.", code = "UNAUTHORIZED", statusCode = StatusCodes.Status401Unauthorized });
            }

            Application.Dtos.Result result = await _userService.ChangePasswordUserAsync(userId, rq);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new // Mặc định 200OK cho thành công
                {
                    message = result.Message,
                    statusCode = result.StatusCode ?? StatusCodes.Status200OK,
                    code = result.Code
                });
            }
            else
            {
                var statusCode = result.StatusCode ?? StatusCodes.Status400BadRequest;
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors,

                    code = result.Code
                });
            }
        }

        /// <summary>
        /// Đặt lại mật khẩu của một người dùng cụ thể bởi ADMIN.
        /// </summary>
        /// <param name="userId">ID của người dùng cần đặt lại mật khẩu.</param>
        /// <param name="rq">Đối tượng chứa mật khẩu mới.</param>
        /// <returns>ActionResult chứa kết quả của thao tác đặt lại mật khẩu.</returns>
        [HttpPatch("{userId}/reset-password")] // Sửa lại route cho rõ ràng hơn
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ResetPasswordByAdmin(string userId, [FromBody] UserResetPasswordDto rq)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "ID người dùng không được để trống.", code = "INVALID", statusCode = StatusCodes.Status400BadRequest });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage),
                    code = "INVALID",
                    statusCode = StatusCodes.Status400BadRequest
                });
            }

            Application.Dtos.Result result = await _userService.ResetPasswordByAdminAsync(userId, rq);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new // Mặc định 200OK cho thành công
                {
                    message = result.Message,
                    statusCode = result.StatusCode ?? StatusCodes.Status200OK,
                    code = result.Code
                });
            }
            else
            {
                var statusCode = result.StatusCode ?? StatusCodes.Status400BadRequest;
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors,

                    code = result.Code
                });
            }
        }

        /// <summary>
        /// Tìm kiếm người dùng theo từ khóa. Chỉ ADMIN và HR mới có quyền.
        /// </summary>
        /// <param name="keyword">Từ khóa để tìm kiếm (tên đầy đủ hoặc email).</param>
        /// <param name="rq">Tham số truy vấn cho phân trang và sắp xếp.</param>
        /// <returns>ActionResult chứa danh sách người dùng đã tìm thấy và thông tin phân trang.</returns>
        [HttpGet("search")] // Sửa lại route để keyword là query param hoặc bỏ {keyword} và lấy từ query
        // Nếu muốn keyword là route param: [HttpGet("search/{keyword}")]
        // Nếu muốn keyword là query param: [HttpGet("search")] public async Task<IActionResult> SearchUser([FromQuery] string keyword, [FromQuery] BaseQueryParam rq)
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> SearchUser([FromQuery] string keyword, [FromQuery] BaseQueryParam rq) // Lấy keyword từ query param
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return BadRequest(new { message = "Từ khóa tìm kiếm không được để trống.", code = "INVALID", statusCode = StatusCodes.Status400BadRequest });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu truy vấn không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage),
                    code = "INVALID",
                    statusCode = StatusCodes.Status400BadRequest
                });
            }

            Application.Dtos.Result<PagedResult<UserDto>> result = await _userService.SearchUserAsync(keyword, rq);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new // Mặc định 200OK cho thành công
                {
                    message = result.Message,
                    statusCode = result.StatusCode ?? StatusCodes.Status200OK,
                    code = result.Code,
                    data = result.Data
                });
            }
            else
            {
                var statusCode = result.StatusCode ?? StatusCodes.Status500InternalServerError;
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors,

                    code = result.Code
                });
            }
        }

        /// <summary>
        /// Xóa mềm (soft delete) một người dùng. Chỉ ADMIN mới có quyền.
        /// </summary>
        /// <param name="userId">ID của người dùng cần xóa mềm.</param>
        /// <returns>ActionResult chứa kết quả của thao tác xóa mềm.</returns>
        [HttpDelete("{userId}/soft-delete")] // Sửa lại tên endpoint để khớp với mô tả
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> SoftDeleteUser(string userId) // Đổi tên phương thức cho rõ ràng hơn
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "ID người dùng không được để trống.", code = "INVALID", statusCode = StatusCodes.Status400BadRequest });
            }

            // Kiểm tra ADMIN không tự xóa mình
            var (currentUserId, _) = _userService.GetCurrentUserAuthenticationInfo();
            if (currentUserId == userId)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new // Thay đổi từ BadRequest sang Forbidden
                {
                    message = "Bạn không thể thực hiện hành động này trên chính tài khoản của mình.",
                    code = "FORBIDDEN", // Mã lỗi chung: FORBIDDEN
                    statusCode = StatusCodes.Status403Forbidden
                });
            }

            Application.Dtos.Result result = await _userService.SoftDeleteUserAsync(userId);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new // Mặc định 200OK cho thành công
                {
                    message = result.Message,
                    statusCode = result.StatusCode ?? StatusCodes.Status200OK,
                    code = result.Code
                });
            }
            else
            {
                var statusCode = result.StatusCode ?? StatusCodes.Status400BadRequest;
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors,

                    code = result.Code
                });
            }
        }
    }
}