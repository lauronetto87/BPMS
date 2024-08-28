using System.Collections.Generic;

namespace SatelittiBpms.Models.Integration.Signer
{
    public class SubscriberTypeDescriptionListItem
    {
        public LanguageDescription LanguageDescription { get; set; }
        public string Title { get; set; }
        public IList<SubscriberTypeDescription> SubscriberTypeDescriptionList { get; set; }
    }
}
