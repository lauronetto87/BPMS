using SatelittiBpms.Models.Infos;
using SatelittiBpms.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace SatelittiBpms.Workflow.ActivityTypes
{
    public abstract class DataReplicationActivityBase : ActivityBase
    {
        public int FlowId { get; set; }

        internal readonly IFlowPathService _flowPathService;
        internal readonly IFieldValueService _fieldValueService;

        public DataReplicationActivityBase(
            ITaskService taskService,
            IFieldValueService fieldValueService,
            IFlowPathService flowPathService) : base(taskService)
        {
            _flowPathService = flowPathService;
            _fieldValueService = fieldValueService;
        }

        internal static new Dictionary<string, object> GetInputs(int tenantId, int activityId)
        {
            var inputs = ActivityBase.GetInputs(tenantId, activityId);
            inputs.Add("FlowId", "data.FlowId");
            inputs.Add("TaskId", "data.TaskId");
            return inputs;
        }

        public abstract override Task<ExecutionResult> RunAsync(IStepExecutionContext context);

        public virtual async Task<int> InsertTask(bool isBackgroundTask = true)
        {
            return await base.InsertTask(FlowId, isBackgroundTask);
        }

        public async Task InsertFlowPath(int targetTaskId)
        {
            await _flowPathService.Insert(new FlowPathInfo()
            {
                TenantId = TenantId,
                FlowId = FlowId,
                SourceTaskId = TaskId,
                TargetTaskId = targetTaskId
            });
        }

        public async Task ReplicateFieldValues(int targetTaskId)
        {
            await _fieldValueService.ReplicateFieldValues(TaskId, targetTaskId, TenantId);
        }
    }
}
