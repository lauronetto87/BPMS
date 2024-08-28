using Microsoft.Extensions.Logging;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Workflow.Interfaces;
using SatelittiBpms.Workflow.Models;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Services.DefinitionStorage;

namespace SatelittiBpms.Workflow.Services
{
    public class WorkflowHostService : IWorkflowHostService
    {
        private readonly IWorkflowHost _workflowHost;
        private readonly IDefinitionLoader _definitionLoader;
        private readonly IProcessService _processService;
        private readonly ILogger<WorkflowHostService> _logger;

        public WorkflowHostService(
            IWorkflowHost workflowHost,
            IDefinitionLoader definitionLoader,
            IProcessService processService,
            ILogger<WorkflowHostService> logger)
        {
            _workflowHost = workflowHost;
            _workflowHost.OnStepError += _workflowHost_OnStepError;
            _definitionLoader = definitionLoader;
            _processService = processService;
            _logger = logger;
        }

        private void _workflowHost_OnStepError(WorkflowCore.Models.WorkflowInstance workflow, WorkflowCore.Models.WorkflowStep step, System.Exception exception)
        {
            _logger.LogError(exception, $"FlowId: {((FlowDataInfo)workflow.Data).FlowId}, TaskId: {((FlowDataInfo)workflow.Data).TaskId}, Workflow: {workflow.Id}, Step: {step.ExternalId}");
        }

        public void LoadWorkflowFromJson(string workflowJsonAsString)
        {
            _definitionLoader.LoadDefinition(workflowJsonAsString, Deserializers.Json);
        }

        public async Task<string> StartFlow(int processId, int version, int requesterId, string connectionId)
        {
            FlowDataInfo initialData = new FlowDataInfo()
            {
                RequesterId = requesterId,
                ConnectionId = connectionId
            };

            return await _workflowHost.StartWorkflow(processId.ToString(), version, initialData);
        }

        public async Task ExecuteTaskSignerIntegration(int taskId)
        {
            var enventUser = new EventUserInfo(taskId);
            await _workflowHost.PublishEvent(enventUser.eventName, enventUser.eventKey, taskId, System.DateTime.UtcNow);
        }

        public async Task NextTask(NextStepDTO nextStepDTO)
        {
            FlowDataInfo initialData = new FlowDataInfo()
            {
                TaskId = nextStepDTO.TaskId,
                Option = nextStepDTO.OptionId
            };

            var enventUser = new EventUserInfo(initialData.TaskId);
            await _workflowHost.PublishEvent(enventUser.eventName, enventUser.eventKey, initialData, System.DateTime.UtcNow);
        }

        public void LoadPublishedWorkflows()
        {
            var workFlowList = _processService.ListWorkFlows();
            foreach (var workFlow in workFlowList)
            {
                _definitionLoader.LoadDefinition(workFlow, Deserializers.Json);
            }
        }
    }
}
