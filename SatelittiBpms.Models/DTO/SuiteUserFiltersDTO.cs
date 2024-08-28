using Newtonsoft.Json;
using System.Collections.Generic;

namespace SatelittiBpms.Models.DTO
{
    public class SuiteUserFiltersDTO
    {
        [JsonProperty("selectAll")]
        public bool SelectAll { set; get; } = false;

        [JsonProperty("filter")]
        public string Filter { set; get; }

        [JsonProperty("notIn")]
        public IList<int> NotIn { set; get; }

        [JsonProperty("in")]
        public IList<int> In { set; get; }
    }
}
