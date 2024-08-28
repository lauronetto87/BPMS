using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using System.Threading.Tasks;

namespace SatelittiBpms.Repository
{
    public class RoleUserRepository : AbstractRepositoryBase<RoleUserInfo>, IRoleUserRepository
    {
        public RoleUserRepository(DbContext context) : base(context)
        {
        }
        public async Task<RoleUserInfo> GetDefaultByUserAndTenant(int tenantId, int defaultRoleId, int userId)
        {
            var query = GetByTenant(tenantId);
            return await query.FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == defaultRoleId);
        }
    }
}
