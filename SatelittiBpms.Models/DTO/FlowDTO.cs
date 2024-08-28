using SatelittiBpms.Models.Enums;
using System;

namespace SatelittiBpms.Models.DTO
{
    public class FlowDTO
    {
        public long TenantId { get; set; }
        public int ProcessVersionId { get; set; }
        public DateTime CreatedDate { get; set; }
        public FlowStatusEnum Status { get; set; }

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
