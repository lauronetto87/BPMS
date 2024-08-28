using SatelittiBpms.Models.Enums;

namespace SatelittiBpms.FluentDataBuilder.Process.Data
{
    public class ActivitySignerSignatoryData : BaseData
    {
        public SignerRegistrationLocationEnum RegistrationLocation { get; set; }

        public FieldBaseData NameField { get; set; }

        public FieldBaseData CpfField { get; set; }

        public FieldBaseData EmailField { get; set; }

        public int SubscriberTypeId { get; set; }

        public int SignatureTypeId { get; set; }

        public DataId OriginActivityId { get; set; }
    }
}
