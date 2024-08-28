using SatelittiBpms.Models.Integration.Signer;
using SatelittiBpms.Services.Interfaces.Integration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Integration.Mock
{
    public class MockSignerSignatureTypeService : ISignerSignatureTypeService
    {
        public Task<List<SignatureTypeDescriptionListItem>> List(string tenantSubdomain, string signerAccessToken)
        {
            return Task.FromResult(new List<SignatureTypeDescriptionListItem>()
            {
                new SignatureTypeDescriptionListItem(){
                    Title = "Tipo de Assinatura",
                    LanguageDescription = new LanguageDescription() {Id = LanguageEnum.PORTUGUESE, Description="Portuguese" },
                    SignatureTypeDescriptionList= new List<SignatureTypeDescription>() {
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.BOTH, Description="Eletrônica ou Digital" },
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.DIGITAL, Description="Digital" },
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.ELETRONIC, Description="Eletrônica" },
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.INPERSON, Description="Presencial" }
                    }
                },
                new SignatureTypeDescriptionListItem(){
                    Title = "Signature Type",
                    LanguageDescription = new LanguageDescription() {Id = LanguageEnum.ENGLISH, Description="English" },
                    SignatureTypeDescriptionList= new List<SignatureTypeDescription>() {
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.BOTH, Description="Electronic or Digital" },
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.DIGITAL, Description="Digital" },
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.ELETRONIC, Description="Eletronic" },
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.INPERSON, Description="In Person" }
                    }
                },
                new SignatureTypeDescriptionListItem(){
                    Title = "Tipo de Firma",
                    LanguageDescription = new LanguageDescription() {Id = LanguageEnum.SPANISH, Description="Spanish" },
                    SignatureTypeDescriptionList= new List<SignatureTypeDescription>() {
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.BOTH, Description="Electrónico o Digital" },
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.DIGITAL, Description="Digital" },
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.ELETRONIC, Description="Electrónica" },
                        new SignatureTypeDescription() { Id = SignatureTypeEnum.INPERSON, Description="En persona" }
                    }
                }
            });
        }
    }
}
