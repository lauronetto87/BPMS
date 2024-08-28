using Newtonsoft.Json;
using Satelitti.Model;

namespace SatelittiBpms.Models.Infos
{
    public class ProcessVersionRoleInfo : BaseInfo
    {
        #region Relationship
        public int ProcessVersionId { get; set; }
        [JsonIgnore]
        public ProcessVersionInfo ProcessVersion { get; set; }

        public int RoleId { get; set; }
        [JsonIgnore]
        public RoleInfo Role { get; set; }
        #endregion
    }
}
