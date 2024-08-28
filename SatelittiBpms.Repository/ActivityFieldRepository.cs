using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;

namespace SatelittiBpms.Repository
{
    public class ActivityFieldRepository : AbstractRepositoryBase<ActivityFieldInfo>, IActivityFieldRepository
    {
        public ActivityFieldRepository(DbContext context) : base(context)
        {
        }
    }
}
