using SatelittiBpms.ApiGatewayManagementApi.Interfaces;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Workflow.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace SatelittiBpms.Workflow.ActivityTypes
{
    public class UserTaskActivity : DataReplicationActivityBase
    {
        public static string TypeDescription { get { return "SatelittiBpms.Workflow.ActivityTypes.UserTaskActivity, SatelittiBpms.Workflow"; } }

        public string ConnectionId { get; set; }
        public int RequesterId { get; set; }

        public int Option { get; set; }
        public UserTaskExecutorTypeEnum ActivityUserExecutorType { get; set; }

        private readonly ITaskHistoryService _taskHistoryService;
        private readonly IActivityUserService _activityUserService;
        private readonly IFrontendNotifyService _frontendNotifyService;
        internal readonly INotificationService _notificationService;

        public UserTaskActivity(
            IFieldValueService fieldValueService,
            IFlowPathService flowPathService,
            ITaskService taskService,
            ITaskHistoryService taskHistoryService,
            IActivityUserService activityUserService,
            IFrontendNotifyService frontendNotifyService,
            INotificationService notificationService) : base(taskService, fieldValueService, flowPathService)
        {
            _taskHistoryService = taskHistoryService;
            _activityUserService = activityUserService;
            _frontendNotifyService = frontendNotifyService;
            _notificationService = notificationService;
        }

        public static Dictionary<string, object> GetInputs(int tenantId, int activityId, UserTaskExecutorTypeEnum executorType)
        {
            var inputs = DataReplicationActivityBase.GetInputs(tenantId, activityId);
            inputs.Add("ConnectionId", "data.ConnectionId");
            inputs.Add("RequesterId", "data.RequesterId");
            inputs.Add("ActivityUserExecutorType", $"\"{executorType}\"");
            return inputs;
        }

        public static new Dictionary<string, object> GetOutputs()
        {
            var outputs = ActivityBase.GetOutputs();
            outputs.Add("Option", "step.Option");
            return outputs;
        }

        public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            var stepRunToInsertTaskAndPersist = context.PersistenceData == null && !context.ExecutionPointer.EventPublished;
            var stepRunToExecuteTask = context.ExecutionPointer.EventPublished;
            var stepRunToWaitForEvent = !stepRunToInsertTaskAndPersist && !stepRunToExecuteTask;

            var openFormToExecuteTaskInFrontEnd = await OpenFormToExecuteTaskInFrontEnd();

            if (stepRunToInsertTaskAndPersist)
            {
                var currentTaskId = await InsertTask(false);
                await InsertFlowPath(currentTaskId);
                await ReplicateFieldValues(currentTaskId);
                await InsertNotification(currentTaskId);

                var message = new ExpandoObject();

                if (openFormToExecuteTaskInFrontEnd)
                {
                    message.TryAdd("taskIdToExecute", currentTaskId);
                    message.TryAdd("canExecute", true);
                }
                else
                {
                    message.TryAdd("canExecute", false);
                }
                await NotifyClient(message);
                return ExecutionResult.Persist(currentTaskId);
            }

            if (stepRunToWaitForEvent)
            {
                var currentTaskId = Convert.ToInt32(context.PersistenceData);
                TaskId = currentTaskId;
                var enventUser = new EventUserInfo(currentTaskId);
                return ExecutionResult.WaitForEvent(enventUser.eventName, enventUser.eventKey, DateTime.UtcNow);
            }

            if (stepRunToExecuteTask)
            {
                FlowDataInfo flowData = (FlowDataInfo)context.ExecutionPointer.EventData;
                Option = flowData.Option;
                return await ExecuteStep(flowData.TaskId);
            }

            throw new Exception("Unhandled step execution sequence. TaskId: " + TaskId);
        }

        private Task NotifyClient(object message)
        {
            try
            {
                return _frontendNotifyService.Notify(ConnectionId, message);
            }
            catch
            {
                return Task.CompletedTask;
                // adicionado try catch para que mesmo que dê erro na notificação a task seja criada e replicado os valores dos campos
                // assim, não impactando no andamento do fluxo só por não conseguir notificar.
                // apenas será necessário que o usuário atualize a tela manualmente para listar o fluxo no estado atual
            }
        }

        private async Task<ExecutionResult> ExecuteStep(int taskId)
        {
            await UpdateFinishedDateFromTask(taskId);
            await _taskHistoryService.UpdateEndDateFromTaskHistories(taskId, TenantId);
            TaskId = taskId;
            return ExecutionResult.Next();
        }

        public override async Task<int> InsertTask(bool isBackgroundTask = true)
        {
            var taskInsertResult = await _taskService.Insert(new TaskInfo()
            {
                TenantId = TenantId,
                FlowId = FlowId,
                ActivityId = ActivityId,
                CreatedDate = DateTime.UtcNow,
                ExecutorId = await GetExecutorId()
            });

            return taskInsertResult.Value;
        }

        private async Task<int?> GetExecutorId()
        {
            switch (ActivityUserExecutorType)
            {
                case UserTaskExecutorTypeEnum.REQUESTER:
                    return RequesterId;
                case UserTaskExecutorTypeEnum.PERSON:
                    var userActivity = await _activityUserService.Get(ActivityId);
                    return userActivity.Value.PersonId;
                default:
                    return null;
            }
        }

        private async Task InsertNotification(int taskId)
        {
            var executorId = await GetExecutorId();
            if (executorId == null)
            {
                return;
            }
            var notification = new NotificationInfo
            {
                Date = DateTime.UtcNow,
                RoleId = null,
                Deleted = false,
                FlowId = FlowId,
                Flow = null,
                TaskId = taskId,
                Task = null,
                Read = false,
                Role = null,
                TenantId = TenantId,
                Type = NotificationTypeEnum.YouNeedToRunTask,
                UserId = executorId.Value,
                User = null,
            };
            await _notificationService.Insert(notification);
        }

        private async Task<bool> OpenFormToExecuteTaskInFrontEnd()
        {
            if (RequesterId != await GetExecutorId())
            {
                return false;
            }
            var sourceTaskResult = await _taskService.Get(TaskId);
            if (!sourceTaskResult.Success)
            {
                throw new Exception($"Code {TaskId} task not found.");
            }
            if (sourceTaskResult.Value.Activity.Type != WorkflowActivityTypeEnum.START_EVENT_ACTIVITY)
            {
                return false;
            }

            return true;
        }
    }
}
