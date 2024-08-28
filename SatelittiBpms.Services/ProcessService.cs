using AutoMapper;
using Satelitti.Authentication.Context.Interface;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Extensions;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class ProcessService : AbstractServiceBase<ProcessDTO, ProcessInfo, IProcessRepository>, IProcessService
    {
        private readonly IContextDataService<UserInfo> _contextDataService;
        private readonly IProcessVersionRepository _processVersionRepository;

        public ProcessService(
            IProcessVersionRepository processVersionRepository,
            IProcessRepository repository,
            IMapper mapper,
            IContextDataService<UserInfo> contextDataService) : base(repository, mapper)
        {
            _contextDataService = contextDataService;
            _processVersionRepository = processVersionRepository;
        }

        public async Task<ResultContent<ProcessCounterViewModel>> GetCounterProcess(ProcessFilterDTO filters)
        {
            return await Task.Run(() =>
            {
                var lstProcessToReturn = ListProcess(filters);
                return Result.Success(new ProcessCounterViewModel { TotalAll = lstProcessToReturn.Count });
            });
        }

        public async Task<ResultContent<List<ProcessListiningViewModel>>> ListProcessListViewModel(ProcessFilterDTO filters)
        {   
            return await Task.Run(() =>
            {
                var lstProcessToReturn = ListProcess(filters);
                lstProcessToReturn = ApplyInfinityScrollFilter(lstProcessToReturn, filters);
                return Result.Success(lstProcessToReturn.Select(x => x.AsListingViewModel()).ToList());
            });
        }

        private List<ProcessInfo> ListProcess(ProcessFilterDTO filters)
        {
            var context = _contextDataService.GetContextData();
            filters.SetTenantId(context.Tenant.Id);
            var lstProcessFromTenant = _repository.GetByTenantIncludingRelationship(filters.GetTenantId());

            var lstProcessToReturn = new List<ProcessInfo>();
            foreach (var process in lstProcessFromTenant)
            {
                process.ProcessVersions = ApplyFilters(process.ProcessVersions, filters, process.CurrentVersion);
                if (process.ProcessVersions.Any())
                    lstProcessToReturn.Add(process);
            }

            return lstProcessToReturn;
        }

        private IList<ProcessVersionInfo> ApplyFilters(IEnumerable<ProcessVersionInfo> processVersions, ProcessFilterDTO filters, int? currentVersion)
        {
            if (filters.State == null)
            {
                var processFilters = processVersions
                    .WhereIf(currentVersion != null, x => (x.Version > x.Process.CurrentVersion && x.Status == ProcessStatusEnum.EDITING));

                if (processFilters.Any()) processVersions = processFilters;
                else
                    processVersions = processVersions
                        .WhereIf(currentVersion != null, x => (x.Version == x.Process.CurrentVersion && x.Status == ProcessStatusEnum.PUBLISHED));

            }
            var context = _contextDataService.GetContextData();

            return processVersions
                .WhereIf(currentVersion == null, x => x.Status == ProcessStatusEnum.EDITING)
                .WhereIf(filters.CreationDateRange != null && filters.CreationDateRange.BeginDate.HasValue,
                            x => x.CreatedDate >= filters.CreationDateRange.BeginDate.Value.Date.AddMinutes(filters.CreationDateRange.TimezoneOffset.Value))
                .WhereIf(filters.CreationDateRange != null && filters.CreationDateRange.EndDate.HasValue,
                            x => x.CreatedDate < filters.CreationDateRange.EndDate.Value.Date.AddDays(1).AddMinutes(filters.CreationDateRange.TimezoneOffset.Value))
                .WhereIf(filters.State != null,
                            x => x.Status.Equals(filters.State))
                .WhereIf(filters.RolesFromUser != null && context.User.Id > 0,
                            x => x.ProcessVersionRoles.Any(pr => pr.Role.RoleUsers.Any(y => y.UserId == context.User.Id)))
                .WhereIf(!String.IsNullOrWhiteSpace(filters.TextSearch),
                            x => x.Name.Contains(filters.TextSearch, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        private List<ProcessInfo> ApplyInfinityScrollFilter(List<ProcessInfo> processList, ProcessFilterDTO filters)
        {
            var result = processList
              .OrderSort(filters.IsOrderAsc(), x => x.ProcessVersions.FirstOrDefault().CreatedDate);

            if (filters.Skip > 0)
                result = result.Skip(filters.Skip);

            if (filters.Take > 0)
                result = result.Take(filters.Take);

            return result.ToList();
        }

        public async Task<ResultContent<ProcessInfo>> GetByTenant(int processId, int tenantId = 0)
        {
            if (tenantId == 0)
            {
                var context = _contextDataService.GetContextData();
                tenantId = context.Tenant.Id;
            }
            var process = await _repository.GetByIdAndTenantId(processId, tenantId);
            return Result.Success(process);
        }

        public async Task UpdateCurrentVersion(int processId, int tenantId = 0)
        {
            if (tenantId == 0)
            {
                var context = _contextDataService.GetContextData();
                tenantId = context.Tenant.Id;
            }

            var processVersion = await _processVersionRepository.GetLastPublishedProcessVersion(processId, tenantId);
            var process = await _repository.Get(processId);
            process.CurrentVersion = processVersion.Version;
            await _repository.Update(process);
        }

        public async Task UpdateTaskSequance(int processId, int taskSequance = 0)
        {
            var process = await _repository.Get(processId);
            process.TaskSequance = taskSequance;
            await _repository.Update(process);
        }

        public List<string> ListWorkFlows()
        {
            List<string> listWorkflowContent = new List<string>();

            var processList = _repository.List();
            foreach (var process in processList)
            {
                if (process.ProcessVersions.Count > 0)
                {
                    foreach (var processVersion in process.ProcessVersions)
                    {
                        if (processVersion.Version.Equals(process.CurrentVersion))
                            listWorkflowContent.Add(process.ProcessVersions.FirstOrDefault(x => x.Version == process.CurrentVersion).WorkflowContent);

                        else
                        {
                            var workflowContentActive = processVersion.Flows.Where(x => x.ProcessVersionId == processVersion.Id && x.Status == Models.Enums.FlowStatusEnum.INPROGRESS).Select(x => x.ProcessVersion.WorkflowContent).FirstOrDefault();

                            if (!string.IsNullOrEmpty(workflowContentActive))
                                listWorkflowContent.Add(workflowContentActive);
                        }

                    }
                }
            }

            return listWorkflowContent;
        }

        public async Task<ResultContent> ListToFilters()
        {
            var context = _contextDataService.GetContextData();
            var lstProcess = await _repository.ListAsync();
            var lstProcessToFilter = new List<FlowGroupViewModel>();

            foreach (var process in lstProcess.Where(x => x.TenantId == context.Tenant.Id))
            {
                var processGrouped = process.ProcessVersions.GroupBy(x => x.Name,
                    (key, group) => new FlowGroupViewModel()
                    {
                        Id = process.Id,
                        Name = key,
                        Description = key,
                        Ids = group.Select(y => y.Id).ToList()
                    });

                lstProcessToFilter.AddRange(processGrouped);
            }

            return Result.Success(lstProcessToFilter);
        }
    }
}
