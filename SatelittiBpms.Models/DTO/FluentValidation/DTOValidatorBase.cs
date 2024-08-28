using FluentValidation;
using FluentValidation.Results;

namespace SatelittiBpms.Models.DTO.FluentValidation
{
    public abstract class DTOValidatorBase<TDto> : IDtoValidator
        where TDto : class
    {
        public virtual void BeforeValidate()
        { }

        public ValidationResult Validate()
        {
            object dto = this;
            return CreateValidator().Validate((TDto)dto);
        }

        public abstract IValidator<TDto> CreateValidator();
    }
}
