using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using SatelittiBpms.Analyzers.BestPractices.OneRepositoryPerService;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Analyzers.DisableDateTimeNow
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OneRepositoryPerServiceCodeFixProvider)), Shared]
    public class OneRepositoryPerServiceCodeFixProvider : CodeFixProvider
    {
        private const string title = "Retire a importação do repositório e crie um método no serviço do repositório equivantente";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(OneRepositoryPerServiceAnalyzer.DiagnosticId); }
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
                    createChangedDocument: c => RemoveParamterConstrcutor(context.Document, diagnosticSpan),
                    equivalenceKey: nameof(title)),
                diagnostic);
        }

        private async Task<Document> RemoveParamterConstrcutor(Document document, TextSpan span)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync();
            var paramterNode = syntaxRoot.FindNode(span);
            syntaxRoot = syntaxRoot.RemoveNode(paramterNode, SyntaxRemoveOptions.KeepNoTrivia);
            return document.WithSyntaxRoot(syntaxRoot);
        }
    }
}
