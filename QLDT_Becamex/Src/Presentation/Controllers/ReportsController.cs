using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Reports.Dtos;
using QLDT_Becamex.Src.Application.Features.Reports.Queries;

namespace QLDT_Becamex.Src.Presentation.Controllers
{
    [Route("api/Report")]
    [ApiController]
    [Authorize(Roles = "ADMIN, HR")]
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

        [HttpGet("data-report")]
        public async Task<IActionResult> GetDataReport(int? month, int? quarter, int? year)
        {
            var result = await _mediator.Send(new GetDataReportQuery(month, quarter, year));
            return Ok(ApiResponse<DataReportDto>.Ok(result));
        }

        [HttpGet("students-of-course")]
        public async Task<IActionResult> GetStudentsOfCourse()
        {
            var result = await _mediator.Send(new GetListStudentOfCourseQuery());
            return Ok(ApiResponse<List<StudentOfCourseDto>>.Ok(result));
        }

        [HttpGet("top-department")]
        public async Task<IActionResult> GetTopDepartment()
        {
            var result = await _mediator.Send(new GetDepartmentCourseReportQuery());
            return Ok(ApiResponse<List<DepartmentCourseReportDto>>.Ok(result));
        }

        [HttpGet("report-status")]
        public async Task<IActionResult> GetReportStatus()
        {
            var result = await _mediator.Send(new GetCourseStatusDistributionQuery());
            return Ok(ApiResponse<List<StatusCourseReportDto>>.Ok(result));
        }
    }
}
