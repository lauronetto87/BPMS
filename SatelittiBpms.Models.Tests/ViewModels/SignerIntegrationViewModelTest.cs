using NUnit.Framework;
using SatelittiBpms.Models.Integration.Signer;
using SatelittiBpms.Models.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Models.Tests.ViewModels
{
    public class SignerIntegrationViewModelTest
    {
        [Test]
        public void ensureThatAsViewModelFillSegments()
        {
            var segmentList = new List<Segment>() {
                new Segment(){ IntegrationCode = 1, Active = true, Name = "segment1" },
                new Segment(){ IntegrationCode = 2, Active = false, Name = "segment2" },
                new Segment(){ IntegrationCode = 3, Active = true, Name = "segment3" },
                new Segment(){ IntegrationCode = 4, Active = true, Name = "segment4" },
                new Segment(){ IntegrationCode = 5, Active = false, Name = "segment5" },
                new Segment(){ IntegrationCode = 6, Active = true, Name = "segment6" },
            };

            var result = new SignerIntegrationViewModel().AsSignerIntegrationStepConfigurationViewModel(segmentList, new List<EnvelopeReminderDescriptionListItem>(), new List<SignatureTypeDescriptionListItem>(), new List<SubscriberTypeDescriptionListItem>());
            var resultSegmentsIds = result.Segments.Select(x => x.Id).ToList();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Segments);
            Assert.AreEqual(4, result.Segments.Count);
            Assert.Contains(1, resultSegmentsIds);
            Assert.Contains(3, resultSegmentsIds);
            Assert.Contains(4, resultSegmentsIds);
            Assert.Contains(6, resultSegmentsIds);
            Assert.AreEqual("segment1", result.Segments.FirstOrDefault(x => x.Id == 1)?.Description);
            Assert.AreEqual("segment3", result.Segments.FirstOrDefault(x => x.Id == 3)?.Description);
            Assert.AreEqual("segment4", result.Segments.FirstOrDefault(x => x.Id == 4)?.Description);
            Assert.AreEqual("segment6", result.Segments.FirstOrDefault(x => x.Id == 6)?.Description);
        }

        [Test]
        public void ensureThatAsViewModelFillReminders()
        {
            var reminderList = new List<EnvelopeReminderDescriptionListItem>() {
                new EnvelopeReminderDescriptionListItem() {
                    Title = "Frequência de Lembretes",
                    LanguageDescription = new LanguageDescription() { Id = LanguageEnum.PORTUGUESE, Description = "Portuguese" },
                    EnvelopeReminderDescriptionList = new List<EnvelopeReminderDescription>() {
                        new EnvelopeReminderDescription() { Id = ReminderFrequencyEnum.DO_NOT_SEND, Description = "Não enviar" },
                        new EnvelopeReminderDescription() { Id = ReminderFrequencyEnum.EVERY_DAY, Description = "Todos os dias" },
                        new EnvelopeReminderDescription() { Id = ReminderFrequencyEnum.EVERY_2_DAYS, Description = "A cada 2 dias" },
                        new EnvelopeReminderDescription() { Id = ReminderFrequencyEnum.EVERY_4_DAYS, Description = "A cada 4 dias" },
                        new EnvelopeReminderDescription() { Id = ReminderFrequencyEnum.EVERY_7_DAYS, Description = "A cada 7 dias" }
                    }
                },
                new EnvelopeReminderDescriptionListItem() {
                    Title="Reminder Frequency",
                    LanguageDescription = new LanguageDescription() { Id = LanguageEnum.ENGLISH, Description = "English" },
                    EnvelopeReminderDescriptionList = new List<EnvelopeReminderDescription>() {
                        new EnvelopeReminderDescription() { Id = ReminderFrequencyEnum.DO_NOT_SEND, Description = "Do not send" },
                        new EnvelopeReminderDescription() { Id = ReminderFrequencyEnum.EVERY_DAY, Description = "Every day" },
                        new EnvelopeReminderDescription() { Id = ReminderFrequencyEnum.EVERY_2_DAYS, Description = "Every 2 days" },
                        new EnvelopeReminderDescription() { Id = ReminderFrequencyEnum.EVERY_4_DAYS, Description = "Every 4 days" },
                        new EnvelopeReminderDescription() { Id = ReminderFrequencyEnum.EVERY_7_DAYS, Description = "Every 7 days" }
                    }
                },
                new EnvelopeReminderDescriptionListItem() {
                    Title="Frecuencia de Recordatorio",
                    LanguageDescription = new LanguageDescription() { Id = LanguageEnum.SPANISH, Description = "Spanish" },
                    EnvelopeReminderDescriptionList = new List<EnvelopeReminderDescription>() {
                        new EnvelopeReminderDescription() { Id = ReminderFrequencyEnum.DO_NOT_SEND, Description = "No envíe" },
                        new EnvelopeReminderDescription() { Id = ReminderFrequencyEnum.EVERY_DAY, Description = "Todos los días" },
                        new EnvelopeReminderDescription() { Id = ReminderFrequencyEnum.EVERY_2_DAYS, Description = "Cada 2 dias" },
                        new EnvelopeReminderDescription() { Id = ReminderFrequencyEnum.EVERY_4_DAYS, Description = "Cada 4 dias" },
                        new EnvelopeReminderDescription() { Id = ReminderFrequencyEnum.EVERY_7_DAYS, Description = "Cada 7 dias" }
                    }
                }
            };

            var result = new SignerIntegrationViewModel().AsSignerIntegrationStepConfigurationViewModel(new List<Segment>(), reminderList, new List<SignatureTypeDescriptionListItem>(), new List<SubscriberTypeDescriptionListItem>());
            var resultLanguages = result.Reminders.Select(x => x.Language).ToList();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Reminders);
            Assert.AreEqual(3, result.Reminders.Count);
            Assert.Contains("pt", resultLanguages);
            Assert.Contains("en", resultLanguages);
            Assert.Contains("es", resultLanguages);

            var portugueseResult = result.Reminders.FirstOrDefault(x => x.Language == "pt");
            Assert.AreEqual(5, portugueseResult.Items.Count);
            Assert.AreEqual("Não enviar", portugueseResult.Items.FirstOrDefault(x => x.Id == (int)ReminderFrequencyEnum.DO_NOT_SEND)?.Description);
            Assert.AreEqual("Todos os dias", portugueseResult.Items.FirstOrDefault(x => x.Id == (int)ReminderFrequencyEnum.EVERY_DAY)?.Description);
            Assert.AreEqual("A cada 2 dias", portugueseResult.Items.FirstOrDefault(x => x.Id == (int)ReminderFrequencyEnum.EVERY_2_DAYS)?.Description);
            Assert.AreEqual("A cada 4 dias", portugueseResult.Items.FirstOrDefault(x => x.Id == (int)ReminderFrequencyEnum.EVERY_4_DAYS)?.Description);
            Assert.AreEqual("A cada 7 dias", portugueseResult.Items.FirstOrDefault(x => x.Id == (int)ReminderFrequencyEnum.EVERY_7_DAYS)?.Description);

            var englishResult = result.Reminders.FirstOrDefault(x => x.Language == "en");
            Assert.AreEqual(5, englishResult.Items.Count);
            Assert.AreEqual("Do not send", englishResult.Items.FirstOrDefault(x => x.Id == (int)ReminderFrequencyEnum.DO_NOT_SEND)?.Description);
            Assert.AreEqual("Every day", englishResult.Items.FirstOrDefault(x => x.Id == (int)ReminderFrequencyEnum.EVERY_DAY)?.Description);
            Assert.AreEqual("Every 2 days", englishResult.Items.FirstOrDefault(x => x.Id == (int)ReminderFrequencyEnum.EVERY_2_DAYS)?.Description);
            Assert.AreEqual("Every 4 days", englishResult.Items.FirstOrDefault(x => x.Id == (int)ReminderFrequencyEnum.EVERY_4_DAYS)?.Description);
            Assert.AreEqual("Every 7 days", englishResult.Items.FirstOrDefault(x => x.Id == (int)ReminderFrequencyEnum.EVERY_7_DAYS)?.Description);

            var spanishResult = result.Reminders.FirstOrDefault(x => x.Language == "es");
            Assert.AreEqual(5, spanishResult.Items.Count);
            Assert.AreEqual("No envíe", spanishResult.Items.FirstOrDefault(x => x.Id == (int)ReminderFrequencyEnum.DO_NOT_SEND)?.Description);
            Assert.AreEqual("Todos los días", spanishResult.Items.FirstOrDefault(x => x.Id == (int)ReminderFrequencyEnum.EVERY_DAY)?.Description);
            Assert.AreEqual("Cada 2 dias", spanishResult.Items.FirstOrDefault(x => x.Id == (int)ReminderFrequencyEnum.EVERY_2_DAYS)?.Description);
            Assert.AreEqual("Cada 4 dias", spanishResult.Items.FirstOrDefault(x => x.Id == (int)ReminderFrequencyEnum.EVERY_4_DAYS)?.Description);
            Assert.AreEqual("Cada 7 dias", spanishResult.Items.FirstOrDefault(x => x.Id == (int)ReminderFrequencyEnum.EVERY_7_DAYS)?.Description);
        }

        [Test]
        public void ensureThatAsViewModelFillSignatureTypes()
        {
            var signatureTypeList = new List<SignatureTypeDescriptionListItem>()
            {
                new SignatureTypeDescriptionListItem(){
                    Title = "Tipo de Assinatura",
                    LanguageDescription = new LanguageDescription() { Id = LanguageEnum.PORTUGUESE, Description = "Portuguese" },
                    SignatureTypeDescriptionList= new List<SignatureTypeDescription>() {
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.BOTH, Description = "Eletrônica ou Digital" },
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.DIGITAL, Description = "Digital" },
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.ELETRONIC, Description = "Eletrônica" },
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.INPERSON, Description = "Presencial" }
                    }
                },
                new SignatureTypeDescriptionListItem(){
                    Title = "Signature Type",
                    LanguageDescription = new LanguageDescription() { Id = LanguageEnum.ENGLISH, Description = "English" },
                    SignatureTypeDescriptionList= new List<SignatureTypeDescription>() {
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.BOTH, Description = "Electronic or Digital" },
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.DIGITAL, Description = "Digital" },
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.ELETRONIC, Description = "Eletronic" },
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.INPERSON, Description = "In Person" }
                    }
                },
                new SignatureTypeDescriptionListItem(){
                    Title = "Tipo de Firma",
                    LanguageDescription = new LanguageDescription() { Id = LanguageEnum.SPANISH, Description = "Spanish" },
                    SignatureTypeDescriptionList= new List<SignatureTypeDescription>() {
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.BOTH, Description = "Electrónico o Digital" },
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.DIGITAL, Description = "Digital" },
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.ELETRONIC, Description = "Electrónica" },
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.INPERSON, Description = "En persona" }
                    }
                }
            };

            var result = new SignerIntegrationViewModel().AsSignerIntegrationStepConfigurationViewModel(new List<Segment>(), new List<EnvelopeReminderDescriptionListItem>(), signatureTypeList, new List<SubscriberTypeDescriptionListItem>());
            var resultLanguages = result.SignatureTypes.Select(x => x.Language).ToList();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.SignatureTypes);
            Assert.AreEqual(3, result.SignatureTypes.Count);
            Assert.Contains("pt", resultLanguages);
            Assert.Contains("en", resultLanguages);
            Assert.Contains("es", resultLanguages);

            var portugueseResult = result.SignatureTypes.FirstOrDefault(x => x.Language == "pt");
            Assert.AreEqual(4, portugueseResult.Items.Count);
            Assert.AreEqual("Eletrônica ou Digital", portugueseResult.Items.FirstOrDefault(x => x.Id == (int)SignatureTypeEnum.BOTH)?.Description);
            Assert.AreEqual("Digital", portugueseResult.Items.FirstOrDefault(x => x.Id == (int)SignatureTypeEnum.DIGITAL)?.Description);
            Assert.AreEqual("Eletrônica", portugueseResult.Items.FirstOrDefault(x => x.Id == (int)SignatureTypeEnum.ELETRONIC)?.Description);
            Assert.AreEqual("Presencial", portugueseResult.Items.FirstOrDefault(x => x.Id == (int)SignatureTypeEnum.INPERSON)?.Description);

            var englishResult = result.SignatureTypes.FirstOrDefault(x => x.Language == "en");
            Assert.AreEqual(4, englishResult.Items.Count);
            Assert.AreEqual("Electronic or Digital", englishResult.Items.FirstOrDefault(x => x.Id == (int)SignatureTypeEnum.BOTH)?.Description);
            Assert.AreEqual("Digital", englishResult.Items.FirstOrDefault(x => x.Id == (int)SignatureTypeEnum.DIGITAL)?.Description);
            Assert.AreEqual("Eletronic", englishResult.Items.FirstOrDefault(x => x.Id == (int)SignatureTypeEnum.ELETRONIC)?.Description);
            Assert.AreEqual("In Person", englishResult.Items.FirstOrDefault(x => x.Id == (int)SignatureTypeEnum.INPERSON)?.Description);

            var spanishResult = result.SignatureTypes.FirstOrDefault(x => x.Language == "es");
            Assert.AreEqual(4, spanishResult.Items.Count);
            Assert.AreEqual("Electrónico o Digital", spanishResult.Items.FirstOrDefault(x => x.Id == (int)SignatureTypeEnum.BOTH)?.Description);
            Assert.AreEqual("Digital", spanishResult.Items.FirstOrDefault(x => x.Id == (int)SignatureTypeEnum.DIGITAL)?.Description);
            Assert.AreEqual("Electrónica", spanishResult.Items.FirstOrDefault(x => x.Id == (int)SignatureTypeEnum.ELETRONIC)?.Description);
            Assert.AreEqual("En persona", spanishResult.Items.FirstOrDefault(x => x.Id == (int)SignatureTypeEnum.INPERSON)?.Description);
        }

        [Test]
        public void ensureThatAsViewModelFillSubscribeTypes()
        {
            var subscriberTypeList = new List<SubscriberTypeDescriptionListItem>()
            {
                new SubscriberTypeDescriptionListItem(){
                    Title = "Tipo de Assinante",
                    LanguageDescription = new LanguageDescription(){ Id = LanguageEnum.PORTUGUESE, Description = "Portuguese" },
                    SubscriberTypeDescriptionList = new List<SubscriberTypeDescription>(){
                        new SubscriberTypeDescription() { Id = SubscriberTypeEnum.Signatory, Description = "Signatário" },
                        new SubscriberTypeDescription() { Id = SubscriberTypeEnum.Witness, Description = "Testemunha" },
                        new SubscriberTypeDescription() { Id = SubscriberTypeEnum.Authorizer, Description = "Autorizador" },
                    }
                },
                new SubscriberTypeDescriptionListItem(){
                    Title = "Subscriber Type",
                    LanguageDescription = new LanguageDescription(){ Id = LanguageEnum.ENGLISH, Description = "English" },
                    SubscriberTypeDescriptionList = new List<SubscriberTypeDescription>(){
                        new SubscriberTypeDescription() { Id = SubscriberTypeEnum.Signatory, Description = "Signatory" },
                        new SubscriberTypeDescription() { Id = SubscriberTypeEnum.Witness, Description = "Witness" },
                        new SubscriberTypeDescription() { Id = SubscriberTypeEnum.Authorizer, Description = "Authorizer" },
                    }
                },
                new SubscriberTypeDescriptionListItem(){
                    Title = "Tipo de Suscriptor",
                    LanguageDescription = new LanguageDescription(){ Id = LanguageEnum.SPANISH, Description = "Spanish" },
                    SubscriberTypeDescriptionList = new List<SubscriberTypeDescription>(){
                        new SubscriberTypeDescription() { Id = SubscriberTypeEnum.Signatory, Description = "Signatario" },
                        new SubscriberTypeDescription() { Id = SubscriberTypeEnum.Witness, Description = "Testigo" },
                        new SubscriberTypeDescription() { Id = SubscriberTypeEnum.Authorizer, Description = "Autorizador" },
                    }
                }
            };

            var result = new SignerIntegrationViewModel().AsSignerIntegrationStepConfigurationViewModel(new List<Segment>(), new List<EnvelopeReminderDescriptionListItem>(), new List<SignatureTypeDescriptionListItem>(), subscriberTypeList);
            var resultLanguages = result.SubscriberTypes.Select(x => x.Language).ToList();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.SubscriberTypes);
            Assert.AreEqual(3, result.SubscriberTypes.Count);
            Assert.Contains("pt", resultLanguages);
            Assert.Contains("en", resultLanguages);
            Assert.Contains("es", resultLanguages);

            var portugueseResult = result.SubscriberTypes.FirstOrDefault(x => x.Language == "pt");
            Assert.AreEqual(3, portugueseResult.Items.Count);
            Assert.AreEqual("Signatário", portugueseResult.Items.FirstOrDefault(x => x.Id == (int)SubscriberTypeEnum.Signatory)?.Description);
            Assert.AreEqual("Testemunha", portugueseResult.Items.FirstOrDefault(x => x.Id == (int)SubscriberTypeEnum.Witness)?.Description);
            Assert.AreEqual("Autorizador", portugueseResult.Items.FirstOrDefault(x => x.Id == (int)SubscriberTypeEnum.Authorizer)?.Description);

            var englishResult = result.SubscriberTypes.FirstOrDefault(x => x.Language == "en");
            Assert.AreEqual(3, englishResult.Items.Count);
            Assert.AreEqual("Signatory", englishResult.Items.FirstOrDefault(x => x.Id == (int)SubscriberTypeEnum.Signatory)?.Description);
            Assert.AreEqual("Witness", englishResult.Items.FirstOrDefault(x => x.Id == (int)SubscriberTypeEnum.Witness)?.Description);
            Assert.AreEqual("Authorizer", englishResult.Items.FirstOrDefault(x => x.Id == (int)SubscriberTypeEnum.Authorizer)?.Description);

            var spanishResult = result.SubscriberTypes.FirstOrDefault(x => x.Language == "es");
            Assert.AreEqual(3, spanishResult.Items.Count);
            Assert.AreEqual("Signatario", spanishResult.Items.FirstOrDefault(x => x.Id == (int)SubscriberTypeEnum.Signatory)?.Description);
            Assert.AreEqual("Testigo", spanishResult.Items.FirstOrDefault(x => x.Id == (int)SubscriberTypeEnum.Witness)?.Description);
            Assert.AreEqual("Autorizador", spanishResult.Items.FirstOrDefault(x => x.Id == (int)SubscriberTypeEnum.Authorizer)?.Description);
        }
    }
}
