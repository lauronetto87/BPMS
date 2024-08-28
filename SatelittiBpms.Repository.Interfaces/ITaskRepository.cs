using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Repository.Interfaces
{
    public interface ITaskRepository : IRepositoryBase<TaskInfo>
    {
        IQueryable<TaskInfo> ListByTenantFilters(TaskFilterDTO filters, int userId);
        TaskInfo GetDetailsById(int id);
    }
}
