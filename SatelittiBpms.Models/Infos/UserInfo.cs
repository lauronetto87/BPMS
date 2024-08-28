using Satelitti.Authentication.Model;
using Satelitti.Model;
using SatelittiBpms.Models.Enums;
using System.Collections.Generic;

namespace SatelittiBpms.Models.Infos
{
    public class UserInfo : BaseInfo
    {
        #region Properties
        public BpmsUserTypeEnum Type { get; set; }
        public bool Enable { get; set; }
        public float? Timezone { get; set; }
        #endregion

        #region Relationship
        public IList<RoleUserInfo> RoleUsers { get; set; }
        public IList<FlowInfo> RequesterFlows { get; set; }
        public IList<TaskHistoryInfo> ExecutorTaskHistories { get; set; }
        public IList<TaskInfo> ExecutorTasks { get; set; }
        public IList<NotificationInfo> Notifications { get; set; }
        #endregion

        public static UserInfo AsBpmsAdminUser(SuiteUser suiteUser)
        {
            return new UserInfo
            {
                Id = suiteUser.Id,
                Enable = true,
                TenantId = suiteUser.Tenant,
                Type = BpmsUserTypeEnum.ADMINISTRATOR,
                Timezone = suiteUser.Timezone
            };
        }
    }
}
