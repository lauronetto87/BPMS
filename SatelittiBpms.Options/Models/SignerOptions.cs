
namespace SatelittiBpms.Options.Models
{
    public class SignerOptions
    {
        public const string SECTION_NAME = "Signer";

        public string BasePath { get; set; }
        public string SegmentIntegrationPath { get; set; }
        public string ReminderIntegrationPath { get; set; }
        public string SubscriberTypeIntegrationPath { get; set; }
        public string SignatureTypeIntegrationPath { get; set; }
    }
}
