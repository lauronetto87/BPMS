using SatelittiBpms.Models.Enums;
using System.Collections.Generic;

namespace SatelittiBpms.Models.DTO
{
    public class TaskFilterDTO : PaginationBase
    {
        public int SortOrder { get; set; }
        public DateRangeFilterDTO CreationDateRange { get; set; }
        private long TenantId { get; set; }
        public string TextSearch { get; set; }
        public List<int>? GroupId { get; set; }       
        public List<int>? ProcessVersionId { get; set; }
        public TaskQueryType TaskQueryType { get; set; }
        public TaskGroupType TaskGroupType { get; set; }
        public bool? HideFinalized { get; set; }
        public bool? HideAlreadyParticipated { get; set; }
        public bool OnlyMyRequests { get; set; }
        public DateRangeFilterDTO FinalizedDateRange { get; set; }
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
