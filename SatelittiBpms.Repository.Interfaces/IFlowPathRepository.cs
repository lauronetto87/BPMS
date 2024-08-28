using SatelittiBpms.Models.Infos;
using System.Threading.Tasks;

namespace SatelittiBpms.Repository.Interfaces
{
    public interface IFlowPathRepository : IRepositoryBase<FlowPathInfo>
    {
        Task<FlowPathInfo> getFlowPathInfoByTargetTaskId(int targetTaskId);
    }
}
