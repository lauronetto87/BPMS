using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using System.Threading.Tasks;

namespace SatelittiBpms.Repository.Interfaces
{
    public interface IProcessVersionRepository : IRepositoryBase<ProcessVersionInfo>
    {
        Task<ProcessVersionInfo> GetByProcessAndStatus(int processId, ProcessStatusEnum status, int tenantId);
        Task<ProcessVersionInfo> GetByProcessAndVersionAndTenant(int processId, int version, int tenantId);
        Task UpdateStatusAndWorkflowContent(int processVersionId, ProcessStatusEnum status, string workflowJsonStirng);
        Task<ProcessVersionInfo> GetLastPublishedProcessVersion(int processId, long tenantId);
    }
}
