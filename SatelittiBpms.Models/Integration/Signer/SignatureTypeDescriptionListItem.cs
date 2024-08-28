using System.Collections.Generic;

namespace SatelittiBpms.Models.Integration.Signer
{
    public class SignatureTypeDescriptionListItem
    {
        public LanguageDescription LanguageDescription { get; set; }
        public string Title { get; set; }
        public IList<SignatureTypeDescription> SignatureTypeDescriptionList { get; set; }
    }
}
