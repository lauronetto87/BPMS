using FluentValidation;
using SatelittiBpms.Models.DTO.FluentValidation;
using SatelittiBpms.Models.Extensions;
using SatelittiBpms.Models.HandleException;
using System.Collections.Generic;

namespace SatelittiBpms.Models.DTO
{
    public class ProcessVersionDTO : DTOValidatorBase<ProcessVersionDTO>
    {
        public long TenantId { get; set; }
        public int ProcessId { get; set; }
        public bool NeedPublish { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DescriptionFlow { get; set; }
        public string DiagramContent { get; set; }
        public string FormContent { get; set; }
        public IList<int> RolesIds { get; set; }
        public IList<ActivityDTO> Activities { get; set; }
        public int Version { get; set; }
        public int TaskSequance { get; set; }
        public IList<SignerIntegrationActivityDTO> SignerTasks { get; set; }

        public void SetTenantId(int tenantId)
        {
            TenantId = tenantId;
        }
        public long GetTenantId() => TenantId;

        public override IValidator<ProcessVersionDTO> CreateValidator()
        {
            InlineValidator<ProcessVersionDTO> _validator = new();
            _validator.RuleFor(process => process.Name)
                    .IsNotNullAndNotEmpty(ExceptionCodes.PROCESS_VERSION_NAME_REQUIRED);

            _validator.RuleFor(process => process.DiagramContent)
                    .IsNotNullAndNotEmpty(ExceptionCodes.PROCESS_VERSION_CONTENT_DIAGRAM_REQUIRED)
                    .When(process => process.NeedPublish);

            _validator.RuleFor(process => process.RolesIds)
                    .NotEmpty()
                    .WithMessage(ExceptionCodes.PROCESS_VERSION_ROLES_REQUIRED)
                    .When(process => process.NeedPublish);
            return _validator;
        }
    }
}
