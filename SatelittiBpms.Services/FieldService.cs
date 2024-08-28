using AutoMapper;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;

namespace SatelittiBpms.Services
{
    public class FieldService : AbstractServiceBase<FieldDTO, FieldInfo, IFieldRepository>, IFieldService
    {
        public FieldService(IFieldRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
