    
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Departments.Commands;
using QLDT_Becamex.Src.Application.Features.Departments.Dtos;
using QLDT_Becamex.Src.Application.Features.Departments.Queries;


namespace QLDT_Becamex.Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DepartmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentRequestDto request)
        {
            var result = await _mediator.Send(new CreateDepartmentCommand(request));
            return Ok(ApiResponse<string>.Ok("Tạo phòng ban thành công"));

        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> GetAllDepartments()
        {
            var result = await _mediator.Send(new GetAllDepartmentQuery());
            return Ok(ApiResponse<List<DepartmentDto>>.Ok(result));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var result = await _mediator.Send(new GetDepartmentByIdQuery(id));
            return Ok(ApiResponse<DepartmentDto>.Ok(result));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] DepartmentRequestDto request)
        {
            var result = await _mediator.Send(new UpdateDepartmentCommand(id, request));
            return Ok(ApiResponse.Ok("Cập nhật phòng ban thành công"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var result = await _mediator.Send(new DeleteDepartmentCommand(id));
            return Ok(ApiResponse.Ok("Xóa phòng ban thành công"));
        }
    }
}
