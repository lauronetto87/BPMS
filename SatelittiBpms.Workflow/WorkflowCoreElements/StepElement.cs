namespace SatelittiBpms.Workflow.WorkflowCoreElements
{
    public class StepElement : StepElementBase
    {        
        public string NextStepId { get; set; }
        public string CancelCondition { get; set; }
    }
}
