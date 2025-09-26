using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QLDT_Becamex.Src.Application.Features.Devices.Commands;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Feedbacks.Queries;
using QLDT_Becamex.Src.Application.Features.Devices.Dtos;
using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Presentation.Controllers
{
    [ApiController] // Đánh dấu đây là một API controller
    [Authorize]
    [Route("api/devices")] // Định nghĩa route cho controller này
    public class DevicesController : ControllerBase
    {
        private readonly IMediator _mediator;

        // Inject IMediator vào constructor để gửi các lệnh và truy vấn
        public DevicesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> Create([FromBody] CreateDeviceDto request)
        {
            var command = new CreateDevicesCommand(request);

            // Gửi command đến MediatR để Handler xử lý
            // Handler sẽ trả về CourseAttachedFileDto sau khi tạo thành công
            var createdDeviceId = await _mediator.Send(command);

            // Trả về HTTP 201 Created cùng với thông tin của file đã tạo
            return Ok(ApiResponse<string>.Ok(createdDeviceId, "Tạo thiết bị thành công"));

        }
        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetDevicesQuery();
            var devices = await _mediator.Send(query);
            return Ok(ApiResponse<List<DeviceDto>>.Ok(devices, "Lấy danh sách thiết bị thành công"));
        }
    }
}