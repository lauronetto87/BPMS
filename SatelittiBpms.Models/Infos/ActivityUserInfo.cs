using Satelitti.Model;
using SatelittiBpms.Models.Enums;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SatelittiBpms.Models.Infos
{
    public class ActivityUserInfo : BaseInfo
    {
        #region Properties
        public UserTaskExecutorTypeEnum ExecutorType { get; set; }
        #endregion

        #region Relantionship        
        public ActivityInfo Activity { get; set; }

        public int? RoleId { get; set; }
        [JsonIgnore]
        public RoleInfo Role { get; set; }

        public int? PersonId { get; set; }
        [JsonIgnore]
        public UserInfo Person { get; set; }

        public IList<ActivityUserOptionInfo> ActivityUsersOptions { get; set; }
        #endregion   
    }
}
