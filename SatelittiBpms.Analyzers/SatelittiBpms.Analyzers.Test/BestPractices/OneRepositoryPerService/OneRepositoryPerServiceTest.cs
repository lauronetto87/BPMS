using NUnit.Framework;
using SatelittiBpms.Analyzers.DisableDateTimeNow;
using SatelittiBpms.Analyzers.BestPractices.OneRepositoryPerService;
using System.Threading.Tasks;
using VerifyCS = SatelittiBpms.Analyzers.Test.CSharpCodeFixVerifier<
    SatelittiBpms.Analyzers.BestPractices.OneRepositoryPerService.OneRepositoryPerServiceAnalyzer,
    SatelittiBpms.Analyzers.DisableDateTimeNow.OneRepositoryPerServiceCodeFixProvider>;
using System.IO;
using System.Linq;

namespace SatelittiBpms.Analyzers.Test.DisableDateTimeNow
{
    public class OneRepositoryPerServiceTest : BaseTest<OneRepositoryPerServiceAnalyzer, OneRepositoryPerServiceCodeFixProvider>
    {
        [Test]
        public async Task NoDiagnosticsExpected()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [Test]
        public async Task DiagnosticAndCodeFixForDateReplaceInMethod()
        {
            var folderFiles = Path.Combine("BestPractices", "TestData");
            var files = Directory.GetFiles(folderFiles, "*", SearchOption.AllDirectories)
                    .Select(f => new FileInfo(f))
                    .ToList();

            var expected = VerifyCS.Diagnostic(OneRepositoryPerServiceAnalyzer.DiagnosticId).WithSpan("FlowService.cs", 14, 15, 14, 51);
            await VerifyCodeFixAsync(files, new[] { expected }, (file, source) =>
            {
                if (file.Name == "FlowService.cs")
                {
                    return RemoveProcessRepositoryFromConstructor(source);
                }
                return source;
            });
        }

        private static string RemoveProcessRepositoryFromConstructor(string source)
        {
            return source.Remove(421, 53);
        }
    }
}
