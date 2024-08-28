using Satelitti.Model;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SatelittiBpms.Models.Infos
{
    public class SignerIntegrationActivityInfo : BaseInfo
    {
        #region Properties
        public string EnvelopeTitle { get; set; }
        public int Language { get; set; }
        public int Segment { get; set; }
        public int SendReminders { get; set; }
        public bool SignatoryAccessAuthentication { get; set; }
        public bool AuthorizeEnablePriorAuthorizationOfTheDocument { get; set; }
        public bool AuthorizeAccessAuthentication { get; set; }
        #endregion

        #region Relationships
        public int ActivityId { get; set; }
        [JsonIgnore]
        public ActivityInfo Activity { get; set; }

        public int? ExpirationDateFieldId { get; set; }
        [JsonIgnore]
        public FieldInfo ExpirationDateField { get; set; }

        public IList<SignerIntegrationActivityAuthorizerInfo> Authorizers { get; set; }
        public IList<SignerIntegrationActivitySignatoryInfo> Signatories { get; set; }
        public IList<SignerIntegrationActivityFileInfo> Files { get; set; }
        #endregion

    }
}
