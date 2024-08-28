using System.Collections.Generic;

namespace SatelittiBpms.Models.Integration.Signer
{
    public class EnvelopeReminderDescriptionListItem
    {
        public LanguageDescription LanguageDescription { get; set; }
        public string Title { get; set; }
        public IList<EnvelopeReminderDescription> EnvelopeReminderDescriptionList { get; set; }
    }
}
