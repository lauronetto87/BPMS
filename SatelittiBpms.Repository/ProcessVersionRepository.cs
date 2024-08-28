using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Repository
{
    public class ProcessVersionRepository : AbstractRepositoryBase<ProcessVersionInfo>, IProcessVersionRepository
    {
        public ProcessVersionRepository(DbContext context) : base(context)
        {
        }

        public override Task<ProcessVersionInfo> GetByIdAndTenantId(int processVersionId, long tenantId)
        {
            return GetByTenantIncludingRelationship(tenantId).FirstOrDefaultAsync(x => x.Id == processVersionId);
        }

        public async Task<ProcessVersionInfo> GetLastPublishedProcessVersion(int processId, long tenantId)
        {
            var query = GetByTenant(tenantId);
            return await query.Where(x => x.ProcessId == processId && x.Status == ProcessStatusEnum.PUBLISHED).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        }

        public async Task<ProcessVersionInfo> GetByProcessAndVersionAndTenant(int processId, int version, int tenantId)
        {
            var query = GetByTenant(tenantId);
            return await query.FirstOrDefaultAsync(x => x.ProcessId == processId && x.Version == version);
        }

        public async Task<ProcessVersionInfo> GetByProcessAndStatus(int processId, ProcessStatusEnum status, int tenantId)
        {
            var query = GetByTenantIncludingRelationship(tenantId);
            return await query.FirstOrDefaultAsync(x => x.ProcessId == processId && x.Status == status);
        }

        public async Task UpdateStatusAndWorkflowContent(int processVersionId, ProcessStatusEnum status, string workflowJsonStirng)
        {
            var processVersion = await Get(processVersionId);
            if (processVersion.Status == ProcessStatusEnum.EDITING && status == ProcessStatusEnum.PUBLISHED)
            {
                processVersion.PublishedDate = DateTime.UtcNow;
            }
            processVersion.Status = status;
            processVersion.WorkflowContent = workflowJsonStirng;
            await base.Update(processVersion);
        }

        private IQueryable<ProcessVersionInfo> GetByTenantIncludingRelationship(long tenantId)
        {
            return GetByTenant(tenantId)
                .Include(x => x.ProcessVersionRoles).ThenInclude(x => x.Role)
                .Include(x => x.Activities).ThenInclude(x => x.ActivityUser)
                .Include(x => x.Activities).ThenInclude(x => x.SignerIntegrationActivity).ThenInclude(x => x.Authorizers).ThenInclude(x => x.OriginActivity)
                .Include(x => x.Activities).ThenInclude(x => x.SignerIntegrationActivity).ThenInclude(x => x.Signatories).ThenInclude(x => x.OriginActivity)
                .Include(x => x.Activities).ThenInclude(x => x.SignerIntegrationActivity).ThenInclude(x => x.Files)
                .Include(x => x.ActivityFields).ThenInclude(x => x.Field)
                .Include(x => x.Process);
        }
    }
}
