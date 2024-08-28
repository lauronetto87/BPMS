using AutoMapper;
using Satelitti.Authentication.Context.Interface;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Integration.Signer;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Workflow.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using SatelittiBpms.Models.HandleException;
using Microsoft.Extensions.Logging;
using System;

namespace SatelittiBpms.Services
{
    public class TaskSignerService : AbstractServiceBase<TaskSignerDTO, TaskSignerInfo, ITaskSignerRepository>, ITaskSignerService
    {
        private readonly IContextDataService<UserInfo> _contextDataService;
        private readonly IWorkflowHostService _workflowHostService;
        private readonly ILogger<TaskSignerService> _logger;                

        public TaskSignerService(
            IContextDataService<UserInfo> contextDataService,
            ITaskSignerRepository repository,
            IMapper mapper,
            IWorkflowHostService workflowHostService,            
            ILogger<TaskSignerService> logger) : base(repository, mapper)
        {
            _contextDataService = contextDataService;
            _workflowHostService = workflowHostService;            
            _logger = logger;              
        }

        public async Task<ResultContent> ActionPerformedOnsigner(ActionPerformedOnSignerDTO actionPerformedOnSigner)
        {
            if (actionPerformedOnSigner.EnvelopeId <= 0)
            {
                _logger.LogError($"ENVELOPE_ID_SSIGN_NOT_INFORMED: EnvelopeId: {actionPerformedOnSigner.EnvelopeId}, Action: {actionPerformedOnSigner.Action}");                
                return Result.Error(ExceptionCodes.ENVELOPE_ID_SSIGN_NOT_INFORMED);                
            }

            if (actionPerformedOnSigner.Action.Equals(EnvelopeCallbackAction.COMPLETED))
            {   
                var taskSigner = _repository.GetQuery(taskSigner => taskSigner.EnvelopeId == actionPerformedOnSigner.EnvelopeId).FirstOrDefault();

                if (taskSigner == null)
                {
                    _logger.LogError($"ENVELOPE_SSIGN_NOT_FOUND:  EnvelopeId: {actionPerformedOnSigner.EnvelopeId}, Action: {actionPerformedOnSigner.Action}");                    
                    return Result.Error(ExceptionCodes.ENVELOPE_SSIGN_NOT_FOUND);
                }

                try
                {
                    taskSigner.Status = Models.Enums.TaskSignerStatusEnum.CONCLUDED;
                    await Update(taskSigner);
                    
                    await _workflowHostService.ExecuteTaskSignerIntegration(taskSigner.TaskId);
                    return Result.Success();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"ACTION_ENVELOPE_SSIGN_NOT_FOUND: EnvelopeId: {actionPerformedOnSigner.EnvelopeId}, Action: {actionPerformedOnSigner.Action}");
                    return Result.Error(ExceptionCodes.ACTION_ENVELOPE_SSIGN_NOT_FOUND);
                }
            }

            _logger.LogError($"ACTION_ENVELOPE_SSIGN_NOT_FOUND: EnvelopeId: {actionPerformedOnSigner.EnvelopeId}, Action: {actionPerformedOnSigner.Action}");
            return Result.Error(ExceptionCodes.ACTION_ENVELOPE_SSIGN_NOT_FOUND);
        }
    }
}
