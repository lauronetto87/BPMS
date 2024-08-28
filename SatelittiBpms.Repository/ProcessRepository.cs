using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Repository
{
    public class ProcessRepository : AbstractRepositoryBase<ProcessInfo>, IProcessRepository
    {
        public ProcessRepository(DbContext context) : base(context)
        {
        }

        public async override Task<ProcessInfo> GetByIdAndTenantId(int id, long tenantId)
        {
            var query = GetByTenantIncludingRelationship(tenantId);
            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public IQueryable<ProcessInfo> GetByTenantIncludingRelationship(long tenantId)
        {
            return GetByTenant(tenantId).Include(x => x.ProcessVersions).ThenInclude(x => x.ProcessVersionRoles).ThenInclude(x => x.Role).ThenInclude(x => x.RoleUsers);
        }

        public async override Task<List<ProcessInfo>> ListAsync()
        {
            return await _dbSet.Include(x => x.ProcessVersions).ToListAsync();
        }

        public List<ProcessInfo> List()
        {
            return _dbSet.Include(x => x.ProcessVersions).ThenInclude(x => x.Flows).ToList();
        }
    }
}
