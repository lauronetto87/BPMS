using NUnit.Framework;
using SatelittiBpms.Models.Exceptions;
using SatelittiBpms.Models.Result;
using System.Collections.Generic;

namespace SatelittiBpms.Models.Tests
{
    class ArgumentHandleExceptionTest
    {
        [Test]
        public void ensureThatTitleReturnExceptionTypeWhenTitleIsNull()
        {
            ArgumentHandleException ex = new ArgumentHandleException("Title", new List<Error>());
            ex.Title = null;
            var result = ex.GetDetails();
            Assert.AreEqual("ArgumentHandleException", result.Title);
        }
    }
}
