using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.AuditLogs.Dtos;
using QLDT_Becamex.Src.Application.Features.AuditLogs.Queries;


namespace QLDT_Becamex.Src.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuditLogController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet("course")]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> GetCourseAuditLogs([FromQuery] string courseId)
        {
            var result = await _mediator.Send(new GetDetailCourseAuditLogsQuery(courseId));
            return Ok(ApiResponse<List<AuditLogDto>>.Ok(result));
        }
    }
}
