using SatelittiBpms.Models.Integration.Signer;
using SatelittiBpms.Services.Interfaces.Integration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Integration.Mock
{
    public class MockSignerSubscriberTypeService : ISignerSubscriberTypeService
    {
        public Task<List<SubscriberTypeDescriptionListItem>> List(string tenantSubdomain, string signerAccessToken)
        {
            return Task.FromResult(new List<SubscriberTypeDescriptionListItem>()
            {
                new SubscriberTypeDescriptionListItem(){
                    Title = "Tipo de Assinante",
                    LanguageDescription = new LanguageDescription(){ Id = LanguageEnum.PORTUGUESE, Description="Portuguese" },
                    SubscriberTypeDescriptionList = new List<SubscriberTypeDescription>(){
                        new SubscriberTypeDescription() { Id = SubscriberTypeEnum.Signatory, Description = "Signatário" },
                        new SubscriberTypeDescription() { Id = SubscriberTypeEnum.Witness, Description = "Testemunha" },
                        new SubscriberTypeDescription() { Id = SubscriberTypeEnum.Authorizer, Description = "Autorizador" },
                    }
                },
                new SubscriberTypeDescriptionListItem(){
                    Title = "Subscriber Type",
                    LanguageDescription = new LanguageDescription(){ Id = LanguageEnum.ENGLISH, Description="English" },
                    SubscriberTypeDescriptionList = new List<SubscriberTypeDescription>(){
                        new SubscriberTypeDescription() { Id = SubscriberTypeEnum.Signatory, Description = "Signatory" },
                        new SubscriberTypeDescription() { Id = SubscriberTypeEnum.Witness, Description = "Witness" },
                        new SubscriberTypeDescription() { Id = SubscriberTypeEnum.Authorizer, Description = "Authorizer" },
                    }
                },
                new SubscriberTypeDescriptionListItem(){
                    Title = "Tipo de Suscriptor",
                    LanguageDescription = new LanguageDescription(){ Id = LanguageEnum.SPANISH, Description="Spanish" },
                    SubscriberTypeDescriptionList = new List<SubscriberTypeDescription>(){
                        new SubscriberTypeDescription() { Id = SubscriberTypeEnum.Signatory, Description = "Signatario" },
                        new SubscriberTypeDescription() { Id = SubscriberTypeEnum.Witness, Description = "Testigo" },
                        new SubscriberTypeDescription() { Id = SubscriberTypeEnum.Authorizer, Description = "Autorizador" },
                    }
                }
            });
        }
    }
}
