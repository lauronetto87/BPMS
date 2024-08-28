using AutoMapper;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class ProcessRoleService : AbstractServiceBase<ProcessRoleDTO, ProcessVersionRoleInfo, IProcessRoleRepository>, IProcessRoleService
    {
        public ProcessRoleService(IProcessRoleRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }

        public async Task InsertMany(List<int> rolesIds, int processVersionId, int tenantId)
        {
            if (rolesIds == null)
                return;

            foreach (var rolesId in rolesIds)
            {
                await _repository.Insert(new ProcessVersionRoleInfo()
                {
                    RoleId = rolesId,
                    ProcessVersionId = processVersionId,
                    TenantId = tenantId
                });
            }
        }
    }
}
