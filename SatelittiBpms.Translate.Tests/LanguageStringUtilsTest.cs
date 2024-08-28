using NUnit.Framework;
using SatelittiBpms.Translate.Utils;

namespace SatelittiBpms.Translate.Tests
{
    public class LanguageStringUtilsTest
    {
        const string EXPECTED_PT = "pt";
        const string EXPECTED_EN = "en";
        const string EXPECTED_ES = "es";

        [Test]
        public void ensurePtLanguage()
        {            
            string result = LanguageStringUtils.GetLanguagePart("pt");
            Assert.AreEqual(EXPECTED_PT, result);
        }

        [Test]
        public void ensurePtBrLanguage()
        {
            string result = LanguageStringUtils.GetLanguagePart("pt-br");
            Assert.AreEqual(EXPECTED_PT, result);
        }

        [Test]
        public void ensureEnLanguage()
        {
            string result = LanguageStringUtils.GetLanguagePart("en");
            Assert.AreEqual(EXPECTED_EN, result);
        }

        [Test]
        public void ensureEnUsLanguage()
        {
            string result = LanguageStringUtils.GetLanguagePart("en-us");
            Assert.AreEqual(EXPECTED_EN, result);
        }

        [Test]
        public void ensureEsLanguage()
        {
            string result = LanguageStringUtils.GetLanguagePart("es");
            Assert.AreEqual(EXPECTED_ES, result);
        }
    }
}
