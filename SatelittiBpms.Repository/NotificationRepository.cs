using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Repository
{
    public class NotificationRepository : AbstractRepositoryBase<NotificationInfo>, INotificationRepository
    {
        public NotificationRepository(DbContext context) : base(context)
        {
        }

        public List<NotificationInfo> GetToUser(int userId)
        {
            return GetQuery(x => !x.Deleted && x.UserId == userId)
                    .Include(x => x.Task.Activity)
                    .Include(x => x.Flow.ProcessVersion)
                    .Include(x => x.Role)
                    .OrderByDescending(x => x.Date)
                    .ToList();
        }
    }
}
