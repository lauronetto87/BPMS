using Satelitti.Model;
using System.Text.Json.Serialization;

namespace SatelittiBpms.Models.Infos
{
    public class SignerIntegrationActivityFileInfo : BaseInfo
    {
        #region Properties

        #endregion

        #region Relationships
        public int FileFieldId { get; set; }
        [JsonIgnore]
        public FieldInfo FileField { get; set; }

        public int SignerIntegrationActivityId { get; set; }
        [JsonIgnore]
        public SignerIntegrationActivityInfo SignerIntegrationActivity { get; set; }
        #endregion
    }
}
