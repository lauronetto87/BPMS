using SatelittiBpms.Models.Enums;
using System.Collections.Generic;

namespace SatelittiBpms.Models.DTO
{
    public class ActivityUserDTO
    {
        internal long TenantId { get; set; }        
        public int ActivityId { get; set; }
        public int RoleId { get; set; }
        public UserTaskExecutorTypeEnum TypeExecutor { get; set; }
        public IList<ActivityUserOptionDTO> ActivityUserOptions { get; set; }

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
