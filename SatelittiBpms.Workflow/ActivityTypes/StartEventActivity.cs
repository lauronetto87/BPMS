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
    public class StartEventActivity : ActivityBase
    {
        public static string TypeDescription { get { return "SatelittiBpms.Workflow.ActivityTypes.StartEventActivity, SatelittiBpms.Workflow"; } }

        public int FlowId { get; set; }
        public int ProcessVersionId { get; set; }
        public int RequesterId { get; set; }

        private readonly IFlowService _flowService;

        public StartEventActivity(
             ITaskService taskService,
             IFlowService flowService) : base(taskService)
        {
            _flowService = flowService;
        }

        public static Dictionary<string, object> GetInputs(int tenantId, int activityId, int processVersionId)
        {
            var inputs = ActivityBase.GetInputs(tenantId, activityId);
            inputs.Add("ProcessVersionId", $"\"{processVersionId}\"");
            inputs.Add("RequesterId", "data.RequesterId");
            return inputs;
        }

        public static new Dictionary<string, object> GetOutputs()
        {
            var outputs = ActivityBase.GetOutputs();
            outputs.Add("FlowId", "step.FlowId");
            return outputs;
        }

        public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            var flowInsertResult = await _flowService.Insert(new FlowInfo()
            {
                TenantId = TenantId,
                ProcessVersionId = ProcessVersionId,
                RequesterId = RequesterId,
                CreatedDate = DateTime.UtcNow,
                Status = FlowStatusEnum.INPROGRESS
            });

            FlowId = flowInsertResult.Value;

            TaskId = await InsertTask(FlowId);

            return ExecutionResult.Next();
        }

        public override async Task<int> InsertTask(int flowId, bool isBackgroundTask = true)
        {
            var task = new TaskInfo()
            {
                TenantId = TenantId,
                FlowId = flowId,
                ActivityId = ActivityId,
                CreatedDate = DateTime.UtcNow,
                FinishedDate = isBackgroundTask ? DateTime.UtcNow : null
            };

            var fieldsValueInfo = new List<FieldValueInfo>();

            var fields = _flowService.GetFields(task.FlowId);
            foreach (var field in fields)
            {
                fieldsValueInfo.Add(new FieldValueInfo
                {
                    FieldValue = field.Type == FieldTypeEnum.FILE ? "[]" : null,
                    FlowId = task.FlowId,
                    FieldId = field.Id,
                    TenantId = TenantId,
                });
            }
            task.FieldsValues = fieldsValueInfo;

            var taskInsertResult = await _taskService.Insert(task);
            return taskInsertResult.Value;
        }
    }
}
