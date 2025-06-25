using MediatR;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Commands;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Queries;
using QLDT_Becamex.Src.Application.Features.Status.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;

namespace QLDT_Becamex.Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CoursesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Tạo mới một khóa học.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromForm] CreateCourseDto request)
        {
            var result = await _mediator.Send(new CreateCourseCommand(request));
            return Ok(ApiResponse.Ok(result));
        }

        /// <summary>
        /// Cập nhật một khóa học theo Id.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(string id, [FromForm] CreateCourseDto request)
        {
            var result = await _mediator.Send(new UpdateCourseCommand(id, request));
            return Ok(ApiResponse.Ok(result));
        }

        /// <summary>
        /// Lấy chi tiết khóa học theo Id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(string id)
        {
            var result = await _mediator.Send(new GetCourseByIdQuery(id));
            return Ok(ApiResponse<CourseDto>.Ok(result));
        }

        /// <summary>
        /// Lấy danh sách khóa học (dùng phân trang và sắp xếp).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCourses([FromQuery] BaseQueryParam queryParam)
        {
            var result = await _mediator.Send(new GetListCourseQuery(queryParam));
            return Ok(ApiResponse<PagedResult<CourseDto>>.Ok(result));
        }

        /// <summary>
        /// Tìm kiếm khóa học theo nhiều tiêu chí.
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchCourses([FromQuery] BaseQueryParamFilter queryParam)
        {
            var result = await _mediator.Send(new SearchCoursesQuery(queryParam));
            return Ok(ApiResponse<PagedResult<CourseDto>>.Ok(result));
        }

        [HttpDelete("soft-delete")]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> DeleteCourse([FromQuery] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu truy vấn không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage),
                    code = "INVALID",
                    statusCode = 404
                });
            }

            var result = await _courseService.DeleteCourseAsync(id);

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
    }
}
