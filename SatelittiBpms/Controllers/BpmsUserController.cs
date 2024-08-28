using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SatelittiBpms.Authentication.Models;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace SatelittiBpms.Controllers
{
    public class BpmsUserController : BpmsApiControllerBase
    {
        private readonly IUserService _userService;

        public BpmsUserController(
            ILogger<BpmsUserController> logger,
            IUserService userService
        ) : base(logger)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Policy = Policies.OBSERVERS)]
        public async Task<ActionResult<UserBpmsViewModel>> GetAsync([FromRoute] int id)
        {
            var bpmsUser = await _userService.Get(id);
            if (bpmsUser == null)
            {
                return NotFound();
            }

            return Ok(bpmsUser);
        }

        [HttpPost]
        [Authorize(Policy = Policies.ADMINISTRATORS)]
        public async Task<ActionResult> Post([FromBody] UserDTO bpmsUser)
        {
            try
            {
                await _userService.Insert(bpmsUser);
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Policy = Policies.ADMINISTRATORS)]
        public async Task<ActionResult> Put([FromRoute] int id, [FromBody] UserDTO bpmsUser)
        {
            try
            {
                await _userService.Update(id, bpmsUser);
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Policy = Policies.ADMINISTRATORS)]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _userService.Disable(id);
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Authorize(Policy = Policies.ADMINISTRATORS)]
        public async Task<ActionResult> Delete([FromBody] DeleteBpmsUserDTO info)
        {
            await _userService.Disable(info.ids);
            return new JsonResult(new object());
        }
    }
}
