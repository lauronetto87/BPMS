using SatelittiBpms.Models.Integration.Signer;
using SatelittiBpms.Services.Interfaces.Integration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Integration.Mock
{
    public class MockSignerReminderService : ISignerReminderService
    {
        public Task<List<EnvelopeReminderDescriptionListItem>> List(string tenantSubdomain, string signerAccessToken)
        {
            return Task.FromResult(new List<EnvelopeReminderDescriptionListItem>(){
                new EnvelopeReminderDescriptionListItem(){
                    Title="Frequência de Lembretes",
                    LanguageDescription = new LanguageDescription(){Id= LanguageEnum.PORTUGUESE, Description="Portuguese" },
                    EnvelopeReminderDescriptionList = new List<EnvelopeReminderDescription>(){
                    new EnvelopeReminderDescription(){Id=ReminderFrequencyEnum.DO_NOT_SEND, Description="Não enviar"  },
                    new EnvelopeReminderDescription(){Id=ReminderFrequencyEnum.EVERY_DAY, Description="Todos os dias"  },
                    new EnvelopeReminderDescription(){Id=ReminderFrequencyEnum.EVERY_2_DAYS, Description="A cada 2 dias"  },
                    new EnvelopeReminderDescription(){Id=ReminderFrequencyEnum.EVERY_4_DAYS, Description="A cada 4 dias"  },
                    new EnvelopeReminderDescription(){Id=ReminderFrequencyEnum.EVERY_7_DAYS, Description="A cada 7 dias"  }
                    }
                },
                new EnvelopeReminderDescriptionListItem(){
                    Title="Reminder Frequency",
                    LanguageDescription = new LanguageDescription(){Id= LanguageEnum.ENGLISH, Description="English" },
                    EnvelopeReminderDescriptionList = new List<EnvelopeReminderDescription>(){
                    new EnvelopeReminderDescription(){Id=ReminderFrequencyEnum.DO_NOT_SEND, Description="Do not send"  },
                    new EnvelopeReminderDescription(){Id=ReminderFrequencyEnum.EVERY_DAY, Description="Every day"  },
                    new EnvelopeReminderDescription(){Id=ReminderFrequencyEnum.EVERY_2_DAYS, Description="Every 2 days"  },
                    new EnvelopeReminderDescription(){Id=ReminderFrequencyEnum.EVERY_4_DAYS, Description="Every 4 days"  },
                    new EnvelopeReminderDescription(){Id=ReminderFrequencyEnum.EVERY_7_DAYS, Description="Every 7 days"  }
                    }
                },
                new EnvelopeReminderDescriptionListItem(){
                    Title="Frecuencia de Recordatorio",
                    LanguageDescription = new LanguageDescription(){Id= LanguageEnum.SPANISH, Description="Spanish" },
                    EnvelopeReminderDescriptionList = new List<EnvelopeReminderDescription>(){
                    new EnvelopeReminderDescription(){Id=ReminderFrequencyEnum.DO_NOT_SEND, Description="No envíe"  },
                    new EnvelopeReminderDescription(){Id=ReminderFrequencyEnum.EVERY_DAY, Description="Todos los días"  },
                    new EnvelopeReminderDescription(){Id=ReminderFrequencyEnum.EVERY_2_DAYS, Description="Cada 2 dias"  },
                    new EnvelopeReminderDescription(){Id=ReminderFrequencyEnum.EVERY_4_DAYS, Description="Cada 4 dias"  },
                    new EnvelopeReminderDescription(){Id=ReminderFrequencyEnum.EVERY_7_DAYS, Description="Cada 7 dias"  }
                    }
                }
            });
        }



    }
}
