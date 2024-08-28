using FluentValidation;
using FluentValidation.Results;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Integration.Signer;
using SatelittiBpms.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Services.ProcessVersionValidation
{
    internal class SignerIntegrationActivityValidator : ProcessValidationBase
    {
        private ProcessVersionDTO _processVersionDTO;

        public SignerIntegrationActivityValidator(ProcessVersionDTO processVersionDTO)
        {
            _processVersionDTO = processVersionDTO;
        }

        public override List<ValidationFailure> Validate()
        {
            if ((_processVersionDTO.SignerTasks?.Count ?? 0) == 0)
            {
                return new List<ValidationFailure>();
            }
            var validator = new InlineValidator<ProcessVersionDTO>();

            validator.RuleForEach(x => x.SignerTasks).SetValidator(CreateSignerActivityValidation());
            validator.RuleFor(x => x).Custom(FileFieldKeyIsNotDuplicate);

            return validator.Validate(_processVersionDTO).Errors;
        }

        private void FileFieldKeyIsNotDuplicate(ProcessVersionDTO dto, ValidationContext<ProcessVersionDTO> context)
        {
            var filefieldDuplicatedList = dto.SignerTasks.SelectMany(x => x.FileFieldKeys)
               .GroupBy(x => x)
               .Where(x => x.Count() > 1)
               .Select(x => x.Key).ToList();

            if (filefieldDuplicatedList.Any())
            {
                var allComponentsJson = FormIoHelper.GetAllComponents(dto.FormContent);
                filefieldDuplicatedList.ForEach(file => context.AddFailure(new ValidationFailure("FileFieldKeys", ExceptionCodes.FILE_FIELD_ASSOCIATED_TO_MORE_THAN_ONE_INTEGRATION_ACTIVITY, new { duplicatedField = allComponentsJson.FirstOrDefault(c => c.Value<string>("key") == file).Value<string>("label") })));
            }
        }

        private static InlineValidator<SignerIntegrationActivityDTO> CreateSignerActivityValidation()
        {
            var validator = new InlineValidator<SignerIntegrationActivityDTO>();

            validator.RuleFor(x => x.ActivityKey).NotEmpty();

            validator.RuleFor(x => x.AuthorizeEnablePriorAuthorizationOfTheDocument)
                .Equal(true)
                .When(x => (x.Authorizers?.Count) > 0);

            validator.RuleFor(x => x.EnvelopeTitle).NotEmpty();

            validator.RuleFor(x => x.FileFieldKeys).NotEmpty();

            validator.RuleFor(x => x.Language).NotEmpty();

            validator.RuleFor(x => x.SendReminders).NotEmpty();

            validator.RuleForEach(x => x.Signatories).SetValidator(CreateSignerSignatoryValidation());

            validator.RuleForEach(x => x.Authorizers).SetValidator(CreateSignerAuthorizerValidation());

            return validator;
        }
        private static InlineValidator<SignerIntegrationActivitySignatoryDTO> CreateSignerSignatoryValidation()
        {
            var validator = new InlineValidator<SignerIntegrationActivitySignatoryDTO>();

            validator.RuleFor(x => x.RegistrationLocation).NotNull();

            validator.RuleFor(x => x.NameFieldKey)
                .NotEmpty()
                .When(x => x.RegistrationLocation == Models.Enums.SignerRegistrationLocationEnum.FormFields);
            validator.RuleFor(x => x.NameFieldKey)
                .Empty()
                .When(x => x.RegistrationLocation != Models.Enums.SignerRegistrationLocationEnum.FormFields);

            validator.RuleFor(x => x.EmailFieldKey)
                .NotEmpty()
                .When(x => x.RegistrationLocation == Models.Enums.SignerRegistrationLocationEnum.FormFields);
            validator.RuleFor(x => x.EmailFieldKey)
                .Empty()
                .When(x => x.RegistrationLocation != Models.Enums.SignerRegistrationLocationEnum.FormFields);

            validator.RuleFor(x => x.OriginActivityId)
                .NotEmpty()
                .When(x => x.RegistrationLocation == Models.Enums.SignerRegistrationLocationEnum.UserTask);
            validator.RuleFor(x => x.OriginActivityId)
                .Empty()
                .When(x => x.RegistrationLocation != Models.Enums.SignerRegistrationLocationEnum.UserTask);

            validator.RuleFor(x => x.SignatureTypeId).InclusiveBetween((int)SignatureTypeEnum.BOTH, (int)SignatureTypeEnum.INPERSON);

            validator.RuleFor(x => x.SubscriberTypeId).InclusiveBetween((int)SubscriberTypeEnum.Signatory, (int)SubscriberTypeEnum.Authorizer);

            return validator;
        }

        private static InlineValidator<SignerIntegrationActivityAuthorizerDTO> CreateSignerAuthorizerValidation()
        {
            var validator = new InlineValidator<SignerIntegrationActivityAuthorizerDTO>();

            validator.RuleFor(x => x.RegistrationLocation).NotNull();

            validator.RuleFor(x => x.NameFieldKey)
                .NotEmpty()
                .When(x => x.RegistrationLocation == Models.Enums.SignerRegistrationLocationEnum.FormFields);
            validator.RuleFor(x => x.NameFieldKey)
                .Empty()
                .When(x => x.RegistrationLocation != Models.Enums.SignerRegistrationLocationEnum.FormFields);

            validator.RuleFor(x => x.EmailFieldKey)
                .NotEmpty()
                .When(x => x.RegistrationLocation == Models.Enums.SignerRegistrationLocationEnum.FormFields);
            validator.RuleFor(x => x.EmailFieldKey)
                .Empty()
                .When(x => x.RegistrationLocation != Models.Enums.SignerRegistrationLocationEnum.FormFields);

            validator.RuleFor(x => x.OriginActivityId)
                .NotEmpty()
                .When(x => x.RegistrationLocation == Models.Enums.SignerRegistrationLocationEnum.UserTask);
            validator.RuleFor(x => x.OriginActivityId)
                .Empty()
                .When(x => x.RegistrationLocation != Models.Enums.SignerRegistrationLocationEnum.UserTask);

            return validator;
        }
    }
}
