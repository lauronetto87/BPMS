using FluentValidation.Results;

namespace SatelittiBpms.Models.Result
{
    public class Result
    {
        public static ResultContent<T> Success<T>(T value)
        {
            return new ResultContent<T>(value, success: true, errorId: null);
        }

        public static ResultContent Success()
        {
            return new ResultContent(success: true, errorId: null);
        }

        public static ResultContent Error(string messageId)
        {
            return new ResultContent(success: false, errorId: messageId);
        }

        public static ResultContent Error(ValidationResult validationResult, bool mergeErrorsList = false)
        {
            return new ResultContent(validationResult, mergeErrorsList);
        }

        public static ResultContent<T> Error<T>(T value, ValidationResult validationResult, bool mergeErrorsList = false)
        {
            return new ResultContent<T>(value, validationResult, mergeErrorsList);
        }
    }
}
