namespace SatelittiBpms.Models.DTO
{
    public class FlowFilterDTO : PaginationBase
    {
        public int SortOrder { get; set; }
        public DateRangeFilterDTO CreationDateRange { get; set; }
        private long TenantId { get; set; }
        public string TextSearch { get; set; }
        public int? UserId { get; set; }

        public bool IsOrderAsc()
        {
            return SortOrder == 0;
        }
        public long GetTenantId() => TenantId;
        public void SetTenantId(long tenantId)
        {
            TenantId = tenantId;
        }
    }
}
