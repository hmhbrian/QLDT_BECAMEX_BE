
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Dtos.Params;
using QLDT_Becamex.Src.Dtos.Results;
using QLDT_Becamex.Src.Dtos.Users;
using QLDT_Becamex.Src.Services.Implementations;
using QLDT_Becamex.Src.Services.Interfaces;


namespace QLDT_Becamex.Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Hoặc [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtService _jwtService;

        public UsersController(IUserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> CreateUser([FromBody] UserDtoRq dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                });
            }

            try
            {
                Result result = await _userService.CreateUserAsync(dto);

                if (result.IsSuccess)
                {
                    return Ok(new { message = result.Message, statusCode = result.StatusCode, code = result.Code });
                }

                return BadRequest(new
                {
                    message = result.Message,
                    errors = result.Errors,
                    statusCode = result.StatusCode,
                    code = result.Code
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Code = "SYSTEM_ERROR",
                    message = "Đã xảy ra lỗi hệ thống.",
                    error = ex.Message,
                });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {

                    message = "Dữ liệu không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                });
            }
            try
            {
                Result<UserDto> result = await _userService.LoginAsync(dto);

                if (result.IsSuccess)
                {

                    string id = result.Data?.Id!;
                    string email = result.Data?.Email!;
                    string role = result.Data?.Role!;
                    if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(role))
                    {
                        string token = _jwtService.GenerateJwtToken(id, email, role);
                        return Ok(new { message = result.Message, statusCode = result.StatusCode, code = result.Code, data = result.Data, accessToken = token });
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            message = "Đăng nhập thất bại!",
                            errors = "Lỗi đăng nhập!",
                            statusCode = 400,
                            code = "FAILED"
                        });
                    }

                }

                return BadRequest(new
                {
                    message = result.Message,
                    errors = result.Errors,
                    statusCode = result.StatusCode,
                    code = result.Code
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Code = "SYSTEM_ERROR",
                    message = "Đã xảy ra lỗi hệ thống.",
                    error = ex.Message,
                });
            }
        }


        [HttpGet("{userId}")]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(userId);
            }
            try
            {
                Result<UserDto> result = await _userService.GetUserAsync(userId);

                if (result.IsSuccess)
                {

                    return Ok(new
                    {
                        message = result.Message,
                        data = result.Data,
                        statusCode = result.StatusCode,
                        code = result.Code
                    });
                }

                return BadRequest(new
                {
                    message = result.Message,
                    errors = result.Errors,
                    statusCode = result.StatusCode,
                    code = result.Code
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Code = "SYSTEM_ERROR",
                    message = "Đã xảy ra lỗi hệ thống.",
                    error = ex.Message,
                });
            }
        }



        [HttpGet]
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