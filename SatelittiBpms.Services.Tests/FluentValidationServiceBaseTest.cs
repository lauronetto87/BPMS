using FluentValidation.Results;
using NUnit.Framework;
using SatelittiBpms.Services.Tests.ServicesHelper;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Services.Tests
{
    class FluentValidationServiceBaseTest
    {
        [Test]
        public void ensureThatAddErrorsToValidationWhenKeyAndMessageArePassed()
        {
            FluentValidationServiceBaseHelper fluentValidationServiceBase = new();
            fluentValidationServiceBase.AddErrors("key1", "message1");
            fluentValidationServiceBase.AddErrors("key2", "message2");

            Assert.AreEqual(2, fluentValidationServiceBase.ValidationResults.Errors.Count);
            Assert.IsTrue(fluentValidationServiceBase.ValidationResults.Errors.Any(x => x.ErrorMessage == "message1"));
            Assert.IsTrue(fluentValidationServiceBase.ValidationResults.Errors.Any(x => x.ErrorMessage == "message2"));
        }

        [Test]
        public void ensureThatAddErrorsToValidationWhenListErrosIsPassed()
        {
            FluentValidationServiceBaseHelper fluentValidationServiceBase = new();

            List<ValidationFailure> lstErros = new List<ValidationFailure>();
            lstErros.Add(new ValidationFailure("prop1", "lstErrorMessage1"));
            lstErros.Add(new ValidationFailure("prop1", "lstErrorMessage2"));
            fluentValidationServiceBase.AddErrors(lstErros);

            Assert.AreEqual(2, fluentValidationServiceBase.ValidationResults.Errors.Count);
            Assert.IsTrue(fluentValidationServiceBase.ValidationResults.Errors.Any(x => x.ErrorMessage == "lstErrorMessage1"));
            Assert.IsTrue(fluentValidationServiceBase.ValidationResults.Errors.Any(x => x.ErrorMessage == "lstErrorMessage2"));
        }
    }
}
