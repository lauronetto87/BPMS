using Satelitti.Model;
using Newtonsoft.Json;

namespace SatelittiBpms.Models.Infos
{
    public class RoleUserInfo : BaseInfo
    {
        #region Relationship
        public int RoleId { get; set; }
        [JsonIgnore]
        public RoleInfo Role { get; set; }

        public int UserId { get; set; }
        [JsonIgnore]
        public UserInfo User { get; set; }
        #endregion
    }
}
