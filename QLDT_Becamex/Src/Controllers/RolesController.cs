
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Dtos.Roles;
using QLDT_Becamex.Src.Services.Interfaces;


namespace QLDT_Becamex.Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Tiền tố 'api' là phổ biến
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleRq request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    message = "Dữ liệu đầu vào không hợp lệ.",
                    errors = errors,
                    code = "INVALID_INPUT"
                });
            }

            var result = await _roleService.CreateRoleAsync(request);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status201Created, new
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
                    errors = result.Errors != null && result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var result = await _roleService.GetRoleByIdAsync(id);

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
                return StatusCode(result.StatusCode ?? StatusCodes.Status500InternalServerError, new
                {
                    message = result.Message,
                    errors = result.Errors != null && result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code
                });
            }
        }

        [HttpGet("byName/{name}")] // Endpoint riêng để tránh xung đột với GetById
        public async Task<IActionResult> GetRoleByName(string name)
        {
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
                return StatusCode(result.StatusCode ?? StatusCodes.Status500InternalServerError, new
                {
                    message = result.Message,
                    errors = result.Errors != null && result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code
                });
            }
        }

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
                return StatusCode(result.StatusCode ?? StatusCodes.Status500InternalServerError, new
                {
                    message = result.Message,
                    errors = result.Errors != null && result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleRq request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    message = "Dữ liệu đầu vào không hợp lệ.",
                    errors = errors,
                    code = "INVALID_INPUT"
                });
            }

            var result = await _roleService.UpdateRoleAsync(id, request);

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
                return StatusCode(result.StatusCode ?? StatusCodes.Status500InternalServerError, new
                {
                    message = result.Message,
                    errors = result.Errors != null && result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code
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
                return StatusCode(result.StatusCode ?? StatusCodes.Status500InternalServerError, new
                {
                    message = result.Message,
                    errors = result.Errors != null && result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code
                });
            }
        }
    }
}