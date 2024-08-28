using Newtonsoft.Json;
using Satelitti.Model;
using SatelittiBpms.Models.Enums;
using System.Collections.Generic;

namespace SatelittiBpms.Models.Infos
{
    public class FieldInfo : BaseInfo
    {
        #region Properties
        public string ComponentInternalId { get; set; }
        public string Name { get; set; }
        public FieldTypeEnum Type { get; set; }
        #endregion

        #region Relationships
        public int ProcessVersionId { get; set; }
        [JsonIgnore]
        public ProcessVersionInfo ProcessVersion { get; set; }

        public IList<ActivityFieldInfo> ActivityFields { get; set; }
        public IList<FieldValueInfo> FieldValues { get; set; }
        public IList<SignerIntegrationActivityInfo> SignerIntegrationActivityExpirationDate { get; set; }
        public IList<SignerIntegrationActivityFileInfo> SignerIntegrationActivityFile { get; set; }
        public IList<SignerIntegrationActivityAuthorizerInfo> SignerIntegrationActivityAuthorizerName { get; set; }
        public IList<SignerIntegrationActivityAuthorizerInfo> SignerIntegrationActivityAuthorizerCpf { get; set; }
        public IList<SignerIntegrationActivityAuthorizerInfo> SignerIntegrationActivityAuthorizerEmail { get; set; }
        public IList<SignerIntegrationActivitySignatoryInfo> SignerIntegrationActivitySignatoryName { get; set; }
        public IList<SignerIntegrationActivitySignatoryInfo> SignerIntegrationActivitySignatoryCpf { get; set; }
        public IList<SignerIntegrationActivitySignatoryInfo> SignerIntegrationActivitySignatoryEmail { get; set; }
        #endregion
    }
}
