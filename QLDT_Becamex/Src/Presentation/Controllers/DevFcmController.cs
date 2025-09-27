using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace QLDT_Becamex.Src.Presentation.Controllers
{

    [ApiController]
    [Route("api/dev/fcm")]
    public sealed class DevFcmController : ControllerBase
    {
        private readonly FirebaseApp _fb;
        public DevFcmController(FirebaseApp fb) => _fb = fb;

        // POST /api/dev/fcm/test?token=xxxx
        [HttpPost("test")]
        public async Task<IActionResult> Test([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Missing token");

            var msg = new Message
            {
                Token = token,
                Notification = new Notification { Title = "Test", Body = "Hello from .NET Firebase Admin" },
                Data = new Dictionary<string, string> { ["type"] = "Ping" }
            };

            var id = await FirebaseMessaging.GetMessaging(_fb).SendAsync(msg);
            return Ok(new { messageId = id });
        }
    }
}
