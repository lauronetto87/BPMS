using FluentValidation.Results;
using System.Collections.Generic;

namespace SatelittiBpms.Services.ProcessVersionValidation
{
    internal abstract class ProcessValidationBase
    {
        protected ValidationFailure CreateValidationFailure(string key, object attemptedValue = null)
        {
            return new ValidationFailure(key, key, attemptedValue);
        }

        public abstract List<ValidationFailure> Validate();
    }
}
