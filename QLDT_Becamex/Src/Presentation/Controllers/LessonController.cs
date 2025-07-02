using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using static QLDT_Becamex.Src.Application.Features.Lessons.Dtos.LessonResponseDTO;
using QLDT_Becamex.Src.Application.Features.Lessons.Queries;

namespace QLDT_Becamex.Src.Presentation.Controllers
{
    [Route("api/[controller]")]
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
        /// <param name="courseId">ID của khóa học cần lấy danh sách bài học.</param>
        /// <returns>ActionResult chứa danh sách bài học hoặc lỗi nếu không tìm thấy.</returns>
        [HttpGet("OfCourse")]
        public async Task<IActionResult> GetListLessonOfCourse([FromQuery] string courseId)
        {
            var result = await _mediator.Send(new GetListLessonOfCourseQuery(courseId));
            return Ok(ApiResponse<List<AllLessonDto>>.Ok(result));
        }

        /// <summary>
        /// Lấy chi tiết bài học của khóa học. HOCVIEN, HR, ADMIN có quyền truy cập.
        /// </summary>
        /// <param name="LessonId">ID của bài học cần lấy thông tin.</param>
        /// <returns>ActionResult chứa thông tin chi tiết bài học hoặc lỗi nếu không tìm thấy.</returns>
        [HttpGet("OfCourse/{id}")]
        public async Task<IActionResult> GetLessonById(int id)
        {
            var result = await _mediator.Send(new GetLessonByIdQuery(id));
            return Ok(ApiResponse<DetailLessonDto>.Ok(result));
        }
    }
}
