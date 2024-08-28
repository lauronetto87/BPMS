using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface IFlowPathService : IServiceBase<FlowPathDTO, FlowPathInfo>
    {
        Task<ResultContent<int>> Insert(FlowPathInfo info);

        int getPreviousTaskId(int taskId);
    }
}
