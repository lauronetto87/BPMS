namespace SatelittiBpms.Models.DTO.Integration.Signer
{
    public class IntegrationEnvelopeSignerDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string IndividualIdentificationCode { get; set; }
        public int SignatureType { get; set; }
        public int SignerType { get; set; }
        public int Order { get; set; }
    }
}
