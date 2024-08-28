using SatelittiBpms.Models.DTO.Integration.Signer;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Integration.Mock
{
    public class MockSignerIntegrationRestService : ISignerIntegrationRestService
    {
        public Task<TaskSignerInfo> CreateEnvelope(IntegrationEnvelopeDTO integrationEnvelope, List<SignerIntegrationEnvelopeFileDTO> filesSend, TenantInfo tenantInfo)
        {
            var filesInfo = new List<TaskSignerFileInfo>();

            var taskSignerInfo = new TaskSignerInfo
            {
                DateSendEvelope = DateTime.UtcNow,
                Status = TaskSignerStatusEnum.SEND,
                TenantId = tenantInfo.Id,
                EnvelopeId = DateTime.UtcNow.Millisecond,
            };

            foreach (var file in filesSend)
            {
                filesInfo.Add(new TaskSignerFileInfo
                {
                    SignerId = DateTime.UtcNow.Millisecond + 3,
                    FieldValueFileId = file.FieldValueFileId,
                    TenantId = tenantInfo.Id,
                });
            }

            taskSignerInfo.Files = filesInfo;

            return Task.FromResult(taskSignerInfo);
        }

        public Task<FileViewModel> DownloadFile(int signerFileId, SignerEnvelopeFileSuffixEnum fileType, int tenantId)
        {
            return Task.FromResult(new FileViewModel());
        }
    }
}
