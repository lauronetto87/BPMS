using FluentValidation;
using System.Collections;

namespace SatelittiBpms.Models.Extensions
{
    public static class FluentValidationExtension
    {
        public static IRuleBuilderOptions<T, TElement> IsNotNullAndNotEmpty<T, TElement>(this IRuleBuilder<T, TElement> ruleBuilder, string message)
        {
            return ruleBuilder
                        .Must(x => x != null &&
                            !(x is string && string.IsNullOrEmpty(x.ToString())) &&
                            !(x is IList list && list.Count == 0)
                        )
                        .WithMessage(message);
        }
    }
}
