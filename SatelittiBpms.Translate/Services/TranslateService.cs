using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using SatelittiBpms.Translate.Interfaces;
using SatelittiBpms.Translate.Utils;
using System.Text;

namespace SatelittiBpms.Translate.Services
{
    public class TranslateService : ITranslateService
    {
        private readonly IStringLocalizer<TranslateService> _localizer;

        public TranslateService(
            IStringLocalizer<TranslateService> localizer)
        {
            _localizer = localizer;
        }

        public string Localize(string key)
        {
            return _localizer[key];
        }

        public JObject GetTranslateJsonObject(string language)
        {
            string objectName = LanguageStringUtils.GetLanguagePart(language);
            var translateJson = Properties.Resources.ResourceManager.GetObject(objectName) as byte[];
            return JObject.Parse(Encoding.UTF8.GetString(translateJson));
        }
    }
}
