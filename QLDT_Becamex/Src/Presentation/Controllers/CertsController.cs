
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Certificates.Dtos;
using QLDT_Becamex.Src.Application.Features.Certificates.Queries;
using QLDT_Becamex.Src.Application.Features.Departments.Commands;
using QLDT_Becamex.Src.Application.Features.Departments.Dtos;
using QLDT_Becamex.Src.Application.Features.Departments.Queries;


namespace QLDT_Becamex.Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CertsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CertsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{courseId}")]
        [Authorize]
        public async Task<IActionResult> GetCertByCourseId(string courseId)
        {
            var result = await _mediator.Send(new GetDetailCertQuery(courseId));
            return Ok(ApiResponse<CertDetailDto>.Ok(result));
        }

        [HttpGet()]
        [Authorize]
        public async Task<IActionResult> GetListCert()
        {
            var result = await _mediator.Send(new GetListCertQuery());
            return Ok(ApiResponse<List<CertListDto>>.Ok(result));
        }
    }
}
