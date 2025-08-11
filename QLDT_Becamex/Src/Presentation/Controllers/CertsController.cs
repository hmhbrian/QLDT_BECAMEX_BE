
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Certificates.Commands;
using QLDT_Becamex.Src.Application.Features.Certificates.Dtos;
using QLDT_Becamex.Src.Application.Features.Certificates.Queries;



namespace QLDT_Becamex.Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CertsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CertsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{courseId}")]
        [Authorize(Roles = "ADMIN,HR,HOCVIEN")]
        public async Task<IActionResult> GetCertByCourseId(string courseId)
        {
            var result = await _mediator.Send(new GetDetailCertQuery(courseId));
            return Ok(ApiResponse<CertDetailDto>.Ok(result));
        }

        [HttpGet()]
        [Authorize(Roles = "ADMIN,HR,HOCVIEN")]
        public async Task<IActionResult> GetListCert()
        {
            var result = await _mediator.Send(new GetListCertQuery());
            return Ok(ApiResponse<List<CertListDto>>.Ok(result));
        }

        [HttpPost("{courseId}")]
        [Authorize]
        public async Task<IActionResult> CreateCert(string courseId)
        {
            var reusult = await _mediator.Send(new CreateCertCommand(courseId));
            return Ok(ApiResponse<string>.Ok(reusult, "Tạo chứng chỉ thành công"));
        }
    }
}
