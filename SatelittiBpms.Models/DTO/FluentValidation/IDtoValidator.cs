using FluentValidation.Results;

namespace SatelittiBpms.Models.DTO.FluentValidation
{
    public interface IDtoValidator
    {
        public void BeforeValidate();

        public ValidationResult Validate();
    }
}
