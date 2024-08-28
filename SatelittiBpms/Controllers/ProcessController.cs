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
    public class ProcessController : BpmsApiControllerBase
    {
        private readonly IProcessService _processService;

        public ProcessController(
            ILogger<ProcessController> logger,
            IProcessService processService) : base(logger)
        {
            _processService = processService;
        }

        [HttpPost]
        [Route("list")]
        [Authorize(Policy = Policies.PUBLISHERS)]
        public async Task<ActionResult<ProcessListingViewModel>> List([FromBody] ProcessFilterDTO filters)
        {
            return await HandleExceptionAsync(async () => await _processService.ListProcessListViewModel(filters));
        }

        [HttpPost]
        [Route("total")]
        [Authorize(Policy = Policies.PUBLISHERS)]
        public async Task<ActionResult<ProcessCounterViewModel>> Total([FromBody] ProcessFilterDTO filters)
        {
            return await HandleExceptionAsync(async () => await _processService.GetCounterProcess(filters));
        }


        [HttpGet]
        [Route("listToFilters")]
        [Authorize(Policy = Policies.OBSERVERS)]
        public async Task<ActionResult<ProcessListingViewModel>> ListToFilters()
        {
            return await HandleExceptionAsync(async () => await _processService.ListToFilters());
        }
    }
}
