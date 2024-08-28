namespace SatelittiBpms.Models.DTO.Integration.Signer
{
    public class SignerIntegrationEnvelopeFileDTO
    {
        public string Base64Content { get; set; }
        public string Name { get; set; }
        public int FieldValueFileId { get; set; }
    }
}
