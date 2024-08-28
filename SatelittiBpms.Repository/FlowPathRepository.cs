using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using System.Threading.Tasks;

namespace SatelittiBpms.Repository
{
    public class FlowPathRepository : AbstractRepositoryBase<FlowPathInfo>, IFlowPathRepository
    {
        public FlowPathRepository(DbContext context) : base(context)
        {
        }

        public Task<FlowPathInfo> getFlowPathInfoByTargetTaskId(int targetTaskId)
        {
            return GetQuery(x => x.TargetTaskId == targetTaskId).FirstOrDefaultAsync();
        }
    }
}
