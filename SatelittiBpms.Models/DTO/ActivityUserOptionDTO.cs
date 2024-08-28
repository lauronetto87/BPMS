namespace SatelittiBpms.Models.DTO
{
    public class ActivityUserOptionDTO
    {
        public string Description { get; set; }
        public int ActivityUserId { get; set; }

        internal long TenantId { get; set; }
        
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
