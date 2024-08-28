using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.ViewModel;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface ISignerIntegrationService
    {
        Task<ResultContent> GetSignerInformation();
        Task CreateEnvelopeOnSigner(int taskId);
        Task<FileViewModel> GetFilePrint(string fileKey);
        Task<FileViewModel> GetFileSigned(string fileKey);
    }
}
