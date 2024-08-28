using SatelittiBpms.Models.Enums;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SatelittiBpms.Models.ViewModel
{
    public class TaskDetailsViewModel
    {
        public int TaskId { get; set; }
        public WorkflowActivityTypeEnum TaskType { get; set; }
        public string ActivityName { get; set; }
        public int FlowId { get; set; }
        public int ProcessVersionId { get; set; }
        public string ProcessName { get; set; }
        public string DescriptionFlow { get; set; }
        public int? ExecutorId { get; set; }
        public string ExecutorName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? RequesterId { get; set; }
        public string RequesterName { get; set; }
        public bool Finished { get; set; }
        public UserTaskExecutorTypeEnum? ExecutorType { get; set; }
        public string CurrentRoleName { get; set; }
        public DateTime? FinishedDate { get; set; }
        public bool YouRequest { get; set; }
        public bool YouNeedToRun { get; set; }
        public bool YouCanAssociate { get; set; }
        public bool YouWillRun { get; set; }
        public bool YourRoleWillTask { get; set; }
        public bool YouHaveRunTask { get; set; }
        public bool YourRolePerformedTask { get; set; }
        public IList<FieldValueFileViewModel> FieldValueFiles { get; set; }
    }
}
