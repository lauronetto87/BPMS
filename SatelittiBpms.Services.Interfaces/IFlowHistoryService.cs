using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.ViewModel;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface IFlowHistoryService
    {
        Task<ResultContent<FlowHistoryViewModel>> Get(int flowId);
    }
}