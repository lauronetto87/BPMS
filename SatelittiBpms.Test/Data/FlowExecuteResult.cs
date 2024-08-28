using SatelittiBpms.FluentDataBuilder.FlowExecute.Data;
using SatelittiBpms.FluentDataBuilder.Process.Data;
using System.Collections.Generic;

namespace SatelittiBpms.Test.Data
{
    public class FlowExecuteResult
    {
        public ProcessVersionData ProcessVersion { get; set; }
        public List<FlowExecutedData> FlowsExecuted { get; set; }
    }
}
