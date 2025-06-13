using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLDT_Becamex.Src.Dtos;
using QLDT_Becamex.Src.Dtos.Positions;
using QLDT_Becamex.Src.Services.Interfaces;
using System.Collections.Generic; // Cần thiết cho List<string>
using System.Linq; // Cần thiết cho .Any()
using System.Threading.Tasks;

namespace QLDT_Becamex.Src.Controllers
{
    [ApiController]
    [Route("[controller]")] // Hoặc [Route("api/[controller]")]
    public class PositionsController : ControllerBase
    {
        private readonly IPositionService _positionService;

        public PositionsController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePosition([FromBody] PositionRq request)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(request);
            }

            var result = await _positionService.CreatePositionAsync(request);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status201Created, new
                {
                    message = result.Message,
                    code = result.Code,

                });
            }
            else
            {
                return StatusCode(result.StatusCode ?? 500, new
                {
                    message = result.Message,
                    errors = result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code
                });
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetPositionById(int id)
        {
            var result = await _positionService.GetPositionByIdAsync(id);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new
                {
                    message = result.Message,
                    code = result.Code,
                    data = result.Data
                });
            }
            else
            {
                return StatusCode(result.StatusCode ?? 500, new
                {
                    message = result.Message,
                    errors = result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code
                });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllPositions() // Đổi tên thành GetAllPositions để rõ ràng hơn
        {
            var result = await _positionService.GetAllPositionsAsync();

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new
                {
                    message = result.Message,
                    code = result.Code,
                    data = result.Data
                });
            }
            else
            {
                return StatusCode(result.StatusCode ?? 500, new
                {
                    message = result.Message,
                    errors = result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code
                });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePosition(int id, [FromBody] PositionRq request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    message = "Dữ liệu đầu vào không hợp lệ.",
                    errors = errors,
                    code = "INVALID_INPUT"
                });
            }

            var result = await _positionService.UpdatePositionAsync(id, request);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new
                {
                    message = result.Message,
                    code = result.Code,

                });
            }
            else
            {
                return StatusCode(result.StatusCode ?? 500, new
                {
                    message = result.Message,
                    errors = result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code
                });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePosition(int id)
        {
            var result = await _positionService.DeletePositionAsync(id);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? StatusCodes.Status200OK, new
                {
                    message = result.Message,
                    code = result.Code
                    // Không có data khi xóa thành công
                });
            }
            else
            {
                return StatusCode(result.StatusCode ?? 500, new
                {
                    message = result.Message,
                    errors = result.Errors.Any() ? result.Errors : new List<string> { result.Message },
                    code = result.Code
                });
            }
        }
    }
}