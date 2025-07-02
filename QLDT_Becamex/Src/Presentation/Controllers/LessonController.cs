using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;
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
        /// Lấy danh sách bài học của khóa học.
        /// </summary>
        [HttpGet("Course")]
        public async Task<IActionResult> GetListLessonOfCourse([FromQuery] string courseId)
        {
            var result = await _mediator.Send(new GetListLessonOfCourseQuery(courseId));
            return Ok(ApiResponse<List<AllLessonDto>>.Ok(result));
        }
    }
}
