using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SatelittiBpms.Authentication.Models;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Services.Interfaces;
using System.Threading.Tasks;

namespace SatelittiBpms.Controllers
{
    public class ProcessVersionController : BpmsApiControllerBase
    {
        private readonly IProcessVersionService _processVersionService;

        public ProcessVersionController(
            ILogger<ProcessVersionController> logger,
            IProcessVersionService processVersionService
            ) : base(logger)
        {
            _processVersionService = processVersionService;
        }

        [HttpPost]
        [Authorize(Policy = Policies.ADMINISTRATORS)]
        public async Task<ActionResult> Save([FromBody] ProcessVersionDTO process)
        {
            return await HandleExceptionAsync(async () => await _processVersionService.Save(process));
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Policy = Policies.ADMINISTRATORS)]
        public async Task<ActionResult> Get(int id)
        {
            return await HandleExceptionAsync(async () => await _processVersionService.GetByTenant(id));
        }

        [HttpGet]
        [Route("IsNameValidCheckDuplicate")]
        [Authorize(Policy = Policies.ADMINISTRATORS)]
        public IActionResult IsNameValidCheckDuplicate([FromQuery] string processName, [FromQuery] int editProcessVersionId)
        {
            return HandleOk(() => _processVersionService.IsNameValidCheckDuplicate(processName, editProcessVersionId));
        }
    }
}
