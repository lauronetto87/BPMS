using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Models.ViewModel
{
    public class UserBpmsViewModel
    {
        public int Id { get; set; }
        public BpmsUserTypeEnum Type { get; set; }
        public bool Enable { get; set; }
        public float? Timezone { get; set; }
        public int? TenantId { get; private set; }

        public static UserBpmsViewModel AsViewModel(UserInfo userInfo)
        {
            return new UserBpmsViewModel
            {
                Id = userInfo.Id,
                Type = userInfo.Type,
                Enable = userInfo.Enable,
                Timezone = userInfo.Timezone,
                TenantId = userInfo.TenantId
            };
        }
    }
}
