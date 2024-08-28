using Satelitti.Model;
using SatelittiBpms.Models.Enums;
using System.Text.Json.Serialization;

namespace SatelittiBpms.Models.Infos
{
    public class ActivityNotificationInfo : BaseInfo
    {
        #region Properties   
        public SendTaskDestinataryTypeEnum DestinataryType { get; set; }
        public string TitleMessage { get; set; }
        public string Message { get; set; }
        #endregion

        #region Relantionship        
        public ActivityInfo Activity { get; set; }

        public int? RoleId { get; set; }

        [JsonIgnore]
        public RoleInfo Role { get; set; }

        public int? PersonId { get; set; }

        public string CustomEmail { get; set; }

        #endregion

    }
}
