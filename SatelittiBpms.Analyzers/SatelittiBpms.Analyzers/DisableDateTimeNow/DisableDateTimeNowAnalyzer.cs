using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace SatelittiBpms.Analyzers.DisableDateTimeNow
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DisableDateTimeNowAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "IBPMS001";
        private const string Category = "IllegalMethodCalls";


        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.DisableDateTimeNowAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.DisableDateTimeNowAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.DisableDateTimeNowAnalyzerDescription), Resources.ResourceManager, typeof(Resources));

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction((compilationStartContext) =>
            {
                var dateTimeType = compilationStartContext.Compilation.GetTypeByMetadataName("System.DateTime");
                compilationStartContext.RegisterSyntaxNodeAction((analysisContext) =>
                {
                    var invocations = analysisContext.Node.DescendantNodes().OfType<MemberAccessExpressionSyntax>();
                    foreach (var invocation in invocations)
                    {
                        ExpressionSyntax e;
                        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                        {
                            e = memberAccess;
                        }
                        else if (invocation.Expression is IdentifierNameSyntax identifierName)
                        {
                            e = identifierName;
                        }
                        else
                        {
                            continue;
                        }

                        if (e == null)
                            continue;
                        var typeInfo = analysisContext.SemanticModel.GetTypeInfo(e).Type as INamedTypeSymbol;
                        if (typeInfo?.ConstructedFrom == null)
                            continue;

#pragma warning disable RS1024 // Compare symbols correctly
                        if (!typeInfo.ConstructedFrom.Equals(dateTimeType))
#pragma warning restore RS1024 // Compare symbols correctly
                            continue;
                        if (invocation.Name.ToString() == "Now")
                        {
                            analysisContext.ReportDiagnostic(Diagnostic.Create(Rule, invocation.GetLocation()));
                        }
                    }
                }, SyntaxKind.ClassDeclaration);
            });
        }

    }
}
