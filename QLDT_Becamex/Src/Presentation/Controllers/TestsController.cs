using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Commands;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Queries;

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
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> GetListTestOfCourse([FromRoute] string courseId)
        {
            var result = await _mediator.Send(new GetListTestOfCourseQuery(courseId));
            return Ok(ApiResponse<List<AllTestDto>>.Ok(result));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN, HR")]
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
        public async Task<IActionResult> CreateTest([FromRoute] string courseId ,[FromBody] TestCreateDto request, CancellationToken cancellationToken)
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
    }
}