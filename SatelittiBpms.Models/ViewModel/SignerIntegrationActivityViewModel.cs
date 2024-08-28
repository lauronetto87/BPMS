using SatelittiBpms.Models.ViewModel;
using System.Collections.Generic;

namespace SatelittiBpms.Models.ViewModel
{
    public class SignerIntegrationActivityViewModel
    {
        public List<string> FileFieldKeys { get; set; }
        public string ActivityKey { get; set; }
        public string ActivityName { get; set; }
        public string EnvelopeTitle { get; set; }
        public string ExpirationDateFieldKey { get; set; }
        public int Language { get; set; }
        public int Segment { get; set; }
        public int SendReminders { get; set; }
        public bool SignatoryAccessAuthentication { get; set; }
        public bool AuthorizeEnablePriorAuthorizationOfTheDocument { get; set; }
        public bool AuthorizeAccessAuthentication { get; set; }
        public List<SignerIntegrationActivityAuthorizerViewModel> Authorizers { get; set; }
        public List<SignerIntegrationActivitySignatoryViewModel> Signatories { get; set; }
    }
}
