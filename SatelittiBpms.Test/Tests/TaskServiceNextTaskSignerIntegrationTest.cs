using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SatelittiBpms.FluentDataBuilder;
using SatelittiBpms.FluentDataBuilder.Process.Data;
using SatelittiBpms.Models.DTO.Integration.Signer;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Services.Helpers;
using SatelittiBpms.Services.Integration.Mock;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Test.Extensions;
using SatelittiBpms.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Test.Tests
{
    public class TaskServiceNextTaskSignerIntegrationTest : BaseTest
    {
        MockServices mockServices;

        IntegrationEnvelopeDTO integrationEnvelopeDto;
        List<SignerIntegrationEnvelopeFileDTO> filesDto;

        readonly DataId expirationDateFieldId = new();
        readonly DataId fileFieldId = new();
        readonly DataId authorizerNameFieldId = new();
        readonly DataId authorizerCpfFieldId = new();
        readonly DataId authorizerEmailFieldId = new();
        readonly DataId signatoryNameFieldId = new();
        readonly DataId signatoryCpfFieldId = new();
        readonly DataId signatoryEmailFieldId = new();

        readonly DataId activityUserId = new();

        Data.FlowExecuteResult executeResult;

        [SetUp]
        public void Setup()
        {
            mockServices = new MockServices();
            mockServices.AddCustomizeServices(services =>
            {
                var mockSignerIntegrationRestService = new Mock<ISignerIntegrationRestService>();
                mockSignerIntegrationRestService
                .Setup(f => f.CreateEnvelope(It.IsAny<IntegrationEnvelopeDTO>(), It.IsAny<List<SignerIntegrationEnvelopeFileDTO>>(), It.IsAny<TenantInfo>()))
                .Callback((IntegrationEnvelopeDTO integrationEnvelope, List<SignerIntegrationEnvelopeFileDTO> files, TenantInfo tenantInfo) =>
                {
                    integrationEnvelopeDto = integrationEnvelope;
                    filesDto = files;
                })
                .Returns(
                    (IntegrationEnvelopeDTO integrationEnvelope, List<SignerIntegrationEnvelopeFileDTO> filesSend, TenantInfo tenantInfo) =>
                        new MockSignerIntegrationRestService().CreateEnvelope(integrationEnvelope, filesSend, tenantInfo)
                    );
                services.AddScoped((p) => mockSignerIntegrationRestService.Object);
            });
        }

        [Test(Description = "Valida a atividade de integração desde execução dela pelo motor até a montegem do envelope para o envio ao Signer.")]
        public async Task TestIntegration()
        {
            var wildcardTestData = WildcardHelper.BuildWildcardTestData(authorizerNameFieldId.InternalId);

            executeResult = await mockServices
                .BeginCreateProcess()
                    .Field(signatoryNameFieldId).Type(FieldTypeEnum.TEXTFIELD)
                    .Field(signatoryCpfFieldId).Type(FieldTypeEnum.TEXTFIELD)
                    .Field(signatoryEmailFieldId).Type(FieldTypeEnum.EMAIL)
                    .Field(authorizerNameFieldId).Type(FieldTypeEnum.TEXTFIELD)
                    .Field(authorizerCpfFieldId).Type(FieldTypeEnum.TEXTFIELD)
                    .Field(authorizerEmailFieldId).Type(FieldTypeEnum.EMAIL)
                    .Field(expirationDateFieldId).Type(FieldTypeEnum.DATETIME)
                    .Field(fileFieldId).Type(FieldTypeEnum.FILE)
                    .ActivityUser(activityUserId)
                        .Field(signatoryNameFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(signatoryCpfFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(signatoryEmailFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(authorizerNameFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(authorizerCpfFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(authorizerEmailFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(expirationDateFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                        .Field(fileFieldId).State(ProcessTaskFieldStateEnum.MANDATORY)
                    .ActivitySigner()
                        .EnvelopeTitle(wildcardTestData.BuildUserInput())
                        .ExpirationDateField(expirationDateFieldId)
                        .Files(fileFieldId)
                        .Authorizer()
                            .RegistrationLocation(SignerRegistrationLocationEnum.FormFields)
                            .NameField(authorizerNameFieldId)
                            .CpfField(authorizerCpfFieldId)
                            .EmailField(authorizerEmailFieldId)
                        .Authorizer()
                            .RegistrationLocation(SignerRegistrationLocationEnum.UserTask)
                            .OriginActivity(activityUserId)
                            .CpfField(authorizerCpfFieldId)
                        .Signatory()
                            .RegistrationLocation(SignerRegistrationLocationEnum.FormFields)
                            .NameField(signatoryNameFieldId)
                            .CpfField(signatoryCpfFieldId)
                            .EmailField(signatoryEmailFieldId)
                        .Signatory()
                            .RegistrationLocation(SignerRegistrationLocationEnum.FormFields)
                            .NameField(signatoryNameFieldId)
                            .CpfField(signatoryCpfFieldId)
                            .EmailField(signatoryEmailFieldId)
                        .Signatory()
                            .RegistrationLocation(SignerRegistrationLocationEnum.UserTask)
                            .OriginActivity(activityUserId)
                            .CpfField(signatoryCpfFieldId)
                .EndCreateProcess()
                .NewFlow()
                    .ExecuteAllTasks()
                .EndCreateFlows()
                .ExecuteInDataBase();

            var flowExecuteResult = executeResult.FlowsExecuted.First();
            Assert.AreEqual(4, flowExecuteResult.FlowInfo.Tasks.Count);
            Assert.AreEqual(WorkflowActivityTypeEnum.START_EVENT_ACTIVITY, flowExecuteResult.FlowInfo.Tasks[0].Activity.Type);
            Assert.AreEqual(WorkflowActivityTypeEnum.USER_TASK_ACTIVITY, flowExecuteResult.FlowInfo.Tasks[1].Activity.Type);
            Assert.AreEqual(WorkflowActivityTypeEnum.SIGNER_TASK, flowExecuteResult.FlowInfo.Tasks[2].Activity.Type);
            Assert.AreEqual(WorkflowActivityTypeEnum.END_EVENT_ACTIVITY, flowExecuteResult.FlowInfo.Tasks[3].Activity.Type);

            TestIntegrationSignerTask();
            await TestIntegrationRequestEnvelope(wildcardTestData);
            TestIntegrationRequestFiles();
            TestIntegrationEndTask();
        }

        private void TestIntegrationSignerTask()
        {
            var flowExecuteResult = executeResult.FlowsExecuted.First();
            var signerTask = flowExecuteResult.FlowInfo.Tasks[2];

            AssertDateEqualNowWithDelay(signerTask.CreatedDate);
            AssertDateEqualNowWithDelay(signerTask.FinishedDate);
            Assert.IsNotNull(signerTask.FieldsValues);
            Assert.AreEqual(8, signerTask.FieldsValues.Count);

            Assert.IsNotNull(signerTask.SourceTasks);
            Assert.AreEqual(1, signerTask.SourceTasks.Count);
            Assert.AreEqual(WorkflowActivityTypeEnum.USER_TASK_ACTIVITY, signerTask.SourceTasks[0].SourceTask.Activity.Type);
            Assert.AreEqual(flowExecuteResult.FlowInfo.Tasks[1].Id, signerTask.SourceTasks[0].SourceTaskId);
            Assert.AreEqual(flowExecuteResult.FlowInfo.Tasks[1].Id, signerTask.SourceTasks[0].SourceTask.Id);
            Assert.IsNotNull(signerTask.TargetTasks);
            Assert.AreEqual(1, signerTask.TargetTasks.Count);
            Assert.AreEqual(WorkflowActivityTypeEnum.END_EVENT_ACTIVITY, signerTask.TargetTasks[0].TargetTask.Activity.Type);
            Assert.AreEqual(flowExecuteResult.FlowInfo.Tasks[3].Id, signerTask.TargetTasks[0].TargetTaskId);
            Assert.AreEqual(flowExecuteResult.FlowInfo.Tasks[3].Id, signerTask.TargetTasks[0].TargetTask.Id);
            Assert.IsTrue(signerTask.TasksHistories == null || signerTask.TasksHistories.Count == 0);
            Assert.IsNull(signerTask.ExecutorId);
            Assert.IsNull(signerTask.OptionId);
            Assert.AreEqual(mockServices.ContextData.Tenant.Id, signerTask.TenantId);
        }

        private void TestIntegrationEndTask()
        {
            var flowExecuteResult = executeResult.FlowsExecuted.First();
            var endTask = flowExecuteResult.FlowInfo.Tasks.Last();

            AssertDateEqualNowWithDelay(endTask.CreatedDate);
            AssertDateEqualNowWithDelay(endTask.FinishedDate);
            Assert.IsNotNull(endTask.FieldsValues);
            Assert.AreEqual(8, endTask.FieldsValues.Count);

            Assert.IsNotNull(endTask.SourceTasks);
            Assert.AreEqual(1, endTask.SourceTasks.Count);
            Assert.AreEqual(WorkflowActivityTypeEnum.SIGNER_TASK, endTask.SourceTasks[0].SourceTask.Activity.Type);
            Assert.AreEqual(flowExecuteResult.FlowInfo.Tasks[2].Id, endTask.SourceTasks[0].SourceTaskId);
            Assert.AreEqual(flowExecuteResult.FlowInfo.Tasks[2].Id, endTask.SourceTasks[0].SourceTask.Id);
            Assert.IsTrue((endTask.TargetTasks?.Count ?? 0) == 0);
            Assert.IsTrue(endTask.TasksHistories == null || endTask.TasksHistories.Count == 0);
            Assert.IsNull(endTask.ExecutorId);
            Assert.IsNull(endTask.OptionId);
            Assert.AreEqual(mockServices.ContextData.Tenant.Id, endTask.TenantId);

            var filesFieldValue = endTask.FieldsValues.Where(x => x.Field.ComponentInternalId == fileFieldId.InternalId).ToList();
            Assert.AreEqual(1, filesFieldValue.Count);

            var fieldValueFiles = filesFieldValue[0].FieldValueFiles;

            Assert.IsTrue(fieldValueFiles.All(x => x.TaskSignerFile != null));
        }

        private async Task TestIntegrationRequestEnvelope(WildcardHelper.WildcardTestData wildcardTestData)
        {
            var flowExecuteResult = executeResult.FlowsExecuted.First();
            var signerTask = flowExecuteResult.FlowInfo.Tasks[2];

            var signerActivityInput = executeResult.ProcessVersion.Activities[1] as ActivitySignerData;

            var fieldValuesFormRequest = flowExecuteResult.Tasks.First().TaskInput.FieldValues;
            var fieldValues = signerTask.FieldsValues;

            var fieldValue = fieldValues.FirstOrDefault(f => f.Field.ComponentInternalId == expirationDateFieldId.InternalId);
            Assert.IsNotNull(fieldValue);
            Assert.AreEqual(FieldValueInfoHelper.CovertType<DateTime>(fieldValue), integrationEnvelopeDto.Expiration);
            var currentUserSuite = await mockServices.GetUserSuite(mockServices.ContextData.User.Id);
            Assert.AreEqual(currentUserSuite.Name, integrationEnvelopeDto.Sender.Name);
            Assert.AreEqual(currentUserSuite.Mail, integrationEnvelopeDto.Sender.Email);
            Assert.IsEmpty(integrationEnvelopeDto.Sender.IndividualIdentificationCode);
            Assert.AreEqual(signerActivityInput.Segment, integrationEnvelopeDto.Segment);
            Assert.AreEqual(signerActivityInput.SendReminders, integrationEnvelopeDto.Notify);
            Assert.AreEqual(wildcardTestData.BuildResult(fieldValuesFormRequest, flowExecuteResult.FlowId), integrationEnvelopeDto.Name);
            Assert.AreEqual(signerActivityInput.Language, integrationEnvelopeDto.Language);
            Assert.AreEqual(signerActivityInput.SignatoryAccessAuthentication, integrationEnvelopeDto.NeedAuth);
            Assert.AreEqual(signerActivityInput.AuthorizeAccessAuthentication, integrationEnvelopeDto.AuthorizerNeedAuth);
            Assert.IsEmpty(integrationEnvelopeDto.EmailNotification);
            Assert.IsEmpty(integrationEnvelopeDto.Message);
            Assert.IsTrue(integrationEnvelopeDto.ShowDetails);
            Assert.AreEqual(0, integrationEnvelopeDto.WidthInMm);

            Assert.AreEqual(2, integrationEnvelopeDto.Authorizers.Count);

            for (int i = 0; i < integrationEnvelopeDto.Authorizers.Count; i++)
            {
                var signer = integrationEnvelopeDto.Signers[i];
                var authorizerInput = signerActivityInput.Authorizers[i];
                switch (authorizerInput.RegistrationLocation)
                {
                    case SignerRegistrationLocationEnum.FormFields:
                        TestIntegrationRequestEnvelopeAuthorizerForm(fieldValues, integrationEnvelopeDto.Authorizers[i]);
                        break;
                    case SignerRegistrationLocationEnum.UserTask:
                        await TestIntegrationRequestEnvelopeAuthorizerUserTask(fieldValues, integrationEnvelopeDto.Authorizers[i]);
                        break;
                    default:
                        throw new NotImplementedException($"Não implementado para validar quando a origem dos dados for {authorizerInput.RegistrationLocation}.");
                }
            }
            

            Assert.AreEqual(3, integrationEnvelopeDto.Signers.Count);

            for (int i = 0; i < integrationEnvelopeDto.Signers.Count; i++)
            {
                var signer = integrationEnvelopeDto.Signers[i];
                var signatoryInput = signerActivityInput.Signatories[i];
                switch (signatoryInput.RegistrationLocation)
                {
                    case SignerRegistrationLocationEnum.FormFields:
                        TestIntegrationRequestEnvelopeSignerForm(fieldValues, signer, signatoryInput);
                        break;
                    case SignerRegistrationLocationEnum.UserTask:
                        await TestIntegrationRequestEnvelopeSignerUserTask(fieldValues, signer, signatoryInput);
                        break;
                    default:
                        throw new NotImplementedException($"Não implementado para validar quando a origem dos dados for {signatoryInput.RegistrationLocation}.");
                }
            }
        }

        private void TestIntegrationRequestEnvelopeAuthorizerForm(IList<FieldValueInfo> fieldValues, IntegrationEnvelopeAuthorizerDTO authorizer)
        {
            FieldValueInfo fieldValue = fieldValues.FirstOrDefault(f => f.Field.ComponentInternalId == authorizerEmailFieldId.InternalId);
            Assert.IsNotNull(fieldValue);
            Assert.AreEqual(fieldValue.FieldValue, authorizer.Email);

            fieldValue = fieldValues.FirstOrDefault(f => f.Field.ComponentInternalId == authorizerNameFieldId.InternalId);
            Assert.IsNotNull(fieldValue);
            Assert.AreEqual(fieldValue.FieldValue, authorizer.Name);

            fieldValue = fieldValues.FirstOrDefault(f => f.Field.ComponentInternalId == authorizerCpfFieldId.InternalId);
            Assert.IsNotNull(fieldValue);
            Assert.AreEqual(fieldValue.FieldValue?.Replace("-", "").Replace(".", ""), authorizer.IndividualIdentificationCode);
        }

        private async Task TestIntegrationRequestEnvelopeAuthorizerUserTask(IList<FieldValueInfo> fieldValues, IntegrationEnvelopeAuthorizerDTO authorizer)
        {
            var executorId = executeResult.FlowsExecuted.First().FlowDataInfoRequestFlow.RequesterId;
            var executor = await mockServices.GetUserSuite(executorId);

            Assert.AreEqual(executor.Mail, authorizer.Email);
            Assert.AreEqual(executor.Name, authorizer.Name);

            var fieldValue = fieldValues.FirstOrDefault(f => f.Field.ComponentInternalId == authorizerCpfFieldId.InternalId);
            Assert.IsNotNull(fieldValue);
            Assert.AreEqual(fieldValue.FieldValue?.Replace("-", "").Replace(".", ""), authorizer.IndividualIdentificationCode);
        }

        private void TestIntegrationRequestEnvelopeSignerForm(IList<FieldValueInfo> fieldValues, IntegrationEnvelopeSignerDTO signer, ActivitySignerSignatoryData signatoryInput)
        {
            FieldValueInfo fieldValue = fieldValues.FirstOrDefault(f => f.Field.ComponentInternalId == signatoryEmailFieldId.InternalId);
            Assert.IsNotNull(fieldValue);
            Assert.AreEqual(fieldValue.FieldValue, signer.Email);

            fieldValue = fieldValues.FirstOrDefault(f => f.Field.ComponentInternalId == signatoryNameFieldId.InternalId);
            Assert.IsNotNull(fieldValue);
            Assert.AreEqual(fieldValue.FieldValue, signer.Name);

            fieldValue = fieldValues.FirstOrDefault(f => f.Field.ComponentInternalId == signatoryCpfFieldId.InternalId);
            Assert.IsNotNull(fieldValue);
            Assert.AreEqual(fieldValue.FieldValue?.Replace("-", "").Replace(".", ""), signer.IndividualIdentificationCode);

            Assert.AreEqual(1, signer.Order);
            Assert.AreEqual(signatoryInput.SignatureTypeId, signer.SignatureType);
            Assert.AreEqual(signatoryInput.SubscriberTypeId, signer.SignerType);
        }

        private async Task TestIntegrationRequestEnvelopeSignerUserTask(IList<FieldValueInfo> fieldValues, IntegrationEnvelopeSignerDTO signer, ActivitySignerSignatoryData signatoryInput)
        {
            var executorId = executeResult.FlowsExecuted.First().FlowDataInfoRequestFlow.RequesterId;

            var executor = await mockServices.GetUserSuite(executorId);
            
            Assert.AreEqual(executor.Mail, signer.Email);

            Assert.AreEqual(executor.Name, signer.Name);

            var fieldValue = fieldValues.FirstOrDefault(f => f.Field.ComponentInternalId == signatoryCpfFieldId.InternalId);
            Assert.IsNotNull(fieldValue);
            Assert.AreEqual(fieldValue.FieldValue?.Replace("-", "").Replace(".", ""), signer.IndividualIdentificationCode);

            Assert.AreEqual(1, signer.Order);
            Assert.AreEqual(signatoryInput.SignatureTypeId, signer.SignatureType);
            Assert.AreEqual(signatoryInput.SubscriberTypeId, signer.SignerType);
        }

        private void TestIntegrationRequestFiles()
        {
            var flowExecuteResult = executeResult.FlowsExecuted.First();
            var signerTask = flowExecuteResult.FlowInfo.Tasks[2];

            var allFiles = signerTask.FieldsValues
                .Where(f => f.Field.Type == FieldTypeEnum.FILE)
                .SelectMany(f => f.FieldValueFiles)
                .ToList();

            Assert.AreEqual(allFiles.Count, filesDto.Count);

            for (int i = 0; i < allFiles.Count; i++)
            {
                var fileInfo = allFiles[i];
                var fileDto = filesDto[i];

                Assert.AreEqual(fileInfo.Name, fileDto.Name);
                Assert.AreEqual(fileInfo.Id, fileDto.FieldValueFileId);
                Assert.IsNotNull(fileDto.Base64Content);
            }
        }
    }
}