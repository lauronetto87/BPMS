using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface IUserService : IServiceBase<UserDTO, UserInfo>
    {
        Task<ResultContent<UserInfo>> Get(int userId, int tenantId);
        ResultContent<List<int>> ListIdsByTenant();
        Task<IList<SuiteUserViewModel>> ListUsersSuite();
        Task<ResultContent> Disable(int id);
        Task<ResultContent> Disable(List<int> id);
        Task<SuiteUserViewModel> GetUsersSuite(int id);
    }
}
