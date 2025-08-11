
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Roles.Commands;
using QLDT_Becamex.Src.Application.Features.Roles.Dtos;
using QLDT_Becamex.Src.Application.Features.Roles.Queries;

namespace QLDT_Becamex.Src.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _mediator.Send(new GetAllRolesQuery());
            return Ok(ApiResponse<IEnumerable<RoleDto>>.Ok(result));
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto request)
        {
            var result = await _mediator.Send(new CreateRoleCommand(request));
            return Ok(ApiResponse.Ok("Tạo thành công"));
        }
        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] CreateRoleDto request)
        {
            var result = await _mediator.Send(new UpdateRoleCommand(id, request));
            return Ok(ApiResponse.Ok("Cập nhật thành công"));
        }
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var result = await _mediator.Send(new DeleteRoleCommand(id));
            return Ok(ApiResponse.Ok("Xóa thành công"));
        }
    }
}
