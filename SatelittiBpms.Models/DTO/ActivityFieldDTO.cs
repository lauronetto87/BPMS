using SatelittiBpms.Models.Enums;

namespace SatelittiBpms.Models.DTO
{
    public class ActivityFieldDTO
    {
        
        internal long TenantId { get; set; }
        public int ProcessVersionId { get; set; }
        public int TaskId { get; set; }
        public int SystemFieldId { get; set; }
        public string FieldId { get; set; }
        public string FieldLabel { get; set; }
        public FieldTypeEnum FieldType { get; set; }          
        public ProcessTaskFieldStateEnum State { get; set; } = ProcessTaskFieldStateEnum.EDITABLE;
        
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
