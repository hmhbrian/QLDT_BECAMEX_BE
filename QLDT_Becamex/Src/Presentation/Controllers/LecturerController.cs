using MediatR;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Lecturer.Commands;
using QLDT_Becamex.Src.Application.Features.Lecturer.Dtos;
using QLDT_Becamex.Src.Application.Features.Lecturer.Queries;

namespace QLDT_Becamex.Src.Presentation.Controllers
{
    [ApiController]
    [Route("api/lecturer")]
    public class LecturerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LecturerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLecturers()
        {
            var result = await _mediator.Send(new GetAlLecturerQuery());
            return Ok(ApiResponse<IEnumerable<LecturerDto>>.Ok(result));
        }

        [HttpPost]
        public async Task<IActionResult> CreateLecturer([FromBody] LecturerDtoRq dto)
        {
            var result = await _mediator.Send(new CreateLecturerCommand(dto));
            return StatusCode(201, ApiResponse.Ok("Thêm giảng viên thành công"));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Updatelecturer(int id, [FromBody] LecturerDtoRq dto)
        {
            await _mediator.Send(new UpdateLecturerCommand(id, dto));
            return Ok(ApiResponse.Ok("Cập nhật thành công"));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteLecturer([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest(ApiResponse.Fail("Danh sách ID không được để trống"));

            await _mediator.Send(new DeleteLecturerCommand(ids));
            return Ok(ApiResponse.Ok("Xóa thành công"));
        }
    }
}
