using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using QLDT_Becamex.Src.Application.Common.Dtos;
using MediatR;
using QLDT_Becamex.Src.Application.Features.Users.Queries;


namespace QLDT_Becamex.Src.Controllers
{
    /// <summary>
    /// API Controller để quản lý người dùng.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {

        private readonly IMediator _mediator;

        /// <summary>
        /// Khởi tạo một phiên bản mới của lớp <see cref="UsersController"/>.
        /// </summary>
        /// <param name="userService">Dịch vụ người dùng.</param>
        /// <param name="jwtService">Dịch vụ JWT để tạo token (nếu được sử dụng trực tiếp trong controller).</param>
        public UsersController(IMediator mediator)
        {

            _mediator = mediator;

        }

        /// <summary>
        /// Tạo một người dùng mới. Chỉ ADMIN và HR mới có quyền.
        /// </summary>
        /// <param name="dto">Đối tượng chứa thông tin người dùng cần tạo.</param>
        /// <returns>ActionResult chứa kết quả của thao tác tạo người dùng.</returns>
        [HttpPost("create")]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateUserCommand(request), cancellationToken);
            return Ok(ApiResponse<string>.Ok(result, "Đăng ký thành công")); // Bao kết quả tại đây
        }

        /// <summary>
        /// Đăng nhập người dùng.
        /// </summary>
        /// <param name="dto">Đối tượng chứa thông tin đăng nhập (email và mật khẩu).</param>
        /// <returns>ActionResult chứa thông tin người dùng và token nếu đăng nhập thành công, hoặc lỗi nếu thất bại.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {


            var result = await _mediator.Send(new LoginUserCommand(request));
            return Ok(ApiResponse<UserDto>.Ok(result, "Đăng nhập thành công")); // Bao kết quả tại đây
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
                return BadRequest(ApiResponse.Fail("UserId không hợp lệ"));
            }

            var result = await _mediator.Send(new GetUserByIdQuery(userId));
            return Ok(ApiResponse<UserDto>.Ok(result)); // Bao kết quả tại đây
        }

        /// <summary>
        /// Cập nhật thông tin hồ sơ của người dùng đang đăng nhập.
        /// Người dùng chỉ có thể cập nhật thông tin của chính mình.
        /// </summary>
        /// <param name="rq">Đối tượng chứa thông tin cập nhật hồ sơ.</param>
        /// <returns>ActionResult chứa kết quả của thao tác cập nhật.</returns>
        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUserByUser([FromForm] UserUserUpdateDto rq, CancellationToken cancellationToken)
        {


            var result = await _mediator.Send(new UpdateUserByUserCommand(rq), cancellationToken);
            return Ok(ApiResponse.Ok("Cập nhật thành công")); // Bao kết quả tại đây
        }

        /// <summary>
        /// Cập nhật thông tin người dùng bởi ADMIN.
        /// </summary>
        /// <param name="userId">ID của người dùng cần cập nhật.</param>
        /// <param name="rq">Đối tượng chứa thông tin cập nhật.</param>
        /// <returns>ActionResult chứa kết quả của thao tác cập nhật.</returns>
        [HttpPut("admin/{userId}/update")]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> UpdateUserByAdmin(string userId, [FromBody] UserAdminUpdateDto rq)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(ApiResponse.Fail("UserId không hợp lệ"));
            }



            var result = await _mediator.Send(new UpdateUserByAdminCommand(userId, rq));
            return Ok(ApiResponse.Ok("Cập nhật thành công")); // Bao kết quả tại đây
        }

        /// <summary>
        /// Lấy tất cả người dùng (có phân trang và lọc). Chỉ ADMIN và HR mới có quyền.
        /// </summary>
        /// <param name="queryParams">Tham số truy vấn cho phân trang và sắp xếp.</param>
        /// <returns>ActionResult chứa danh sách người dùng đã phân trang.</returns>
        [HttpGet]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> GetListUsers([FromQuery] BaseQueryParam qr)
        {


            var result = await _mediator.Send(new GetListUsersQuery(qr));
            return Ok(ApiResponse<PagedResult<UserDto>>.Ok(result));

        }

        [HttpGet("manager-dept")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetListManager()
        {
            var result = await _mediator.Send(new GetManagerForDeptQuery());
            return Ok(ApiResponse<List<ManagerDto>>.Ok(result));

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


            var result = await _mediator.Send(new ChangePasswordUserCommand(rq));
            return Ok(ApiResponse.Ok("Đổi mật khẩu thành công"));
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
                return BadRequest(ApiResponse.Fail("UserId không hợp lệ"));
            }


            var result = await _mediator.Send(new ResetPasswordByAdminCommand(userId, rq));
            return Ok(ApiResponse.Ok("Cấp lại mật khẩu thành công"));
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
                return BadRequest(ApiResponse.Fail("Keyword không hợp lệ"));
            }



            var result = await _mediator.Send(new SearchUsersQuery(keyword, rq));
            return Ok(ApiResponse<PagedResult<UserDto>>.Ok(result)); // Bao kết quả tại đây
        }

        /// <summary>
        /// Xóa mềm (soft delete) một người dùng. Chỉ ADMIN mới có quyền.
        /// </summary>
        /// <param name="userId">ID của người dùng cần xóa mềm.</param>
        /// <returns>ActionResult chứa kết quả của thao tác xóa mềm.</returns>
        [HttpDelete("{userId}/soft-delete")] // Sửa lại tên endpoint để khớp với mô tả
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> SoftDeleteUser(string userId) // Đổi tên phương thức cho rõ ràng hơn
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(ApiResponse.Fail("UserId không hợp lệ"));
            }

            var result = await _mediator.Send(new SoftDeleteUserCommand(userId));
            return Ok(ApiResponse.Ok("Xóa thành công")); // Bao kết quả tại đây
        }



        /// <summary>
        /// Xóa cứng (force delete) một người dùng. Chỉ ADMIN mới có quyền.
        /// </summary>
        /// <param name="userId">ID của người dùng cần xóa mềm.</param>
        /// <returns>ActionResult chứa kết quả của thao tác xóa mềm.</returns>
        [HttpDelete("{userId}/force-delete")] // Sửa lại tên endpoint để khớp với mô tả
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> ForceDeleteUser(string userId) // Đổi tên phương thức cho rõ ràng hơn
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(ApiResponse.Fail("UserId không hợp lệ"));
            }

            var result = await _mediator.Send(new ForceDeleteUserCommand(userId));
            return Ok(ApiResponse.Ok("Xóa thành công")); // Bao kết quả tại đây
        }
    }
}