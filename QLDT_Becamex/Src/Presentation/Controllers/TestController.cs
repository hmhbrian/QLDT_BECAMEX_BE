using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Commands;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Presentation.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly ITestRepository _testRepository;
        public readonly IMediator _mediator;
        public TestController(ITestRepository testRepository, IMediator mediator)
        {
            _testRepository = testRepository;
            _mediator = mediator;
        }
        [HttpGet]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> GetAllTest()
        {
            var result = await _mediator.Send(new GetAllTestQuery());
            return Ok(ApiResponse<List<TestDto>>.Ok(result));
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> GetTestById(int id)
        {
            var result = await _mediator.Send(new GetTestByIdQuery(id));
            if (result == null)
            {
                return NotFound(ApiResponse.Fail("Bài kiểm tra không tồn tại"));
            }
            return Ok(ApiResponse<TestDto>.Ok(result));
        }
        [HttpPost("create")]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> CreateTest([FromBody] TestCreateDto request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateTestCommand(request), cancellationToken);
            return Ok(ApiResponse<string>.Ok(result, "Thêm bài kiểm tra thành công"));
        }
        [HttpPut("update/{id}")]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] TestUpdateDto request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdateTestCommand(id, request), cancellationToken);
            return Ok(ApiResponse.Ok("Cập nhật bài kiểm tra thành công"));
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> DeleteDepartment(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteTestCommand(id), cancellationToken);
            return Ok(ApiResponse.Ok("Xóa bài kiểm tra thành công"));
        }
    }
}