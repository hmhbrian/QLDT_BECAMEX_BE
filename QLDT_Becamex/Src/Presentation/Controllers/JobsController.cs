using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace QLDT_Becamex.Src.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public JobsController(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        [HttpPost("run-review-reminder")]
        public async Task<IActionResult> RunReviewReminder()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.TriggerJob(new JobKey("ReviewReminderJob"));
            return Ok("ReviewReminderJob triggered manually!");
        }
    }
}
