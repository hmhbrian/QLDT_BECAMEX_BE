using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.EmployeeLevels.Commands;
using QLDT_Becamex.Src.Application.Features.EmployeeLevels.Dtos;
using QLDT_Becamex.Src.Application.Features.EmployeeLevels.Queries;
using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeLevelController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeLevelController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN,HR,HOCVIEN")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllELevelsQuery());
            return Ok(ApiResponse<IEnumerable<ELevelDto>>.Ok(result));
        }


        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Create([FromBody] CreateELevelDto request)
        {
            var result = await _mediator.Send(new CreateELevelCommand(request));
            return Ok(ApiResponse.Ok("Tạo thành công" + result));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateELevelDto request)
        {
            var result = await _mediator.Send(new UpdateELevelCommand(id, request));
            return Ok(ApiResponse.Ok("Cập nhật thành công " + result));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteELevelCommand(id));
            return Ok(ApiResponse.Ok("Xóa thành công " + result));
        }
    }
}
