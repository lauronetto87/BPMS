using SatelittiBpms.Models.Infos;
using System.Collections.Generic;

namespace SatelittiBpms.Repository.Interfaces
{
    public interface INotificationRepository : IRepositoryBase<NotificationInfo>
    {
        public List<NotificationInfo> GetToUser(int userId);
    }
}
