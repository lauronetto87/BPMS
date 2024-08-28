using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface ITemplateService : IServiceBase<TemplateDTO, TemplateInfo>
    {
        Task<ResultContent> GetById(int templateId);
    }
}
