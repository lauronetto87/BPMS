using SatelittiBpms.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace SatelittiBpms.Workflow.ActivityTypes
{
    public class ExclusiveGatewayActivity : DataReplicationActivityBase
    {
        public static string TypeDescription { get { return "SatelittiBpms.Workflow.ActivityTypes.ExclusiveGatewayActivity, SatelittiBpms.Workflow"; } }

        public int Option { get; set; }

        public ExclusiveGatewayActivity(
            IFieldValueService fieldValueService,
            IFlowPathService flowPathService,
            ITaskService taskService) : base(taskService, fieldValueService, flowPathService)
        { }

        public static new Dictionary<string, object> GetInputs(int tenantId, int activityId)
        {
            var inputs = DataReplicationActivityBase.GetInputs(tenantId, activityId);
            inputs.Add("Option", "data.Option");
            return inputs;
        }

        public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            var currentTaskId = await InsertTask();
            await InsertFlowPath(currentTaskId);
            await ReplicateFieldValues(currentTaskId);

            TaskId = currentTaskId;

            return ExecutionResult.Outcome(Option);
        }
    }
}
