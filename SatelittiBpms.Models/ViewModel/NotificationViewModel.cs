using SatelittiBpms.Models.Enums;
using System;

namespace SatelittiBpms.Models.ViewModel
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public bool Read { get; set; }
        public DateTime Date { get; set; }
        public NotificationTypeEnum Type { get; set; }
        public int? TaskId { get; set; }
        public int? RoleId { get; set; }
        public int FlowId { get; set; }
        public string TaskName { get; set; }
        public string RoleName { get; set; }
        public string ProcessName { get; set; }

    }
}
