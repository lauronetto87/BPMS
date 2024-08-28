using Microsoft.AspNetCore.Mvc.Filters;
using SatelittiBpms.Models.DTO.FluentValidation;
using SatelittiBpms.Models.Exceptions;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Result;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Attributes
{
    public class ValidationFilterAttribute : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var param = context.ActionArguments.SingleOrDefault(p => p.Value is IDtoValidator);
            if (!param.Equals(default(KeyValuePair<string, object>)) && param.Value != null)
            {
                var dto = (IDtoValidator)param.Value;
                dto.BeforeValidate();
                var dtoValidate = dto.Validate();
                if (!dtoValidate.IsValid)
                    throw new ArgumentHandleException(ExceptionCodes.PARAMETERS_VALIDATION_ERRORS, dtoValidate.Errors.Select(x => new Error(x.ErrorMessage, x.AttemptedValue)).ToList());
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
