using NUnit.Framework;
using SatelittiBpms.Models.Exceptions;

namespace SatelittiBpms.Models.Tests
{
    public class ArgumentNullHandleExceptionTest
    {
        [Test]
        public void ensureThatTitleReturnExceptionTypeWhenTitleIsNull()
        {
            ArgumentNullHandleException ex = new ArgumentNullHandleException("Title");
            ex.Title = null;
            var result = ex.GetDetails();
            Assert.AreEqual("ArgumentNullHandleException", result.Title);
        }
    }
}
