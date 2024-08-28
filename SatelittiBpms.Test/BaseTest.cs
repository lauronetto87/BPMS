using NUnit.Framework;
using SatelittiBpms.Test.Helpers;
using System;
using System.Threading.Tasks;

namespace SatelittiBpms.Test
{
    public abstract class BaseTest
    {
        protected static void WaitUntil(Func<bool> test)
        {
            var task = WaitUntil(() => Task.FromResult(test()));
            task.Wait();
        }

        protected static Task WaitUntil(Func<Task<bool>> test)
        {
            return ThreadHelper.WaitUntil(test);
        }

        protected static void AssertDateEqualNowWithDelay(DateTime? date)
        {
            Assert.IsNotNull(date);
            Assert.LessOrEqual(Math.Abs(date.Value.Subtract(DateTime.UtcNow).TotalSeconds), 10);
        }
    }
}