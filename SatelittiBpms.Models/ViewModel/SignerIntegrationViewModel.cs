using SatelittiBpms.Models.Integration.Signer;
using SatelittiBpms.Models.ViewModel.Integration;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Models.ViewModel
{
    public class SignerIntegrationViewModel
    {
        public List<SignerItemViewModel> Segments { get; set; }
        public List<SignerLocalizedItemViewModel> Reminders { get; set; }
        public List<SignerLocalizedItemViewModel> SignatureTypes { get; set; }
        public List<SignerLocalizedItemViewModel> SubscriberTypes { get; set; }

        public SignerIntegrationViewModel AsSignerIntegrationStepConfigurationViewModel(
            List<Segment> lstSegments,
            List<EnvelopeReminderDescriptionListItem> lstReminders,
            List<SignatureTypeDescriptionListItem> lstSignatureTypes,
            List<SubscriberTypeDescriptionListItem> lstSubscriberTypes)
        {
            return new SignerIntegrationViewModel()
            {
                Segments = lstSegments?.Where(x => x.Active).Select(x => new SignerItemViewModel() { Id = x.IntegrationCode, Description = x.Name }).ToList(),
                Reminders = lstReminders?.Select(x => new SignerLocalizedItemViewModel()
                {
                    Language = GetLanguage(x.LanguageDescription.Id),
                    Items = x.EnvelopeReminderDescriptionList?.Select(y => new SignerItemViewModel() { Id = (int)y.Id, Description = y.Description }).ToList()
                }).ToList(),
                SignatureTypes = lstSignatureTypes?.Select(x => new SignerLocalizedItemViewModel()
                {
                    Language = GetLanguage(x.LanguageDescription.Id),
                    Items = x.SignatureTypeDescriptionList?.Select(y => new SignerItemViewModel() { Id = (int)y.Id, Description = y.Description }).ToList()
                }).ToList(),
                SubscriberTypes = lstSubscriberTypes?.Select(x => new SignerLocalizedItemViewModel()
                {
                    Language = GetLanguage(x.LanguageDescription.Id),
                    Items = x.SubscriberTypeDescriptionList?.Select(y => new SignerItemViewModel() { Id = (int)y.Id, Description = y.Description }).ToList()
                }).ToList()
            };
        }

        private string GetLanguage(LanguageEnum key)
        {
            return key switch
            {
                LanguageEnum.ENGLISH => "en",
                LanguageEnum.SPANISH => "es",
                _ => "pt",
            };
        }
    }
}
