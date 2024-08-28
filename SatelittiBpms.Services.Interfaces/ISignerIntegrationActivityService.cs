using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface ISignerIntegrationActivityService : IServiceBase<SignerIntegrationActivityDTO, SignerIntegrationActivityInfo>
    {
        Task InsertMany(IList<SignerIntegrationActivityDTO> signerTasks, ProcessVersionInfo processVersion, Models.BpmnIo.Definitions bpmnDefinitions);
    }
}
