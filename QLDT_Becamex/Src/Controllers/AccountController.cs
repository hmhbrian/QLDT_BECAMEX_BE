using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Dtos.Results;
using QLDT_Becamex.Src.Dtos.Users;
using QLDT_Becamex.Src.Services.Implementations;
using QLDT_Becamex.Src.Services.Interfaces;

namespace QLDT_Becamex.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtService _jwtService;



        public AccountController(IUserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;


        }


        [HttpPost("register")]
        //[Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
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
                Result result = await _userService.RegisterAsync(dto);

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
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
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
    }
}
