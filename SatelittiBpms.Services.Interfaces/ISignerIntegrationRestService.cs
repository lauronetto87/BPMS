using SatelittiBpms.Models.DTO.Integration.Signer;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface ISignerIntegrationRestService
    {
        Task<TaskSignerInfo> CreateEnvelope(IntegrationEnvelopeDTO integrationEnvelope, List<SignerIntegrationEnvelopeFileDTO> filesSend, TenantInfo tenantInfo);
        Task<FileViewModel> DownloadFile(int signerFileId, SignerEnvelopeFileSuffixEnum fileType, int tenantId);
    }
}
