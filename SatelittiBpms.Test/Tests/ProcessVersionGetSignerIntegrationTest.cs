using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SatelittiBpms.FluentDataBuilder;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Test.Extensions;
using SatelittiBpms.Tests.Executors;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Test.Tests
{
    public class ProcessVersionGetSignerIntegrationTest : BaseTest
    {
        MockServices mockServices;
        DbContext dbContext;

        [SetUp]
        public async Task Setup()
        {
            mockServices = new MockServices();
            await mockServices.BuildServiceProvider();
            await mockServices.ActivationTenant();
            dbContext = mockServices.GetService<DbContext>();
        }

        [Test]
        public async Task GetProcessWithSignerIntegrationData()
        {
            var expirationDateFieldId = new DataId();
            var fileFieldId = new DataId();

            var authorizerNameFieldId = new DataId();
            var authorizerCpfFieldId = new DataId();
            var authorizerEmailFieldId = new DataId();

            var signatoryNameFieldId = new DataId();
            var signatoryCpfFieldId = new DataId();
            var signatoryEmailFieldId = new DataId();

            var processVersionData = mockServices
                .BeginCreateProcess()
                    .NoNeedPublish()
                    .Field(signatoryNameFieldId).Type(FieldTypeEnum.TEXTFIELD)
                    .Field(signatoryCpfFieldId).Type(FieldTypeEnum.TEXTFIELD)
                    .Field(signatoryEmailFieldId).Type(FieldTypeEnum.EMAIL)
                    .Field(authorizerNameFieldId).Type(FieldTypeEnum.TEXTFIELD)
                    .Field(authorizerCpfFieldId).Type(FieldTypeEnum.TEXTFIELD)
                    .Field(authorizerEmailFieldId).Type(FieldTypeEnum.EMAIL)
                    .Field(expirationDateFieldId).Type(FieldTypeEnum.DATETIME)
                    .Field(fileFieldId).Type(FieldTypeEnum.FILE)
                    .ActivityUser()
                        .Field(signatoryNameFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(signatoryCpfFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(signatoryEmailFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(authorizerNameFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(authorizerCpfFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(authorizerEmailFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(expirationDateFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(fileFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                    .ActivitySigner()
                        .ExpirationDateField(expirationDateFieldId)
                        .Files(fileFieldId)
                        .Authorizer()
                            .RegistrationLocation(SignerRegistrationLocationEnum.FormFields)
                            .NameField(authorizerNameFieldId)
                            .CpfField(authorizerCpfFieldId)
                            .EmailField(authorizerEmailFieldId)
                        .Signatory()
                            .RegistrationLocation(SignerRegistrationLocationEnum.FormFields)
                            .NameField(signatoryNameFieldId)
                            .CpfField(signatoryCpfFieldId)
                            .EmailField(signatoryEmailFieldId)
                .MakeProcess();

            var result = await ProcessVersionExecutor.SaveAndReturnResult(mockServices, processVersionData.AsDto());
            Assert.IsTrue(result.Success);
            var processVersionId = ResultContent<int>.GetValue(result);

            var contentResult = mockServices.GetService<IProcessVersionService>().GetByTenant(processVersionId);
            var queryResult = ResultContent<ProcessVersionEditViewModel>.GetValue(contentResult.Result);

            this.GetProcessWithSignerIntegrationDataValidate(processVersionData.AsDto().SignerTasks[0], queryResult.SignerTasks[0]);
        }

        private void GetProcessWithSignerIntegrationDataValidate(SignerIntegrationActivityDTO mockData, SignerIntegrationActivityViewModel loadedData)
        {
            Assert.AreEqual(mockData.FileFieldKeys, loadedData.FileFieldKeys);
            Assert.AreEqual(mockData.ActivityKey, loadedData.ActivityKey);
            Assert.AreEqual(mockData.EnvelopeTitle, loadedData.EnvelopeTitle);
            Assert.AreEqual(mockData.ExpirationDateFieldKey, loadedData.ExpirationDateFieldKey);
            Assert.AreEqual(mockData.Language, loadedData.Language);
            Assert.AreEqual(mockData.Segment, loadedData.Segment);
            Assert.AreEqual(mockData.SendReminders, loadedData.SendReminders);
            Assert.AreEqual(mockData.SignatoryAccessAuthentication, loadedData.SignatoryAccessAuthentication);
            Assert.AreEqual(mockData.AuthorizeEnablePriorAuthorizationOfTheDocument, loadedData.AuthorizeEnablePriorAuthorizationOfTheDocument);
            Assert.AreEqual(mockData.AuthorizeAccessAuthentication, loadedData.AuthorizeAccessAuthentication);

            Assert.AreEqual(mockData.Authorizers[0].CpfFieldKey, loadedData.Authorizers[0].CpfFieldKey);
            Assert.AreEqual(mockData.Authorizers[0].EmailFieldKey, loadedData.Authorizers[0].EmailFieldKey);
            Assert.AreEqual(mockData.Authorizers[0].NameFieldKey, loadedData.Authorizers[0].NameFieldKey);
            Assert.AreEqual(mockData.Authorizers[0].RegistrationLocation, loadedData.Authorizers[0].RegistrationLocation);
            Assert.AreEqual(mockData.Authorizers[0].OriginActivityId, loadedData.Authorizers[0].OriginActivityId);

            Assert.AreEqual(mockData.Signatories[0].CpfFieldKey, loadedData.Signatories[0].CpfFieldKey);
            Assert.AreEqual(mockData.Signatories[0].EmailFieldKey, loadedData.Signatories[0].EmailFieldKey);
            Assert.AreEqual(mockData.Signatories[0].NameFieldKey, loadedData.Signatories[0].NameFieldKey);
            Assert.AreEqual(mockData.Signatories[0].RegistrationLocation, loadedData.Signatories[0].RegistrationLocation);
            Assert.AreEqual(mockData.Signatories[0].SignatureTypeId, loadedData.Signatories[0].SignatureTypeId);
            Assert.AreEqual(mockData.Signatories[0].SubscriberTypeId, loadedData.Signatories[0].SubscriberTypeId);
            Assert.AreEqual(mockData.Signatories[0].OriginActivityId, loadedData.Signatories[0].OriginActivityId);
        }
    }
}
