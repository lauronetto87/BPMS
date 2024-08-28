using FluentValidation.Results;
using SatelittiBpms.Models.BpmnIo;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Services.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Services.ProcessVersionValidation
{
    internal class FieldVisibilityValidationToSignIntegration : ProcessValidationBase
    {
        private Definitions _bpmnDefinitions;
        private IList<Models.DTO.ActivityDTO> _activities;
        private Models.DTO.ProcessVersionDTO _processVersionDTO;
        private List<string> _allFieldKeys;

        public FieldVisibilityValidationToSignIntegration(Definitions bpmnDefinitions, Models.DTO.ProcessVersionDTO processVersionDTO)
        {
            _bpmnDefinitions = bpmnDefinitions;
            _activities = processVersionDTO.Activities;
            _processVersionDTO = processVersionDTO;
        }

        public override List<ValidationFailure> Validate()
        {
            var errors = new List<ValidationFailure>();
            var allComponentsJson = FormIoHelper.GetAllComponents(_processVersionDTO.FormContent);
            _allFieldKeys = allComponentsJson.Select(c => c.Value<string>("key")).ToList();

            if (_bpmnDefinitions.Process.SatelittiSigner != null)
            {
                if (_bpmnDefinitions.Process.SatelittiSigner.Count != (_processVersionDTO.SignerTasks?.Count ?? 0))
                {
                    var attemptedValue = new
                    {
                        numberOfSignerIntegration = _bpmnDefinitions.Process.SatelittiSigner.Count,
                        numberOfSignerIntegrationConfigured = (_processVersionDTO.SignerTasks?.Count ?? 0),
                    };
                    errors.Add(CreateValidationFailure(ExceptionCodes.INTEGRATION_ACTIVITY_WITHOUT_CONFIGURING, attemptedValue));
                    return errors;
                }
            }

            foreach (var activity in _bpmnDefinitions.Process.SatelittiSigner)
            {
                errors.AddRange(ValidateIntegration(activity, allComponentsJson));
            }

            var errorsWithOutDuplication = new List<ValidationFailure>();
            foreach (var error in errors)
            {
                if (errorsWithOutDuplication.Any(s => s.ErrorMessage == error.ErrorMessage && s.AttemptedValue.Equals(error.AttemptedValue)))
                {
                    continue;
                }
                errorsWithOutDuplication.Add(error);
            }

            return errorsWithOutDuplication;
        }

        public List<ValidationFailure> ValidateIntegration(SatelittiSigner activity, List<Newtonsoft.Json.Linq.JObject> allComponentsJson)
        {
            var fields = GetFieldsUsedInIntegrationSigner(activity);
            var previous = activity.GetPreviousTaskPossible();

            var fieldsWrong = RemoveCorrectFields(fields, previous).SelectMany(f => f);

            var errors = new List<ValidationFailure>();

            foreach (var fieldKey in fieldsWrong)
            {
                var component = allComponentsJson.FirstOrDefault(c => c.Value<string>("key") == fieldKey);
                var attemptedValue = new { nameField = component.Value<string>("label") };
                errors.Add(CreateValidationFailure(ExceptionCodes.MANDATORY_FIELD_FOR_INTEGRATION_WITH_SIGNER_ERROR, attemptedValue));
            }
            return errors;
        }

        private List<List<string>> RemoveCorrectFields(List<string> fields, List<ActivityBase> previous)
        {
            var fieldsRemovedAllChild = new List<List<string>>();
            foreach (var activity in previous)
            {
                List<string> fieldsRemoved = fields;
                if (activity is UserTask)
                {
                    fieldsRemoved = RemoveMandatoryFields(activity.Id, fields);
                }
                var previousChild = activity.GetPreviousTaskPossible();

                if (previousChild.Count > 0)
                {
                    fieldsRemoved = RemoveCorrectFields(fieldsRemoved, previousChild).SelectMany(f => f).ToList();
                }
                if (fieldsRemoved.Count == 0)
                {
                    continue;
                }
                fieldsRemovedAllChild.Add(fieldsRemoved);
            }
            return fieldsRemovedAllChild;
        }

        private List<string> RemoveMandatoryFields(string activityId, List<string> fields)
        {
            var activityDto = _activities.FirstOrDefault(a => a.ActivityId == activityId);
            if (activityDto == null)
            {
                return fields;
            }
            return fields
                .Where(f => activityDto.Fields.All(fDto => fDto.State != ProcessTaskFieldStateEnum.MANDATORY || fDto.FieldId != f))
                .ToList();
        }

        public List<string> GetFieldsUsedInIntegrationSigner(SatelittiSigner activity)
        {
            var activityDto = _processVersionDTO.SignerTasks.FirstOrDefault(a => a.ActivityKey == activity.Id);
            if (activityDto == null)
            {
                throw new System.Exception($"Não foi encontrado a atividade de ID {activity.Id} na lista de de configuração.");
            }
            return _allFieldKeys
                .Where(key =>
                    activityDto.Authorizers.Any(
                        a => a.RegistrationLocation == SignerRegistrationLocationEnum.FormFields
                        && (a.NameFieldKey == key || a.CpfFieldKey == key || a.EmailFieldKey == key)
                    ) ||
                    activityDto.Signatories.Any(
                        a => a.RegistrationLocation == SignerRegistrationLocationEnum.FormFields
                        && (a.NameFieldKey == key || a.CpfFieldKey == key || a.EmailFieldKey == key)
                    ) ||
                    activityDto.FileFieldKeys.Any(f => f == key)
                )
                .ToList();
        }
    }
}
