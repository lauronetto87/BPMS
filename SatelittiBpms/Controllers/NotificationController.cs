using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SatelittiBpms.Authentication.Models;
using SatelittiBpms.Services.Interfaces;
using System.Threading.Tasks;

namespace SatelittiBpms.Controllers
{
    public class NotificationController : BpmsApiControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(ILogger<ProcessController> logger,
            INotificationService notificationService) : base(logger)
        {
            _notificationService = notificationService;
        }

        [HttpDelete]
        [Route("{notificationId}")]
        [Authorize(Policy = Policies.OBSERVERS)]
        public async Task<ActionResult> Delete(int notificationId)
        {
            return await HandleExceptionAsync(async () => await _notificationService.SetToDeleted(notificationId));
        }

        [HttpPut]
        [Route("SetToRead/{notificationId}")]
        [Authorize(Policy = Policies.OBSERVERS)]
        public async Task<ActionResult> SetToRead(int notificationId)
        {
            return await HandleExceptionAsync(async () => await _notificationService.SetToRead(notificationId));
        }


        [HttpGet]
        [Route("")]
        [Authorize(Policy = Policies.OBSERVERS)]
        public async Task<ActionResult> Get()
        {
            return Ok(await _notificationService.List());
        }
    }
}
