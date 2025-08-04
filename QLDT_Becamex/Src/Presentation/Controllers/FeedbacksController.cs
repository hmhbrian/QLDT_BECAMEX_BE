using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Feedbacks.Commands;
using QLDT_Becamex.Src.Application.Features.Feedbacks.Dtos;
using QLDT_Becamex.Src.Application.Features.Feedbacks.Queries;

namespace QLDT_Becamex.Src.Presentation.Controllers
{
    [ApiController]
    [Route("api/feedback/{courseId}")]
    [Authorize]
    public class FeedbacksController : ControllerBase
    {
        public readonly IMediator _mediator;
        public FeedbacksController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackDto request, string courseId)
        {
            var command = new CreateFeedbackCommand(request, courseId);
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<string>.Ok(result, "Thêm feedback thành công"));
        }
        [HttpGet]
        public async Task<IActionResult> GetListFeedback(string courseId)
        {
            var query = new GetListFeedbackQuery(courseId);
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<List<FeedbacksDto>>.Ok(result, "Lấy danh sách feedback thành công"));
        }
    }
}