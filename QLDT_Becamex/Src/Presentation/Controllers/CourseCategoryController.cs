using MediatR;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.CourseCategory.Dtos;
using QLDT_Becamex.Src.Application.Features.CourseCategory.Queries;
using  QLDT_Becamex.Src.Application.Features.CourseCategory.Commands;
using Microsoft.AspNetCore.Authorization;

namespace QLDT_Becamex.Src.Presentation.Controllers
{
    [Route("api/CourseCategory")]
    [ApiController]
    public class CourseCategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CourseCategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN,HR,HOCVIEN")]
        public async Task<IActionResult> GetAllCourseCategory()
        {
            var result = await _mediator.Send(new GetAlCourseCategoryQuery());
            return Ok(ApiResponse<IEnumerable<CourseCategoryDto>>.Ok(result));
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN,HR")]

        public async Task<IActionResult> CreateCourseCategory([FromBody] CourseCategoryRqDto dto)
        {
            var result = await _mediator.Send(new CreateCourseCategoryCommand(dto));
            return StatusCode(201, ApiResponse.Ok("Thêm loại khóa học thành công"));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> UpdateCourseCategory(int id, [FromBody] CourseCategoryRqDto dto)
        {
            await _mediator.Send(new UpdateCourseCategoryCommand(id, dto));
            return Ok(ApiResponse.Ok("Cập nhật thành công"));
        }

        [HttpDelete]
        [Authorize]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> DeleteTypeDocument([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest(ApiResponse.Fail("Danh sách ID không được để trống"));

            await _mediator.Send(new DeleteCourseCategoryCommand(ids));
            return Ok(ApiResponse.Ok("Xóa thành công"));
        }
    }
}
