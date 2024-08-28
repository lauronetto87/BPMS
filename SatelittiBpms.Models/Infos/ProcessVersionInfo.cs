using Newtonsoft.Json;
using Satelitti.Model;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Models.Infos
{
    public class ProcessVersionInfo : BaseInfo
    {
        #region Properties
        public int Version { get; set; } = 1;
        public string Name { get; set; }
        public string Description { get; set; }
        public string DescriptionFlow { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByUserName { get; set; }
        public int CreatedByUserId { get; set; }
        public ProcessStatusEnum Status { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string DiagramContent { get; set; }
        public string FormContent { get; set; }
        public string WorkflowContent { get; set; }
        #endregion

        #region Relationships
        public int ProcessId { get; set; }
        [JsonIgnore]
        public ProcessInfo Process { get; set; }

        public IList<ProcessVersionRoleInfo> ProcessVersionRoles { get; set; }
        public IList<FieldInfo> Fields { get; set; }
        public IList<ActivityInfo> Activities { get; set; }
        public IList<FlowInfo> Flows { get; set; }
        public IList<ActivityFieldInfo> ActivityFields { get; set; }
        #endregion

        public ProcessVersionEditViewModel AsEditViewModel()
        {
            return new ProcessVersionEditViewModel()
            {
                ProcessVersionId = Id,
                ProcessId = ProcessId,
                Name = Name,
                Description = Description,
                DescriptionFlow = DescriptionFlow,
                DiagramContent = DiagramContent,
                FormContent = FormContent,
                TaskSequance = Process.TaskSequance,
                RolesIds = ProcessVersionRoles.Select(x => new ProcessVersionRoleEditViewModel
                {
                    Id = x.RoleId,
                    Value = x.Role.Name
                }).ToList(),
                ProcessTaskSettingViewModelList = Activities.Where(x => x.Type == WorkflowActivityTypeEnum.USER_TASK_ACTIVITY).Select(x => new ActivityEditViewModel
                {
                    ActivityId = x.ComponentInternalId,
                    ActivityName = x.Name,
                    Fields = x.ActivityFields != null ? x.ActivityFields.Select(y => new FieldEditViewModel
                    {
                        FieldId = y.Field.ComponentInternalId,
                        FieldLabel = y.Field.Name,
                        FieldType = y.Field.Type.ToString(),
                        State = y.State
                    }).ToList() : new List<FieldEditViewModel>()
                }).ToList(),
                SignerTasks = Activities.Where(x => x.SignerIntegrationActivity != null).Select(x => x.SignerIntegrationActivity).ToList().Count > 0 ? Activities.Where(x => x.SignerIntegrationActivity != null).Select(x => new SignerIntegrationActivityViewModel()
                {
                    ActivityKey = x.SignerIntegrationActivity.Activity.ComponentInternalId,
                    ActivityName = x.SignerIntegrationActivity.Activity.Name,
                    FileFieldKeys = x.SignerIntegrationActivity.Files.Select(y => y.FileField.ComponentInternalId).ToList(),
                    EnvelopeTitle = x.SignerIntegrationActivity.EnvelopeTitle,
                    ExpirationDateFieldKey = x.SignerIntegrationActivity.ExpirationDateField?.ComponentInternalId,
                    Language = x.SignerIntegrationActivity.Language,
                    Segment = x.SignerIntegrationActivity.Segment,
                    SendReminders = x.SignerIntegrationActivity.SendReminders,
                    SignatoryAccessAuthentication = x.SignerIntegrationActivity.SignatoryAccessAuthentication,
                    AuthorizeEnablePriorAuthorizationOfTheDocument = x.SignerIntegrationActivity.AuthorizeEnablePriorAuthorizationOfTheDocument,
                    AuthorizeAccessAuthentication = x.SignerIntegrationActivity.AuthorizeAccessAuthentication,
                    Authorizers = x.SignerIntegrationActivity.Authorizers.Select(y => new SignerIntegrationActivityAuthorizerViewModel()
                    {
                        RegistrationLocation = y.RegistrationLocation,
                        NameFieldKey = y.NameField?.ComponentInternalId,
                        CpfFieldKey = y.CpfField?.ComponentInternalId,
                        EmailFieldKey = y.EmailField?.ComponentInternalId,
                        OriginActivityId = y.OriginActivity?.ComponentInternalId,
                    }).ToList(),
                    Signatories = x.SignerIntegrationActivity.Signatories.Select(y => new SignerIntegrationActivitySignatoryViewModel()
                    {
                        RegistrationLocation = y.RegistrationLocation,
                        NameFieldKey = y.NameField?.ComponentInternalId,
                        CpfFieldKey = y.CpfField?.ComponentInternalId,
                        EmailFieldKey = y.EmailField?.ComponentInternalId,
                        SubscriberTypeId = y.SubscriberTypeId,
                        SignatureTypeId = y.SignatureTypeId,
                        OriginActivityId = y.OriginActivity?.ComponentInternalId,
                    }).ToList(),
                }).ToList() : new List<SignerIntegrationActivityViewModel>()
            };
        }

        public FlowGroupViewModel AsProcessVersionListingViewModel()
        {
            return new FlowGroupViewModel()
            {
                Id = Id,
                Name = Name,
                Ids = new List<int>()
            };
        }
    }
}
