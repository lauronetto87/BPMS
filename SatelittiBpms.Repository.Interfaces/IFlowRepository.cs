using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using System.Linq;

namespace SatelittiBpms.Repository.Interfaces
{
    public interface IFlowRepository : IRepositoryBase<FlowInfo>
    {
        IQueryable<FlowInfo> ListByTenantFilters(TaskFilterDTO filters, int[] roleIds, int userId);
    }
}
