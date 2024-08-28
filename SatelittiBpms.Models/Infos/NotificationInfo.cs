using Newtonsoft.Json;
using Satelitti.Model;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.ViewModel;
using System;

namespace SatelittiBpms.Models.Infos
{
    public class NotificationInfo : BaseInfo
    {
        #region Properties

        public bool Read { get; set; }

        public DateTime Date { get; set; }

        public bool Deleted { get; set; }

        public NotificationTypeEnum Type { get; set; }

        #endregion


        #region Relationship

        [JsonIgnore]
        public UserInfo User { get; set; }
        public int UserId { get; set; }

        [JsonIgnore]
        public TaskInfo Task { get; set; }
        public int? TaskId { get; set; }

        public FlowInfo Flow { get; set; }
        public int FlowId { get; set; }

        public RoleInfo Role { get; set; }
        public int? RoleId { get; set; }

        public NotificationViewModel AsViewModel()
        {
            return new NotificationViewModel
            {
                Id = Id,
                Date = Date,
                Type = Type,
                ProcessName = Flow.ProcessVersion.Name,
                FlowId = FlowId,
                TaskId = TaskId,
                Read = Read,
                TaskName = Task?.Activity?.Name,
                RoleName = Role?.Name,
                RoleId = RoleId,
            };
        }

        #endregion

    }
}
