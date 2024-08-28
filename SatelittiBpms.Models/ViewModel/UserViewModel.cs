using Satelitti.Authentication.Model;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Models.ViewModel
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public float? Timezone { get; set; }
        public BpmsUserTypeEnum Type { get; set; }

        public static UserViewModel Create(SuiteUser suiteUser, UserInfo userInfo)
        {
            return new UserViewModel
            {
                Timezone = suiteUser.Timezone,
                Id = suiteUser.Id,
                Name = suiteUser.Name,
                Mail = suiteUser.Mail,
                Type = userInfo.Type
            };
        }
    }
}
