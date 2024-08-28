using System.Collections.Generic;

namespace SatelittiBpms.Workflow.WorkflowCoreElements
{
    public class StepElementExclusive : StepElementBase
    {   
        public Dictionary<string, object> SelectNextStep { get; set; }

        public StepElementExclusive()
        {
            SelectNextStep = new Dictionary<string, object>();
        }
        
    }
}
