using FluentValidation;
using SatelittiBpms.Models.DTO.FluentValidation;
using SatelittiBpms.Models.Extensions;
using SatelittiBpms.Models.HandleException;
using System.Collections.Generic;

namespace SatelittiBpms.Models.DTO
{
    public class RoleDTO : DTOValidatorBase<RoleDTO>
    {
        public long TenantId { get; set; }
        public string Name { get; set; }
        public IList<int> UsersIds { get; set; }

        public void SetTenantId(int tenantId)
        {
            TenantId = tenantId;
        }
        public long GetTenantId() => TenantId;

        public override IValidator<RoleDTO> CreateValidator()
        {
            InlineValidator<RoleDTO> _validator = new();

            _validator.RuleFor(role => role.Name)
                .IsNotNullAndNotEmpty(ExceptionCodes.ROLE_NAME_REQUIRED);

            _validator.RuleFor(role => role.UsersIds)
                .NotEmpty()
                .WithMessage(ExceptionCodes.ROLE_USERS_IDS_REQUIRED);

            return _validator;
        }
    }
}
