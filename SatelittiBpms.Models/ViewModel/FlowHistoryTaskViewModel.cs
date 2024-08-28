using SatelittiBpms.Models.Enums;
using System;

namespace SatelittiBpms.Models.ViewModel
{
    public class FlowHistoryTaskViewModel
    {
        public string TaskName { get; set; }
        public string ExecutorName { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public DateTime? FinishedDatetime { get; set; }
        public string ActionDescription { get; set; }
        public WorkflowActivityTypeEnum ActivityType { get; set; }
    }
}
