using SatelittiBpms.ApiGatewayManagementApi.Interfaces;
using SatelittiBpms.Mail.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace SatelittiBpms.Workflow.ActivityTypes
{
    public class SendTaskActivity : DataReplicationActivityBase
    {
        public static string TypeDescription { get { return "SatelittiBpms.Workflow.ActivityTypes.SendTaskActivity, SatelittiBpms.Workflow"; } }

        public string ConnectionId { get; set; }
        public int RequesterId { get; set; }

        private readonly IMailerService _mailerService;
        private readonly IMessageService _messageService;
        private readonly IFrontendNotifyService _frontendNotifyService;

        public SendTaskActivity(
           IFieldValueService fieldValueService,
           IFlowPathService flowPathService,
           ITaskService taskService,
           IMailerService mailerService,
           IMessageService messageService,
           IFrontendNotifyService frontendNotifyService) : base(taskService, fieldValueService, flowPathService)
        {
            _mailerService = mailerService;
            _messageService = messageService;
            _frontendNotifyService = frontendNotifyService;
        }

        public static new Dictionary<string, object> GetInputs(int tenantId, int activityId)
        {
            var inputs = DataReplicationActivityBase.GetInputs(tenantId, activityId);
            inputs.Add("ConnectionId", "data.ConnectionId");
            inputs.Add("RequesterId", "data.RequesterId");
            return inputs;
        }

        public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            var stepRunToInsertTaskAndPersist = context.PersistenceData == null;
            var stepRunToExecuteTask = context.PersistenceData != null;

            if (stepRunToInsertTaskAndPersist)
            {
                try
                {
                    var message = new ExpandoObject();
                    message.TryAdd("taskIdToExecute", null);
                    message.TryAdd("canExecute", false);
                    await _frontendNotifyService.Notify(ConnectionId, message);
                }
                catch
                {
                    // adicionado try catch para que mesmo que dê erro na notificação a task seja criada e replicado os valores dos campos
                    // assim, não impactando no andamento do fluxo só por não conseguir notificar.
                    // apenas será necessário que o usuário atualize a tela manualmente para listar o fluxo no estado atual
                }
                var currentTaskId = await InsertTask(false);
                await InsertFlowPath(currentTaskId);
                await ReplicateFieldValues(currentTaskId);
                return ExecutionResult.Persist(currentTaskId);
            }

            if (stepRunToExecuteTask)
            {
                var currentTaskId = Convert.ToInt32(context.PersistenceData);
                TaskId = currentTaskId;
                await _mailerService.SendMail(await _messageService.CreateMessage(TenantId, ActivityId, RequesterId, TaskId), null);
                await UpdateFinishedDateFromTask(currentTaskId);

                return ExecutionResult.Next();
            }

            throw new Exception("Unhandled step execution sequence. TaskId: " + TaskId);
        }

    }
}
