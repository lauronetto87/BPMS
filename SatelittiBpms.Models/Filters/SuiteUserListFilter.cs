using SatelittiBpms.Models.Enums;
using System.Collections.Generic;

namespace SatelittiBpms.Models.Filters
{
    public class SuiteUserListFilter
    {
        public AuthorizationTypeEnum AuthorizationType { get; set; }
        public string SuiteToken { get; set; }
        public List<int> InUserIds { get; set; }

        public string TenantSubDomain { get; set; }
        public string TenantAccessKey { get; set; }
    }
}
