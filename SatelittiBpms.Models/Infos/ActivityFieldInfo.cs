using SatelittiBpms.Models.Enums;
using Newtonsoft.Json;
using Satelitti.Model;

namespace SatelittiBpms.Models.Infos
{
    public class ActivityFieldInfo : BaseInfo
    {
        #region Properties
        public ProcessTaskFieldStateEnum State { get; set; }
        #endregion

        #region Relantionship
        public int ProcessVersionId { get; set; }
        [JsonIgnore]
        public ProcessVersionInfo ProcessVersion { get; set; }

        public int ActivityId { get; set; }
        [JsonIgnore]
        public ActivityInfo Activity { get; set; }

        public int FieldId { get; set; }
        [JsonIgnore]
        public FieldInfo Field { get; set; }
        #endregion        
    }
}
