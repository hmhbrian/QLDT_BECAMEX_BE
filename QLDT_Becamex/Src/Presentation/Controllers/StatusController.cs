using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Status.Commands;
using QLDT_Becamex.Src.Application.Features.Status.Dtos;
using QLDT_Becamex.Src.Application.Features.Status.Queries;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Queries;

namespace QLDT_Becamex.Src.Presentation.Controllers
{
    [ApiController]
    [Route("api/status")]

    public class StatusController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StatusController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ===== Course Status =====

        [HttpGet("courses")]
        public async Task<IActionResult> GetAllCourseStatuses()
        {
            var result = await _mediator.Send(new GetAllCourseStatusesQuery());
            return Ok(ApiResponse<IEnumerable<StatusDto>>.Ok(result));
        }

        [HttpPost("courses")]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> CreateCourseStatus([FromBody] CreateStatusDto dto)
        {
            await _mediator.Send(new CreateCourseStatusCommand(dto));
            return StatusCode(201, ApiResponse.Ok("Tạo thành công"));
        }

        [Authorize(Roles = "ADMIN, HR")]
        [HttpPut("courses/{id:int}")]
        public async Task<IActionResult> UpdateCourseStatus(int id, [FromBody] CreateStatusDto dto)
        {
            await _mediator.Send(new UpdateCourseStatusCommand(id, dto));
            return Ok(ApiResponse.Ok("Cập nhật thành công"));
        }
        [Authorize(Roles = "ADMIN, HR")]
        [HttpDelete("courses")]
        public async Task<IActionResult> DeleteCourseStatuses([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest(ApiResponse.Fail("Danh sách ID không được để trống"));

            await _mediator.Send(new DeleteCourseStatusesCommand(ids));
            return Ok(ApiResponse.Ok("Xóa thành công"));
        }

        // ===== User Status =====

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUserStatuses()
        {
            var result = await _mediator.Send(new GetAllUserStatusesQuery());
            return Ok(ApiResponse<IEnumerable<UserStatusDto>>.Ok(result));
        }

        [HttpPost("users")]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> CreateUserStatus([FromBody] UserStatusDtoRq dto)
        {
            var result = await _mediator.Send(new CreateUserStatusCommand(dto));
            return StatusCode(201, ApiResponse.Ok("Tạo thành công"));
        }
        [Authorize(Roles = "ADMIN, HR")]
        [HttpPut("users/{id:int}")]
        public async Task<IActionResult> UpdateUserStatus(int id, [FromBody] UserStatusDtoRq dto)
        {
            await _mediator.Send(new UpdateUserStatusCommand(id, dto));
            return Ok(ApiResponse.Ok("Cập nhật thành công"));
        }
        [Authorize(Roles = "ADMIN, HR")]
        [HttpDelete("users")]
        public async Task<IActionResult> DeleteUserStatuses([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest(ApiResponse.Fail("Danh sách ID không được để trống"));

            await _mediator.Send(new DeleteUserStatusesCommand(ids));
            return Ok(ApiResponse.Ok("Xóa thành công"));
        }

        // ===== Department Status =====

        [HttpGet("department")]
        public async Task<IActionResult> GetAllDepartmentStatuses()
        {
            var result = await _mediator.Send(new GetAllDepartmentStatusesQuery());
            return Ok(ApiResponse<IEnumerable<StatusDto>>.Ok(result));
        }

        [HttpPost("department")]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> CreateDepartmentStatus([FromBody] CreateStatusDto dto)
        {
            await _mediator.Send(new CreateDepartmentStatusCommand(dto));
            return StatusCode(201, ApiResponse.Ok("Tạo thành công"));
        }
        [Authorize(Roles = "ADMIN, HR")]
        [HttpPut("department/{id:int}")]
        public async Task<IActionResult> UpdateDepartmentStatus(int id, [FromBody] CreateStatusDto dto)
        {
            await _mediator.Send(new UpdateDepartmentStatusCommand(id, dto));
            return Ok(ApiResponse.Ok("Cập nhật thành công"));
        }
        [Authorize(Roles = "ADMIN, HR")]
        [HttpDelete("department")]
        public async Task<IActionResult> DeleteDepartmentStatuses([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest(ApiResponse.Fail("Danh sách ID không được để trống"));

            await _mediator.Send(new DeleteDepartmentStatusCommand(ids));
            return Ok(ApiResponse.Ok("Xóa thành công"));
        }
    }
}
