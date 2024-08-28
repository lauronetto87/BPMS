using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;

namespace SatelittiBpms.Repository
{
    public class ActivityUserRepository : AbstractRepositoryBase<ActivityUserInfo>, IActivityUserRepository
    {
        public ActivityUserRepository(DbContext context) : base(context)
        {
        }
    }
}
