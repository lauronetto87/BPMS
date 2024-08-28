using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SatelittiBpms.FluentDataBuilder;
using SatelittiBpms.Models.DTO.Integration.Signer;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Services.Integration.Mock;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Test.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Test.Tests
{
    public class SignerIntegrationServiceGetFileTest : BaseTest
    {
        MockServices mockServices;

        readonly DataId fileFieldId = new();
        readonly DataId signatoryNameFieldId = new();
        readonly DataId signatoryCpfFieldId = new();
        readonly DataId signatoryEmailFieldId = new();

        Data.FlowExecuteResult executeResult;

        private int _resultSignerFileId;
        private int _resultTenantId;
        private SignerEnvelopeFileSuffixEnum _resultFileType;

        [SetUp]
        public void Setup()
        {
            mockServices = new MockServices();

            mockServices.AddCustomizeServices(services =>
            {
                var mockSignerIntegrationRestService = new Mock<ISignerIntegrationRestService>();
                
                mockSignerIntegrationRestService
                .Setup(f => f.DownloadFile(It.IsAny<int>(), It.IsAny<SignerEnvelopeFileSuffixEnum>(), It.IsAny<int>()))
                .Callback((int signerFileId, SignerEnvelopeFileSuffixEnum fileType, int tenantId) =>
                {
                    _resultSignerFileId = signerFileId;
                    _resultTenantId = tenantId;
                    _resultFileType = fileType;
                });

                mockSignerIntegrationRestService
                .Setup(f => f.CreateEnvelope(It.IsAny<IntegrationEnvelopeDTO>(), It.IsAny<List<SignerIntegrationEnvelopeFileDTO>>(), It.IsAny<TenantInfo>()))
                .Returns(
                    (IntegrationEnvelopeDTO integrationEnvelope, List<SignerIntegrationEnvelopeFileDTO> filesSend, TenantInfo tenantInfo) =>
                        new MockSignerIntegrationRestService().CreateEnvelope(integrationEnvelope, filesSend, tenantInfo)
                    );

                services.AddScoped((p) => mockSignerIntegrationRestService.Object);
            });
        }

        [Test(Description = "Valida se vai ser efetuados os downloads corretamente de uma integração com o S-Signer finalizada.")]
        public async Task TestDownload()
        {
            executeResult = await mockServices
                .BeginCreateProcess()
                    .Field(signatoryNameFieldId).Type(FieldTypeEnum.TEXTFIELD)
                    .Field(signatoryCpfFieldId).Type(FieldTypeEnum.TEXTFIELD)
                    .Field(signatoryEmailFieldId).Type(FieldTypeEnum.EMAIL)
                    .Field(fileFieldId).Type(FieldTypeEnum.FILE)
                    .ActivityUser()
                        .Field(signatoryNameFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(signatoryCpfFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(signatoryEmailFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(fileFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                    .ActivitySigner()
                        .Files(fileFieldId)
                        .Signatory()
                            .RegistrationLocation(SignerRegistrationLocationEnum.FormFields)
                            .NameField(signatoryNameFieldId)
                            .CpfField(signatoryCpfFieldId)
                            .EmailField(signatoryEmailFieldId)
                .EndCreateProcess()
                .NewFlow()
                    .ExecuteAllTasks()
                .EndCreateFlows()
                .ExecuteInDataBase();

            var signerIntegrationService = mockServices.GetService<ISignerIntegrationService>();

            var fieldValueInfo = executeResult.FlowsExecuted[0].FlowInfo.Tasks.Last().FieldsValues.FirstOrDefault(f => f.Field.ComponentInternalId == fileFieldId.InternalId);

            Assert.IsNotNull(fieldValueInfo);
            Assert.IsNotNull(fieldValueInfo.FieldValueFiles);

            foreach (var fieldValueFileInfo in fieldValueInfo.FieldValueFiles)
            {
                await signerIntegrationService.GetFilePrint(fieldValueFileInfo.FileKey);
                Assert.AreEqual(_resultSignerFileId, fieldValueFileInfo.TaskSignerFile.SignerId);
                Assert.AreEqual(_resultTenantId, fieldValueFileInfo.TenantId);
                Assert.AreEqual(_resultFileType, SignerEnvelopeFileSuffixEnum.Report);

                await signerIntegrationService.GetFileSigned(fieldValueFileInfo.FileKey);
                Assert.AreEqual(_resultSignerFileId, fieldValueFileInfo.TaskSignerFile.SignerId);
                Assert.AreEqual(_resultTenantId, fieldValueFileInfo.TenantId);
                Assert.AreEqual(_resultFileType, SignerEnvelopeFileSuffixEnum.Signed);
            }
        }

    }
}