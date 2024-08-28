using SatelittiBpms.FluentDataBuilder.FlowExecute.Data;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Workflow.Models;

namespace SatelittiBpms.Test.Data
{
    public class TaskExecutedData
    {
        public FlowDataInfo FlowDataInfo { get; set; }
        public int TaskId { get; set; }
        public ActivityUserOptionInfo OptionButton { get; set; }
        public TaskInfo TaskInfo { get; set; }
        public FlowTaskData TaskInput { get; set; }
    }
}
