using System;

namespace SatelittiBpms.Models.DTO
{
    public class TaskHistoryDTO
    {
        internal long TenantId { get; set; }
        public int TaskId { get; set; }
        public int ExecutorId { get; set;}
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public void SetTenantId(int tenantId)
        {
            TenantId = tenantId;
        }
        public long GetTenantId() => TenantId;
    }
}
