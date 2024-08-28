using SatelittiBpms.Models.Enums;

namespace SatelittiBpms.FluentDataBuilder.Process.Data
{
    public class ActivitySignerAuthorizerData : BaseData
    {
        public SignerRegistrationLocationEnum RegistrationLocation { get; set; }

        public FieldBaseData NameField { get; set; }

        public FieldBaseData CpfField { get; set; }

        public FieldBaseData EmailField { get; set; }

        public string OriginActivityId { get; set; }
    }
}
