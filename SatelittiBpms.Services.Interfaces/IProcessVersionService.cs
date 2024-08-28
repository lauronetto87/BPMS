using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface IProcessVersionService : IServiceBase<ProcessVersionDTO, ProcessVersionInfo>
    {
        Task<ResultContent> GetByTenant(int processVersionId);
        Task<ResultContent> Save(ProcessVersionDTO processVersion);
        Task<ResultContent> UpdateWorkFlowContent(int Id, string workflowContent);
        ResultContent IsNameValidCheckDuplicate(string processName, int editProcessVersionId);
    }
}
