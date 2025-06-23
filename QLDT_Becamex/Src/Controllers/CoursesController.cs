using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Dtos.Courses;
using QLDT_Becamex.Src.Dtos.Departments;
using QLDT_Becamex.Src.Dtos.Results;
using QLDT_Becamex.Src.Services.Interfaces;

namespace QLDT_Becamex.Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CourseDtoRq rq)
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
                var result = await _courseService.CreateAsync(rq);
                if (result.IsSuccess)
                {
                    return StatusCode(201, new
                    {
                        message = result.Message,
                        statusCode = result.StatusCode,
                        code = result.Code,
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromForm] CourseDtoRq rq)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(id);
            }
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
                var result = await _courseService.UpdateAsync(id, rq);
                if (result.IsSuccess)
                {
                    return StatusCode(201, new
                    {
                        message = result.Message,
                        statusCode = result.StatusCode,
                        code = result.Code,
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(id);
            }
            try
            {
                Result<CourseDto> result = await _courseService.GetCourseAsync(id);

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
    }
}
