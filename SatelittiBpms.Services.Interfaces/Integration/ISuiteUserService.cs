using SatelittiBpms.Models.Filters;
using SatelittiBpms.Models.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces.Integration
{
    public interface ISuiteUserService
    {
        Task<IList<SuiteUserViewModel>> ListWithContext(SuiteUserListFilter suiteUserListFilter);
        Task<IList<SuiteUserViewModel>> ListWithoutContext(SuiteUserListFilter suiteUserListFilter);
    }
}
