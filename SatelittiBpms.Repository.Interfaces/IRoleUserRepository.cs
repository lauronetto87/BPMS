using SatelittiBpms.Models.Infos;
using System.Threading.Tasks;

namespace SatelittiBpms.Repository.Interfaces
{
    public interface IRoleUserRepository : IRepositoryBase<RoleUserInfo>
    {
        Task<RoleUserInfo> GetDefaultByUserAndTenant(int tenantId, int defaultRoleId, int userId);
    }
}
