using System.Collections.Generic;

namespace SatelittiBpms.Workflow.WorkflowCoreElements
{
    public class WorkflowElement
    {
        public string Id { get; set; }
        public int Version { get; set; }
        public string DataType { get; set; }
        public List<StepElementBase> Steps { get; set; }

        public WorkflowElement()
        {
            Steps = new List<StepElementBase>();
        }
    }
}
