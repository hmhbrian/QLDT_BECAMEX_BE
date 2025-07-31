using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Dashboard.Dtos;
using QLDT_Becamex.Src.Application.Features.Dashboard.Queries;

namespace QLDT_Becamex.Src.Presentation.Controllers
{
    [Route("api/Dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }
        //HOCVIEN
        [HttpGet("Student")]
        [Authorize(Roles = "HOCVIEN")]
        public async Task<IActionResult> GetDataReportUser()
        {
            var result = await _mediator.Send(new GetDashBoardUserQuery());
            return Ok(ApiResponse<DataReportUserDto>.Ok(result));
        }
    }
}
