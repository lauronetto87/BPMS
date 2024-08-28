using System.Collections.Generic;

namespace SatelittiBpms.FluentDataBuilder.Process.Data
{
    public class ActivitySignerData : ActivityBaseData
    {
        public List<FieldBaseData> FileField { get; set; } = new();
        public string EnvelopeTitle { get; set; }
        public FieldBaseData ExpirationDateField { get; set; }
        public int Language { get; set; }
        public int Segment { get; set; }
        public int SendReminders { get; set; }
        public bool SignatoryAccessAuthentication { get; set; }
        public bool AuthorizeEnablePriorAuthorizationOfTheDocument { get; set; }
        public bool AuthorizeAccessAuthentication { get; set; }
        public List<ActivitySignerSignatoryData> Signatories { get; set; } = new();
        public List<ActivitySignerAuthorizerData> Authorizers { get; set; } = new();
    }
}
