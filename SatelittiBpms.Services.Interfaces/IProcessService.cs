using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface IProcessService : IServiceBase<ProcessDTO, ProcessInfo>
    {
        Task<ResultContent<List<ProcessListiningViewModel>>> ListProcessListViewModel(ProcessFilterDTO filters);
        Task<ResultContent<ProcessInfo>> GetByTenant(int processId, int tenantId = 0);
        Task UpdateCurrentVersion(int processId, int tenantId = 0);
        Task UpdateTaskSequance(int processId, int taskSequance);
        List<string> ListWorkFlows();
        Task<ResultContent<ProcessCounterViewModel>> GetCounterProcess(ProcessFilterDTO filters);
        Task<ResultContent> ListToFilters();
    }
}
