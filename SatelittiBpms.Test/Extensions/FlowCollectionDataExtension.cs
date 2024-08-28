using SatelittiBpms.FluentDataBuilder.FlowExecute.Data;
using SatelittiBpms.Test.Data;
using SatelittiBpms.Tests.Executors;
using System.Threading.Tasks;

namespace SatelittiBpms.Test.Extensions
{
    public static class FlowCollectionDataExtension
    {
        public static async Task<FlowExecuteResult> ExecuteInDataBase(this FlowCollectionData flows)
        {
            var contextBuilder = flows.ProcessVersionData.ContextBuilder;

            var flowExecuteResult = new FlowExecuteResult
            {
                ProcessVersion = flows.ProcessVersionData,
                FlowsExecuted = await new FlowCollectionExecutor(contextBuilder, flows).Execute(),
            };
            return flowExecuteResult;
        }
    }
}
