using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;

namespace SatelittiBpms.Repository
{
    public class ProcessRoleRepository : AbstractRepositoryBase<ProcessVersionRoleInfo>, IProcessRoleRepository
    {
        public ProcessRoleRepository(DbContext context) : base(context)
        {
        }
    }
}
