using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Dtos;
using QLDT_Becamex.Src.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLDT_Becamex.Src.Controllers
{
    /// <summary>
    /// API Controller để quản lý các trạng thái khóa học.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")] // Route sẽ là /api/CourseStatus
    public class CourseStatusController : ControllerBase
    {
        private readonly ICourseStatusService _courseStatusService;

        /// <summary>
        /// Khởi tạo một phiên bản mới của lớp <see cref="CourseStatusController"/>.
        /// </summary>
        /// <param name="courseStatusService">Dịch vụ quản lý trạng thái khóa học.</param>
        public CourseStatusController(ICourseStatusService courseStatusService)
        {
            _courseStatusService = courseStatusService;
        }

        /// <summary>
        /// Tạo một trạng thái khóa học mới.
        /// </summary>
        /// <param name="rq">Đối tượng yêu cầu chứa thông tin trạng thái khóa học mới.</param>
        /// <returns>ActionResult chứa kết quả của thao tác tạo trạng thái khóa học.</returns>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Create([FromBody] CourseStatusDtoRq rq)
        {
            if (!ModelState.IsValid)
            {
                // Dữ liệu đầu vào không hợp lệ
                return BadRequest(new
                {
                    message = "Dữ liệu không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                    code = "INVALID", // Mã lỗi chung: INVALID
                    statusCode = StatusCodes.Status400BadRequest
                });
            }

            var result = await _courseStatusService.CreateAsync(rq);

            // Kiểm tra trạng thái thành công/thất bại của Result từ Service
            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status201Created, new
                {
                    message = result.Message,
                    code = result.Code
                });
            }
            else
            {
                // Sử dụng StatusCode từ Result nếu có, nếu không thì mặc định là BadRequest (400)
                var statusCode = result.StatusCode ?? StatusCodes.Status400BadRequest;
                return StatusCode(statusCode, new // Dùng StatusCode thay vì BadRequest để linh hoạt hơn
                {
                    message = result.Message,
                    errors = result.Errors,
                    code = result.Code,
                    statusCode = statusCode
                });
            }
        }

        /// <summary>
        /// Lấy tất cả các trạng thái khóa học.
        /// </summary>
        /// <returns>ActionResult chứa danh sách các trạng thái khóa học.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _courseStatusService.GetAllAsync();

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
                var statusCode = result.StatusCode ?? StatusCodes.Status500InternalServerError; // Mặc định 500 cho lỗi GetAll
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors,
                    code = result.Code,
                    statusCode = statusCode
                });
            }
        }

        /// <summary>
        /// Cập nhật thông tin của một trạng thái khóa học hiện có.
        /// </summary>
        /// <param name="id">ID của trạng thái khóa học cần cập nhật.</param>
        /// <param name="rq">Đối tượng yêu cầu chứa thông tin cập nhật.</param>
        /// <returns>ActionResult chứa kết quả của thao tác cập nhật trạng thái khóa học.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Update(int id, [FromBody] CourseStatusDtoRq rq)
        {
            // ID trong URL không cần kiểm tra is null/empty vì kiểu int
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                    code = "INVALID", // Mã lỗi chung: INVALID
                    statusCode = StatusCodes.Status400BadRequest
                });
            }

            var result = await _courseStatusService.UpdateAsync(id, rq);

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
                var statusCode = result.StatusCode ?? StatusCodes.Status400BadRequest;
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors,
                    code = result.Code,
                    statusCode = statusCode
                });
            }
        }

        /// <summary>
        /// Xóa một hoặc nhiều trạng thái khóa học.
        /// </summary>
        /// <param name="ids">Danh sách các ID của trạng thái khóa học cần xóa.</param>
        /// <returns>ActionResult chứa kết quả của thao tác xóa trạng thái khóa học.</returns>
        [HttpDelete]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete([FromForm] List<int> ids)
        {
            // Kiểm tra danh sách ID hợp lệ
            if (ids == null || !ids.Any())
            {
                return BadRequest(new
                {
                    message = "Dữ liệu không hợp lệ.",
                    error = "Danh sách ID không được để trống.", // 'error' thay vì 'errors' nếu chỉ có một lỗi
                    code = "INVALID", // Mã lỗi chung: INVALID
                    statusCode = StatusCodes.Status400BadRequest
                });
            }

            var result = await _courseStatusService.DeleteAsync(ids);

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
                var statusCode = result.StatusCode ?? StatusCodes.Status400BadRequest;
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors,
                    code = result.Code,
                    statusCode = statusCode
                });
            }
        }
    }
}