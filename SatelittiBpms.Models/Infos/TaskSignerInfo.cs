using Newtonsoft.Json;
using Satelitti.Model;
using SatelittiBpms.Models.Enums;
using System;
using System.Collections.Generic;

namespace SatelittiBpms.Models.Infos
{
    public class TaskSignerInfo : BaseInfo
    {
        #region Properties
        public DateTime DateSendEvelope { get; set; }
        public int EnvelopeId { get; set; }
        public TaskSignerStatusEnum Status { get; set; }
        #endregion

        #region Relationship
        public int TaskId { get; set; }
        [JsonIgnore]
        public TaskInfo Task { get; set; }

        public IList<TaskSignerFileInfo> Files { get; set; }

        public TaskSignerInfo AsReplicatedNewInfo(int taskId)
        {
            return new TaskSignerInfo
            {
                DateSendEvelope = DateSendEvelope,
                EnvelopeId = EnvelopeId,
                Status = Status,
                TenantId = TenantId,
                TaskId = taskId,
            };
        }
        #endregion
    }
}
