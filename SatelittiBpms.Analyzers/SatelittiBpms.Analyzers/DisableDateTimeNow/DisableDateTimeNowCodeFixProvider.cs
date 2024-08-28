using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SatelittiBpms.Analyzers.DisableDateTimeNow
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DisableDateTimeNowCodeFixProvider)), Shared]
    public class DisableDateTimeNowCodeFixProvider : CodeFixProvider
    {
        private const string title = "Chame DateTime.UtcNow em vez de DateTime.Now para evitar erros de salvar dados com TimeZone";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DisableDateTimeNowAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => ReplaceWithUtcNowAsync(context.Document, diagnosticSpan),
                    equivalenceKey: nameof(title)),
                diagnostic);
        }

        private async Task<Document> ReplaceWithUtcNowAsync(Document document, TextSpan span)
        {
            var text = await document.GetTextAsync();
            var repl = "DateTime.UtcNow";
            if (Regex.Replace(text.GetSubText(span).ToString(), @"\s+", string.Empty) == "System.DateTime.Now")
                repl = "System.DateTime.UtcNow";
            var newtext = text.Replace(span, repl);
            return document.WithText(newtext);
        }
    }
}
