using Newtonsoft.Json;
using Satelitti.Model;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Models.Infos
{
    public class FlowInfo : BaseInfo
    {
        #region Properties
        public DateTime CreatedDate { get; set; }
        public DateTime? FinishedDate { get; set; }
        public FlowStatusEnum Status { get; set; }
        #endregion

        #region Relationship
        public int ProcessVersionId { get; set; }
        [JsonIgnore]
        public ProcessVersionInfo ProcessVersion { get; set; }

        public int RequesterId { get; set; }
        [JsonIgnore]
        public UserInfo Requester { get; set; }

        public IList<FieldValueInfo> FieldValues { get; set; }
        public IList<FlowPathInfo> FlowPaths { get; set; }
        public IList<TaskInfo> Tasks { get; set; }
        public IList<NotificationInfo> Notifications { get; set; }
        #endregion

        public FlowViewModel AsListingViewModel(IList<SuiteUserViewModel>? userViewModel, int UserLoggedId)
        {
            var lastTask = Tasks.LastOrDefault();
            var executorId = Tasks.LastOrDefault(t => t.FinishedDate == null)?.ExecutorId;
            var roleUser = lastTask.Activity.ActivityUser?.Role?.RoleUsers.Any(ru => ru.UserId == UserLoggedId);

            return new FlowViewModel
            {
                Id = lastTask.Id,
                FlowId = Id,
                Name = ProcessVersion.Name,
                ActivityId = lastTask.Activity.Id,
                Description = ProcessVersion.Description,
                DescriptionFlow = ProcessVersion.DescriptionFlow,
                CreationDate = CreatedDate,
                CreatedByUserName = ProcessVersion.CreatedByUserName,
                CreatedByUserId = ProcessVersion.CreatedByUserId,
                ProcessStatus = ProcessVersion.Status,
                ExecutorId = executorId,
                ExecutorType = lastTask.Activity?.ActivityUser?.ExecutorType,
                isMyRole = roleUser,
                Finished = Status == FlowStatusEnum.FINISHED,
                ProcessVersionId = ProcessVersionId,
                ProcessVersionName = ProcessVersion.Name,
                CurrentRoleName = lastTask.Activity.ActivityUser?.Role?.Name,
                ActivityName = lastTask.Activity.Name,
                ExecutorName = executorId > 0 ? userViewModel?.FirstOrDefault(u => u.Id == executorId).Name : "",
            };
        }

        public FlowHistoryViewModel AsHistoryViewModel(IList<SuiteUserViewModel> userViewModel)
        {
            return new FlowHistoryViewModel
            {
                DiagramContent = ProcessVersion.DiagramContent,
                FlowDescription = ProcessVersion.DescriptionFlow,
                FlowId = Id,
                ProcessName = ProcessVersion.Name,
                FlowHistoryTasks = Tasks.Select(
                    t => new FlowHistoryTaskViewModel
                    {

                        ActivityType = t.Activity.Type,
                        CreatedDatetime = t.CreatedDate,
                        FinishedDatetime = t.FinishedDate,
                        TaskName = t.Activity.Name,
                        ExecutorName = NameExecutor(t, userViewModel),
                        ActionDescription = ActionDescription(t),
                    }
                ).OrderByDescending(x => x.CreatedDatetime).ToList(),
            };
        }

        private static string NameExecutor(TaskInfo task, IList<SuiteUserViewModel> userViewModel)
        {
            return task.Activity.Type switch
            {
                WorkflowActivityTypeEnum.START_EVENT_ACTIVITY => userViewModel.First(u => u.Id == task.Flow.RequesterId).Name,
                WorkflowActivityTypeEnum.USER_TASK_ACTIVITY => task.ExecutorId.HasValue && task.ExecutorId > 0 ? userViewModel.First(u => u.Id == task.ExecutorId).Name : "",
                _ => "flows.flowHistory.table.labels.executorSystem",
            };
        }

        private static string ActionDescription(TaskInfo task)
        {
            return task.Activity.Type switch
            {
                WorkflowActivityTypeEnum.START_EVENT_ACTIVITY => "flows.flowHistory.table.labels.request",
                WorkflowActivityTypeEnum.END_EVENT_ACTIVITY => "flows.flowHistory.table.labels.finished",
                WorkflowActivityTypeEnum.USER_TASK_ACTIVITY => task.Option?.Description,
                _ => "flows.flowHistory.table.labels.system",
            };
        }
    }
}
