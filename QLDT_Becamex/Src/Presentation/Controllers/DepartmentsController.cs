using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Dtos;
using QLDT_Becamex.Src.Services.Interfaces;

namespace QLDT_Becamex.Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentRq dto)
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

        [HttpGet]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> GetAllDepartments()
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

        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> GetDepartmentById(int id)
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

            var result = await _departmentService.GetDepartmentByIdAsync(id);
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

            return NotFound(new
            {
                message = result.Message,
                errors = result.Errors,
                statusCode = result.StatusCode,
                code = result.Code
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] DepartmentRq dto)
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

            var result = await _departmentService.UpdateDepartmentAsync(id, dto);
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteDepartment(int id)
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

            var result = await _departmentService.DeleteDepartmentAsync(id);
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

            return NotFound(new
            {
                message = result.Message,
                errors = result.Errors,
                statusCode = result.StatusCode,
                code = result.Code
            });
        }
    }
}
