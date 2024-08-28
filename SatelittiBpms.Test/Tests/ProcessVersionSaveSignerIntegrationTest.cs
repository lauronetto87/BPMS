using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SatelittiBpms.FluentDataBuilder;
using SatelittiBpms.FluentDataBuilder.Process.Data;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Services.Helpers;
using SatelittiBpms.Test.Extensions;
using SatelittiBpms.Tests.Executors;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Test.Tests
{
    [TestFixture]
    public class ProcessVersionSaveSignerIntegrationTest : BaseTest
    {
        MockServices mockServices;

        [SetUp]
        public void Setup()
        {
            mockServices = new MockServices();
        }

        [Test(Description = "Efetua o teste para as regras de validação da visibilidade dos campos para quando tem uma tarefa de integração com o signer.")]
        public async Task TestAllRoles()
        {
            var expirationDateFieldId = new DataId();
            var file1FieldId = new DataId();
            var file2FieldId = new DataId();

            var authorizerNameFieldId = new DataId();
            var authorizerCpfFieldId = new DataId();
            var authorizerEmailFieldId = new DataId();

            var signatoryNameFieldId = new DataId();
            var signatoryCpfFieldId = new DataId();
            var signatoryEmailFieldId = new DataId();

            var withOutValidationFieldId = new DataId();
            
            var processVersionData = mockServices
                .BeginCreateProcess()
                    .Field(signatoryNameFieldId).Type(FieldTypeEnum.TEXTFIELD)
                    .Field(signatoryCpfFieldId).Type(FieldTypeEnum.TEXTFIELD)
                    .Field(signatoryEmailFieldId).Type(FieldTypeEnum.EMAIL)
                    .Field(authorizerNameFieldId).Type(FieldTypeEnum.TEXTFIELD)
                    .Field(authorizerCpfFieldId).Type(FieldTypeEnum.TEXTFIELD)
                    .Field(authorizerEmailFieldId).Type(FieldTypeEnum.EMAIL)
                    .Field(expirationDateFieldId).Type(FieldTypeEnum.DATETIME)
                    .Field(file1FieldId).Type(FieldTypeEnum.FILE)
                    .Field(file2FieldId).Type(FieldTypeEnum.FILE)
                    .Field(withOutValidationFieldId).Type(FieldTypeEnum.TEXTFIELD)
                    .ActivityUser()
                        .Field(signatoryNameFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(signatoryCpfFieldId).State(ProcessTaskFieldStateEnum.ONLYREADING)
                        .Field(signatoryEmailFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(authorizerNameFieldId).State(ProcessTaskFieldStateEnum.INVISIBLE)
                        .Field(authorizerCpfFieldId).State(ProcessTaskFieldStateEnum.INVISIBLE)
                        .Field(authorizerEmailFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(expirationDateFieldId).State(ProcessTaskFieldStateEnum.ONLYREADING)
                        .Field(file1FieldId).State(ProcessTaskFieldStateEnum.INVISIBLE)
                        .Field(file2FieldId).State(ProcessTaskFieldStateEnum.ONLYREADING)
                        .Field(withOutValidationFieldId).State(ProcessTaskFieldStateEnum.INVISIBLE)
                    .ExclusiveGateway()
                        .Branch()
                            .ActivityUser()
                                .Field(signatoryNameFieldId).State(ProcessTaskFieldStateEnum.INVISIBLE)
                                .Field(signatoryCpfFieldId).State(ProcessTaskFieldStateEnum.ONLYREADING)
                                .Field(signatoryEmailFieldId).State(ProcessTaskFieldStateEnum.INVISIBLE)
                                .Field(authorizerNameFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                                .Field(authorizerCpfFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                                .Field(authorizerEmailFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                                .Field(expirationDateFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                                .Field(file1FieldId).State(ProcessTaskFieldStateEnum.INVISIBLE)
                                .Field(file2FieldId).State(ProcessTaskFieldStateEnum.ONLYREADING)
                                .Field(withOutValidationFieldId).State(ProcessTaskFieldStateEnum.INVISIBLE)
                        .Branch()
                            .ActivityUser()
                                .Field(signatoryNameFieldId).State(ProcessTaskFieldStateEnum.ONLYREADING)
                                .Field(signatoryCpfFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                                .Field(signatoryEmailFieldId).State(ProcessTaskFieldStateEnum.INVISIBLE)
                                .Field(authorizerNameFieldId).State(ProcessTaskFieldStateEnum.INVISIBLE)
                                .Field(authorizerCpfFieldId).State(ProcessTaskFieldStateEnum.ONLYREADING)
                                .Field(authorizerEmailFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                                .Field(expirationDateFieldId).State(ProcessTaskFieldStateEnum.INVISIBLE)
                                .Field(file1FieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                                .Field(file2FieldId).State(ProcessTaskFieldStateEnum.ONLYREADING)
                                .Field(withOutValidationFieldId).State(ProcessTaskFieldStateEnum.INVISIBLE)
                            .ActivitySigner()
                                .ExpirationDateField(expirationDateFieldId)
                                .Files(file1FieldId, file2FieldId)
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
            Assert.IsFalse(result.Success);

            var erros = result.ValidationResult.Errors;
            Assert.AreEqual(erros.Count, 4);

            const string NAME_FIELD = "nameField";

            Assert.AreEqual(erros[0].ErrorMessage, ExceptionCodes.MANDATORY_FIELD_FOR_INTEGRATION_WITH_SIGNER_ERROR);
            Assert.AreEqual(TypeHelper.GetBalueByProperty(erros[0].AttemptedValue, NAME_FIELD), processVersionData.FindFieldById(authorizerNameFieldId).Label);

            Assert.AreEqual(erros[1].ErrorMessage, ExceptionCodes.MANDATORY_FIELD_FOR_INTEGRATION_WITH_SIGNER_ERROR);
            Assert.AreEqual(TypeHelper.GetBalueByProperty(erros[1].AttemptedValue, NAME_FIELD), processVersionData.FindFieldById(authorizerCpfFieldId).Label);

            Assert.AreEqual(erros[2].ErrorMessage, ExceptionCodes.MANDATORY_FIELD_FOR_INTEGRATION_WITH_SIGNER_ERROR);
            Assert.AreEqual(TypeHelper.GetBalueByProperty(erros[2].AttemptedValue, NAME_FIELD), processVersionData.FindFieldById(expirationDateFieldId).Label);

            Assert.AreEqual(erros[3].ErrorMessage, ExceptionCodes.MANDATORY_FIELD_FOR_INTEGRATION_WITH_SIGNER_ERROR);
            Assert.AreEqual(TypeHelper.GetBalueByProperty(erros[3].AttemptedValue, NAME_FIELD), processVersionData.FindFieldById(file2FieldId).Label);
        }

        [Test(Description = "Verfica se todos os dados são salvos corretamente no banco de dados.")]
        public async Task TestSave()
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

            var processVersion = mockServices.GetService<DbContext>().Set<ProcessVersionInfo>()
                                    .Include(p => p.Activities).ThenInclude(a => a.SignerIntegrationActivity.Authorizers)
                                    .Include(p => p.Activities).ThenInclude(a => a.SignerIntegrationActivity.Files).ThenInclude(f => f.FileField)
                                    .Include(p => p.Activities).ThenInclude(a => a.SignerIntegrationActivity.Signatories)
                                    .Include(p => p.Activities).ThenInclude(a => a.SignerIntegrationActivity.ExpirationDateField)

                                    .Include(p => p.Activities).ThenInclude(a => a.SignerIntegrationActivity.Authorizers).ThenInclude(a => a.CpfField)
                                    .Include(p => p.Activities).ThenInclude(a => a.SignerIntegrationActivity.Authorizers).ThenInclude(a => a.EmailField)
                                    .Include(p => p.Activities).ThenInclude(a => a.SignerIntegrationActivity.Authorizers).ThenInclude(a => a.NameField)
                                    .Include(p => p.Activities).ThenInclude(a => a.SignerIntegrationActivity.Authorizers).ThenInclude(a => a.OriginActivity)

                                    .Include(p => p.Activities).ThenInclude(a => a.SignerIntegrationActivity.Signatories).ThenInclude(a => a.CpfField)
                                    .Include(p => p.Activities).ThenInclude(a => a.SignerIntegrationActivity.Signatories).ThenInclude(a => a.EmailField)
                                    .Include(p => p.Activities).ThenInclude(a => a.SignerIntegrationActivity.Signatories).ThenInclude(a => a.NameField)
                                    .Include(p => p.Activities).ThenInclude(a => a.SignerIntegrationActivity.Signatories).ThenInclude(a => a.OriginActivity)

                                    .FirstOrDefault(p => p.Id == processVersionId);

            Assert.IsNotNull(processVersion);

            Assert.AreEqual(processVersion.Activities.Count, 2);
            Assert.AreEqual(processVersion.Activities[0].Type, WorkflowActivityTypeEnum.USER_TASK_ACTIVITY);

            var signerActivityInput = processVersionData.AllActivities.First(a => a.ActivityType == WorkflowActivityTypeEnum.SIGNER_TASK);

            var signerActivity = processVersion.Activities[1];
            TestSaveSignerActivity((ActivitySignerData)signerActivityInput, signerActivity);
        }

        private void TestSaveSignerActivity(ActivitySignerData signerActivityInput, ActivityInfo signerActivity)
        {
            Assert.AreEqual(signerActivity.ComponentInternalId, signerActivityInput.ActivityId);
            Assert.Greater(signerActivity.Id, 0);
            Assert.AreEqual(signerActivity.Name, signerActivityInput.ActivityName);
            Assert.Greater(signerActivity.ProcessVersionId, 0);
            Assert.IsNotNull(signerActivity.SignerIntegrationActivity);
            Assert.AreEqual(signerActivity.TenantId, mockServices.ContextData.Tenant.Id);
            Assert.AreEqual(signerActivity.Type, WorkflowActivityTypeEnum.SIGNER_TASK);

            TestSaveSignerIntegrationActivity(signerActivityInput, signerActivity.SignerIntegrationActivity);
        }

        private void TestSaveSignerIntegrationActivity(ActivitySignerData signerActivityInput, SignerIntegrationActivityInfo signerintegration)
        {
            Assert.Greater(signerintegration.ActivityId, 0);
            Assert.AreEqual(signerintegration.AuthorizeAccessAuthentication, signerActivityInput.AuthorizeAccessAuthentication);
            Assert.AreEqual(signerintegration.AuthorizeEnablePriorAuthorizationOfTheDocument, signerActivityInput.AuthorizeEnablePriorAuthorizationOfTheDocument);
            Assert.AreEqual(signerintegration.EnvelopeTitle, signerActivityInput.EnvelopeTitle);
            Assert.AreEqual(signerintegration.ExpirationDateField.ComponentInternalId, signerActivityInput.ExpirationDateField.Id.InternalId);
            Assert.AreEqual(signerintegration.Language, signerActivityInput.Language);
            Assert.AreEqual(signerintegration.Segment, signerActivityInput.Segment);
            Assert.AreEqual(signerintegration.SendReminders, signerActivityInput.SendReminders);
            Assert.AreEqual(signerintegration.SignatoryAccessAuthentication, signerActivityInput.SignatoryAccessAuthentication);
            Assert.AreEqual(signerintegration.TenantId, mockServices.ContextData.Tenant.Id);

            Assert.AreEqual(signerintegration.Signatories.Count, 1);
            Assert.AreEqual(signerintegration.Files.Count, 1);
            Assert.AreEqual(signerintegration.Authorizers.Count, 1);

            TestSaveSignerIntegrationActivityFile(signerintegration.Files[0], signerActivityInput.FileField[0]);
            TestSaveSignerIntegrationActivitySignatory(signerintegration.Signatories[0], signerActivityInput.Signatories[0]);
            TestSignerIntegrationActivityAuthorizer(signerintegration.Authorizers[0], signerActivityInput.Authorizers[0]);
        }

        private static void TestSaveSignerIntegrationActivityFile(SignerIntegrationActivityFileInfo file, FieldBaseData fileInput)
        {
            Assert.Greater(file.Id, 0);
            Assert.Greater(file.FileFieldId, 0);
            Assert.Greater(file.SignerIntegrationActivityId, 0);
            Assert.AreEqual(file.FileField.ComponentInternalId, fileInput.Id.InternalId);
        }

        private void TestSignerIntegrationActivityAuthorizer(SignerIntegrationActivityAuthorizerInfo authorizer, ActivitySignerAuthorizerData authorizerInput)
        {
            Assert.AreEqual(authorizer.CpfField.ComponentInternalId, authorizerInput.CpfField.Id.InternalId);
            Assert.AreEqual(authorizer.EmailField.ComponentInternalId, authorizerInput.EmailField.Id.InternalId);
            Assert.AreEqual(authorizer.NameField.ComponentInternalId, authorizerInput.NameField.Id.InternalId);
            Assert.AreEqual(authorizer.RegistrationLocation, authorizerInput.RegistrationLocation);
            Assert.Greater(authorizer.Id, 0);
            Assert.AreEqual(authorizer.TenantId, mockServices.ContextData.Tenant.Id);
        }

        private void TestSaveSignerIntegrationActivitySignatory(SignerIntegrationActivitySignatoryInfo signatory, ActivitySignerSignatoryData signatoryInput)
        {
            Assert.AreEqual(signatory.CpfField.ComponentInternalId, signatoryInput.CpfField.Id.InternalId);
            Assert.AreEqual(signatory.EmailField.ComponentInternalId, signatoryInput.EmailField.Id.InternalId);
            Assert.AreEqual(signatory.NameField.ComponentInternalId, signatoryInput.NameField.Id.InternalId);
            Assert.AreEqual(signatory.RegistrationLocation, signatoryInput.RegistrationLocation);
            Assert.AreEqual(signatory.SignatureTypeId, signatoryInput.SignatureTypeId);
            Assert.AreEqual(signatory.SubscriberTypeId, signatoryInput.SubscriberTypeId);
            Assert.Greater(signatory.Id, 0);
            Assert.AreEqual(signatory.TenantId, mockServices.ContextData.Tenant.Id);
            Assert.AreEqual(signatory.OriginActivity?.ComponentInternalId, signatoryInput.OriginActivityId?.InternalId);
        }
    }
}
