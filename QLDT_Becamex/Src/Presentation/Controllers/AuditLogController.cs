using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.AuditLogs.Dtos;
using QLDT_Becamex.Src.Application.Features.AuditLogs.Queries;
using QLDT_Becamex.Src.Application.Features.Courses.Commands;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;
using QLDT_Becamex.Src.Application.Features.Lessons.Queries;
using QLDT_Becamex.Src.Domain.Entities;

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
