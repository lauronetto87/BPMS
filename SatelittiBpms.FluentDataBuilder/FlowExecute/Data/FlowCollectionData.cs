using SatelittiBpms.FluentDataBuilder.Process.Data;
using System.Collections.Generic;

namespace SatelittiBpms.FluentDataBuilder.FlowExecute.Data
{
    public class FlowCollectionData
    {
        public List<FlowData> FlowsData { get; set; }
        public ProcessVersionData ProcessVersionData { get; set; }
    }
}
