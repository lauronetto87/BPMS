using System;
using System.Collections.Generic;

namespace SatelittiBpms.Models.DTO.Integration.Signer
{
    public class IntegrationEnvelopeDTO
    {
        public string Name { get; set; }
        public DateTime? Expiration { get; set; }
        public int Language { get; set; }
        public int Segment { get; set; }
        public string Message { get; set; }
        public int Notify { get; set; }
        public bool NeedAuth { get; set; }
        public bool AuthorizerNeedAuth { get; set; }
        public bool ShowDetails { get; set; }
        public IntegrationEnvelopeSenderDTO Sender { get; set; }
        public List<IntegrationEnvelopeSignerDTO> Signers { get; set; }
        public List<IntegrationEnvelopeAuthorizerDTO> Authorizers { get; set; }
        public List<IntegrationEnvelopeFileDTO> Documents { get; set; }
        public string EmailNotification { get; set; }
        public int WidthInMm { get; set; }
    }
}
