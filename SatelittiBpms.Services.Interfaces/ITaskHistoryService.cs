using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface ITaskHistoryService : IServiceBase<TaskHistoryDTO, TaskHistoryInfo>
    {
        Task<int> Insert(int taskId, int executorId);
        Task UnassignHistory(int taskId, int executorId);
        Task UpdateEndDateFromTaskHistories(int taskId, int tenantId);
    }
}
