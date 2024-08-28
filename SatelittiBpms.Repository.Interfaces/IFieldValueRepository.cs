using SatelittiBpms.Models.Infos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Repository.Interfaces
{
    public interface IFieldValueRepository : IRepositoryBase<FieldValueInfo>
    {
        Task UpdateFieldValues(List<FieldValueInfo> fieldValueInfoList);
    }
}
