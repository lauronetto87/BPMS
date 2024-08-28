using Satelitti.Model;
using SatelittiBpms.Models.Enums;
using System.Text.Json.Serialization;

namespace SatelittiBpms.Models.Infos
{
    public class SignerIntegrationActivitySignatoryInfo : BaseInfo
    {
        #region Properties
        public SignerRegistrationLocationEnum RegistrationLocation { get; set; }
        public int SubscriberTypeId { get; set; }
        public int SignatureTypeId { get; set; }
        #endregion

        #region Relationships


        public int? NameFieldId { get; set; }
        [JsonIgnore]
        public FieldInfo NameField { get; set; }

        public int? CpfFieldId { get; set; }
        [JsonIgnore]
        public FieldInfo CpfField { get; set; }

        public int? EmailFieldId { get; set; }
        [JsonIgnore]
        public FieldInfo EmailField { get; set; }

        public int SignerIntegrationActivityId { get; set; }
        [JsonIgnore]
        public SignerIntegrationActivityInfo SignerIntegrationActivity { get; set; }

        public int? OriginActivityId { get; set; }
        [JsonIgnore]
        public ActivityInfo OriginActivity { get; set; }
        #endregion
    }
}
