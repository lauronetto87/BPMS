using SatelittiBpms.Models.Enums;

namespace SatelittiBpms.Models.ViewModel
{
    public class SignerIntegrationActivityAuthorizerViewModel
    {
        public SignerRegistrationLocationEnum RegistrationLocation { get; set; }
        public string NameFieldKey { get; set; }
        public string CpfFieldKey { get; set; }
        public string EmailFieldKey { get; set; }
        public string OriginActivityId { get; set; }
    }
}
