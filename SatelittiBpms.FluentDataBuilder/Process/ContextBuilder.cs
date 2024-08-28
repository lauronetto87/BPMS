using SatelittiBpms.Models.Infos;
using System.Collections.Generic;

namespace SatelittiBpms.FluentDataBuilder.Process
{
    public class ContextBuilder
    {
        public List<UserInfo> Users { get; set; }

        public List<RoleInfo> Roles { get; set; }

        public List<TenantInfo> Tenants { get; set; }
        public object ExtendData { get; set; }
    }
}
