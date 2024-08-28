using System.Collections.Generic;

namespace SatelittiBpms.Workflow.WorkflowCoreElements
{
    public abstract class StepElementBase    {
        public string Id { get; set; }
        public string StepType { get; set; }
        public Dictionary<string, object> Inputs { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> Outputs { get; set; } = new Dictionary<string, object>();
    }
}
