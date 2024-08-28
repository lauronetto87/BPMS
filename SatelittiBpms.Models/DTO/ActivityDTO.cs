using SatelittiBpms.Models.Enums;
using System.Collections.Generic;

namespace SatelittiBpms.Models.DTO
{
    public class ActivityDTO
    {
        internal long TenantId { get; set; }
        public int ProcessVersionId { get; set; }
        public string ActivityId { get; set; }
        public string ActivityName { get; set; }
        public WorkflowActivityTypeEnum ActivityType { get; set; }
        public IList<ActivityFieldDTO> Fields { get; set; }

        public void SetTenantId(int tenantId)
        {
            TenantId = tenantId;
        }
        public void SetTenantId(long tenantId)
        {
            TenantId = tenantId;
        }
        public long GetTenantId() => TenantId;
    }
}
