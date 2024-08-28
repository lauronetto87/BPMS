using System;

namespace SatelittiBpms.Models.DTO
{
    public class TaskDTO
    {
        public int Id { get; set; }
        internal long TenantId { get; set; }
        public int FlowId { get; set; }
        public int ActivityId { get; set; }
        public int ExecutorId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime FinishedDate { get; set; }

        public void SetTenantId(int tenantId)
        {
            TenantId = tenantId;
        }
        public long GetTenantId() => TenantId;
    }
}
