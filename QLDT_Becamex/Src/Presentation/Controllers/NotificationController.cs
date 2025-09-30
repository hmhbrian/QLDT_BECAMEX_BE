using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Certificates.Dtos;
using QLDT_Becamex.Src.Application.Features.Certificates.Queries;
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
    }
}
