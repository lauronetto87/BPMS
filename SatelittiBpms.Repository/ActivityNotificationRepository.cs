using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;

namespace SatelittiBpms.Repository
{
    public class ActivityNotificationRepository : AbstractRepositoryBase<ActivityNotificationInfo>, IActivityNotificationRepository
    {
        public ActivityNotificationRepository(DbContext context) : base(context)
        {
        }
    }
}
