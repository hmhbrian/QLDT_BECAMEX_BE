using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Dtos;
using QLDT_Becamex.Src.Services.Interfaces;
using System.Collections.Generic; // Cần thiết cho List<string>
using System.Linq; // Cần thiết cho .Any()
using System.Threading.Tasks;

namespace QLDT_Becamex.Src.Controllers
{
    /// <summary>
    /// API Controller để quản lý các vị trí (chức danh).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PositionsController : ControllerBase
    {
        private readonly IPositionService _positionService;

        /// <summary>
        /// Khởi tạo một phiên bản mới của lớp <see cref="PositionsController"/>.
        /// </summary>
        /// <param name="positionService">Dịch vụ quản lý vị trí.</param>
        public PositionsController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        /// <summary>
        /// Tạo một vị trí (chức danh) mới.
        /// </summary>
        /// <param name="request">Đối tượng chứa thông tin yêu cầu tạo vị trí.</param>
        /// <returns>ActionResult chứa kết quả của thao tác tạo vị trí.</returns>
        [HttpPost]
        public async Task<IActionResult> CreatePosition([FromBody] PositionRq request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                    code = "INVALID", // Mã lỗi chung cho dữ liệu không hợp lệ
                    statusCode = StatusCodes.Status400BadRequest
                });
            }

            var result = await _positionService.CreatePositionAsync(request);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status201Created, new
                {
                    message = result.Message,
                    code = result.Code,
                    // Có thể bao gồm 'data = result.Data' nếu dịch vụ trả về dữ liệu (ví dụ: đối tượng đã tạo)
                });
            }
            else
            {
                // Sử dụng StatusCode từ Result, nếu không thì mặc định là 400 hoặc 500 tùy ngữ cảnh lỗi
                var statusCode = result.StatusCode ?? StatusCodes.Status400BadRequest;
                // Nếu lỗi do hệ thống và service trả về 500, thì statusCode sẽ là 500.
                // Nếu lỗi do logic nghiệp vụ (ví dụ: trùng tên) và service trả về 409, thì statusCode sẽ là 409.
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors.Any() ? result.Errors : new List<string> { result.Message }, // Đảm bảo errors luôn là một mảng
                    code = result.Code,

                });
            }
        }

        /// <summary>
        /// Lấy thông tin vị trí theo ID.
        /// </summary>
        /// <param name="id">ID của vị trí cần lấy.</param>
        /// <returns>ActionResult chứa thông tin vị trí hoặc lỗi nếu không tìm thấy.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPositionById(int id)
        {
            var result = await _positionService.GetPositionByIdAsync(id);

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
                var statusCode = result.StatusCode ?? StatusCodes.Status404NotFound; // Mặc định 404 nếu không tìm thấy
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code,

                });
            }
        }

        /// <summary>
        /// Lấy tất cả các vị trí (chức danh) hiện có.
        /// </summary>
        /// <returns>ActionResult chứa danh sách các vị trí.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllPositions()
        {
            var result = await _positionService.GetAllPositionsAsync();

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
                var statusCode = result.StatusCode ?? StatusCodes.Status500InternalServerError; // Mặc định 500 cho lỗi hệ thống khi lấy tất cả
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code,

                });
            }
        }

        /// <summary>
        /// Cập nhật thông tin của một vị trí (chức danh) hiện có.
        /// </summary>
        /// <param name="id">ID của vị trí cần cập nhật.</param>
        /// <param name="request">Đối tượng chứa thông tin yêu cầu cập nhật vị trí.</param>
        /// <returns>ActionResult chứa kết quả của thao tác cập nhật vị trí.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePosition(int id, [FromBody] PositionRq request)
        {
            // ID trong URL không cần kiểm tra is null/empty vì kiểu int
            if (!ModelState.IsValid)
            {

                return BadRequest(new
                {
                    message = "Dữ liệu đầu vào không hợp lệ.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                    code = "INVALID", // Thay đổi từ "INVALID_INPUT" sang "INVALID" theo bảng chung
                    statusCode = StatusCodes.Status400BadRequest
                });
            }

            var result = await _positionService.UpdatePositionAsync(id, request);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new
                {
                    message = result.Message,
                    code = result.Code,
                });
            }
            else
            {
                var statusCode = result.StatusCode ?? StatusCodes.Status400BadRequest; // Mặc định 400 cho lỗi cập nhật
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code,

                });
            }
        }

        /// <summary>
        /// Xóa một vị trí (chức danh) theo ID.
        /// </summary>
        /// <param name="id">ID của vị trí cần xóa.</param>
        /// <returns>ActionResult chứa kết quả của thao tác xóa vị trí.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePosition(int id)
        {
            var result = await _positionService.DeletePositionAsync(id);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new
                {
                    message = result.Message,
                    code = result.Code
                    // Không có data khi xóa thành công
                });
            }
            else
            {
                var statusCode = result.StatusCode ?? StatusCodes.Status404NotFound; // Mặc định 404 cho trường hợp không tìm thấy để xóa
                return StatusCode(statusCode, new
                {
                    message = result.Message,
                    errors = result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code,

                });
            }
        }
    }
}