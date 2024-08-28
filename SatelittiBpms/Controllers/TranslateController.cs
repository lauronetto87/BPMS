using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SatelittiBpms.Translate.Interfaces;

namespace SatelittiBpms.Controllers
{
    public class TranslateController : BpmsApiControllerBase
    {
        private readonly ITranslateService _translateService;
        public TranslateController(
            ILogger<TranslateController> logger,
            ITranslateService translateService
            ) : base(logger)
        {
            _translateService = translateService;
        }

        [HttpGet]
        public JsonResult Get(string lang)
        {
            return new JsonResult(_translateService.GetTranslateJsonObject(lang));
        }

        [HttpPost]
        [Route("localize")]
        public ActionResult Localize([FromBody] string key)
        {
            return Ok(_translateService.Localize(key));
        }
    }
}
