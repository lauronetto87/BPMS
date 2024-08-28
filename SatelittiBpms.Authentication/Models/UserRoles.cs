using SatelittiBpms.Models.Enums;
using System.ComponentModel;

namespace SatelittiBpms.Authentication.Models
{
    public class UserRoles
    {
        public const string ADMINISTRATOR = "Administrator";
        public const string PUBLISHER = "Publisher";
        public const string READER = "Reader";

        public static string FromBpmsUserType(BpmsUserTypeEnum bpmsUserType)
        {
            switch (bpmsUserType)
            {
                case BpmsUserTypeEnum.ADMINISTRATOR: return ADMINISTRATOR;
                case BpmsUserTypeEnum.PUBLISHER: return PUBLISHER;
                case BpmsUserTypeEnum.READER: return READER;
                default:
                    throw new InvalidEnumArgumentException(nameof(bpmsUserType));
            }
        }
    }
}
