using SatelittiBpms.Models.Infos;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Repository.Interfaces
{
    public interface IProcessRepository : IRepositoryBase<ProcessInfo>
    {
        List<ProcessInfo> List();
        IQueryable<ProcessInfo> GetByTenantIncludingRelationship(long tenantId);
    }
}
