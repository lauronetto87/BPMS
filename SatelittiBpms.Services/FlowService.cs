using AutoMapper;
using Satelitti.Authentication.Context.Interface;
using SatelittiBpms.Data.Extensions;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Translate.Interfaces;
using SatelittiBpms.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class FlowService : AbstractServiceBase<FlowDTO, FlowInfo, IFlowRepository>, IFlowService
    {
        private readonly IContextDataService<UserInfo> _contextDataService;
        private readonly IWorkflowHostService _workflowHostService;
        private readonly IProcessRepository _processRepository;
        private readonly ITranslateService _translateService;
        private readonly IUserService _userService;
        private readonly IRoleUserService _roleUserService;
        private readonly IWildcardService _wildcardService;

        public FlowService(
              IFlowRepository repository,
              IMapper mapper,
              IProcessRepository processRepository,
              IContextDataService<UserInfo> contextDataService,
              IWorkflowHostService workflowHostService,
              ITranslateService translateService,
              IUserService userService,
              IRoleUserService roleUserService,
              IWildcardService wildcardService) : base(repository, mapper)
        {
            _contextDataService = contextDataService;
            _processRepository = processRepository;
            _workflowHostService = workflowHostService;
            _translateService = translateService;
            _userService = userService;
            _roleUserService = roleUserService;
            _wildcardService = wildcardService;
        }

        public async Task<ResultContent<string>> Request(FlowRequestDTO flowRequestDTO)
        {
            ProcessInfo process = await _processRepository.Get(flowRequestDTO.ProcessId);
            var result = await _workflowHostService.StartFlow(flowRequestDTO.ProcessId, process.CurrentVersion.Value, _contextDataService.GetContextData().User.Id, flowRequestDTO.ConnectionId);
            return new ResultContent<string>(result, true, null);
        }

        public async Task<ResultContent<int>> Insert(FlowInfo info)
        {
            return Result.Success(await _repository.Insert(info));
        }

        public async Task<List<FlowGroupViewModel>> GetAllTaskGroup(TaskFilterDTO filters)
        {
            var context = _contextDataService.GetContextData();
            filters.SetTenantId(context.Tenant.Id);
            var groupList = new List<FlowGroupViewModel>();
            var roleIds = await _roleUserService.GetRulesIdByUser(_contextDataService.GetContextData().User.Id);

            switch (filters.TaskGroupType)
            {
                case TaskGroupType.TASK:
                    groupList = await ListGroupTaskAsync(filters, roleIds?.ToArray());
                    break;
                case TaskGroupType.PROCESS:
                    groupList = await ListGroupProcessAsync(filters, roleIds?.ToArray());
                    break;
            }

            return groupList;
        }

        private async Task<List<FlowGroupViewModel>> ListGroupTaskAsync(TaskFilterDTO filters, int[] roleIds)
        {
            var context = _contextDataService.GetContextData();
            var users = await _userService.ListUsersSuite();
            var groupList = new List<FlowGroupViewModel>();

            var lstAllTask = _repository.ListByTenantFilters(filters, roleIds, context.User.Id).ToList()
                .WhereIf(filters.OnlyMyRequests, x => x.RequesterId == context.User.Id)
                .WhereIf(filters.TaskGroupType == TaskGroupType.TASK && filters.GroupId?.Count > 0, x => filters.GroupId.Contains(x.Tasks.Last().Activity.Id));

            var myFlowViewModel = new List<FlowViewModel>();
            foreach (var flow in lstAllTask)
            {
                var taskViewModel = flow.AsListingViewModel(users, context.User.Id);
                taskViewModel.DescriptionFlow = _wildcardService.FormatDescriptionWildcard(flow.ProcessVersion.DescriptionFlow, flow, users);
                myFlowViewModel.Add(taskViewModel);
            }

            myFlowViewModel = ApplyFilters(myFlowViewModel, filters);

            var lstAllTasksOrder = myFlowViewModel.Select(x => x.AsActivitieListingViewModel()).ToList().GroupBy(x => new { x.Name, x.Description }).Select(x => x.ToList()).ToList();
            foreach (var item in lstAllTasksOrder)
            {
                FlowGroupViewModel flowGroupViewModel = new()
                {
                    Id = item[0].Id,
                    Name = item[0].Name ?? _translateService.Localize("flows.labels.finished"),
                    Description = item[0].Description,
                    Ids = item.Select(x => x.Id).Distinct().ToList()
                };

                groupList.Add(flowGroupViewModel);
            }
            return groupList;
        }

        private async Task<List<FlowGroupViewModel>> ListGroupProcessAsync(TaskFilterDTO filters, int[] roleIds)
        {
            var context = _contextDataService.GetContextData();
            var groupList = new List<FlowGroupViewModel>();
            var users = await _userService.ListUsersSuite();

            var lstAllProcess = _repository.ListByTenantFilters(filters, roleIds, context.User.Id).ToList()
                .WhereIf(filters.OnlyMyRequests, x => x.RequesterId == context.User.Id)
                .WhereIf(filters.TaskGroupType == TaskGroupType.PROCESS && filters.GroupId?.Count > 0, x => filters.GroupId.Contains(x.ProcessVersionId));

            var myFlowViewModel = new List<FlowViewModel>();
            foreach (var flow in lstAllProcess)
            {
                var taskViewModel = flow.AsListingViewModel(users, context.User.Id);
                taskViewModel.DescriptionFlow = _wildcardService.FormatDescriptionWildcard(flow.ProcessVersion.DescriptionFlow, flow, users);
                myFlowViewModel.Add(taskViewModel);
            }

            myFlowViewModel = ApplyFilters(myFlowViewModel, filters);

            var lstAllProcessOrder = myFlowViewModel.Select(x => x.AsProcessVersionListingViewModel()).ToList().GroupBy(x => x.Name).Select(x => x.ToList()).ToList();
            foreach (var item in lstAllProcessOrder)
            {
                FlowGroupViewModel flowGroupViewModel = new()
                {
                    Id = item[0].Id,
                    Name = item[0].Name,
                    Ids = item.Select(x => x.Id).Distinct().ToList()
                };

                groupList.Add(flowGroupViewModel);
            }

            return groupList;
        }

        private List<FlowViewModel> ApplyFilters(List<FlowViewModel> flowViewModel, TaskFilterDTO filters)
        {
            if (!string.IsNullOrWhiteSpace(filters.TextSearch))
            {
                flowViewModel = flowViewModel.Where(x => x.Name.Contains(filters.TextSearch, StringComparison.CurrentCultureIgnoreCase) ||
                    (!x.Finished && x.ActivityName.Contains(filters.TextSearch, StringComparison.CurrentCultureIgnoreCase)) ||
                    x.FlowId.ToString().Contains(filters.TextSearch, StringComparison.CurrentCultureIgnoreCase) ||
                    x.DescriptionFlow.Contains(filters.TextSearch, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            return flowViewModel;
        }

        public async Task<ResultContent> ListAll(TaskFilterDTO filters)
        {
            var users = await _userService.ListUsersSuite();
            var roleIds = await _roleUserService.GetRulesIdByUser(_contextDataService.GetContextData().User.Id);
            var lstAll = await GetListFlowViewModelAsync(filters, roleIds.ToArray(), users);
            var lstMytasksScrollApplied = ApplyInfinityScrollFilter(lstAll, filters);

            var flowQueryViewModel = new FlowQueryViewModel
            {
                Quantity = lstAll.Count,
                List = lstMytasksScrollApplied.ToList()
            };

            return Result.Success(flowQueryViewModel);
        }

        public IQueryable<FlowInfo> GetQueryableListAll(TaskFilterDTO filters, int[] roleIds)
        {
            var context = _contextDataService.GetContextData();
            filters.SetTenantId(context.Tenant.Id);

            return _repository.ListByTenantFilters(filters, roleIds, context.User.Id)
                .WhereIf(filters.OnlyMyRequests, x => x.RequesterId == context.User.Id)
                .WhereIf(filters.TaskGroupType == TaskGroupType.TASK && filters.GroupId?.Count > 0, x => filters.GroupId.Contains(x.Tasks.OrderByDescending(x => x.Id).FirstOrDefault().ActivityId))
                .WhereIf(filters.TaskGroupType == TaskGroupType.PROCESS && filters.GroupId?.Count > 0, x => filters.GroupId.Contains(x.ProcessVersionId));
        }

        private async Task<IList<FlowViewModel>> GetListFlowViewModelAsync(TaskFilterDTO filters, int[] roleIds, IList<SuiteUserViewModel>? users)
        {
            return await Task.Run(() =>
            {
                var context = _contextDataService.GetContextData();
                filters.SetTenantId(context.Tenant.Id);

                var lstAllProcess = _repository.ListByTenantFilters(filters, roleIds, context.User.Id).ToList()
                    .WhereIf(filters.OnlyMyRequests, x => x.RequesterId == context.User.Id)
                    .WhereIf(filters.TaskGroupType == TaskGroupType.TASK && filters.GroupId?.Count > 0, x => filters.GroupId.Contains(x.Tasks.OrderByDescending(x => x.Id).FirstOrDefault().ActivityId))
                    .WhereIf(filters.TaskGroupType == TaskGroupType.PROCESS && filters.GroupId?.Count > 0, x => filters.GroupId.Contains(x.ProcessVersionId));

                var myFlowViewModel = new List<FlowViewModel>();
                foreach (var flow in lstAllProcess)
                {
                    var taskViewModel = flow.AsListingViewModel(users, context.User.Id);
                    taskViewModel.DescriptionFlow = _wildcardService.FormatDescriptionWildcard(flow.ProcessVersion.DescriptionFlow, flow, users);
                    myFlowViewModel.Add(taskViewModel);
                }
                return ApplyFilters(myFlowViewModel, filters);
            });
        }

        public async Task<ResultContent<int>> CountAll(TaskFilterDTO filters)
        {
            var roleIds = await _roleUserService.GetRulesIdByUser(_contextDataService.GetContextData().User.Id);
            var total = GetListFlowViewModelAsync(filters, roleIds.ToArray(), null).Result.Count;
            return new ResultContent<int>(total, true, null);
        }

        public bool ContainsFlowForProcessVersion(int processVersionId)
        {
            return _repository
                .GetByTenant(_contextDataService.GetContextData().Tenant.Id)
                .Any(f => f.ProcessVersionId == processVersionId);
        }

        private IEnumerable<FlowViewModel> ApplyInfinityScrollFilter(IEnumerable<FlowViewModel> lstAll, TaskFilterDTO filters)
        {
            var ignoreListId = new List<int>();

            if (filters.IgnoreListId != null)
            {
                ignoreListId.AddRange(filters.IgnoreListId);
            }

            var filteredData = lstAll.Where(x => !ignoreListId.Contains(x.Id));

            if (filters.IsOrderAsc())
            {
                filteredData = filteredData.OrderBy(x => x.CreationDate);
            }

            if (filters.TotalByQuery > 0)
            {
                filteredData = filteredData.Take(filters.TotalByQuery);
            }

            return filteredData;
        }

        public List<FieldInfo> GetFields(int flowId)
        {
            return _repository
                .GetQuery(flowId)
                .Select(f => f.ProcessVersion.Fields).First().ToList();
        }
    }
}