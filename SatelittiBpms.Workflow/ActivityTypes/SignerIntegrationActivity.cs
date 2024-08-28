using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Workflow.Models;
using System;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace SatelittiBpms.Workflow.ActivityTypes
{
    public class SignerIntegrationActivity : DataReplicationActivityBase
    {
        public static string TypeDescription { get { return "SatelittiBpms.Workflow.ActivityTypes.SignerIntegrationActivity, SatelittiBpms.Workflow"; } }

        ISignerIntegrationService _signerIntegrationService;

        public SignerIntegrationActivity(
            IFieldValueService fieldValueService,
            IFlowPathService flowPathService,
            ITaskService taskService,
            ISignerIntegrationService signerIntegrationService) : base(taskService, fieldValueService, flowPathService)
        {
            _signerIntegrationService = signerIntegrationService;
        }

        public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            var stepRunToInsertTaskAndPersist = context.PersistenceData == null && !context.ExecutionPointer.EventPublished;
            var stepRunIntegrationFinished = context.ExecutionPointer.EventPublished;
            var stepRunToWaitForEvent = !stepRunToInsertTaskAndPersist && !stepRunIntegrationFinished;

            if (stepRunToInsertTaskAndPersist)
            {
                var currentTaskId = await InsertTask(false);
                await InsertFlowPath(currentTaskId);
                await ReplicateFieldValues(currentTaskId);                
                return ExecutionResult.Persist(currentTaskId);
            }

            if (stepRunToWaitForEvent)
            {
                var currentTaskId = Convert.ToInt32(context.PersistenceData);
                await _signerIntegrationService.CreateEnvelopeOnSigner(currentTaskId);
                TaskId = currentTaskId;
                var enventUser = new EventUserInfo(currentTaskId);
                return ExecutionResult.WaitForEvent(enventUser.eventName, enventUser.eventKey, DateTime.UtcNow);
            }

            if (stepRunIntegrationFinished)
            {
                var taskId = Convert.ToInt32(context.ExecutionPointer.EventData);
                TaskId = taskId;
                await UpdateFinishedDateFromTask(taskId);
                return ExecutionResult.Next();
            }

            throw new Exception("Unhandled step execution sequence. TaskId: " + TaskId);
        }

    }
}
