using Newtonsoft.Json;
using Satelitti.Model;

namespace SatelittiBpms.Models.Infos
{
    public class FlowPathInfo : BaseInfo
    {
        #region Relationship
        public int FlowId { get; set; }
        [JsonIgnore]
        public FlowInfo Flow { get; set; }

        public int SourceTaskId { get; set; }
        [JsonIgnore]
        public TaskInfo SourceTask { get; set; }

        public int TargetTaskId { get; set; }
        [JsonIgnore]
        public TaskInfo TargetTask { get; set; }
        #endregion
    }
}
