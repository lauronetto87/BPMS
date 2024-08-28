using FluentValidation;
using SatelittiBpms.Models.DTO.FluentValidation;
using System;

namespace SatelittiBpms.Models.Tests.DTO
{
    internal class DTOValidatorBaseHelper : DTOValidatorBase<DTOValidatorBaseHelper>
    {

        public override void BeforeValidate()
        {
            base.BeforeValidate();
        }

        public override IValidator<DTOValidatorBaseHelper> CreateValidator()
        {
            throw new NotImplementedException();
        }
    }
}
