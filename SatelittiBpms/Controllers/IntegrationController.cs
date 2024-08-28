using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SatelittiBpms.Authentication.Models;
using SatelittiBpms.Services.Interfaces;
using System.Threading.Tasks;

namespace SatelittiBpms.Controllers
{
    public class IntegrationController : BpmsApiControllerBase
    {
        private readonly ITenantService _tenantService;
        private readonly ISignerIntegrationService _signerIntegrationService;

        public IntegrationController(ILogger<ProcessController> logger,
            ITenantService tenantService,
            ISignerIntegrationService signerIntegrationService) : base(logger)
        {
            _tenantService = tenantService;
            _signerIntegrationService = signerIntegrationService;
        }

        [HttpGet]
        [Route("SignerIsEnable")]
        [Authorize(Policy = Policies.ADMINISTRATORS)]
        public ActionResult SignerIntegrationIsEnable()
        {
            return CustomResponse(_tenantService.SignerIntegrationIsEnable());
        }

        [HttpGet]
        [Route("getSignerInformation")]
        [Authorize(Policy = Policies.ADMINISTRATORS)]
        public async Task<ActionResult> GetSignerInformation()
        {
            return await HandleExceptionAsync(async () => await _signerIntegrationService.GetSignerInformation());
        }
    }
}
