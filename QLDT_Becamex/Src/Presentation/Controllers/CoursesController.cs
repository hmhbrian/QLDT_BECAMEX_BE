using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Commands;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Queries;

namespace QLDT_Becamex.Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CoursesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Tạo mới một khóa học.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> CreateCourse([FromForm] CreateCourseDto request)
        {
            var result = await _mediator.Send(new CreateCourseCommand(request));
            return Ok(ApiResponse.Ok("Tạo khóa học thành công"));
        }

        /// <summary>
        /// Cập nhật một khóa học theo Id.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> UpdateCourse(string id, [FromForm] UpdateCourseDto request)
        {
            var result = await _mediator.Send(new UpdateCourseCommand(id, request));
            return Ok(ApiResponse.Ok("Cập nhật khóa học thành công"));
        }

        /// <summary>
        /// Lấy chi tiết khóa học theo Id.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCourseById(string id)
        {
            var result = await _mediator.Send(new GetCourseByIdQuery(id));
            return Ok(ApiResponse<CourseDto>.Ok(result));
        }

        /// <summary>
        /// Lấy danh sách khóa học (dùng phân trang và sắp xếp).
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCourses([FromQuery] BaseQueryParam queryParam)
        {
            var result = await _mediator.Send(new GetListCourseQuery(queryParam));
            return Ok(ApiResponse<PagedResult<CourseDto>>.Ok(result));
        }

        /// <summary>
        /// Tìm kiếm khóa học theo nhiều tiêu chí.
        /// </summary>
        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> SearchCourses([FromQuery] BaseQueryParamFilter queryParam)
        {
            var result = await _mediator.Send(new SearchCoursesQuery(queryParam));
            return Ok(ApiResponse<PagedResult<CourseDto>>.Ok(result));
        }

        [HttpDelete("soft-delete")]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> DeleteCourse([FromQuery] string id)
        {
            var result = await _mediator.Send(new DeleteCourseCommand(id));
            return Ok(ApiResponse.Ok("Xóa khóa học thành công"));
        }

        [HttpPost("{courseId}/enroll")]
        [Authorize(Roles = "HOCVIEN")]
        public async Task<IActionResult> EnrollCourse(string courseId)
        {
            var result = await _mediator.Send(new EnrollCourseCommand(courseId));
            return Ok(ApiResponse.Ok(result));

        }
        [HttpPost("{courseId}/cancel-enroll")]
        [Authorize(Roles = "HOCVIEN")]
        public async Task<IActionResult> CancelEnrollCourse(string courseId)
        {
            var result = await _mediator.Send(new CancelEnrollCourseCommand(courseId));
            return Ok(ApiResponse.Ok(result));
        }

        /// <summary>
        /// Lấy danh sách khóa học của tôi(Học Viên)
        /// </summary>
        [HttpGet("enroll-courses")]
        [Authorize(Roles = "HR,HOCVIEN")]
        public async Task<IActionResult> GetListEnrollCourse([FromQuery] BaseQueryParamMyCourse queryParams, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetListEnrollCourseQuery(queryParams), cancellationToken);
            return Ok(ApiResponse<PagedResult<UserEnrollCourseDto>>.Ok(result)); // Bao kết quả tại đây
        }

        /// <summary>
        /// chưa sử dụng
        /// </summary>
        [HttpGet("progress/{id}")]
        [Authorize]
        public async Task<IActionResult> GetCourseProgress(string id)
        {
            var result = await _mediator.Send(new GetCourseProgressQuery(id));
            return Ok(ApiResponse<float>.Ok(result));
        }

        /// <summary>
        /// Đếm số lượng khóa học hoàn thành
        /// </summary>
        [HttpGet("completed-count")]
        [Authorize]
        public async Task<IActionResult> GetCompletedCoursesCount()
        {
            var result = await _mediator.Send(new GetCompletedCoursesCountQuery());
            return Ok(ApiResponse<int>.Ok(result));
        }

        /// <summary>
        /// Hiển thị các khóa học sắp tới
        /// </summary>
        [HttpGet("upcoming-courses")]
        [Authorize(Roles = "HR,HOCVIEN")]
        public async Task<IActionResult> GetListUpcomingCourse(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetListUpcomingCourseQuery(), cancellationToken);
            return Ok(ApiResponse<List<UserUpcomingCourseDto>>.Ok(result));
        }
        
        /// <summary>
        ///Hiển thị ds khóa học đã hoàn thành trong hố sơ học viên
        /// </summary>
        [HttpGet("completed-enroll-courses")]
        [Authorize]
        public async Task<IActionResult> GetCompletedEnrollCourses([FromQuery] BaseQueryParam queryParams, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCompletedCoursesQuery(queryParams), cancellationToken);
            return Ok(ApiResponse<PagedResult<UserEnrollCompletedCourseDto>>.Ok(result));
        }


        /// <summary>
        ///Hiển thị danh sách tiến độ của các học viên trong khóa học
        /// </summary>
        [HttpGet("progress-list/{courseId}")]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> GetListUserCoursesProgress(string courseId, [FromQuery] BaseQueryParam queryParams, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetListUserCoursesProgressQuery(queryParams, courseId), cancellationToken);
            return Ok(ApiResponse<PagedResult<UserCourseProgressDto>>.Ok(result));
        }


        /// <summary>
        /// Hiển thị chi tiết tiến độ của 1 học viên trong khóa học
        /// </summary>
        [HttpGet("progress-detail/{courseId}/{userId}")]
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> GetUserCourseProgressDetail(string courseId, string userId)
        {
            var result = await _mediator.Send(new GetDetailUserCoursesProgressQuery(userId, courseId));
            return Ok(ApiResponse<DetailedUserCourseProgressDto>.Ok(result));
        }

        /// <summary>
        /// Tìm kiếm khóa học công khai theo tên.
        /// </summary>
        [HttpGet("search-public-course")]
        [Authorize]
        public async Task<IActionResult> SearchPublicCourses([FromQuery] BaseQueryParamSearch queryParam)
        {
            var result = await _mediator.Send(new SearchPublicCourseQuery(queryParam));
            return Ok(ApiResponse<PagedResult<CourseDto>>.Ok(result));
        }
    }
}
