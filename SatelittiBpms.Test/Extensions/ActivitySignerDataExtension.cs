using SatelittiBpms.FluentDataBuilder.Process.Data;
using SatelittiBpms.Models.DTO;
using System.Linq;

namespace SatelittiBpms.Test.Extensions
{
    public static class ActivitySignerDataExtension
    {
        public static SignerIntegrationActivityDTO AsDto(this ActivitySignerData activity)
        {
            return new SignerIntegrationActivityDTO
            {
                ActivityKey = activity.ActivityId,
                AuthorizeAccessAuthentication = activity.AuthorizeAccessAuthentication,
                AuthorizeEnablePriorAuthorizationOfTheDocument = activity.AuthorizeEnablePriorAuthorizationOfTheDocument,
                EnvelopeTitle = activity.EnvelopeTitle,
                ExpirationDateFieldKey = activity.ExpirationDateField?.Id.InternalId,
                Language = activity.Language,
                Segment = activity.Segment,
                SendReminders = activity.SendReminders,
                SignatoryAccessAuthentication = activity.SignatoryAccessAuthentication,
                FileFieldKeys = activity.FileField.Select(f => f.Id.InternalId).ToList(),
                Authorizers = activity.Authorizers.Select(a => a.AsDto()).ToList(),
                Signatories = activity.Signatories.Select(s => s.AsDto()).ToList(),
            };
        }
    }
}
