using SatelittiBpms.Models.Infos;
using System.Threading.Tasks;

namespace SatelittiBpms.Repository.Interfaces
{
    public interface ITaskHistoryRepository : IRepositoryBase<TaskHistoryInfo>
    {
        Task<TaskHistoryInfo> GetLastByTask(int tenantId, int taskId, int? executorId = null);
    }
}
