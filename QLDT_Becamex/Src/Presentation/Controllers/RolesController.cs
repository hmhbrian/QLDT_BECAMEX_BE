using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Services.Interfaces;
using System.Collections.Generic; // Cần thiết cho List<string>
using System.Linq; // Cần thiết cho .Any()
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using QLDT_Becamex.Src.Application.Dtos; // Cần thiết cho StatusCodes

namespace QLDT_Becamex.Src.Controllers
{
    /// <summary>
    /// API Controller để quản lý các vai trò (Role).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")] // Tiền tố 'api' là phổ biến
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        /// <summary>
        /// Khởi tạo một phiên bản mới của lớp <see cref="RolesController"/>.
        /// </summary>
        /// <param name="roleService">Dịch vụ quản lý vai trò.</param>
        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Tạo một vai trò mới.
        /// </summary>
        /// <param name="request">Đối tượng chứa thông tin yêu cầu tạo vai trò.</param>
        /// <returns>ActionResult chứa kết quả của thao tác tạo vai trò.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleRq request)
        {
            if (!ModelState.IsValid)
            {

                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    message = "Dữ liệu đầu vào không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                    code = "INVALID", // Thay đổi từ "INVALID_INPUT" sang "INVALID"
                    statusCode = StatusCodes.Status400BadRequest
                });
            }

            var result = await _roleService.CreateRoleAsync(request);

            if (result.IsSuccess)
            {
                // Sử dụng ?? StatusCodes.Status201Created cho trường hợp tạo thành công
                return StatusCode(result.StatusCode ?? StatusCodes.Status201Created, new
                {
                    message = result.Message,
                    code = result.Code,
                    data = result.Data
                });
            }
            else
            {
                // Sử dụng StatusCode từ Result, nếu không thì mặc định là 500 (hoặc 400 tùy ngữ cảnh lỗi)
                var statusCode = result.StatusCode ?? StatusCodes.Status500InternalServerError;
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors != null && result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code,

                });
            }
        }

        /// <summary>
        /// Lấy thông tin vai trò theo ID.
        /// </summary>
        /// <param name="id">ID của vai trò cần lấy.</param>
        /// <returns>ActionResult chứa thông tin vai trò hoặc lỗi nếu không tìm thấy.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            // Có thể thêm kiểm tra id rỗng ở đây nếu muốn, tương tự như CoursesController
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { message = "ID không được để trống.", code = "INVALID", statusCode = StatusCodes.Status400BadRequest });
            }

            var result = await _roleService.GetRoleByIdAsync(id);

            if (result.IsSuccess)
            {
                // Sử dụng ?? StatusCodes.Status200OK cho trường hợp thành công
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new
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

                });
            }
        }

        /// <summary>
        /// Lấy thông tin vai trò theo tên.
        /// </summary>
        /// <param name="name">Tên của vai trò cần lấy.</param>
        /// <returns>ActionResult chứa thông tin vai trò hoặc lỗi nếu không tìm thấy.</returns>
        [HttpGet("byName/{name}")] // Endpoint riêng để tránh xung đột với GetById
        public async Task<IActionResult> GetRoleByName(string name)
        {
            // Có thể thêm kiểm tra name rỗng ở đây
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest(new { message = "Tên vai trò không được để trống.", code = "INVALID", statusCode = StatusCodes.Status400BadRequest });
            }

            var result = await _roleService.GetRoleByNameAsync(name);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new
                {
                    message = result.Message,
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
                    errors = result.Errors != null && result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code,

                });
            }
        }

        /// <summary>
        /// Lấy tất cả các vai trò.
        /// </summary>
        /// <returns>ActionResult chứa danh sách tất cả các vai trò hoặc lỗi nếu thất bại.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _roleService.GetAllRolesAsync();

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new
                {
                    message = result.Message,
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
                    errors = result.Errors != null && result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code,

                });
            }
        }

        /// <summary>
        /// Cập nhật thông tin của một vai trò hiện có.
        /// </summary>
        /// <param name="id">ID của vai trò cần cập nhật.</param>
        /// <param name="request">Đối tượng chứa thông tin yêu cầu cập nhật vai trò.</param>
        /// <returns>ActionResult chứa kết quả của thao tác cập nhật vai trò.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleRq request)
        {
            // Kiểm tra ID trống
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { message = "ID không được để trống.", code = "INVALID", statusCode = StatusCodes.Status400BadRequest });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    message = "Dữ liệu đầu vào không hợp lệ.",
                    errors = errors,
                    code = "INVALID", // Thay đổi từ "INVALID_INPUT" sang "INVALID"
                    statusCode = StatusCodes.Status400BadRequest
                });
            }

            var result = await _roleService.UpdateRoleAsync(id, request);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new
                {
                    message = result.Message,
                    code = result.Code,
                    data = result.Data // Có thể có dữ liệu trả về nếu service trả về Result<T>
                });
            }
            else
            {
                var statusCode = result.StatusCode ?? StatusCodes.Status500InternalServerError;
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors != null && result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code,

                });
            }
        }

        /// <summary>
        /// Xóa một vai trò theo ID.
        /// </summary>
        /// <param name="id">ID của vai trò cần xóa.</param>
        /// <returns>Phản hồi xác nhận xóa.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRole(string id)
        {
            // Có thể thêm kiểm tra id rỗng ở đây
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { message = "ID không được để trống.", code = "INVALID", statusCode = StatusCodes.Status400BadRequest });
            }

            var result = await _roleService.DeleteRoleAsync(id);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new
                {
                    message = result.Message,
                    code = result.Code
                });
            }
            else
            {
                var statusCode = result.StatusCode ?? StatusCodes.Status500InternalServerError;
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors != null && result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code,

                });
            }
        }
    }
}