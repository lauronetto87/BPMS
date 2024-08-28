using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SatelittiBpms.Authentication.Models;
using SatelittiBpms.Services.Interfaces;
using System.Threading.Tasks;

namespace SatelittiBpms.Controllers
{
    public class FlowHistoryController : BpmsApiControllerBase
    {
        private readonly IFlowHistoryService _flowHistoryService;

        public FlowHistoryController(ILogger<ProcessController> logger,
            IFlowHistoryService flowHistoryService) : base(logger)
        {
            _flowHistoryService = flowHistoryService;
        }

        [HttpGet]
        [Route("{flowId}")]
        [Authorize(Policy = Policies.OBSERVERS)]
        public new async Task<ActionResult> Request(int flowId)
        {
            return await HandleExceptionAsync(async () => await _flowHistoryService.Get(flowId));
        }
    }
}
