using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Repository
{
    public class ActivityRepository : AbstractRepositoryBase<ActivityInfo>, IActivityRepository
    {
        public ActivityRepository(DbContext context) : base(context)
        { }
        public async override Task<ActivityInfo> GetByIdAndTenantId(int id, long tenantId)
        {
            var query = GetByTenantIncludingRelationship(tenantId);
            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        private IQueryable<ActivityInfo> GetByTenantIncludingRelationship(long tenantId)
        {
            return GetByTenant(tenantId)
                .Include(x => x.ActivityNotification)
                .Include(x => x.ActivityFields)
                .Include(x => x.Tasks).ThenInclude(x => x.FieldsValues).ThenInclude(x => x.Field)
                .Include(x => x.Tasks).ThenInclude(x => x.Flow).ThenInclude(x => x.ProcessVersion);
        }
    }
}
