using FluentValidation;
using SatelittiBpms.Models.DTO.FluentValidation;
using System.Collections.Generic;

namespace SatelittiBpms.Models.DTO
{
    public class SignerIntegrationActivityDTO : DTOValidatorBase<SignerIntegrationActivityDTO>
    {

        public List<string> FileFieldKeys { get; set; }
        public string ActivityKey { get; set; }
        public string EnvelopeTitle { get; set; }
        public string ExpirationDateFieldKey { get; set; }
        public int Language { get; set; }
        public int Segment { get; set; }
        public int SendReminders { get; set; }
        public bool SignatoryAccessAuthentication { get; set; }
        public bool AuthorizeEnablePriorAuthorizationOfTheDocument { get; set; }
        public bool AuthorizeAccessAuthentication { get; set; }
        public List<SignerIntegrationActivityAuthorizerDTO> Authorizers { get; set; }
        public List<SignerIntegrationActivitySignatoryDTO> Signatories { get; set; }

        public override IValidator<SignerIntegrationActivityDTO> CreateValidator()
        {
            InlineValidator<SignerIntegrationActivityDTO> _validator = new();

            _validator.RuleFor(s => s.ActivityKey)
                .NotEmpty();

            return _validator;
        }
    }
}
