using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Result;
using System;
using System.Collections.Generic;
using System.Net;

namespace SatelittiBpms.Models.Exceptions
{
    public class BusinessException<T> : BusinessException
    {
        public T Value { get; set; }

        public BusinessException(T value, List<Error> errors, bool mergeErrorsList = false) : base(errors, mergeErrorsList)
        {
            Value = value;
        }

        public override ProblemDetails<T> GetDetails()
        {
            return new ProblemDetails<T>(base.GetDetails(), Value);
        }
    }


    public class BusinessException : Exception, IHandleException
    {
        public string Title { get; set; }
        public List<Error> Errors { get; set; } = new List<Error>();
        public bool MergeErrorsList { get; set; }

        public BusinessException(List<Error> errors, bool mergeErrorsList = false)
        {
            MergeErrorsList = mergeErrorsList;
            Errors.AddRange(errors);
        }

        public virtual ProblemDetails GetDetails()
        {
            return new ProblemDetails
            {
                Detail = Message,
                Status = (int)HttpStatusCode.BadRequest,
                Title = Title ?? nameof(BusinessException),
                Errors = Errors,
                MergeErrorsList = MergeErrorsList
            };
        }
    }
}
