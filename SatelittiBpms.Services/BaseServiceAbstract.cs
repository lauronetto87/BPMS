using AutoMapper;
using Satelitti.Model;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Repository.Interfaces;

namespace SatelittiBpms.Services
{
    public class BaseServiceAbstract : AbstractServiceBase<BaseInfoDTO, BaseInfo, IRepositoryBase<BaseInfo>>
    {
        public BaseServiceAbstract(IRepositoryBase<BaseInfo> repositoryBase, IMapper mapper) : base(repositoryBase, mapper)
        { }
    }
}
