
using Microsoft.AspNetCore.Mvc;

using QLDT_Becamex.Src.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using QLDT_Becamex.Src.Application.Dtos;


namespace QLDT_Becamex.Src.Controllers
{
    /// <summary>
    /// API Controller để quản lý các khóa học.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        /// <summary>
        /// Khởi tạo một phiên bản mới của lớp <see cref="CoursesController"/>.
        /// </summary>
        /// <param name="courseService">Dịch vụ khóa học.</param>
        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        /// <summary>
        /// Tạo một khóa học mới.
        /// </summary>
        /// <param name="rq">Đối tượng yêu cầu tạo khóa học.</param>
        /// <returns>ActionResult chứa kết quả của thao tác tạo khóa học.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CourseDtoRq rq)
        {
            // Kiểm tra ModelState.IsValid trước khi gọi service
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage),
                    code = "INVALID", // Mã lỗi chung cho dữ liệu không hợp lệ
                    statusCode = StatusCodes.Status400BadRequest // Dữ liệu đầu vào không hợp lệ
                });
            }

            // Gọi service và xử lý kết quả
            var result = await _courseService.CreateAsync(rq);

            if (result.IsSuccess)
            {
                // Sử dụng ?? StatusCodes.Status201Created cho trường hợp tạo thành công
                return StatusCode(result.StatusCode ?? StatusCodes.Status201Created, new
                {
                    message = result.Message,
                    statusCode = result.StatusCode ?? StatusCodes.Status201Created,
                    code = result.Code,

                });
            }
            else
            {
                // Sử dụng ?? StatusCodes.Status400BadRequest cho trường hợp lỗi,
                // hoặc mã lỗi cụ thể từ service nếu có (ví dụ: 409 CONFLICT)
                return BadRequest(new
                {
                    message = result.Message,
                    errors = result.Errors,
                    statusCode = result.StatusCode ?? StatusCodes.Status400BadRequest,
                    code = result.Code
                });
            }
        }

        /// <summary>
        /// Cập nhật thông tin của một khóa học.
        /// </summary>
        /// <param name="id">ID của khóa học cần cập nhật.</param>
        /// <param name="rq">Đối tượng yêu cầu cập nhật khóa học.</param>
        /// <returns>ActionResult chứa kết quả của thao tác cập nhật khóa học.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromForm] CourseDtoRq rq)
        {
            // Kiểm tra ID trống
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { message = "ID không được để trống.", code = "INVALID", statusCode = StatusCodes.Status400BadRequest });
            }

            // Kiểm tra ModelState.IsValid
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage),
                    code = "INVALID",
                    statusCode = StatusCodes.Status400BadRequest
                });
            }

            // Gọi service và xử lý kết quả
            var result = await _courseService.UpdateAsync(id, rq);

            if (result.IsSuccess)
            {
                // Sử dụng ?? StatusCodes.Status200OK cho trường hợp cập nhật thành công
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new
                {
                    message = result.Message,
                    statusCode = result.StatusCode ?? StatusCodes.Status200OK,
                    code = result.Code
                });
            }
            else
            {
                // Sử dụng ?? StatusCodes.Status400BadRequest cho trường hợp lỗi
                return BadRequest(new
                {
                    message = result.Message,
                    errors = result.Errors,
                    statusCode = result.StatusCode ?? StatusCodes.Status400BadRequest,
                    code = result.Code
                });
            }

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(id);
            }
            var result = await _courseService.GetCourseAsync(id);
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
        [HttpGet]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> GetAllCourses([FromQuery] BaseQueryParam queryParam)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(queryParam);
            }
            var result = await _courseService.GetAllCoursesAsync(queryParam);
            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status201Created, new
                {
                    message = result.Message,
                    code = result.Code,
                    data = result.Data

                });
            }
            return StatusCode(result.StatusCode ?? 500, new
            {
                message = result.Message,
                errors = result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                code = result.Code
            });
        }
    }
}