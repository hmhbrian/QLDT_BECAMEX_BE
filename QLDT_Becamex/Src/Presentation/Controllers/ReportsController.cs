using MediatR;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Reports.Dtos;
using QLDT_Becamex.Src.Application.Features.Reports.Queries;
using QLDT_Becamex.Src.Application.Features.TypeDocument.Commands;
using QLDT_Becamex.Src.Application.Features.TypeDocument.Dtos;
using QLDT_Becamex.Src.Application.Features.TypeDocument.Queries;

namespace QLDT_Becamex.Src.Presentation.Controllers
{
    [Route("api/Report")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("course-and-avg-feedback")]
        public async Task<IActionResult> GetAllCourseAndAvgFeedback()
        {
            var result = await _mediator.Send(new GetListCourseAndAvgFeedbackQuery());
            return Ok(ApiResponse<List<CourseAndAvgFeedbackDto>>.Ok(result));
        }

        [HttpGet("avg-feedback")]
        public async Task<IActionResult> GetAvgFeedback()
        {
            var result = await _mediator.Send(new GetAvgFeedbackQuery());
            return Ok(ApiResponse<AvgFeedbackDto>.Ok(result));
        }

        [HttpGet("students-of-course")]
        public async Task<IActionResult> GetStudentsOfCourse()
        {
            var result = await _mediator.Send(new GetListStudentOfCourseQuery());
            return Ok(ApiResponse<List<StudentOfCourseDto>>.Ok(result));
        }
    }
}
