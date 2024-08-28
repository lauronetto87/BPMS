using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Services.Interfaces;
using System.Threading.Tasks;

namespace SatelittiBpms.Controllers
{
    public class TenantController : BpmsApiControllerBase
    {
        private readonly ITenantActivateService _tenantActivateService;

        public TenantController(
            ITenantActivateService tenantActivateService,
            ILogger<TranslateController> logger
            ) : base(logger)
        {
            _tenantActivateService = tenantActivateService;
        }

        [HttpPost]
        [Route("activationTenant")]
        public async Task<ActionResult> ActivationTenant([FromBody] ActivationTenantDTO activationTenantDTO)
        {
            return await HandleExceptionAsync(async () => await _tenantActivateService.ActivationTenant(activationTenantDTO));
        }
    }
}
