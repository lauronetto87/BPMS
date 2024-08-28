using AutoMapper;
using Newtonsoft.Json.Linq;
using Satelitti.Authentication.Context.Interface;
using SatelittiBpms.Data.Extensions;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Extensions;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Helpers;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class TaskService : AbstractServiceBase<TaskDTO, TaskInfo, ITaskRepository>, ITaskService
    {
        private readonly IContextDataService<UserInfo> _contextDataService;
        private readonly ITaskHistoryService _taskHistoryService;
        private readonly IWorkflowHostService _workflowHostService;
        private readonly IFieldValueService _fieldValueService;
        private readonly IFieldValueFileService _fieldValueFileService;
        private readonly IFlowService _flowService;
        private readonly IUserService _userService;
        private readonly IRoleUserService _roleUserService;
        private readonly IWildcardService _wildcardService;
        private readonly IFlowPathService _flowPathService;

        public TaskService(
            ITaskRepository repository,
            IMapper mapper,
            ITaskHistoryService taskHistoryService,
            IWorkflowHostService workflowHostService,
            IFlowService flowService,
            IContextDataService<UserInfo> contextDataService,
            IFieldValueService fieldValueService,
            IFieldValueFileService fieldValueFileService,
            IRoleUserService roleUserService,
            IUserService userService,
            IWildcardService wildcardService,
            IFlowPathService flowPathService) : base(repository, mapper)
        {
            _contextDataService = contextDataService;
            _taskHistoryService = taskHistoryService;
            _workflowHostService = workflowHostService;
            _fieldValueService = fieldValueService;
            _fieldValueFileService = fieldValueFileService;
            _flowService = flowService;
            _userService = userService;
            _roleUserService = roleUserService;
            _wildcardService = wildcardService;
            _flowPathService = flowPathService;
        }


        public async Task<ResultContent> ListTasks(TaskFilterDTO filters)
        {
            return filters.TaskQueryType switch
            {
                TaskQueryType.ALL => await ListAllTasks(filters),
                TaskQueryType.MYTASKS => await ListMyTasks(filters),
                _ => throw new NotImplementedException()
            };
        }

        public async Task<ResultContent> ListTasksGroup(TaskFilterDTO filters)
        {
            var context = _contextDataService.GetContextData();
            filters.SetTenantId(context.Tenant.Id);
            var groupList = new List<FlowGroupViewModel>();

            switch (filters.TaskQueryType)
            {
                case TaskQueryType.MYTASKS:
                    groupList = await ListMyTasksGroup(filters);
                    break;
                case TaskQueryType.ALL:
                    groupList = await _flowService.GetAllTaskGroup(filters);
                    break;
            }

            return Result.Success(groupList);
        }

        private async Task<List<FlowGroupViewModel>> ListMyTasksGroup(TaskFilterDTO filters)
        {
            var context = _contextDataService.GetContextData();
            var groupList = new List<FlowGroupViewModel>();
            var users = await _userService.ListUsersSuite();

            var lstMyTasks = _repository
                .ListByTenantFilters(filters, context.User.Id)
                .Where(x => x.Flow.FinishedDate == null && x.FinishedDate == null)
                .WhereIf(filters.OnlyMyRequests, x => x.Flow.RequesterId == context.User.Id);

            var myTasksViewModel = new List<FlowViewModel>();
            foreach (var task in lstMyTasks)
            {
                var taskViewModel = task.AsListingViewModel(users);
                taskViewModel.DescriptionFlow = _wildcardService.FormatDescriptionWildcard(task.Flow.ProcessVersion.DescriptionFlow, task.Flow, users);
                myTasksViewModel.Add(taskViewModel);
            }

            myTasksViewModel = ApplyFilters(myTasksViewModel, filters);

            switch (filters.TaskGroupType)
            {
                case TaskGroupType.TASK:
                    var lstMytasksOrder = myTasksViewModel.Select(x => x.AsActivitieListingViewModel()).ToList().GroupBy(x => new { x.Name, x.Description }).Select(x => x.ToList()).ToList();
                    foreach (var item in lstMytasksOrder)
                    {
                        FlowGroupViewModel flowGroupViewModel = new()
                        {
                            Id = item[0].Id,
                            Name = item[0].Name,
                            Description = item[0].Description,
                            Ids = item.Select(x => x.Id).Distinct().ToList()
                        };

                        groupList.Add(flowGroupViewModel);
                    }
                    break;
                case TaskGroupType.PROCESS:
                    var lstMytasksOrderProcess = myTasksViewModel.Select(x => x.AsProcessVersionListingViewModel()).ToList().GroupBy(x => x.Name).Select(x => x.ToList()).ToList();
                    foreach (var item in lstMytasksOrderProcess)
                    {
                        FlowGroupViewModel flowGroupViewModel = new()
                        {
                            Id = item[0].Id,
                            Name = item[0].Name,
                            Ids = item.Select(x => x.Id).Distinct().ToList()
                        };

                        groupList.Add(flowGroupViewModel);
                    }
                    break;
            }

            return groupList;
        }

        private IList<FlowViewModel> GetListMyTasksAsync(TaskFilterDTO filters, IList<SuiteUserViewModel>? users)
        {
            var context = _contextDataService.GetContextData();
            filters.SetTenantId(context.Tenant.Id);

            var lstMyTasks = _repository.ListByTenantFilters(filters, context.User.Id).Where(x => x.Flow.FinishedDate == null && x.FinishedDate == null)
                .WhereIf(filters.TaskGroupType == TaskGroupType.TASK && filters.GroupId?.Count > 0, x => filters.GroupId.Contains(x.ActivityId))
                .WhereIf(filters.TaskGroupType == TaskGroupType.PROCESS && filters.GroupId?.Count > 0, x => filters.GroupId.Contains(x.Activity.ProcessVersionId))
                .WhereIf(filters.OnlyMyRequests, x => x.Flow.RequesterId == context.User.Id)
                .ToList();

            var myTasksViewModel = new List<FlowViewModel>();
            foreach (var task in lstMyTasks)
            {
                var taskViewModel = task.AsListingViewModel(users);
                taskViewModel.DescriptionFlow = _wildcardService.FormatDescriptionWildcard(task.Flow.ProcessVersion.DescriptionFlow, task.Flow, users); ;
                myTasksViewModel.Add(taskViewModel);
            }

            return ApplyFilters(myTasksViewModel, filters);
        }

        private async Task<ResultContent> ListMyTasks(TaskFilterDTO filters)
        {
            var users = await _userService.ListUsersSuite();
            var lstMytasks = GetListMyTasksAsync(filters, users).ToList();
            var lstMytasksScrollApplied = ApplyInfinityScrollFilter(lstMytasks, filters);

            FlowQueryViewModel flowQueryViewModel = new()
            {
                Quantity = lstMytasks.Count,
                List = lstMytasksScrollApplied.ToList()
            };

            return Result.Success(flowQueryViewModel);
        }

        private List<FlowViewModel> ApplyFilters(List<FlowViewModel> taskInfo, TaskFilterDTO filters)
        {
            if (!string.IsNullOrWhiteSpace(filters.TextSearch))
            {
                taskInfo = taskInfo.Where(x => x.Name.Contains(filters.TextSearch, StringComparison.CurrentCultureIgnoreCase) ||
                    x.ActivityName.Contains(filters.TextSearch, StringComparison.CurrentCultureIgnoreCase) ||
                    x.FlowId.ToString().Contains(filters.TextSearch, StringComparison.CurrentCultureIgnoreCase) ||
                    x.DescriptionFlow.Contains(filters.TextSearch, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            return taskInfo;
        }

        private async Task<ResultContent> ListAllTasks(TaskFilterDTO filters)
        {
            return await _flowService.ListAll(filters);
        }

        private List<FlowViewModel> ApplyInfinityScrollFilter(List<FlowViewModel> lstMytasks, TaskFilterDTO filters)
        {
            var ignoreListId = new List<int>();

            if (filters.IgnoreListId != null)
            {
                ignoreListId.AddRange(filters.IgnoreListId);
            }

            var result = lstMytasks.Where(x => !ignoreListId.Contains(x.Id)).OrderSort(filters.IsOrderAsc(), x => x.CreationDate);

            if (filters.TotalByQuery > 0)
                result = result.Take(filters.TotalByQuery);

            return result.ToList();
        }

        public async Task<ResultContent> Assign(int taskId)
        {
            using (var transaction = _repository.BeginTransaction())
            {
                int taskHistoryId;

                try
                {
                    var taskInfo = await _repository.Get(taskId);
                    taskInfo.ExecutorId = _contextDataService.GetContextData().User.Id;
                    await _repository.Update(taskInfo);

                    taskHistoryId = await _taskHistoryService.Insert(taskId, _contextDataService.GetContextData().User.Id);

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();

                    AddErrors(ExceptionCodes.UPDATE_TASK_TRANSACTION_ERROR, e.Message);
                    return Result.Error(ValidationResult);
                }

                return Result.Success(taskHistoryId);
            }
        }

        public async Task<ResultContent<int>> Insert(TaskInfo info)
        {
            return Result.Success(await _repository.Insert(info));
        }

        public async Task<ResultContent> Unassign(int taskId)
        {
            using (var transaction = _repository.BeginTransaction())
            {
                try
                {
                    var context = _contextDataService.GetContextData();

                    var taskInfo = await _repository.GetByIdAndTenantId(taskId, context.Tenant.Id);
                    taskInfo.ExecutorId = null;
                    await _repository.Update(taskInfo);

                    await _fieldValueFileService.Delete(taskId);

                    var previousTaskId = _flowPathService.getPreviousTaskId(taskId);

                    var previousTaskInfo = _repository.GetByIdAndTenantId(previousTaskId, context.Tenant.Id).Result;
                    await _fieldValueFileService.Unassign(previousTaskInfo, taskInfo);

                    await _taskHistoryService.UnassignHistory(taskId, _contextDataService.GetContextData().User.Id);

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();

                    AddErrors(ExceptionCodes.UPDATE_TASK_TRANSACTION_ERROR, e.Message);
                    return Result.Error(ValidationResult);
                }

                return Result.Success();
            }
        }

        public async Task<ResultContent> NextTask(NextStepDTO nextStepDTO)
        {
            await _fieldValueService.UpdateFieldValues(nextStepDTO.TaskId, nextStepDTO.FormData);
            await _workflowHostService.NextTask(nextStepDTO);
            await UpdateSelectedOption(nextStepDTO);
            return await Task.FromResult(Result.Success());
        }

        public async Task<ResultContent> ExecuteTaskSignerIntegration(int taskId)
        {
            await _workflowHostService.ExecuteTaskSignerIntegration(taskId);
            return await Task.FromResult(Result.Success());
        }

        private async Task<ResultContent> UpdateSelectedOption(NextStepDTO nextStepDTO)
        {
            var taskInfo = await _repository.Get(nextStepDTO.TaskId);
            taskInfo.OptionId = nextStepDTO.OptionId;
            await _repository.Update(taskInfo);

            return Result.Success();
        }

        public async Task<ResultContent> GetTaskToExecute(int taskId)
        {
            var context = _contextDataService.GetContextData();

            var taskWithDependencies = await _repository.GetByIdAndTenantId(taskId, context.Tenant.Id);

            taskWithDependencies = SuitJsonRuleFields(taskWithDependencies);

            _fieldValueService.GetJsonFieldValues(taskWithDependencies.FieldsValues);
            dynamic dados = _fieldValueService.GetFormatedFormData(taskWithDependencies.FieldsValues ?? GetFieldsFromActivityFields(taskWithDependencies.Activity.ActivityFields));

            var taskToExecute = taskWithDependencies.AsExecuteViewModel(dados);

            return Result.Success(taskToExecute);
        }

        private IList<FieldValueInfo> GetFieldsFromActivityFields(IList<ActivityFieldInfo> activityFields)
        {
            List<FieldValueInfo> fieldValueList = new List<FieldValueInfo>();
            foreach (var field in activityFields)
            {
                fieldValueList.Add(new FieldValueInfo()
                {
                    Field = field.Field,
                    FieldValue = field.Field.Type == FieldTypeEnum.FILE ? "[]" : String.Empty
                });
            }
            return fieldValueList;
        }

        private TaskInfo SuitJsonRuleFields(TaskInfo taskToExecute)
        {
            var jsonContent = taskToExecute.Flow.ProcessVersion.FormContent;
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                return taskToExecute;
            }

            var formIoObj = JObject.Parse(jsonContent);
            if (!String.IsNullOrEmpty(jsonContent))
            {
                var allComponents = FormIoHelper.GetAllComponents(formIoObj);
                SuitJsonRuleFieldsComponents(taskToExecute, allComponents);
                taskToExecute.Flow.ProcessVersion.FormContent = formIoObj.ToString();
            }

            return taskToExecute;
        }

        private TaskInfo SuitJsonRuleFieldsComponents(TaskInfo taskToExecute, List<JObject> allComponents)
        {
            foreach (var component in allComponents)
            {
                var activityField = taskToExecute.Activity.ActivityFields.FirstOrDefault(x => x.Field.ComponentInternalId == component["key"].ToString());
                switch (activityField.State)
                {
                    case ProcessTaskFieldStateEnum.MANDATORY:
                        component["validate"]["required"] = true;
                        break;
                    case ProcessTaskFieldStateEnum.ONLYREADING:
                        component["disabled"] = true;
                        break;
                    case ProcessTaskFieldStateEnum.INVISIBLE:
                        component["hidden"] = true;
                        break;
                }
            }

            return taskToExecute;
        }

        public async Task<ResultContent> GetDetailsById(int taskId)
        {
            var users = await _userService.ListUsersSuite();
            var currentUserRoleIds = await _roleUserService.GetRulesIdByUser(_contextDataService.GetContextData().User.Id);
            var taskDetail = _repository.GetDetailsById(taskId);

            var userId = _contextDataService.GetContextData().User.Id;

            var taskDetailsViewModel = taskDetail.AsDetailsViewModel(users, userId, currentUserRoleIds);
            if (taskDetailsViewModel.YouNeedToRun && taskDetailsViewModel.YouWillRun && taskDetailsViewModel.YourRoleWillTask && taskDetailsViewModel.YouHaveRunTask)
            {
                taskDetailsViewModel.FieldValueFiles = null;
            }
            if (taskDetailsViewModel.FieldValueFiles != null)
            {
                taskDetailsViewModel.FieldValueFiles = taskDetailsViewModel.FieldValueFiles.OrderByDescending(x => x.Signed).ToList();
            }

            taskDetailsViewModel.DescriptionFlow = _wildcardService.FormatDescriptionWildcard(taskDetail.Flow.ProcessVersion.DescriptionFlow, taskDetail.Flow, users);

            return Result.Success(taskDetailsViewModel);
        }

        public async Task<ResultContent<TaskCounterViewModel>> GetCounterTask(TaskFilterDTO filters)
        {
            var allTasks = (await _flowService.CountAll(filters)).Value;
            var myTasks = GetListMyTasksAsync(filters, null).Count;
            return Result.Success(new TaskCounterViewModel { AllTaskCount = allTasks, MyTaskCount = myTasks });
        }
    }
}