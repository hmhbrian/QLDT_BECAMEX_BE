
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Dtos.Params;
using QLDT_Becamex.Src.Dtos.Positions;
using QLDT_Becamex.Src.Dtos.Results;
using QLDT_Becamex.Src.Dtos.Users;
using QLDT_Becamex.Src.Services.Interfaces;


namespace QLDT_Becamex.Src.Controllers
{
    [ApiController]
    [Route("[controller]")] // Hoặc [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> GetAllUsers([FromQuery] BaseQueryParam queryParams)
        {
            // 1. Kiểm tra Model State Validation (từ [Range] attributes trong BaseQueryParam)
            if (!ModelState.IsValid)
            {
                return BadRequest(queryParams);
            }

            // 2. Gọi Service để lấy dữ liệu
            var result = await _userService.GetUsersAsync(queryParams);

            // 3. Xử lý phản hồi từ Service
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
                return StatusCode(result.StatusCode ?? 500, new
                {
                    message = result.Message,
                    errors = result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code
                });
            }
        }
    }
}