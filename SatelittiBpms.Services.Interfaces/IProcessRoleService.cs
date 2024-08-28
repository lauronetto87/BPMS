using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface IProcessRoleService : IServiceBase<ProcessRoleDTO, ProcessVersionRoleInfo>
    {
        Task InsertMany(List<int> rolesIds, int processVersionId, int tenantId);
    }
}
