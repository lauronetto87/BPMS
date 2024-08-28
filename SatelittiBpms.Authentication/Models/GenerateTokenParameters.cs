using Satelitti.Authentication.Model;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Authentication.Models
{
    public class GenerateTokenParameters
    {
        public string Role { get; set; }
        public string Sid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string SuiteToken { get; set; }
        public int TokenLifetimeInMinutes { get; set; }
        public float? Timezone { get; set; }

        public string GetSuiteToken()
        {
            return SuiteToken ?? string.Empty;
        }

        public string GetTimezone()
        {
            return Timezone != null ? Timezone.ToString() : string.Empty;
        }

        public static GenerateTokenParameters AsBpmsUserParameter(
            SuiteUserAuth suiteUser,
            UserInfo bpmsUser,
            int tokenLifetimeInMinutes
        )
        {
            return new GenerateTokenParameters
            {
                Role = UserRoles.FromBpmsUserType(bpmsUser.Type),
                Sid = suiteUser.Id.ToString(),
                Name = suiteUser.Name,
                Email = suiteUser.Mail,
                SuiteToken = suiteUser.SuiteToken,
                TokenLifetimeInMinutes = tokenLifetimeInMinutes,
                Timezone = bpmsUser.Timezone
            };
        }
    }
}
