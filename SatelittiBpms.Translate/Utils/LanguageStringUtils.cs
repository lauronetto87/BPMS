namespace SatelittiBpms.Translate.Utils
{
    public class LanguageStringUtils
    {
        public static string GetLanguagePart(string language)
        {
            var index = language.IndexOf('-');

            if (index == -1)
                return language;

            return language.Substring(0, index);
        }
    }
}
