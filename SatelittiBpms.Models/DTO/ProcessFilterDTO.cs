using SatelittiBpms.Models.Enums;

namespace SatelittiBpms.Models.DTO
{
    public class ProcessFilterDTO : PaginationBase
    {
        public int SortOrder { get; set; }
        public DateRangeFilterDTO CreationDateRange { get; set; }
        private long TenantId { get; set; }
        public string TextSearch { get; set; }
        public ProcessStatusEnum? State { get; set; }
        public bool? RolesFromUser { get; set; }

        public bool IsOrderAsc()
        {
            return SortOrder == 0;
        }

        public long GetTenantId() => TenantId;
        public void SetTenantId(long tenantId)
        {
            TenantId = tenantId;
        }
        public string Language { get; set; }
    }
}
