
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SatelittiBpms.Authentication.Models;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Controllers
{
    [Authorize(Policy = Policies.ADMINISTRATORS)]
    public class SuiteUserController : BpmsApiControllerBase
    {
        private readonly IUserService _userService;

        public SuiteUserController(ILogger<SuiteUserController> logger,          
            IUserService userService) : base(logger)
        {           
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Policy = Policies.ADMINISTRATORS)]
        public async Task<ActionResult<IList<SuiteUserViewModel>>> List()
        {
            return Ok(await _userService.ListUsersSuite());
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Policy = Policies.ADMINISTRATORS)]
        public async Task<ActionResult<SuiteUserViewModel>> Get(int id)
        {
            return Ok(await _userService.GetUsersSuite(id));
        }
    }
}
