using Newtonsoft.Json;
using Satelitti.Model;
using SatelittiBpms.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Models.Infos
{
    public class TaskInfo : BaseInfo
    {
        #region Properties
        public DateTime CreatedDate { get; set; }
        public DateTime? FinishedDate { get; set; }
        #endregion

        #region Relationship

        public int? OptionId { get; set; }
        [JsonIgnore]
        public ActivityUserOptionInfo? Option { get; set; }

        public int FlowId { get; set; }
        [JsonIgnore]
        public FlowInfo Flow { get; set; }

        public int ActivityId { get; set; }
        [JsonIgnore]
        public ActivityInfo Activity { get; set; }

        public int? ExecutorId { get; set; }
        [JsonIgnore]
        public UserInfo? Executor { get; set; }

        public IList<FieldValueInfo> FieldsValues { get; set; }
        public IList<FlowPathInfo> SourceTasks { get; set; }
        public IList<FlowPathInfo> TargetTasks { get; set; }
        public IList<TaskHistoryInfo> TasksHistories { get; set; }

        public IList<NotificationInfo> Notifications { get; set; }

        public IList<TaskSignerInfo> SignerTasks { get; set; }

        #endregion

        public FlowViewModel AsListingViewModel(IList<SuiteUserViewModel>? userViewModel)
        {
            return new FlowViewModel
            {
                Id = Id,
                FlowId = Flow.Id,
                ExecutorId = ExecutorId,
                Name = Flow.ProcessVersion.Name,
                Description = Flow.ProcessVersion.Description,
                DescriptionFlow = Flow.ProcessVersion.DescriptionFlow,
                CreationDate = CreatedDate,
                CreatedByUserName = Flow.ProcessVersion.CreatedByUserName,
                CreatedByUserId = Flow.ProcessVersion.CreatedByUserId,
                ProcessStatus = Flow.ProcessVersion.Status,
                ExecutorType = Activity.ActivityUser.ExecutorType,
                CurrentRoleName = Activity.ActivityUser.Role?.Name,
                ExecutorName = ExecutorId > 0 ? userViewModel?.FirstOrDefault(u => u.Id == ExecutorId).Name : "",
                ActivityName = Activity.Name,
                ActivityId = Activity.Id,
                ProcessVersionId = Flow.ProcessVersionId,
                ProcessVersionName = Flow.ProcessVersion.Name,
            };
        }

        public TaskExecuteViewModel AsExecuteViewModel(object formData)
        {
            return new TaskExecuteViewModel
            {
                TaskId = Id,
                FlowId = FlowId,
                ProcessVersionId = Flow.ProcessVersionId,
                FormContent = Flow.ProcessVersion.FormContent,
                FormData = formData,
                ProcessName = Flow.ProcessVersion.Name,
                ActivityName = Activity.Name,
                Options = Activity.ActivityUser.ActivityUsersOptions.ToList().Select(x => new TaskOptionsViewModel() { Id = x.Id, description = x.Description }).DefaultIfEmpty(new TaskOptionsViewModel()).ToList(),
            };
        }

        public FlowGroupViewModel AsActivitieListingViewModel()
        {
            return new FlowGroupViewModel()
            {
                Id = Activity.Id,
                Name = Activity.Name,
                Description = Flow.ProcessVersion.Name,
                Ids = new List<int>()
            };
        }

        public TaskDetailsViewModel AsDetailsViewModel(IList<SuiteUserViewModel> userViewModels, int userId, List<int> currentUserRoleIds)
        {
            IList<FieldValueFileViewModel> fieldValueFileViewModels = new List<FieldValueFileViewModel>();
            var allActivitiesFromFlow = Flow.ProcessVersion.Activities.Where(a => a.Type == Enums.WorkflowActivityTypeEnum.USER_TASK_ACTIVITY).ToList();

            foreach (var itemFieldValueFiles in FieldsValues.Where(x => x.Field.Type == Enums.FieldTypeEnum.FILE).Select(x => x.FieldValueFiles))
            {
                var canViewFiles = itemFieldValueFiles.Select(x => x.FieldValue.Field)
                    .Where(a => a.ActivityFields.Any(af => af.State != Enums.ProcessTaskFieldStateEnum.INVISIBLE && af.Field.Type == Enums.FieldTypeEnum.FILE &&
                    (
                        af.Activity.ActivityUser.ExecutorType == Enums.UserTaskExecutorTypeEnum.PERSON && af.Activity.ActivityUser.PersonId == userId ||
                        af.Activity.ActivityUser.ExecutorType == Enums.UserTaskExecutorTypeEnum.ROLE && currentUserRoleIds.Contains(af.Activity.ActivityUser.RoleId ?? 0) ||
                        af.Activity.ActivityUser.ExecutorType == Enums.UserTaskExecutorTypeEnum.REQUESTER && Flow.RequesterId == userId
                    )));

                if (canViewFiles.Any())
                {
                    foreach (var fieldValueFile in itemFieldValueFiles)
                    {
                        fieldValueFileViewModels.Add(fieldValueFile.AsFieldValueFileViewModel(userViewModels));
                    }
                }
            }

            return new TaskDetailsViewModel
            {
                TaskId = Id,
                TaskType = Activity.Type,
                ActivityName = Activity.Name,
                FlowId = FlowId,
                ProcessVersionId = Flow.ProcessVersionId,
                ProcessName = Flow.ProcessVersion.Name,
                DescriptionFlow = Flow.ProcessVersion.DescriptionFlow,
                CreatedDate = Flow.CreatedDate,
                ExecutorId = ExecutorId,
                ExecutorName = ExecutorId == null ? "" : userViewModels?.FirstOrDefault(u => u.Id == ExecutorId)?.Name,
                RequesterId = Flow.RequesterId,
                RequesterName = userViewModels?.FirstOrDefault(u => u.Id == Flow.RequesterId)?.Name,
                Finished = FinishedDate != null,
                YouRequest = Flow.RequesterId == userId,
                YouNeedToRun = FinishedDate == null && ExecutorId == userId,
                YouCanAssociate = Activity.ActivityUser?.ExecutorType == Enums.UserTaskExecutorTypeEnum.ROLE && currentUserRoleIds.Contains(Activity.ActivityUser?.RoleId ?? 0),
                YouWillRun = allActivitiesFromFlow.Any
                    (a =>
                            (
                                (a.ActivityUser?.ExecutorType == Enums.UserTaskExecutorTypeEnum.PERSON && Activity.ActivityUser?.PersonId == userId)
                                || (a.ActivityUser?.ExecutorType == Enums.UserTaskExecutorTypeEnum.REQUESTER && Flow.RequesterId == userId)
                            )
                            && Flow.Tasks.All(t => t.ActivityId != a.Id || t.FinishedDate == null)
                    ),

                YourRoleWillTask = FinishedDate == null
                    && allActivitiesFromFlow.Any
                    (a =>
                        Flow.Tasks.All(t => t.ActivityId != a.Id || t.FinishedDate == null)
                        && a.ActivityUser?.ExecutorType == Enums.UserTaskExecutorTypeEnum.ROLE
                        && currentUserRoleIds.Contains(a.ActivityUser.RoleId ?? 0)
                    ),
                YouHaveRunTask = Flow.Tasks.Any
                    (t =>
                            t.FinishedDate != null
                            && (
                                    (t.Activity.ActivityUser?.ExecutorType == Enums.UserTaskExecutorTypeEnum.PERSON && t.Activity.ActivityUser?.PersonId == userId)
                                    || (t.Activity.ActivityUser?.ExecutorType == Enums.UserTaskExecutorTypeEnum.REQUESTER && Flow?.RequesterId == userId)
                                )
                    ),
                YourRolePerformedTask = Flow.Tasks.Any
                    (t =>
                            t.FinishedDate != null
                            && t.Activity.ActivityUser?.ExecutorType == Enums.UserTaskExecutorTypeEnum.ROLE
                            && currentUserRoleIds.Contains(t.Activity.ActivityUser.RoleId ?? 0)
                    ),
                ExecutorType = Activity.ActivityUser?.ExecutorType,
                CurrentRoleName = Activity.ActivityUser?.Role?.Name,
                FinishedDate = Flow.FinishedDate,
                FieldValueFiles = fieldValueFileViewModels
            };
        }
    }
}
