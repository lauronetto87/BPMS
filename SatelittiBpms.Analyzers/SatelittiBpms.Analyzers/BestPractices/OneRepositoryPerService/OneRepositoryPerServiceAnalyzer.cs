using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using SatelittiBpms.Analyzers.Extensions;

namespace SatelittiBpms.Analyzers.BestPractices.OneRepositoryPerService
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class OneRepositoryPerServiceAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "IBPMS002";
        private const string Category = "BestPractice";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.OneRepositoryPerServiceAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.OneRepositoryPerServiceAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.OneRepositoryPerServiceAnalyzerDescription), Resources.ResourceManager, typeof(Resources));

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction((compilationStartContext) =>
            {
                compilationStartContext.RegisterSyntaxNodeAction((analysisContext) =>
                {
                    var constructor = analysisContext.Node as ConstructorDeclarationSyntax;

                    var typeSymbolClass = analysisContext.SemanticModel.GetDeclaredSymbol(constructor).ContainingType;
                    if (!typeSymbolClass.IsService())
                    {
                        return;
                    }
                    var entityInfoServiceTypeSymbol = typeSymbolClass.GetRespositoryEntityFromService();

                    foreach (var paramter in constructor.ParameterList.Parameters)
                    {
                        var typeSymbolParamter = analysisContext.SemanticModel.GetTypeInfo(paramter.Type).Type;

                        if (!typeSymbolParamter.IsRepository())
                        {
                            continue;
                        }

                        var entityInfoParamterTypeSymbol = typeSymbolParamter.GetRespositoryEntityFromRepository();

#pragma warning disable RS1024 // Compare symbols correctly
                        if (entityInfoParamterTypeSymbol.Equals(entityInfoServiceTypeSymbol))
#pragma warning restore RS1024 // Compare symbols correctly
                        {
                            continue;
                        }

                        analysisContext.ReportDiagnostic(Diagnostic.Create(Rule, paramter.GetLocation()));
                    }

                }, SyntaxKind.ConstructorDeclaration);
            });
        }
    }
}
