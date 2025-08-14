using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Lessons.Commands;
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;
using QLDT_Becamex.Src.Application.Features.Lessons.Queries;

namespace QLDT_Becamex.Src.Presentation.Controllers
{
    [Route("api/courses/{courseId}/lessons")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly IMediator _mediator;
        public LessonController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách bài học của khóa học.HOCVIEN, HR, ADMIN có quyền truy cập
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "ADMIN,HR,HOCVIEN")]
        public async Task<IActionResult> GetListLessonOfCourse([FromRoute] string courseId)
        {
            var result = await _mediator.Send(new GetListLessonOfCourseQuery(courseId));
            return Ok(ApiResponse<List<AllLessonDto>>.Ok(result));
        }


        /// <summary>
        /// Thêm bài học cho khóa học.
        /// </summary>
        [HttpPost]

        [Consumes("multipart/form-data")] // Chỉ định rằng API mong đợi form-data (cho IFormFile)
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> CreateLessonOfCourse([FromRoute] string courseId, [FromForm] CreateLessonDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Trả về lỗi nếu validation thất bại
            }
            await _mediator.Send(new CreateLessonCommand(courseId, request));
            return Ok(ApiResponse.Ok("Create lesson success"));
        }

        /// <summary>
        /// Cập nhật thông tin chi tiết của một bài học.
        /// </summary>
        /// <remarks>
        [HttpPut("{lessonId}")] // PUT api/Lesson/{id}
        // Yêu cầu xác thực người dùng
        [Consumes("multipart/form-data")] // Chỉ định rằng API mong đợi form-data (cho IFormFile)
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> UpdateLesson([FromRoute] string courseId, [FromRoute] int lessonId, [FromForm] UpdateLessonDto request)
        {
            // Gửi lệnh cập nhật đến MediatR.
            // Command sẽ chứa ID của bài học (từ route) và dữ liệu cập nhật (từ form).
            await _mediator.Send(new UpdateLessonCommand(courseId, lessonId, request));

            // Trả về phản hồi thành công
            return Ok(ApiResponse.Ok("Lesson updated successfully."));
        }


        /// <summary>
        /// Xoá nhiều bài học theo khoá học.
        /// </summary>
        /// <param name="courseId">ID của khoá học</param>
        /// <param name="lessonIds">Danh sách ID bài học cần xoá</param>
        [HttpDelete]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> DeleteLessons(
            [FromRoute] string courseId,
            [FromBody] List<int> lessonIds)
        {
            if (lessonIds == null || !lessonIds.Any())
                return BadRequest(ApiResponse.Fail("Danh sách bài học không được để trống."));

            await _mediator.Send(new DeleteLessonCommand(courseId, lessonIds));
            return Ok(ApiResponse.Ok("Xoá bài học thành công."));
        }

        /// <summary>
        /// Lấy chi tiết bài học của khóa học. HOCVIEN, HR, ADMIN có quyền truy cập.
        /// </summary>
        /// <param name="LessonId">ID của bài học cần lấy thông tin.</param>
        /// <returns>ActionResult chứa thông tin chi tiết bài học hoặc lỗi nếu không tìm thấy.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,HR,HOCVIEN")]
        public async Task<IActionResult> GetLessonById(int id)
        {
            var result = await _mediator.Send(new GetLessonByIdQuery(id));
            return Ok(ApiResponse<DetailLessonDto>.Ok(result));
        }

        [HttpPut("reorder")]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> UpdatePositionLesson([FromRoute] string courseId, [FromForm] int LessonId, [FromForm] int? PreviousLessonId)
        {

            await _mediator.Send(new UpdatePositionLessonCommand(courseId, LessonId, PreviousLessonId));
            return Ok(ApiResponse.Ok("Position updated successfully."));
        }
        [HttpGet("count-completed")]
        [Authorize(Roles = "HOCVIEN")]
        public async Task<IActionResult> GetCountCompletedLessonOfCourse([FromRoute] string courseId)
        {
            var result = await _mediator.Send(new GetCountCompletedLessonOfCourseQuery(courseId));
            return Ok(ApiResponse<int>.Ok(result));
        }
    }
}
