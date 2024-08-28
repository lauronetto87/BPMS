namespace SatelittiBpms.Models.DTO
{
    public class FieldDTO
    {
        public long TenantId { get; set; }
        public int ProcessVersionId { get; set; }
        public string FieldId { get; set; }
        public string FieldLabel { get; set; }
        public string FieldType { get; set; }

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
