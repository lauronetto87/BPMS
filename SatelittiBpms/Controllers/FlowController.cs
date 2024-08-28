using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SatelittiBpms.Authentication.Models;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Services.Interfaces;
using System.Threading.Tasks;

namespace SatelittiBpms.Controllers
{
    public class FlowController : BpmsApiControllerBase
    {
        private readonly IFlowService _flowService;

        public FlowController(ILogger<ProcessController> logger,
            IFlowService flowService) : base(logger)
        {
            _flowService = flowService;
        }

        [HttpPost]
        [Route("request")]
        [Authorize(Policy = Policies.PUBLISHERS)]
        public new async Task<ActionResult> Request([FromBody] FlowRequestDTO flowRequestDTO)
        {
            return await HandleExceptionAsync(async () => await _flowService.Request(flowRequestDTO));
        }

        [HttpPost]
        [Route("listall")]
        [Authorize(Policy = Policies.OBSERVERS)]
        public async Task<ActionResult<ProcessListingViewModel>> ListAll([FromBody] TaskFilterDTO filters)
        {
            return await HandleExceptionAsync(async () => await _flowService.ListAll(filters));
        }

    }
}
