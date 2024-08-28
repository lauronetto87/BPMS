using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SatelittiBpms.Authentication.Models;
using SatelittiBpms.Services.Interfaces;
using System.Threading.Tasks;

namespace SatelittiBpms.Controllers
{
    public class TemplateController : BpmsApiControllerBase
    {
        private readonly ITemplateService _templateService;

        public TemplateController(
            ILogger<TemplateController> logger,
            ITemplateService templateService
            ) : base(logger)
        {
            _templateService = templateService;
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Policy = Policies.ADMINISTRATORS)]
        public async Task<ActionResult> Get(int id)
        {
            return await HandleExceptionAsync(async () => await _templateService.GetById(id));
        }
    }
}
