using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Repository
{
    public class TaskHistoryRepository : AbstractRepositoryBase<TaskHistoryInfo>, ITaskHistoryRepository
    {
        public TaskHistoryRepository(DbContext context) : base(context)
        {
        }
        public async Task<TaskHistoryInfo> GetLastByTask(int tenantId, int taskId, int? executorId = null)
        {
            var query = base.GetByTenant(tenantId);

            return await query.Where(x => x.TaskId == taskId && (x.ExecutorId == executorId || executorId == null) && x.EndDate == null).OrderBy(x => x.Id).LastOrDefaultAsync();
        }
    }
}
