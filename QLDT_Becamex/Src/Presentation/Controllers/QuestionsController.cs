using MediatR;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Features.Questions.Commands;
using QLDT_Becamex.Src.Application.Features.Questions.Queries;
using QLDT_Becamex.Src.Application.Features.Questions.Dtos;
using QLDT_Becamex.Src.Application.Common.Dtos;

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
        public async Task<IActionResult> GetList([FromRoute] int testId, [FromQuery] BaseQueryParam queryParams)
        {
            var result = await _mediator.Send(new GetListQuestionQuery(testId, queryParams));
            return Ok(ApiResponse<PagedResult<QuestionDto>>.Ok(result));
        }

        /// <summary>
        /// Create new question under a test
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromRoute] int testId, [FromBody] CreateQuestionCommand command)
        {
            command = command with { TestId = testId };
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<string>.Ok(result));
        }

        /// <summary>
        /// Update existing question under a test
        /// </summary>
        [HttpPut("{questionId}")]
        public async Task<IActionResult> Update([FromRoute] int testId, [FromRoute] int questionId, [FromBody] UpdateQuestionCommand command)
        {
            if (questionId != command.QuestionId)
                return BadRequest(ApiResponse<string>.Fail("ID trong route và body không khớp", 400));

            command = command with { TestId = testId };
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<string>.Ok(result));
        }

        /// <summary>
        /// Delete questions under a test
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] int testId, [FromBody] DeleteQuestionsCommand command)
        {
            command = command with { TestId = testId };
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<string>.Ok(result));
        }
    }
}
