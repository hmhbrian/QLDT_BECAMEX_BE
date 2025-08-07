using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.LessonProgresses.Commands;
using QLDT_Becamex.Src.Application.Features.LessonProgresses.Dtos;
using QLDT_Becamex.Src.Application.Features.LessonProgresses.Queries;
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;

namespace QLDT_Becamex.Src.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonProgressController : ControllerBase
    {
        private readonly IMediator _mediator;
        public LessonProgressController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Tạo tiến độ bài học cho người dùng.
        /// </summary>
        [HttpPost("upsert-lesson-progress")]
        [Authorize(Roles = "ADMIN,HR,HOCVIEN")]
        public async Task<IActionResult> UpsertLessonProgressOfUser([FromBody] UpsertLessonProgressDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Trả về lỗi nếu validation thất bại
            }
            await _mediator.Send(new CreateLessonProgressOfUserCommand(request));
            return Ok(ApiResponse.Ok("Upsert lesson progress success"));
        }

        /// <summary>
        /// Lấy danh sách tiến độ bài học của người dùng theo CourseId.
        /// </summary>
        [HttpGet("get-lesson-progress/{courseId}")]
        [Authorize(Roles = "ADMIN,HR,HOCVIEN")]
        public async Task<IActionResult> GetListLessonProgressOfUser(string courseId)
        {

            var result = await _mediator.Send(new GetListLessonProgressOfUserQuery(courseId));
            return Ok(ApiResponse<List<AllLessonProgressDto>>.Ok(result));
        }
    }
}
