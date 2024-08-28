using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SatelittiBpms.Analyzers.Test
{
    public abstract class BaseTest<TAnalyzer, TCodeFix>
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : CodeFixProvider, new()
    {
        public static async Task VerifyCodeFixAsync(List<FileInfo> files, DiagnosticResult[] expected, Func<FileInfo, string, string> fixSource)
        {
            var test = new Test(files, fixSource);
            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None);
        }

        private class Test : CSharpCodeFixTest<TAnalyzer, TCodeFix, MSTestVerifier>
        {
            public Test(List<FileInfo> files, Func<FileInfo, string, string> fixSource)
            {
                foreach (var file in files)
                {
                    var source = File.ReadAllText(file.FullName);
                    TestState.Sources.Add((file.Name, source));
                    FixedState.Sources.Add((file.Name, fixSource( file, source)));
                }

                SolutionTransforms.Add((solution, projectId) =>
                {
                    var compilationOptions = solution.GetProject(projectId).CompilationOptions;
                    var nullableWarningsOption = compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpVerifierHelper.NullableWarnings);
                    compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(nullableWarningsOption);
                    return solution.WithProjectCompilationOptions(projectId, compilationOptions);
                });
            }

        }
    }
}
