using SatelittiBpms.FluentDataBuilder.Process.Data;
using SatelittiBpms.Models.DTO;

namespace SatelittiBpms.Test.Extensions
{
    public static class ActivitySignerSignatoryDataExtension
    {
        public static SignerIntegrationActivitySignatoryDTO AsDto(this ActivitySignerSignatoryData activity)
        {
            return new SignerIntegrationActivitySignatoryDTO
            {
                CpfFieldKey = activity.CpfField?.Id.InternalId,
                EmailFieldKey = activity.EmailField?.Id.InternalId,
                NameFieldKey = activity.NameField?.Id.InternalId,
                RegistrationLocation = activity.RegistrationLocation,
                SignatureTypeId = activity.SignatureTypeId,
                SubscriberTypeId = activity.SubscriberTypeId,
                OriginActivityId = activity.OriginActivityId?.InternalId,
            };
        }
    }
}
