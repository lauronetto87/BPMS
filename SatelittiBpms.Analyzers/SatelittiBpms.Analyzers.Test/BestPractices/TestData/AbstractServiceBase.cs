using AutoMapper;
using Satelitti.Model;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;

namespace SatelittiBpms.Services
{
    public class AbstractServiceBase<DTO, TInfo, TRepo> : FluentValidationServiceBase, IServiceBase<DTO, TInfo>
       where DTO : class
       where TInfo : BaseInfo
       where TRepo : IRepositoryBase<TInfo>
    {
        internal readonly TRepo _repository;
        internal readonly IMapper _mapper;

        public AbstractServiceBase(TRepo repository, IMapper mapper)
        {
            this._repository = repository;
            this._mapper = mapper;
        }

    }
}
