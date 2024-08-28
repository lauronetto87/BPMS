using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SatelittiBpms.Attributes;
using SatelittiBpms.Models.Exceptions;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Controllers
{
    [Route("rest/bpms/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public class BpmsApiControllerBase : ControllerBase
    {
        internal readonly ILogger _logger;
        protected ICollection<Error> Erros = new List<Error>();

        public BpmsApiControllerBase(ILogger logger)
        {
            _logger = logger;
        }

        protected async Task<ActionResult> HandleExceptionAsync(Func<Task<ResultContent>> funcao)
        {
            if (funcao is null)
                throw new ArgumentNullHandleException(ExceptionCodes.MISSING_FUNCTION_TO_HANDLE);

            var result = await funcao().ConfigureAwait(false);
            return CustomResponse(result);
        }

        protected IActionResult HandleOk(Func<ResultContent> function)
        {
            if (function is null)
                throw new ArgumentNullHandleException(ExceptionCodes.MISSING_FUNCTION_TO_HANDLE);

            var result = function();
            if (!result.Success && result.ValidationResult.Errors.Any())
            {
                return Ok(
                        new
                        {
                            success = false,
                            errors = result.ValidationResult.Errors.Select(x => new Error(x.ErrorMessage, x.AttemptedValue))
                        }); ;
            }
            return Ok(
                        new
                        {
                            success = true,
                            errors = Array.Empty<int>()
                        });
        }

        protected ActionResult CustomResponse(ResultContent result)
        {
            if (result.Success || (result.ValidationResult != null && !result.ValidationResult.Errors.Any()))
                return Ok(result);

            if (result is ResultContent<int>)
                throw new BusinessException<int>(ResultContent<int>.GetValue(result), result.ValidationResult.Errors.Select(x => new Error(x.ErrorMessage, x.AttemptedValue)).ToList(), result.MergeErrorsList);

            if (result.ValidationResult == null)
                throw new BusinessException(new List<Error>() { new Error(result.ErrorId) }, result.MergeErrorsList) { Title = result.ErrorId };

            throw new BusinessException(result.ValidationResult.Errors.Select(x => new Error(x.ErrorMessage, x.AttemptedValue)).ToList(), result.MergeErrorsList);
        }
    }
}
