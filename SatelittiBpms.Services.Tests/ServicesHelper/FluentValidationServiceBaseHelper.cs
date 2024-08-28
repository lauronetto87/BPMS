using FluentValidation.Results;
using System.Collections.Generic;

namespace SatelittiBpms.Services.Tests.ServicesHelper
{
    internal class FluentValidationServiceBaseHelper : FluentValidationServiceBase
    {
        public ValidationResult ValidationResults
        {
            get { return base.ValidationResult; }
        }

        internal void AddErrors(string key, string message)
        {
            base.AddErrors(key, message);
        }

        internal new void AddErrors(List<ValidationFailure> errors)
        {
            base.AddErrors(errors);
        }
    }
}
