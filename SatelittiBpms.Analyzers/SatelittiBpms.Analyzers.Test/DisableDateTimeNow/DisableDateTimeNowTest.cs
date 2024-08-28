using NUnit.Framework;
using SatelittiBpms.Analyzers.DisableDateTimeNow;
using System.Threading.Tasks;
using VerifyCS = SatelittiBpms.Analyzers.Test.CSharpCodeFixVerifier<
    SatelittiBpms.Analyzers.DisableDateTimeNow.DisableDateTimeNowAnalyzer,
    SatelittiBpms.Analyzers.DisableDateTimeNow.DisableDateTimeNowCodeFixProvider>;

namespace SatelittiBpms.Analyzers.Test.DisableDateTimeNow
{
    public class DisableDateTimeNowTest
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
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        class ClassTeste
        {
            void Method()
            {
                var dateTime = DateTime.Now;
            }
        }
    }";

            var fixtest = @"
    using System;

    namespace ConsoleApplication1
    {
        class ClassTeste
        {
            void Method()
            {
                var dateTime = DateTime.UtcNow;
            }
        }
    }";
            var expected = VerifyCS.Diagnostic(DisableDateTimeNowAnalyzer.DiagnosticId).WithLocation(10, 32);
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [Test]
        public async Task DiagnosticAndCodeFixForDateReplaceInConstructor()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        class ClassTeste
        {
            ClassTeste()
            {
                var dateTime = DateTime.Now;
            }
        }
    }";

            var fixtest = @"
    using System;

    namespace ConsoleApplication1
    {
        class ClassTeste
        {
            ClassTeste()
            {
                var dateTime = DateTime.UtcNow;
            }
        }
    }";
            var expected = VerifyCS.Diagnostic(DisableDateTimeNowAnalyzer.DiagnosticId).WithLocation(10, 32);
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [Test]
        public async Task DiagnosticAndCodeFixForDateReplacePropertyOfClass()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        class ClassTeste
        {
            DateTime dateTime = DateTime.Now;
        }
    }";

            var fixtest = @"
    using System;

    namespace ConsoleApplication1
    {
        class ClassTeste
        {
            DateTime dateTime = DateTime.UtcNow;
        }
    }";
            var expected = VerifyCS.Diagnostic(DisableDateTimeNowAnalyzer.DiagnosticId).WithLocation(8, 33);
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}
