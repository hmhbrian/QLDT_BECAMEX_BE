using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Certificates.Dtos;
using QLDT_Becamex.Src.Application.Features.Certificates.Queries;
using QLDT_Becamex.Src.Application.Features.Notifications.Command;
using QLDT_Becamex.Src.Application.Features.Notifications.Dtos;
using QLDT_Becamex.Src.Application.Features.Notifications.Queries;

namespace QLDT_Becamex.Src.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "HR,HOCVIEN")]
        public async Task<IActionResult> GetNotificationOfUser(NotificationFilter filter = NotificationFilter.All)
        {
            var result = await _mediator.Send(new GetNotificationOfUserQuery(new NotificationRequest { Filter = filter }));
            return Ok(ApiResponse<NotificationDto>.Ok(result));
        }

        // Đọc / chưa đọc 1 notification
        // PUT /api/notifications/{id}/read?isRead=true
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkRead([FromRoute] int id, [FromQuery] bool isRead = true)
        {
            await _mediator.Send(new MarkNotificationReadCommand(id, isRead));
            return Ok(ApiResponse.Ok("Notification was read !"));
        }

        [HttpPut("mark-all-read")]
        public async Task<IActionResult> MarkAllRead()
        {
            await _mediator.Send(new MarkAllNotificationsReadCommand());
            return Ok(ApiResponse.Ok("All Notification were read !"));
        }
    }
}
