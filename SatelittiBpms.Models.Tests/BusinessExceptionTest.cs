using NUnit.Framework;
using SatelittiBpms.Models.Exceptions;
using SatelittiBpms.Models.Result;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Models.Tests
{
    public class BusinessExceptionTest
    {
        [Test]
        public void ensureThatTitleReturnExceptionTypeWhenTitleIsNull()
        {
            BusinessException ex = new BusinessException(new List<Error>() { new Error("Erro1"), new Error("Erro2"), new Error("Erro3") });
            var result = ex.GetDetails();
            Assert.AreEqual("BusinessException", result.Title);
            Assert.IsTrue(result.Errors.Any(x => x.Message == "Erro1"));
            Assert.IsTrue(result.Errors.Any(x => x.Message == "Erro2"));
            Assert.IsTrue(result.Errors.Any(x => x.Message == "Erro3"));
            Assert.IsFalse(result.MergeErrorsList);

        }

        [Test]
        public void ensure()
        {
            BusinessException<int> ex = new BusinessException<int>(23, new List<Error>() { new Error("Erro1"), new Error("Erro2"), new Error("Erro3") }, true);
            var result = ex.GetDetails();
            Assert.AreEqual("BusinessException", result.Title);
            Assert.IsTrue(result.Errors.Any(x => x.Message == "Erro1"));
            Assert.IsTrue(result.Errors.Any(x => x.Message == "Erro2"));
            Assert.IsTrue(result.Errors.Any(x => x.Message == "Erro3"));
            Assert.IsTrue(result.MergeErrorsList);
        }
    }
}
