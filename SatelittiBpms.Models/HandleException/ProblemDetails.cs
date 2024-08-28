using Newtonsoft.Json;
using SatelittiBpms.Models.Result;
using System.Collections.Generic;

namespace SatelittiBpms.Models.HandleException
{
    public class ProblemDetails<T> : ProblemDetails
    {
        [JsonProperty]
        private T Value { get; set; }

        public ProblemDetails(ProblemDetails baseProblemDetails, T value)
        {
            Title = baseProblemDetails.Title;
            Status = baseProblemDetails.Status;
            Detail = baseProblemDetails.Detail;
            Errors = baseProblemDetails.Errors;
            MergeErrorsList = baseProblemDetails.MergeErrorsList;
            Value = value;
        }
    }

    public class ProblemDetails
    {
        public string Title { get; set; }
        public int Status { get; set; }
        public string Detail { get; set; }
        public IList<Error> Errors { get; set; }
        public bool MergeErrorsList { get; set; } = false;
    }
}
