using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Dtos;
using QLDT_Becamex.Src.Services.Interfaces;

namespace QLDT_Becamex.Src.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentDto dto)
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
                var result = await _departmentService.CreateDepartmentAsync(dto);
                if (result.IsSuccess)
                {
                    return StatusCode(201, new
                    {
                        message = result.Message,
                        statusCode = result.StatusCode,
                        code = result.Code,
                        data = result.Data
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
                    error = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            try
            {
                var result = await _departmentService.GetAllDepartmentsAsync();
                if (result.IsSuccess)
                {
                    return Ok(new
                    {
                        message = result.Message,
                        statusCode = result.StatusCode,
                        code = result.Code,
                        data = result.Data
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
    }
}
