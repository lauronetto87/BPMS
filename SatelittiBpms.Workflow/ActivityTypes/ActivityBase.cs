using SatelittiBpms.Models.Infos;
using SatelittiBpms.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace SatelittiBpms.Workflow.ActivityTypes
{
    public abstract class ActivityBase : StepBodyAsync
    {
        public int TenantId { get; set; }
        public int ActivityId { get; set; }
        public int TaskId { get; set; }

        internal readonly ITaskService _taskService;

        public ActivityBase(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public abstract override Task<ExecutionResult> RunAsync(IStepExecutionContext context);

        internal static Dictionary<string, object> GetInputs(int tenantId, int activityId)
        {
            var inputs = new Dictionary<string, object>();
            inputs.Add("TenantId", $"\"{tenantId}\"");
            inputs.Add("ActivityId", $"\"{activityId}\"");
            return inputs;
        }

        internal static Dictionary<string, object> GetOutputs()
        {
            var outputs = new Dictionary<string, object>();
            outputs.Add("TaskId", "step.TaskId");
            return outputs;
        }

        public virtual async Task<int> InsertTask(int flowId, bool isBackgroundTask = true)
        {
            var taskInsertResult = await _taskService.Insert(new TaskInfo()
            {
                TenantId = TenantId,
                FlowId = flowId,
                ActivityId = ActivityId,
                CreatedDate = DateTime.UtcNow,
                FinishedDate = isBackgroundTask ? DateTime.UtcNow : null
            });

            return taskInsertResult.Value;
        }

        public async Task UpdateFinishedDateFromTask(int taskId)
        {
            var getTaskResult = await _taskService.Get(taskId);
            TaskInfo task = getTaskResult.Value;
            task.FinishedDate = DateTime.UtcNow;
            await _taskService.Update(task);
        }


    }
}
