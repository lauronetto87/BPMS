using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface IRoleUserService : IServiceBase<RoleUserDTO, RoleUserInfo>
    {
        Task<ResultContent> InsertUserDefaultRole(int tenantId, int userId);
        Task<ResultContent> RemoveUserDefaultRole(int tenantId, int userId);
        Task<List<int>> GetRulesIdByUser(int userId);
    }
}
