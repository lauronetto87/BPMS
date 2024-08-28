using FluentValidation.Results;
using System.Collections.Generic;

namespace SatelittiBpms.Services
{
    public abstract class FluentValidationServiceBase
    {
        protected ValidationResult ValidationResult;

        protected FluentValidationServiceBase()
        {
            ValidationResult = new ValidationResult();
        }

        protected void AddErrors(string key, string message, object attemptedValue = null)
        {
            ValidationResult.Errors.Add(new ValidationFailure(key, message, attemptedValue));
        }

        protected void AddErrors(List<ValidationFailure> errors)
        {
            ValidationResult.Errors.AddRange(errors);
        }
    }
}
