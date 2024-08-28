using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace SatelittiBpms.Workflow.ActivityTypes
{
    public class EndEventActivity : DataReplicationActivityBase
    {
        public static string TypeDescription { get { return "SatelittiBpms.Workflow.ActivityTypes.EndEventActivity, SatelittiBpms.Workflow"; } }

        public string ConnectionId { get; set; }

        private readonly IFlowService _flowService;

        public EndEventActivity(
           IFieldValueService fieldValueService,
           IFlowPathService flowPathService,
           ITaskService taskService,
           IFlowService flowService) : base(taskService, fieldValueService, flowPathService)
        {
            _flowService = flowService;
        }

        public static new Dictionary<string, object> GetInputs(int tenantId, int activityId)
        {
            var inputs = DataReplicationActivityBase.GetInputs(tenantId, activityId);
            inputs.Add("ConnectionId", "data.ConnectionId");
            return inputs;
        }

        public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            var currentTaskId = await InsertTask();
            await InsertFlowPath(currentTaskId);
            await ReplicateFieldValues(currentTaskId);

            var getFlowResult = await _flowService.Get(FlowId);
            FlowInfo flow = getFlowResult.Value;
            flow.Status = FlowStatusEnum.FINISHED;
            flow.FinishedDate = DateTime.UtcNow;
            await _flowService.Update(flow);

            return ExecutionResult.Next();
        }
    }
}
