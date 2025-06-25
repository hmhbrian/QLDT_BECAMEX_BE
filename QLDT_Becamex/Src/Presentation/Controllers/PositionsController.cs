using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Positions.Commands;
using QLDT_Becamex.Src.Application.Features.Positions.Dtos;
using QLDT_Becamex.Src.Application.Features.Positions.Queries;
using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PositionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllPositionsQuery());
            return Ok(ApiResponse<IEnumerable<PositionDto>>.Ok(result));
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePositionDto request)
        {
            var result = await _mediator.Send(new CreatePositionCommand(request));
            return Ok(ApiResponse.Ok("Tạo thành công" + result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreatePositionDto request)
        {
            var result = await _mediator.Send(new UpdatePositionCommand(id, request));
            return Ok(ApiResponse.Ok("Cập nhật thành công " + result));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeletePositionCommand(id));
            return Ok(ApiResponse.Ok("Xóa thành công " + result));
        }
    }
}
