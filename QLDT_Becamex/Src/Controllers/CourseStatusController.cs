using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Dtos.Courses;
using QLDT_Becamex.Src.Dtos.Results;
using QLDT_Becamex.Src.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLDT_Becamex.Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Route sẽ là /api/CourseStatus
    public class CourseStatusController : ControllerBase
    {
        private readonly ICourseStatusService _courseStatusService;

        public CourseStatusController(ICourseStatusService courseStatusService)
        {
            _courseStatusService = courseStatusService;
        }

        // --- Tạo mới ---
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Create([FromBody] CourseStatusDtoRq rq)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Result.Failure(
                     message: "Validation failed.",
                     errors: ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                     code: "VALIDATION_ERROR",
                     statusCode: StatusCodes.Status400BadRequest
                 ));
            }

            var result = await _courseStatusService.CreateAsync(rq);

            return StatusCode(result.StatusCode ?? StatusCodes.Status201Created, new
            {
                message = result.Message,
                code = result.Code,
            });
        }

        // --- Lấy danh sách ---
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _courseStatusService.GetAllAsync();

            return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new
            {
                message = result.Message,
                code = result.Code,
                data = result.Data
            });
        }

        // --- Cập nhật ---
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Update(int id, [FromBody] CourseStatusDtoRq rq)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Result.Failure(
                    message: "Validation failed.",
                    errors: ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(),
                    code: "VALIDATION_ERROR",
                    statusCode: StatusCodes.Status400BadRequest
                ));
            }

            var result = await _courseStatusService.UpdateAsync(id, rq);

            return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new
            {
                message = result.Message,
                code = result.Code
            });
        }

        // --- Xóa ---
        [HttpDelete]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete([FromForm] List<int> ids)
        {
            // Kiểm tra danh sách ID hợp lệ
            if (ids == null || !ids.Any())
            {
                return BadRequest(Result.Failure(
                    message: "Validation failed.",
                    error: "Danh sách ID không được để trống.",
                    code: "VALIDATION_ERROR",
                    statusCode: StatusCodes.Status400BadRequest
                ));
            }

            var result = await _courseStatusService.DeleteAsync(ids);

            return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new
            {
                message = result.Message,
                code = result.Code
            });
        }
    }
}
