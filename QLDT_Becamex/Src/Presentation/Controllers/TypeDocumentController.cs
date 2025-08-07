using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.TypeDocument.Commands;
using QLDT_Becamex.Src.Application.Features.TypeDocument.Dtos;
using QLDT_Becamex.Src.Application.Features.TypeDocument.Queries;

namespace QLDT_Becamex.Src.Presentation.Controllers
{
    [Route("api/TypeDocument")]
    [ApiController]
    public class TypeDocumentController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TypeDocumentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTypeDocument()
        {
            var result = await _mediator.Send(new GetAlTypeDocumentQuery());
            return Ok(ApiResponse<IEnumerable<TypeDocumentDto>>.Ok(result));
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> CreateTypeDocument([FromBody] TypeDocumentRqDto dto)
        {
            var result = await _mediator.Send(new CreateTypeDocumentCommand(dto));
            return StatusCode(201, ApiResponse.Ok("Thêm loại tài liệu thành công"));
        }

        [Authorize(Roles = "ADMIN, HR")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateTypeDocument(int id, [FromBody] TypeDocumentRqDto dto)
        {
            await _mediator.Send(new UpdateTypeDocumentCommand(id, dto));
            return Ok(ApiResponse.Ok("Cập nhật thành công"));
        }

        [Authorize(Roles = "ADMIN, HR")]
        [HttpDelete]
        public async Task<IActionResult> DeleteTypeDocument([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest(ApiResponse.Fail("Danh sách ID không được để trống"));

            await _mediator.Send(new DeleteTypeDocumentCommand(ids));
            return Ok(ApiResponse.Ok("Xóa thành công"));
        }
    }
}
