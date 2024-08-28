namespace SatelittiBpms.Models.DTO
{
    public class ActivityNotificationDTO
    {
        internal long TenantId { get; set; }
        public int ActivityId { get; set; }        
        public int DestinataryType { get; set; }
        public int? RoleId { get; set; }
        public string TitleMessage { get; set; }
        public string Message { get; set; }

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
