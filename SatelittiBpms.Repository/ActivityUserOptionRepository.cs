using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;

namespace SatelittiBpms.Repository
{
    public class ActivityUserOptionRepository : AbstractRepositoryBase<ActivityUserOptionInfo>, IActivityUserOptionRepository
    {
        public ActivityUserOptionRepository(DbContext context) : base(context)
        {
        }
    }
}
