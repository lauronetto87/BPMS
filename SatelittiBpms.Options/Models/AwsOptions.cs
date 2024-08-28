namespace SatelittiBpms.Options.Models
{
    public class AwsOptions
    {
        public const string SECTION_NAME = "AWS";

        public SesOptions SES { get; set; }
        public ApiGatewayOptions ApiGateway { get; set; }
        public StorageOptions Storage { get; set; }
    }
}
