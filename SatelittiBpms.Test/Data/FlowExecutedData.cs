using SatelittiBpms.FluentDataBuilder.FlowExecute.Data;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Workflow.Models;
using System.Collections.Generic;

namespace SatelittiBpms.Test.Data
{
    public class FlowExecutedData
    {
        public FlowExecutedData(FlowData flowData)
        {
            FlowData = flowData;
            Tasks = new List<TaskExecutedData>();
        }

        public FlowDataInfo FlowDataInfoRequestFlow { get; set; }
        public int FlowId { get; set; }
        
        public string WorkflowInstanceId { get; set; }
        public FlowData FlowData { get; set; }
        public List<TaskExecutedData> Tasks { get; set; }
        public FlowInfo FlowInfo { get; set; }
    }
}
