using SatelittiBpms.Models.Enums;

namespace SatelittiBpms.Models.DTO
{
    public class SignerIntegrationActivitySignatoryDTO
    {
        public SignerRegistrationLocationEnum RegistrationLocation { get; set; }
        public string NameFieldKey { get; set; }
        public string CpfFieldKey { get; set; }
        public string EmailFieldKey { get; set; }
        public int SubscriberTypeId { get; set; }
        public int SignatureTypeId { get; set; }
        public string OriginActivityId { get; set; }
    }
}
