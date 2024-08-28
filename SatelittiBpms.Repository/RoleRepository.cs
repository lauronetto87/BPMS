using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Repository
{
    public class RoleRepository : AbstractRepositoryBase<RoleInfo>, IRoleRepository
    {
        public RoleRepository(DbContext context) : base(context)
        {
        }

        public async override Task<RoleInfo> GetByIdAndTenantId(int id, long tenantId)
        {
            var query = GetByTenantIncludingRelationship(tenantId);
            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        private IQueryable<RoleInfo> GetByTenantIncludingRelationship(long tenantId)
        {
            return GetByTenant(tenantId).Include(x => x.RoleUsers);
        }
    }
}
