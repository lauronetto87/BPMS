using Newtonsoft.Json;
using Satelitti.Model;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Models.Infos
{
    public class FieldValueInfo : BaseInfo
    {
        #region Properties
        public string FieldValue { get; set; }
        #endregion

        #region Relationship
        public int FlowId { get; set; }
        [JsonIgnore]
        public FlowInfo Flow { get; set; }

        public int TaskId { get; set; }
        [JsonIgnore]
        public TaskInfo Task { get; set; }

        public int FieldId { get; set; }
        [JsonIgnore]
        public FieldInfo Field { get; set; }

        [JsonIgnore]
        public List<FieldValueFileInfo> FieldValueFiles { get; set; }

        [JsonIgnore]
        public List<FieldValueFileInfo> UploadedFieldValueFiles { get; set; }
        #endregion

        public FieldValueInfo AsReplicatedNewFieldValueInfo(int nextTaskId, Dictionary<int, TaskSignerInfo> signerTasksCloned)
        {
            return new FieldValueInfo()
            {
                FieldValue = FieldValue,
                FlowId = FlowId,
                TaskId = nextTaskId,
                FieldId = FieldId,
                TenantId = TenantId,
                FieldValueFiles = FieldValueFiles?.Select(f => f.AsReplicatedNewFieldValueFileInfo(signerTasksCloned)).ToList(),
            };
        }
    }
}
