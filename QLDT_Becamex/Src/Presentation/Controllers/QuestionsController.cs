using MediatR;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Features.Questions.Commands;
using QLDT_Becamex.Src.Application.Features.Questions.Queries;
using QLDT_Becamex.Src.Application.Features.Questions.Dtos;
using QLDT_Becamex.Src.Application.Common.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace QLDT_Becamex.Src.Presentation.Controllers
{
    [ApiController]
    [Route("api/tests/{testId}/questions")]
    public class QuestionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get paginated list of questions for a test
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "ADMIN,HR,HOCVIEN")]
        public async Task<IActionResult> GetList([FromRoute] int testId, [FromQuery] BaseQueryParam queryParams)
        {
            var result = await _mediator.Send(new GetListQuestionQuery(testId, queryParams));
            return Ok(ApiResponse<PagedResult<QuestionDto>>.Ok(result));
        }

        /// <summary>
        /// Create new question under a test
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> Create([FromRoute] int testId, [FromBody] List<CreateQuestionDto> request)
        {
            var command = new CreateQuestionCommand(testId, request);
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<string>.Ok(result));
        }

        /// <summary>
        /// Update existing question under a test
        /// </summary>
        [HttpPut("{questionId}")]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> Update([FromRoute] int testId, [FromRoute] int questionId, [FromBody] UpdateQuestionDto request)
        {
            var command = new UpdateQuestionCommand(questionId, testId, request);
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<string>.Ok(result));
        }

        /// <summary>s
        /// Delete questions under a test
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "ADMIN, HR")]
        public async Task<IActionResult> Delete([FromRoute] int testId, [FromBody] List<int> questionIds)
        {
            var command = new DeleteQuestionsCommand(testId, questionIds);
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<string>.Ok(result));
        }
        [Authorize(Roles = "ADMIN, HR")]
        [HttpDelete("{questionId}")]
        public async Task<IActionResult> DeleteById([FromRoute] int testId, [FromRoute] int questionId)
        {
            var command = new DeleteQuestionCommand(testId, questionId);
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<string>.Ok(result));
        }
    }
}
