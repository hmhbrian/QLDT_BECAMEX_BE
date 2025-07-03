using MediatR;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Queries;
using static QLDT_Becamex.Src.Application.Features.Tests.Dtos.TestReponseDTO;

namespace QLDT_Becamex.Src.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách bài kiểm tra của khóa học.HOCVIEN, HR, ADMIN có quyền truy cập
        /// </summary>
        /// <param name="courseId">ID của khóa học cần lấy danh sách bài kiểm tra.</param>
        /// <returns>ActionResult chứa danh sách bài kiểm tra hoặc lỗi nếu không tìm thấy.</returns>
        [HttpGet("OfCourse")]
        public async Task<IActionResult> GetListTestOfCourse([FromQuery] string courseId)
        {
            var result = await _mediator.Send(new GetListTestOfCourseQuery(courseId));
            return Ok(ApiResponse<List<AllTestDto>>.Ok(result));
        } 
    }
}
