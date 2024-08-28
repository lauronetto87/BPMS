using System;
using System.Threading;
using System.Threading.Tasks;

namespace SatelittiBpms.Test.Helpers
{
    public static class ThreadHelper
    {
        public static async Task WaitUntil(Func<Task<bool>> test)
        {
            const int numberOfLoopTimes = 21;
            const int millisecondsSleep = 150;

            for (int i = 0; i < numberOfLoopTimes; i++)
            {
                if (await test())
                {
                    return;
                }
                if (numberOfLoopTimes - 1 == i)
                {
                    throw new TimeoutException($"Timeout, waiting for {i * millisecondsSleep / 1000.0} seconds.");
                }
                Thread.Sleep(millisecondsSleep);
            }
        }
    }
}
