using Newtonsoft.Json;
using Satelitti.Model;
using System.Collections.Generic;

namespace SatelittiBpms.Models.Infos
{
    public class TaskSignerFileInfo : BaseInfo
    {
        #region Properties
        public int SignerId { get; set; }
        #endregion

        #region Relationship
        public int FieldValueFileId { get; set; }
        [JsonIgnore]
        public FieldValueFileInfo FieldValueFile { get; set; }

        public int TaskSignerId { get; set; }
        [JsonIgnore]
        public TaskSignerInfo TaskSigner { get; set; }

        #endregion

        internal TaskSignerFileInfo AsReplicatedNewInfo(Dictionary<int, TaskSignerInfo> signerTasksCloned)
        {
            return new TaskSignerFileInfo
            {
                SignerId = SignerId,
                TenantId = TenantId,
                TaskSignerId = signerTasksCloned[TaskSignerId].Id,
            };
        }
    }
}
