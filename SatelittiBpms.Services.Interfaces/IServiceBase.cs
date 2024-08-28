using SatelittiBpms.Models.Result;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface IServiceBase<DTO, T>
        where DTO : class
        where T : class
    {
        Task<List<T>> ListAsync(Dictionary<string, string> pFilters = null);

        Task<ResultContent<T>> Get(int id);

        Task<ResultContent<int>> Insert(DTO info);

        Task<ResultContent> Update(int id, DTO info);

        Task Update(T info);

        Task<ResultContent> Delete(DTO info);

        Task<ResultContent> Delete(int id);
    }
}
