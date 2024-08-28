namespace SatelittiBpms.Options.Models
{
    public class AuthenticationOptions
    {
        public const string SECTION_NAME = "Authentication";

        public string SecretKey { get; set; }
        public int TokenLifetimeInMinutes { get; set; }
    }
}
