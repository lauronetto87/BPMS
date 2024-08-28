using FluentValidation.Results;
using Newtonsoft.Json;
using System.ComponentModel;

namespace SatelittiBpms.Models.Result
{
    public class ResultContent<T> : ResultContent
    {
        public T Value { get; private set; }

        public ResultContent(T value, bool success, string errorId) : base(success, errorId)
        {
            Value = value;
        }

        public ResultContent(T value, ValidationResult validationResult, bool mergeErrorsList) : base(validationResult, mergeErrorsList)
        {
            Value = value;
        }

        public static T GetValue(ResultContent resultContent)
        {
            return ((ResultContent<T>)resultContent).Value;
        }
    }

    public class ResultContent
    {
        public bool Success { get; internal set; }
        public string ErrorId { get; internal set; }
        public bool MergeErrorsList { get; private set; }

        [Browsable(false)]
        public ValidationResult ValidationResult { get; private set; }

        public string ToErrorJson()
        {
            return JsonConvert.SerializeObject(ToErrorObject());
        }

        public object ToErrorObject()
        {
            return new
            {
                errorId = ErrorId
            };
        }

        public ResultContent(bool success, string errorId)
        {
            Success = success;
            ErrorId = errorId;
        }

        public ResultContent(ValidationResult validationResult, bool mergeErrorsList)
        {
            Success = false;
            ErrorId = default;
            ValidationResult = validationResult;
            MergeErrorsList = mergeErrorsList;
        }
    }
}
