using Satelitti.Model;
using SatelittiBpms.Models.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Models.Infos
{
    public class RoleInfo : BaseInfo
    {
        #region Properties
        public string Name { get; set; }
        #endregion

        #region Relationship
        public IList<RoleUserInfo> RoleUsers { get; set; }
        public IList<ProcessVersionRoleInfo> ProcessUsers { get; set; }
        public IList<ActivityUserInfo> ActivityUsers { get; set; }
        public IList<ActivityNotificationInfo> ActivityNotifications { get; set; }
        public IList<NotificationInfo> Notifications { get; set; }
        #endregion

        public RoleViewModel AsViewModel() => new RoleViewModel
        {
            Name = Name,
            Id = Id
        };

        public RoleDetailViewModel AsDetailViewModel() => new RoleDetailViewModel
        {
            Name = Name,
            Id = Id,
            UsersIds = RoleUsers.Select(x => x.UserId).ToList()
        };
    }
}
