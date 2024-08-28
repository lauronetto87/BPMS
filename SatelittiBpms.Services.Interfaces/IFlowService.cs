using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface IFlowService : IServiceBase<FlowDTO, FlowInfo>
    {
        Task<ResultContent<string>> Request(FlowRequestDTO flowRequestDTO);
        Task<ResultContent<int>> Insert(FlowInfo info);
        Task<ResultContent> ListAll(TaskFilterDTO filters);
        Task<List<FlowGroupViewModel>> GetAllTaskGroup(TaskFilterDTO filters);
        Task<ResultContent<int>> CountAll(TaskFilterDTO filters);
        bool ContainsFlowForProcessVersion(int id);
        List<FieldInfo> GetFields(int flowId);
    }
}