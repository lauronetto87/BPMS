using Newtonsoft.Json.Linq;

namespace SatelittiBpms.Translate.Interfaces
{
    public interface ITranslateService
    {
        string Localize(string key);
        JObject GetTranslateJsonObject(string language);
    }
}
