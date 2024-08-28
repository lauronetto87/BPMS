using Satelitti.Model;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SatelittiBpms.Models.Infos
{
    public class ActivityUserOptionInfo : BaseInfo
    {
        #region Properties
        public string Description { get; set; }
        #endregion

        #region Relantionship        
        public int ActivityUserId { get; set; }
        [JsonIgnore]
        public ActivityUserInfo ActivityUser { get; set; }
        public IList<TaskInfo> OptionTasks { get; set; }
        
        #endregion   
    }
}
