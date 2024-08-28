using Microsoft.Extensions.Localization;
using Moq;
using NUnit.Framework;
using SatelittiBpms.Translate.Services;

namespace SatelittiBpms.Translate.Tests
{
    public class TranslateServiceTest
    {
        Mock<IStringLocalizer<TranslateService>> _mockLocalizer;

        [SetUp]
        public void init()
        {
            _mockLocalizer = new Mock<IStringLocalizer<TranslateService>>();
        }

        [Test]
        public void ensureThatLocalizeReturns()
        {
            string key = "testKey", value = "AnotherValue";
            var localizedString = new LocalizedString(key, value);
            _mockLocalizer.Setup(x => x[key]).Returns(localizedString);
            TranslateService translateService = new TranslateService(_mockLocalizer.Object);
            var result = translateService.Localize(key);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void ensureThatGetJsonObjectPt()
        {
            TranslateService translateService = new TranslateService(_mockLocalizer.Object);
            var result = translateService.GetTranslateJsonObject("pt");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ContainsKey("codex"));
        }

        [Test]
        public void ensureThatGetJsonObjectEn()
        {
            TranslateService translateService = new TranslateService(_mockLocalizer.Object);
            var result = translateService.GetTranslateJsonObject("en");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ContainsKey("codex"));
        }

        [Test]
        public void ensureThatGetJsonObjectEs()
        {
            TranslateService translateService = new TranslateService(_mockLocalizer.Object);
            var result = translateService.GetTranslateJsonObject("es");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ContainsKey("codex"));
        }
    }
}
