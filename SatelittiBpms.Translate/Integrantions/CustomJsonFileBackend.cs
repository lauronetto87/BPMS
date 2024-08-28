using I18Next.Net.Backends;
using I18Next.Net.TranslationTrees;
using Newtonsoft.Json.Linq;
using SatelittiBpms.Translate.Utils;
using System.Text;
using System.Threading.Tasks;

namespace SatelittiBpms.Translate.Integrantions
{
    public class CustomJsonFileBackend : ITranslationBackend
    {
        private readonly ITranslationTreeBuilderFactory _treeBuilderFactory;

        public CustomJsonFileBackend() : this(new GenericTranslationTreeBuilderFactory<HierarchicalTranslationTreeBuilder>()) { }

        private CustomJsonFileBackend(ITranslationTreeBuilderFactory treeBuilderFactory)
        {
            _treeBuilderFactory = treeBuilderFactory;
        }

        public Task<ITranslationTree> LoadNamespaceAsync(string language, string @namespace)
        {
            string objectName = LanguageStringUtils.GetLanguagePart(language);
            var translateJson = Properties.Resources.ResourceManager.GetObject(objectName) as byte[];
            JObject parsedJson = JObject.Parse(Encoding.UTF8.GetString(translateJson));
            var builder = _treeBuilderFactory.Create();
            PopulateTreeBuilder("", parsedJson, builder);
            return Task.FromResult(builder.Build());
        }

        private static void PopulateTreeBuilder(string path, JObject node, ITranslationTreeBuilder builder)
        {
            if (path != string.Empty)
                path = path + ".";

            foreach (var childNode in node)
            {
                var key = path + childNode.Key;

                if (childNode.Value is JObject jObj)
                    PopulateTreeBuilder(key, jObj, builder);
                else if (childNode.Value is JValue jVal)
                    builder.AddTranslation(key, jVal.Value.ToString());
            }
        }
    }
}
