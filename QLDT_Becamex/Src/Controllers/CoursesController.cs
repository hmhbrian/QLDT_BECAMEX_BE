using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Models;
using QLDT_Becamex.Src.Dtos.Courses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLDT_Becamex.Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        // Tạm thời dùng mock data, sau này sẽ inject service/repository
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            // TODO: Lấy danh sách courses từ database/service
            return Ok(new List<CourseDto>()); // Trả về danh sách rỗng mẫu
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourse(int id)
        {
            // TODO: Lấy chi tiết course theo id
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CourseDto dto)
        {
            // TODO: Thêm mới course
            return Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCourse(int id, [FromBody] CourseDto dto)
        {
            // TODO: Cập nhật course
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCourse(int id)
        {
            // TODO: Xóa course
            return NoContent();
        }
    }
}
