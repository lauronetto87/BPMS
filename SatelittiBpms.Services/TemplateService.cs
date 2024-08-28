using AutoMapper;
using Satelitti.Authentication.Context.Interface;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class TemplateService : AbstractServiceBase<TemplateDTO, TemplateInfo, ITemplateRepository>, ITemplateService
    {        

        public TemplateService(
            ITemplateRepository repository,
            IMapper mapper            
            ) : base(repository, mapper)
        {            
        }

        public async Task<ResultContent> GetById(int templateId)
        {            
            var template = await _repository.Get(templateId);
            return Result.Success(template);
        }
    }
}
