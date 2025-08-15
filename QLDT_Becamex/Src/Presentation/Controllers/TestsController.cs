using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Lessons.Commands;
using QLDT_Becamex.Src.Application.Features.Lessons.Queries;
using QLDT_Becamex.Src.Application.Features.Tests.Commands;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Queries;
using QLDT_Becamex.Src.Domain.Entities;
using System.Security.Claims;

namespace QLDT_Becamex.Src.Presentation.Controllers
{
    [ApiController]
    [Route("api/courses/{courseId}/tests")]
    public class TestsController : ControllerBase
    {
        public readonly IMediator _mediator;
        public TestsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách bài kiểm tra của khóa học.HOCVIEN, HR, ADMIN có quyền truy cập
        /// </summary>
        [HttpGet]

        [Authorize]
        public async Task<IActionResult> GetListTestOfCourse([FromRoute] string courseId)
        {
            var result = await _mediator.Send(new GetListTestOfCourseQuery(courseId));
            return Ok(ApiResponse<List<AllTestDto>>.Ok(result));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetTestById(int id, [FromRoute] string courseId)
        {
            var result = await _mediator.Send(new GetTestByIdQuery(id, courseId));
            if (result == null)
            {
                return NotFound(ApiResponse.Fail("Bài kiểm tra không tồn tại"));
            }
            return Ok(ApiResponse<DetailTestDto>.Ok(result));
        }

        [HttpPost("create")]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> CreateTest([FromRoute] string courseId, [FromBody] TestCreateDto request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateTestCommand(request, courseId), cancellationToken);
            return Ok(ApiResponse<string>.Ok(result, "Thêm bài kiểm tra thành công"));
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> UpdateTest([FromRoute] string courseId, int id, [FromBody] TestUpdateDto request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdateTestCommand(id, request, courseId), cancellationToken);
            return Ok(ApiResponse.Ok("Cập nhật bài kiểm tra thành công"));
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> DeleteTest(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteTestCommand(id), cancellationToken);
            return Ok(ApiResponse.Ok("Xóa bài kiểm tra thành công"));
        }

        [HttpPut("reorder")]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> UpdatePositionTest([FromRoute] string courseId, [FromForm] int TestId, [FromForm] int PreviousTestId)
        {

            await _mediator.Send(new UpdatePositionTestCommand(courseId, TestId, PreviousTestId));
            return Ok(ApiResponse.Ok("Position updated successfully."));
        }

        // Trong API Controller
        [HttpPost("submit/{testId}")]
        public async Task<IActionResult> SubmitTest([FromBody] List<UserAnswerDto> submittedAnswers, [FromRoute] string courseId, [FromRoute] int testId, [FromQuery] DateTime StartedAt)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Cách khởi tạo vẫn y hệt
            var command = new SaveTestResultCommand(testId, courseId, StartedAt, submittedAnswers);

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<TestResultDto>.Ok(result, "Làm bài kiểm tra thành công!"));
        }

        [HttpGet("detail-test-result/{id}")]
        public async Task<IActionResult> GetDetailTestResult([FromRoute] string courseId, int id)
        {
            var command = new GetDetailTestResultQuery(id, courseId);

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<DetailTestResultDto>.Ok(result));
        }

        [HttpGet("no-answer/{testId}")]
        public async Task<IActionResult> GetListQuestionNoAnswer([FromRoute] string courseId, [FromRoute] int testId)
        {
            var result = await _mediator.Send(new GetTestNoAnswerQuery(courseId, testId));
            return Ok(ApiResponse<List<QuestionNoAnswerDto>>.Ok(result));
        }

        [HttpGet("count-completed")]
        [Authorize(Roles = "HOCVIEN")]
        public async Task<IActionResult> GetCountCompletedTestOfCourse([FromRoute] string courseId)
        {
            var result = await _mediator.Send(new GetCountCompletedTestOfCourseQuery(courseId));
            return Ok(ApiResponse<int>.Ok(result));
        }
    }
}