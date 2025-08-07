using MediatR;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Features.CourseAttachedFiles.Commands; // Namespace của Command
using QLDT_Becamex.Src.Application.Features.CourseAttachedFile.Dtos; // Namespace của DTO
using QLDT_Becamex.Src.Application.Common.Dtos;
using Microsoft.AspNetCore.Authorization;
using QLDT_Becamex.Src.Application.Features.CourseAttachedFiles.Queries; // Để sử dụng AppException nếu có

namespace QLDT_Becamex.Src.Presentation.Controllers
{
    [ApiController] // Đánh dấu đây là một API controller
    [Authorize]
    [Route("api/courseattachedfiles")] // Định nghĩa route cho controller này
    public class CourseAttachedFileController : ControllerBase
    {
        private readonly IMediator _mediator;

        // Inject IMediator vào constructor để gửi các lệnh và truy vấn
        public CourseAttachedFileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Tạo một file đính kèm mới cho khóa học.
        /// Chấp nhận file PDF/PPT hoặc một liên kết.
        /// </summary>
        /// <param name="model">Dữ liệu tạo file đính kèm (CourseId, Title, File hoặc Link)</param>
        /// <returns>Thông tin chi tiết của file đính kèm đã tạo.</returns>
        [HttpPost("{courseId}")]
        [Authorize(Roles = "ADMIN,HR")]
        [Consumes("multipart/form-data")] // Quan trọng: Cho phép nhận dữ liệu form-data (bao gồm cả file)
        public async Task<IActionResult> CreateMultiple(
            [FromRoute] string courseId, // Lấy CourseId từ route
            [FromForm] List<CreateCourseAttachedFileDto> request // Lấy danh sách từ form
        )
        {

            var command = new CreateCourseAttachedFileCommand(courseId, request);

            // Gửi command đến MediatR để Handler xử lý
            // Handler sẽ trả về CourseAttachedFileDto sau khi tạo thành công
            var createdFile = await _mediator.Send(command);

            // Trả về HTTP 201 Created cùng với thông tin của file đã tạo
            return Ok(ApiResponse<List<CourseAttachedFileDto>>.Ok(createdFile, "Cập nhật thành công"));

        }

        /// <summary>
        /// Xóa một file đính kèm cụ thể khỏi một khóa học.
        /// </summary>
        /// <param name="courseId">ID của khóa học.</param>
        /// <param name="fileId">ID của file đính kèm cần xóa.</param>
        /// <returns>Thông báo xác nhận xóa thành công.</returns>
        [HttpDelete("{courseId}/{fileId}")] // Định tuyến cụ thể cho action DELETE
        [Authorize(Roles = "ADMIN,HR")]
        public async Task<IActionResult> DeleteAttachedFile(
            [FromRoute] string courseId,
            [FromRoute] int fileId)
        {
            var command = new DeleteCourseAttachedFileCommand(courseId, fileId);
            var result = await _mediator.Send(command);

            // Trả về ApiResponse thành công với thông báo string
            return Ok(ApiResponse<string>.Ok(result, "Xóa file đính kèm thành công!")); // `result` là thông báo xóa thành công từ handler
        }


        /// <summary>
        /// Lấy tất cả các file đính kèm cho một khóa học cụ thể.
        /// </summary>
        /// <param name="courseId">ID của khóa học.</param>
        /// <returns>Danh sách các file đính kèm của khóa học.</returns>
        [HttpGet("{courseId}")] // Định tuyến cụ thể cho action GET (không cần thêm gì vì route đã có courseId)
        [Authorize(Roles = "ADMIN,HR,HOCVIEN")]
        public async Task<IActionResult> GetAllAttachedFiles(
            [FromRoute] string courseId) // Lấy courseId từ URL route
        {
            var query = new GetAllCourseAttachedFileQuery(courseId);
            var result = await _mediator.Send(query);

            // Trả về ApiResponse thành công với danh sách DTO
            return Ok(ApiResponse<List<CourseAttachedFileDto>>.Ok(result, "Lấy danh sách file đính kèm thành công"));
        }
    }
}