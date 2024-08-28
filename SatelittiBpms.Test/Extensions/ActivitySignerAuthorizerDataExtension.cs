using SatelittiBpms.FluentDataBuilder.Process.Data;
using SatelittiBpms.Models.DTO;

namespace SatelittiBpms.Test.Extensions
{
    public static class ActivitySignerAuthorizerDataExtension
    {
        public static SignerIntegrationActivityAuthorizerDTO AsDto(this ActivitySignerAuthorizerData activity)
        {
            return new SignerIntegrationActivityAuthorizerDTO
            {
                CpfFieldKey = activity.CpfField?.Id.InternalId,
                EmailFieldKey = activity.EmailField?.Id.InternalId,
                NameFieldKey = activity.NameField?.Id.InternalId,
                RegistrationLocation = activity.RegistrationLocation,
                OriginActivityId = activity.OriginActivityId,
            };
        }
    }
}
