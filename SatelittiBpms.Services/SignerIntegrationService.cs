using Microsoft.EntityFrameworkCore;
using Satelitti.Authentication.Context.Interface;
using SatelittiBpms.Models.DTO.Integration.Signer;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Integration.Signer;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Helpers;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Services.Interfaces.Integration;
using SatelittiBpms.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class SignerIntegrationService : ISignerIntegrationService
    {
        private readonly IContextDataService<UserInfo> _contextDataService;
        private readonly ITenantService _tenantService;
        private readonly ISignerSegmentService _signerSegmentService;
        private readonly ISignerReminderService _signerReminderService;
        private readonly ISignerSubscriberTypeService _signerSubscriberTypeService;
        private readonly ISignerSignatureTypeService _signerSignatureTypeService;
        private readonly ITaskRepository _taskRepository;
        private readonly ISuiteUserService _suiteUserService;
        private readonly ISignerIntegrationRestService _signerIntegrationRestService;
        private readonly IWildcardService _wildcardService;
        private readonly IStorageService _storageService;
        private readonly IFieldValueFileRepository _fieldValueFileRepository;

        public SignerIntegrationService(
            IContextDataService<UserInfo> contextService,
            ITenantService tenantService,
            ISignerSegmentService signerSegmentService,
            ISignerReminderService signerReminderService,
            ISignerSubscriberTypeService signerSubscriberTypeService,
            ISignerSignatureTypeService signerSignatureTypeService,
            ITaskRepository taskRepository,
            ISuiteUserService suiteUserService,
            ISignerIntegrationRestService signerIntegrationRestService,
            IWildcardService wildcardService,
            IStorageService storageService,
            IFieldValueFileRepository fieldValueFileRepository)
        {
            _contextDataService = contextService;
            _tenantService = tenantService;
            _signerSegmentService = signerSegmentService;
            _signerReminderService = signerReminderService;
            _signerSubscriberTypeService = signerSubscriberTypeService;
            _signerSignatureTypeService = signerSignatureTypeService;
            _taskRepository = taskRepository;
            _suiteUserService = suiteUserService;
            _signerIntegrationRestService = signerIntegrationRestService;
            _wildcardService = wildcardService;
            _storageService = storageService;
            _fieldValueFileRepository = fieldValueFileRepository;
        }

        public async Task<ResultContent> GetSignerInformation()
        {
            var contextData = _contextDataService.GetContextData();
            var tenantInfo = _tenantService.Get(contextData.Tenant.Id);
            if (string.IsNullOrWhiteSpace(tenantInfo.SignerAccessToken))
                return Result.Error(ExceptionCodes.MISSING_SSIGN_INTEGRATION_TOKEN);

            Task<List<Segment>> segmentTask = Task.Run(async () => await _signerSegmentService.List(tenantInfo.SubDomain, tenantInfo.SignerAccessToken));
            Task<List<EnvelopeReminderDescriptionListItem>> reminderTask = Task.Run(async () => await _signerReminderService.List(tenantInfo.SubDomain, tenantInfo.SignerAccessToken));
            Task<List<SignatureTypeDescriptionListItem>> signatureTypeTask = Task.Run(async () => await _signerSignatureTypeService.List(tenantInfo.SubDomain, tenantInfo.SignerAccessToken));
            Task<List<SubscriberTypeDescriptionListItem>> subscriberTypeTask = Task.Run(async () => await _signerSubscriberTypeService.List(tenantInfo.SubDomain, tenantInfo.SignerAccessToken));

            await Task.WhenAll(segmentTask, reminderTask, signatureTypeTask, subscriberTypeTask);

            var subscribersType = subscriberTypeTask.Result;
            RemoveAuthorizer(subscribersType);

            var signerIntegrationViewModel = new SignerIntegrationViewModel()
                .AsSignerIntegrationStepConfigurationViewModel(segmentTask.Result, reminderTask.Result, signatureTypeTask.Result, subscribersType);

            return Result.Success(signerIntegrationViewModel);
        }

        private static void RemoveAuthorizer(List<SubscriberTypeDescriptionListItem> subscribersType)
        {
            foreach (var subscriberTypeLanguage in subscribersType)
            {
                subscriberTypeLanguage.SubscriberTypeDescriptionList = subscriberTypeLanguage.SubscriberTypeDescriptionList.Where(s => s.Id != SubscriberTypeEnum.Authorizer).ToList();
            }
        }

        public Task<FileViewModel> GetFilePrint(string fileKey)
        {
            var fileValueFile = GetTaskSignerFileFromFileDownloadKey(fileKey);
            return _signerIntegrationRestService.DownloadFile(fileValueFile.SignerId, Models.Enums.SignerEnvelopeFileSuffixEnum.Report, fileValueFile.TenantId ?? 0);
        }


        public Task<FileViewModel> GetFileSigned(string fileKey)
        {
            var fileValueFile = GetTaskSignerFileFromFileDownloadKey(fileKey);
            return _signerIntegrationRestService.DownloadFile(fileValueFile.SignerId, Models.Enums.SignerEnvelopeFileSuffixEnum.Signed, fileValueFile.TenantId ?? 0);
        }

        private TaskSignerFileInfo GetTaskSignerFileFromFileDownloadKey(string fileKey)
        {
            var fileValueFile = _fieldValueFileRepository
                            .GetQuery(x => x.FileKey == fileKey && x.FieldValue.Task.Activity.Type == Models.Enums.WorkflowActivityTypeEnum.SIGNER_TASK)
                            .Include(x => x.TaskSignerFile)
                            .FirstOrDefault();

            if (fileValueFile == null)
            {
                throw new ArgumentException($"Não foi encontrado o arquivo com a chave de download `{fileKey}`.");
            }
            if (fileValueFile.TaskSignerFile == null)
            {
                throw new ArgumentException($"O arquivo com a chave de download `{fileKey}` e id {fileValueFile.Id} não é um arquivo do S-Signer.");
            }
            return fileValueFile.TaskSignerFile;
        }

        public async Task CreateEnvelopeOnSigner(int taskId)
        {
            var task = _taskRepository.GetQuery(taskId)
                .Include(t => t.Flow.ProcessVersion)
                .Include(t => t.Flow.Tasks).ThenInclude(t => t.Activity)
                .Include(t => t.FieldsValues).ThenInclude(f => f.FieldValueFiles)
                .Include(t => t.FieldsValues).ThenInclude(f => f.Field)
                .Include(t => t.Activity.SignerIntegrationActivity.Files)
                .Include(t => t.Activity.SignerIntegrationActivity.Authorizers)
                .Include(t => t.Activity.SignerIntegrationActivity.Signatories)
                .FirstOrDefault();
            if (task == null)
            {
                throw new ArgumentException($"Não foi encontrado a tarefa de código {taskId} para criar o envelope.");
            }
            if (task.Activity.Type != Models.Enums.WorkflowActivityTypeEnum.SIGNER_TASK)
            {
                throw new ArgumentException($"Tarefa de código {taskId} não é uma de intergação com o signer.");
            }
            if (task.Activity.SignerIntegrationActivity == null)
            {
                throw new ArgumentException($"Não foi encontrado os dados de integração com o Signer para a tarefa de código {taskId}.");
            }
            var tenant = _tenantService.Get(task.TenantId ?? 0);
            var users = (await _suiteUserService.ListWithoutContext(new Models.Filters.SuiteUserListFilter
            {
                TenantSubDomain = tenant.SubDomain,
                TenantAccessKey = tenant.AccessKey,
            }));

            var userId = task.Flow.RequesterId;

            var currentUser = users.FirstOrDefault(u => u.Id == userId);
            if (currentUser == null)
            {
                throw new Exception($"Não foi encontrado o usuário de código {userId} no Signer.");
            }

            var fieldsValues = task.FieldsValues;

            var signerData = task.Activity.SignerIntegrationActivity;

            var titleFormatted = _wildcardService.FormatDescriptionWildcard(signerData.EnvelopeTitle, task.Flow, users);

            var integrationEnvelope = new IntegrationEnvelopeDTO()
            {
                Name = titleFormatted,
                Expiration = GetValueFromFormFieldAndValueOptional<DateTime?>(nameof(signerData.ExpirationDateFieldId), fieldsValues, signerData.ExpirationDateFieldId),
                Language = signerData.Language,
                Segment = signerData.Segment,
                Notify = signerData.SendReminders,
                NeedAuth = signerData.SignatoryAccessAuthentication,
                AuthorizerNeedAuth = signerData.AuthorizeAccessAuthentication && signerData.Authorizers.Count > 0,
                Message = "",
                EmailNotification = "",
                ShowDetails = true,
                WidthInMm = 0,
                Sender = new IntegrationEnvelopeSenderDTO()
                {
                    Email = currentUser.Mail,
                    Name = currentUser.Name,
                    IndividualIdentificationCode = "",
                },
                Authorizers = signerData.AuthorizeEnablePriorAuthorizationOfTheDocument ? signerData.Authorizers.Select(a => GenerateDataIntegrationAuthorizer(a, task, users)).ToList() : new List<IntegrationEnvelopeAuthorizerDTO>(),
                Signers = signerData.Signatories.Select(a => GenerateDataIntegrationSigner(a, task, users)).ToList(),
            };

            var filesFieldId = signerData.Files.Select(f => f.FileFieldId);
            var fieldValues = fieldsValues.Where(fv => filesFieldId.Contains(fv.Field.Id));
            var filesSend = new List<SignerIntegrationEnvelopeFileDTO>();

            foreach (var file in fieldValues.SelectMany(fv => fv.FieldValueFiles).ToList())
            {
                using var fileSream = await _storageService.Download(file.Key);
                using var memoryStream = new MemoryStream();
                fileSream.CopyTo(memoryStream);
                var bytes = memoryStream.ToArray();

                filesSend.Add(new SignerIntegrationEnvelopeFileDTO
                {
                    Base64Content = Convert.ToBase64String(bytes),
                    FieldValueFileId = file.Id,
                    Name = file.Name,
                });
            }

            var taskSignerInfo = await _signerIntegrationRestService.CreateEnvelope(integrationEnvelope, filesSend, tenant);
            taskSignerInfo.TenantId = tenant.Id;
            taskSignerInfo.TaskId = task.Id;

            if (task.SignerTasks == null)
                task.SignerTasks = new List<TaskSignerInfo>();
            task.SignerTasks.Add(taskSignerInfo);
            await _taskRepository.Update(task);
        }
        private static T GetValueFromFormFieldOptional<T>(string nameOfProperty, IList<FieldValueInfo> fieldsValues, int? fieldId)
        {
            if (fieldId == null)
            {
                return default;
            }
            return GetValueFromFormRequired<T>(nameOfProperty, fieldsValues, fieldId);
        }
        private static T GetValueFromFormRequired<T>(string nameOfProperty, IList<FieldValueInfo> fieldsValues, int? fieldId)
        {
            if (fieldId == null)
            {
                throw new ArgumentException($"Não foi vinculado a configuração da integração com o formulário para o campo {nameOfProperty}.");
            }
            var fieldValue = fieldsValues.FirstOrDefault(f => f.Field.Id == fieldId);
            if (fieldValue == null)
            {
                throw new ArgumentException($"Não foi enconcontrado o campo de {nameOfProperty} nos valores enviado pelo formulário");
            }
            if (string.IsNullOrEmpty(fieldValue.FieldValue))
            {
                throw new ArgumentException($"Não foi informado valor no formulário para o campo \"{fieldValue.Field.Name}\".");
            }
            return FieldValueInfoHelper.CovertType<T>(fieldValue);
        }
        private static T GetValueFromFormFieldAndValueOptional<T>(string nameOfProperty, IList<FieldValueInfo> fieldsValues, int? fieldId)
        {
            if (fieldId == null)
            {
                return default;
            }
            var fieldValue = fieldsValues.FirstOrDefault(f => f.Field.Id == fieldId);
            if (fieldValue == null)
            {
                return default;
            }
            if (string.IsNullOrEmpty(fieldValue.FieldValue))
            {
                return default;
            }
            return FieldValueInfoHelper.CovertType<T>(fieldValue);
        }

        private static IntegrationEnvelopeAuthorizerDTO GenerateDataIntegrationAuthorizer(SignerIntegrationActivityAuthorizerInfo authorizer, TaskInfo taskInfo, IList<SuiteUserViewModel> users)
        {
            var fieldsValues = taskInfo.FieldsValues;

            string name;
            string email;
            switch (authorizer.RegistrationLocation)
            {
                case Models.Enums.SignerRegistrationLocationEnum.FormFields:
                    email = GetValueFromFormRequired<string>(nameof(authorizer.EmailFieldId), fieldsValues, authorizer.EmailFieldId);
                    name = GetValueFromFormRequired<string>(nameof(authorizer.NameFieldId), fieldsValues, authorizer.NameFieldId);
                    break;
                case Models.Enums.SignerRegistrationLocationEnum.UserTask:
                    var taskOrigin = taskInfo.Flow.Tasks.FirstOrDefault(t => t.Activity.Id == authorizer.OriginActivityId);
                    if (taskOrigin == null)
                    {
                        throw new Exception($"Não foi encontrado a tarefa de origem para pegar o usuário que executou, {nameof(SignerIntegrationActivityAuthorizerInfo)}.Id: {authorizer.Id}");
                    }
                    var user = users.FirstOrDefault(u => u.Id == taskOrigin.ExecutorId);
                    if (user == null)
                    {
                        throw new Exception($"Não foi encontrado o executor na suite da tarefa de código {taskOrigin.Id}, Código do Usuário Executou: {taskOrigin.ExecutorId}.");
                    }
                    email = user.Mail;
                    name = user.Name;
                    break;
                default:
                    throw new Exception("Não foi configurado para pegar o valor para integração quando a origem dos dados da integração for " + authorizer.RegistrationLocation);
            }

            return new IntegrationEnvelopeAuthorizerDTO
            {
                Email = email,
                Name = name,
                IndividualIdentificationCode = GetValueCpfCnpj(GetValueFromFormFieldOptional<string>(nameof(authorizer.CpfFieldId), fieldsValues, authorizer.CpfFieldId)),
            };
        }

        private static IntegrationEnvelopeSignerDTO GenerateDataIntegrationSigner(SignerIntegrationActivitySignatoryInfo signatory, TaskInfo taskInfo, IList<SuiteUserViewModel> users)
        {
            var fieldsValues = taskInfo.FieldsValues;

            string name;
            string email;
            switch (signatory.RegistrationLocation)
            {
                case Models.Enums.SignerRegistrationLocationEnum.FormFields:
                    email = GetValueFromFormRequired<string>(nameof(signatory.EmailFieldId), fieldsValues, signatory.EmailFieldId);
                    name = GetValueFromFormRequired<string>(nameof(signatory.NameFieldId), fieldsValues, signatory.NameFieldId);
                    break;
                case Models.Enums.SignerRegistrationLocationEnum.UserTask:
                    var taskOrigin = taskInfo.Flow.Tasks.FirstOrDefault(t => t.Activity.Id == signatory.OriginActivityId);
                    if (taskOrigin == null)
                    {
                        throw new Exception($"Não foi encontrado a tarefa de origem para pegar o usuário que executou, {nameof(SignerIntegrationActivitySignatoryInfo)}.Id: {signatory.Id}");
                    }
                    var user = users.FirstOrDefault(u => u.Id == taskOrigin.ExecutorId);
                    if (user == null)
                    {
                        throw new Exception($"Não foi encontrado o executor na suite da tarefa de código {taskOrigin.Id}, Código do Usuário Executou: {taskOrigin.ExecutorId}.");
                    }
                    email = user.Mail;
                    name = user.Name;
                    break;
                default:
                    throw new Exception("Não foi configurado para pegar o valor para integração quando a origem dos dados da integração for " + signatory.RegistrationLocation);
            }
            return new IntegrationEnvelopeSignerDTO
            {
                Email = email,
                Name = name,
                IndividualIdentificationCode = GetValueCpfCnpj(GetValueFromFormFieldOptional<string>(nameof(signatory.CpfFieldId), fieldsValues, signatory.CpfFieldId)),
                SignatureType = signatory.SignatureTypeId,
                SignerType = signatory.SubscriberTypeId,
                Order = 1,
            };
        }

        private static string GetValueCpfCnpj(string value)
        {
            return FieldValueInfoHelper.ValueToDisplay(value, Models.Enums.FieldTypeEnum.TEXTFIELD)?.Replace(".", "").Replace("/", "").Replace("-", "");
        }
    }
}
