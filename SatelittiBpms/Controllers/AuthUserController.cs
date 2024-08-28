using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Satelitti.Authentication.Authorization;
using Satelitti.Authentication.Service.Interface;
using Satelitti.Model.Attributes;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Services.Interfaces;
using System.Threading.Tasks;

namespace SatelittiBpms.Controllers
{
    public class AuthUserController : BpmsApiControllerBase
    {
        private readonly IAuthUserTokenService<UserViewModel> _authUserTokenService;        

        public AuthUserController(ILogger<AuthUserController> logger,
            IAuthUserTokenService<UserViewModel> authUserTokenService,
            ITenantService tenantService) : base(logger)
        {
            _authUserTokenService = authUserTokenService;            
        }

        // POST: Login
        /// <summary>
        /// Validate token with satelitti system and returns the generated product token
        /// </summary>
        /// <param type="LoginBySuiteTokenDTO" name="suiteToken">Entity data</param>
        /// <response code="200">Successfully validated token and generated token</response>
        /// <response code="401">Invalid suite token</response>
        [Route("Login")]
        [HttpPost]
        [AllowAnonymous]
        [RequestLimitAttribute(nameof(AuthUserController) + nameof(Login), NoOfRequest = 20, Seconds = 10)]
        public async Task<ActionResult<AuthTokenWithUser<UserViewModel>>> Login([FromBody] LoginBySuiteTokenDTO suiteToken)
        {
            var tokenResult = await _authUserTokenService.Validate(suiteToken.Token);

            if (!tokenResult.Success)
            {
                return Unauthorized(tokenResult.ToErrorObject());
            }

            return Ok(tokenResult.Value);
        }
    }
}
