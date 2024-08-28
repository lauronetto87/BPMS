using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.ViewModel;
using System.Collections.Generic;

namespace SatelittiBpms.Services.Interfaces
{
    public interface IWildcardService
    {
        string FormatDescriptionWildcard(string description, FlowInfo flow, IList<SuiteUserViewModel> userViewModel);
    }
}
