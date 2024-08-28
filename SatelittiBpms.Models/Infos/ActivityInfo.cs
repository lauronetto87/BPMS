using System.Collections.Generic;
using Newtonsoft.Json;
using Satelitti.Model;
using SatelittiBpms.Models.Enums;

namespace SatelittiBpms.Models.Infos
{
    public class ActivityInfo : BaseInfo
    {
        #region Properties
        public string ComponentInternalId { get; set; }
        public string Name { get; set; }
        public WorkflowActivityTypeEnum Type { get; set; }
        #endregion

        #region Relationships
        public ActivityUserInfo ActivityUser { get; set; }

        public ActivityNotificationInfo ActivityNotification { get; set; }

        public int ProcessVersionId { get; set; }
        [JsonIgnore]
        public ProcessVersionInfo ProcessVersion { get; set; }

        public IList<ActivityFieldInfo> ActivityFields { get; set; }
        public IList<TaskInfo> Tasks { get; set; }

        public SignerIntegrationActivityInfo SignerIntegrationActivity { get; set; }

        public IList<SignerIntegrationActivityAuthorizerInfo> SignerIntegrationActivityAuthorizerOrigin { get; set; }

        public IList<SignerIntegrationActivitySignatoryInfo> SignerIntegrationActivitySignatoryOrigin { get; set; }
        #endregion

    }
}
