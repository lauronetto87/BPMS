using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface IRoleService : IServiceBase<RoleDTO, RoleInfo>
    {
        Task<ResultContent<bool>> VerifyRoleNameExists(string roleName, int? tenantId = null, int? roleId = null);
        Task<ResultContent> InsertWithRelationship(RoleDTO role);
        Task<ResultContent> ListByTenant(Dictionary<string, string> pFilters = null);
        Task<ResultContent> ListByTenantToConfig(Dictionary<string, string> pFilters = null);
        Task<ResultContent> DeleteMany(List<int> rolesToDelete);
        Task<ResultContent> GetByTenant(int id);
        Task<ResultContent> UpdateWithRelationship(int id, RoleDTO role);
        Task<ResultContent<RoleInfo>> Get(int id, int tenantId);
    }
}
