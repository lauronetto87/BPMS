using FluentValidation.Results;
using Satelitti.Authentication.Context.Interface;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Services.ProcessVersionValidation
{
    internal class ProcessNameDuplicateValidation : ProcessValidationBase
    {
        public ProcessNameDuplicateValidation(IProcessVersionRepository repository, IContextDataService<UserInfo> contextDataService, IFlowService flowService, string processName, int editProcessVersionId, int editProcessId)
        {
            _processName = processName;
            _editProcessVersionId = editProcessVersionId;
            _repository = repository;
            _contextDataService = contextDataService;
            _flowService = flowService;
            _editProcessId = editProcessId;
        }

        private readonly string _processName;
        private readonly int _editProcessVersionId;
        private readonly int _editProcessId;
        private readonly IProcessVersionRepository _repository;
        private readonly IContextDataService<UserInfo> _contextDataService;
        private readonly IFlowService _flowService;



        public override List<ValidationFailure> Validate()
        {
            var context = _contextDataService.GetContextData();
            var processVersionSameName = _repository
                .GetByTenant(context.Tenant.Id)
                .Where(p => p.Name == _processName && p.Id != _editProcessVersionId && p.ProcessId != _editProcessId)
                .Select(p => new { p.Id, p.ProcessId }).ToList();
            foreach (var procesVersion in processVersionSameName)
            {
                if (IsTheActiveProcessVersionOrInEdit(procesVersion.ProcessId, procesVersion.Id) || _flowService.ContainsFlowForProcessVersion(procesVersion.Id))
                {
                    return new List<ValidationFailure> { CreateValidationFailure(ExceptionCodes.PROCESS_VERSION_SAVE_NAME_DUPLICATE) };
                }
            }
            return new List<ValidationFailure>();
        }


        private bool IsTheActiveProcessVersionOrInEdit(int processId, int processVersionId)
        {
            var context = _contextDataService.GetContextData();
            var lastTwoProcessVersion = _repository
                .GetByTenant(context.Tenant.Id)
                .Where(p => p.ProcessId == processId)
                .OrderByDescending(p => p.Id)
                .Select(p => new { p.Id, p.Status })
                .Take(2)
                .ToList();
            if (lastTwoProcessVersion[0].Id == processVersionId)
            {
                return true;
            }
            if (lastTwoProcessVersion[0].Status == ProcessStatusEnum.PUBLISHED)
            {
                return false;
            }
            if (lastTwoProcessVersion.Count > 1 && lastTwoProcessVersion[1].Id == processVersionId)
            {
                return true;
            }
            return false;
        }

    }
}
