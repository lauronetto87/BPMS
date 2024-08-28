using NUnit.Framework;
using SatelittiBpms.Translate.Integrantions;
using System.Threading.Tasks;

namespace SatelittiBpms.Translate.Tests
{
    public class CustomJsonFileBackendTest
    {
        [Test]
        public void ensureThatLoadingTranslateJson()
        {
            CustomJsonFileBackend customJsonFileBackend = new CustomJsonFileBackend();
            var result = customJsonFileBackend.LoadNamespaceAsync("pt", "translation");
            Assert.AreEqual(TaskStatus.RanToCompletion, result.Status);
            Assert.IsNotNull(result.Result);
        }
    }
}
